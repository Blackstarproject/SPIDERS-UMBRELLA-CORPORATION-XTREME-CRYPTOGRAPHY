using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace SPIDERS_UMBRELLA_CORPORATION.Core
{
    public static class SecurityUtils
    {
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool IsDebuggerPresent();

              // Entropy Pool (Mouse Movements)
        private static readonly byte[] _entropyPool = new byte[32];
        private static int _entropyIndex = 0;

        public static void AddEntropy(int x, int y)
        {
                   // Mix mouse coordinates into the pool
            byte b = (byte)((x ^ y) & 0xFF);
            _entropyPool[_entropyIndex] ^= b;
            _entropyIndex = (_entropyIndex + 1) % _entropyPool.Length;
        }

        public static byte[] GetEntropy()
        {
            using (SHA256 sha = SHA256.Create())
            {
                return sha.ComputeHash(_entropyPool);
            }
        }

               // Hardware Binding (Fingerprint)
        public static byte[] GetHardwareFingerprint()
        {
            string cpuId = "UNKNOWN_CPU";
            string mbId = "UNKNOWN_MB";

            try
            {
                       // Wrapped in try/catch to prevent crashes on systems where WMI is disabled
                using (var mc = new ManagementClass("Win32_Processor"))
                using (var moc = mc.GetInstances())
                    foreach (ManagementObject mo in moc.Cast<ManagementObject>()) { cpuId = mo.Properties["ProcessorId"]?.Value?.ToString() ?? "N/A"; break; }

                using (var mc = new ManagementClass("Win32_BaseBoard"))
                using (var moc = mc.GetInstances())
                    foreach (ManagementObject mo in moc.Cast<ManagementObject>()) { mbId = mo.Properties["SerialNumber"]?.Value?.ToString() ?? "N/A"; break; }
            }
            catch
            {
                        // Fallback if WMI fails, ensures app doesn't crash
                cpuId = "FALLBACK_CPU";
            }

            string rawId = $"{cpuId}::{mbId}::UMBRELLA_CORP";
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(rawId));
        }

                 // Secure File Deletion
        public static void SecureDelete(string filePath)
        {
            if (!File.Exists(filePath)) return;
            try
            {
                FileInfo fi = new FileInfo(filePath);
                if (fi.IsReadOnly) fi.IsReadOnly = false; 

                long length = fi.Length;
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Write, FileShare.None))
                {
                    byte[] buffer = new byte[4096];
                       // Overwrite pass
                    new RNGCryptoServiceProvider().GetBytes(buffer);
                    long written = 0;
                    while (written < length)
                    {
                        int toWrite = (int)Math.Min(buffer.Length, length - written);
                        fs.Write(buffer, 0, toWrite);
                        written += toWrite;
                    }
                    fs.Flush();
                }
                File.Delete(filePath);
            }
            catch
            {
                     // If we can't secure wipe (e.g., file in use), try standard delete
                try { File.Delete(filePath); } catch { }
            }
        }

        public static byte[] ToByteArray(SecureString secureString)
        {
            if (secureString == null) return new byte[0];
            IntPtr unmanagedBytes = IntPtr.Zero;
            try
            {
                int length = secureString.Length;
                     // Unicode is safer and more standard than Ansi for global alloc
                unmanagedBytes = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                byte[] bytes = new byte[length * 2];   // *2 for Unicode
                Marshal.Copy(unmanagedBytes, bytes, 0, bytes.Length);
                return bytes;
            }
            finally
            {
                if (unmanagedBytes != IntPtr.Zero) Marshal.FreeHGlobal(unmanagedBytes);
            }
        }

        public static string BioText(string input) => input;
    }
}