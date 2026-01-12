using System;
using System.Diagnostics;
using System.Windows.Forms;
using SPIDERS_UMBRELLA_CORPORATION.Core;
using SPIDERS_UMBRELLA_CORPORATION.UI;

namespace SPIDERS_UMBRELLA_CORPORATION
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Only trigger "Self-Destruct" if we are in RELEASE mode (not Debugging)
#if !DEBUG
            //Anti-Debugging (Immediate Exit if watched)
            if (Debugger.IsAttached || SecurityUtils.IsDebuggerPresent())
            {
                // We use Environment.Exit(0) instead of FailFast to avoid 
                // scaring the CLR into thinking the engine corrupted.
                Environment.Exit(0); 
            }
#endif

            Application.Run(new Form1());
        }
    }
}