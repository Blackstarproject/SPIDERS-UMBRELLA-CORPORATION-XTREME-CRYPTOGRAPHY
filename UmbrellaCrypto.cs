using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;

namespace SPIDERS_UMBRELLA_CORPORATION.Core
{
    public class UmbrellaCrypto
    {
        private const int SaltSize = 32;
        private const int IvSize = 16;
        private const int KeySize = 32;
        private const int HmacSize = 32;
        private const int Iterations = 50000; 
        private const string Extension = ".T_VIRUS";

        public void EncryptFile(string sourceFile, SecureString password)
        {
            if (sourceFile.EndsWith(Extension)) return;
            string tempFile = sourceFile + ".tmp";
            string finalFile = sourceFile + Extension;

            byte[] entropy = SecurityUtils.GetEntropy();
            byte[] salt = GenerateRandomBytes(SaltSize, entropy);
            byte[] iv = GenerateRandomBytes(IvSize, entropy);

            byte[] passBytes = SecurityUtils.ToByteArray(password);
            byte[] hwId = SecurityUtils.GetHardwareFingerprint();

                         // Combine Password + HardwareID
            byte[] combinedKeyMaterial = new byte[passBytes.Length + hwId.Length];
            Buffer.BlockCopy(passBytes, 0, combinedKeyMaterial, 0, passBytes.Length);
            Buffer.BlockCopy(hwId, 0, combinedKeyMaterial, passBytes.Length, hwId.Length);

            try
            {
                using (var derive = new Rfc2898DeriveBytes(combinedKeyMaterial, salt, Iterations))
                {
                    byte[] keyEnc = derive.GetBytes(KeySize);
                    byte[] keyHmac = derive.GetBytes(KeySize);

                              // Encrypt Data
                    using (var fsOut = new FileStream(tempFile, FileMode.Create))
                    {
                        fsOut.Write(salt, 0, salt.Length);
                        fsOut.Write(iv, 0, iv.Length);

                        using (var aes = Aes.Create())
                        {
                            aes.KeySize = 256; aes.Mode = CipherMode.CBC; aes.Padding = PaddingMode.PKCS7;
                            aes.Key = keyEnc; aes.IV = iv;

                            using (var cs = new CryptoStream(fsOut, aes.CreateEncryptor(), CryptoStreamMode.Write))
                            using (var fsIn = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
                            {
                                fsIn.CopyTo(cs);
                            }
                        }
                    }

                                 // Compute HMAC on the Encrypted File (Encrypt-then-MAC)
                    byte[] signature;
                    using (var hmac = new HMACSHA256(keyHmac))
                    using (var fsRead = new FileStream(tempFile, FileMode.Open, FileAccess.Read))
                    {
                        signature = hmac.ComputeHash(fsRead);
                    }

                                // Append HMAC to end of file
                    using (var fsAppend = new FileStream(tempFile, FileMode.Append))
                    {
                        fsAppend.Write(signature, 0, signature.Length);
                    }
                }
            }
            finally
            {
                Array.Clear(passBytes, 0, passBytes.Length);
                Array.Clear(combinedKeyMaterial, 0, combinedKeyMaterial.Length);
            }

            SecurityUtils.SecureDelete(sourceFile);
            if (File.Exists(finalFile)) File.Delete(finalFile);
            File.Move(tempFile, finalFile);
        }

        public void DecryptFile(string sourceFile, SecureString password)
        {
            if (!sourceFile.EndsWith(Extension)) return;
            string tempFile = sourceFile.Replace(Extension, "");
            string workingFile = sourceFile + ".decoding";

            byte[] passBytes = SecurityUtils.ToByteArray(password);
            byte[] hwId = SecurityUtils.GetHardwareFingerprint();
            byte[] combinedKeyMaterial = new byte[passBytes.Length + hwId.Length];
            Buffer.BlockCopy(passBytes, 0, combinedKeyMaterial, 0, passBytes.Length);
            Buffer.BlockCopy(hwId, 0, combinedKeyMaterial, passBytes.Length, hwId.Length);

            try
            {
                using (var fsIn = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
                {
                    if (fsIn.Length < SaltSize + IvSize + HmacSize) throw new InvalidDataException();

                    byte[] salt = new byte[SaltSize];
                    byte[] iv = new byte[IvSize];
                    fsIn.Read(salt, 0, salt.Length);
                    fsIn.Read(iv, 0, iv.Length);

                    using (var derive = new Rfc2898DeriveBytes(combinedKeyMaterial, salt, Iterations))
                    {
                        byte[] keyEnc = derive.GetBytes(KeySize);
                        byte[] keyHmac = derive.GetBytes(KeySize);

                                  // Validate HMAC
                        long dataLen = fsIn.Length - HmacSize;
                        byte[] storedHmac = new byte[HmacSize];
                        fsIn.Seek(-HmacSize, SeekOrigin.End);
                        fsIn.Read(storedHmac, 0, HmacSize);

                        fsIn.Position = 0; // Reset to beginning
                        byte[] calculatedHmac;
                        using (var hmac = new HMACSHA256(keyHmac))
                        {
                            byte[] buffer = new byte[4096];
                            long remaining = dataLen;
                            while (remaining > 0)
                            {
                                int read = fsIn.Read(buffer, 0, (int)Math.Min(buffer.Length, remaining));
                                hmac.TransformBlock(buffer, 0, read, buffer, 0);
                                remaining -= read;
                            }
                            hmac.TransformFinalBlock(new byte[0], 0, 0);
                            calculatedHmac = hmac.Hash;
                        }

                        if (!SafeEquals(storedHmac, calculatedHmac))
                            throw new CryptographicException("INTEGRITY FAILURE: DATA MODIFIED");

                                     // Perform Decryption
                        fsIn.Position = SaltSize + IvSize; // Skip header
                        using (var aes = Aes.Create())
                        {
                            aes.Key = keyEnc; aes.IV = iv; aes.Mode = CipherMode.CBC; aes.Padding = PaddingMode.PKCS7;
                            using (var fsOut = new FileStream(workingFile, FileMode.Create))
                            using (var cs = new CryptoStream(fsOut, aes.CreateDecryptor(), CryptoStreamMode.Write))
                            {
                                long cipherLen = fsIn.Length - HmacSize - SaltSize - IvSize;
                                long remainingCipher = cipherLen;
                                byte[] buffer = new byte[81920]; // 80kb buffer
                                while (remainingCipher > 0)
                                {
                                    int read = fsIn.Read(buffer, 0, (int)Math.Min(buffer.Length, remainingCipher));
                                    cs.Write(buffer, 0, read);
                                    remainingCipher -= read;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                Array.Clear(passBytes, 0, passBytes.Length);
                Array.Clear(combinedKeyMaterial, 0, combinedKeyMaterial.Length);
            }

            SecurityUtils.SecureDelete(sourceFile);
            if (File.Exists(tempFile)) File.Delete(tempFile);
            File.Move(workingFile, tempFile);
        }

        private byte[] GenerateRandomBytes(int length, byte[] seed)
        {
            byte[] b = new byte[length];
            using (var rng = new RNGCryptoServiceProvider()) rng.GetBytes(b);
                    // Simple XOR mixing of seed
            for (int i = 0; i < length; i++) b[i] ^= seed[i % seed.Length];
            return b;
        }

        private bool SafeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= (a[i] ^ b[i]);
            return diff == 0;
        }
    }
}