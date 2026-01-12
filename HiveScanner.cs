using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SPIDERS_UMBRELLA_CORPORATION.Core
{
    public class HiveScanner
    {
        public IEnumerable<string> Scan(string[] rootPaths)
        {
            foreach (var path in rootPaths)
            {
                if (!Directory.Exists(path)) continue;
                IEnumerable<string> files = null;
                try
                {
                    files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                                     .Where(f => !f.EndsWith(".tmp") && !f.EndsWith(".decoding"));
                }
                catch { continue; }

                if (files != null)
                {
                    foreach (var file in files) yield return file;
                }
            }
        }
    }
}