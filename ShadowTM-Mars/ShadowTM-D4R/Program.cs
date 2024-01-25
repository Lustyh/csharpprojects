using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
        static SocketServer socketServer = new SocketServer();

        #region Console_Exit_Event
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(uint sig);
        #endregion

        static void Main(string[] args)
        {
            socketServer.Start();
            Close_Filewall();
            SetConsoleCtrlHandler(new EventHandler(ConsoleExit), true);
            uint port = GetInterfacePort(args);
            Console.WriteLine($"monitor port:{port}");
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
        private static void Close_Filewall()
        {
            for (int i = 0; i < 3; i++)
            {
                if (tRunCmd("netsh", " advfirewall firewall add rule name=\"ShadowTM\" dir=in program=\"" + System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName + "\" action=allow").ToLower().Contains("ok"))
                {
                    break; //netsh advfirewall firewall add rule name="ftp.exe" dir=in program="C:\Windows\System32\ftp.exe" action=allow
                }
                if (tRunCmd("netsh", " advfirewall firewall add rule name=\"Tmfloor_test\" dir=in program=\""+ System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName + "\" action=allow").ToLower().Contains("ok"))
                {
                    break; //netsh advfirewall firewall add rule name="ftp.exe" dir=in program="C:\Windows\System32\ftp.exe" action=allow
                } 
            }
            try
            {
                
            }
            catch (Exception)
            {
                
            }
        }
        public static string tRunCmd(string name, string command)//运行一个cmd命令
        {
            Process p = new Process();
            p.StartInfo.FileName = name;
            p.StartInfo.Arguments = command;    // 执行参数
            p.StartInfo.UseShellExecute = false;        // 关闭Shell的使用
            p.StartInfo.RedirectStandardInput = true;   // 重定向标准输入
            p.StartInfo.RedirectStandardOutput = true;  // 重定向标准输出
            p.StartInfo.RedirectStandardError = true;   // 重定向错误输出
            p.StartInfo.CreateNoWindow = true;          // 设置不显示窗口
            p.Start();   //启动
            //p.WaitForExit();
            //p.StandardInput.WriteLine(command);       // 也可以用这种方式输入要执行的命令
            //p.StandardInput.WriteLine("exit");        // 不过要记得加上Exit，要不然下一行执行的时候会出错
            return p.StandardOutput.ReadToEnd();        // 从输出流取得命令执行结果
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
