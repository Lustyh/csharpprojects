using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShadowTM
{
    class Program
    {
        const uint INVALID_PORT_NUMBER = 99999;
        const string SHADOW_FILE = ".\\Shadow.txt";
        static ProcessCenter Commander = null;

        #region Console_Exit_Event
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(uint sig);
        #endregion

        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(new EventHandler(ConsoleExit), true);
            uint port = GetInterfacePort(args);
            Commander = new ProcessCenter(port);
            while (Commander.IsStopRunning == false)
            {
                // Loop until input is entered.
                while (Console.KeyAvailable == false)
                    Thread.Sleep(50);
                string linex = Console.ReadLine();
                Commander.TMFlow_ReceiveCommand(null, linex);
            }
        }

        public static bool ConsoleExit(uint sig)
        {
            if (sig < 7)
                Commander.Stop();
            return true;
        }

        private static uint GetInterfacePort(string[] args)
        {
            uint port = INVALID_PORT_NUMBER;
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (string.Compare(args[i], "port", true) == 0)
                {
                    if (!uint.TryParse(args[i + 1], out port))
                        port = INVALID_PORT_NUMBER;
                }
            }
            if(port == INVALID_PORT_NUMBER && File.Exists(SHADOW_FILE))
            {
                using (StreamReader stream = new StreamReader(SHADOW_FILE))
                {
                    string line = null;
                    while ((line = stream.ReadLine()) != null)
                    {
                        string[] param = line.Split('=');
                        if (param.Length != 2) continue;
                        string cmd = param[0].Trim();
                        string value = param[1].Trim();
                        if (string.Compare(cmd, "port", true) == 0)
                            uint.TryParse(value, out port);
                    }
                }
            }
            return port;
        }
    }
}
