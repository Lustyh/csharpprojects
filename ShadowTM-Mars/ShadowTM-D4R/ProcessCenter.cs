using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShadowTM
{
    class ProcessCenter
    {
        private FlowInterface TMFlow = null;
        private FlowInterface TFFlow = null;
        private Queue<CommandData> QCommands = null;
        public bool IsStopRunning = false;
        
        public class CommandData
        {
            public object sender = null;
            public string command = string.Empty;
            public CommandData(object _sender,string _cmd)
            {
                sender = _sender;
                command = _cmd;
            }
        }
        /// <summary>
        /// Initialize plugin, and start listen port, start process commands
        /// </summary>
        /// <param name="port"></param>
        public ProcessCenter(uint port)
        {   
            QCommands = new Queue<CommandData>();
            TMFlow = new FlowInterface(port);
            //TFFlow = new FlowInterface(502);
            Logger.Record("SYS", "Start in port " + TMFlow.Port);
            //Logger.Record("SYS", "Start in port " + TFFlow.Port);
            TMFlow.event_ReceiveCommand += TMFlow_ReceiveCommand;
            //TFFlow.event_ReceiveCommand += TMFlow_ReceiveCommand;
            TMFlow.Start();
            
            //TFFlow.Start();
            Logger.Record("INT", "Plugin init " + (PluginSF.Init() ? "success" : "fail"));
            UserCommand.Init(PluginSF.Count, PluginSF.Fix_Holder_No, PluginSF.Group_Num, PluginSF.test_mode);
            Task.Run(() => ProcessQCommands());
        }
        /// <summary>
        /// add new request to Queue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="parameter"></param>
        public void TMFlow_ReceiveCommand(object sender, string parameter)
        {
            if (sender == null) Logger.Record("SYS", "Console:" + parameter);
            if (sender as Socket != null) Logger.Record("TCP", "Recived:" + parameter);
            CommandData cmd = new CommandData(sender, parameter);
            QCommands.Enqueue(cmd);
        }
        /// <summary>
        /// process request in Queue, and feedback result
        /// </summary>
        private void ProcessQCommands()
        {
            while (IsStopRunning == false)
            {
                try
                {
                    SpinWait.SpinUntil(() => IsStopRunning || QCommands.Count > 0, -1);
                    if (IsStopRunning) break;
                    CommandData data = QCommands.Dequeue();
                    Logger.Record("RUN", "CMD:" + data.command);
                    Socket _sender = data.sender as Socket;
                    string buffer = UserCommand.ProcessCommand(data.command);
                    if (data.sender as Socket != null)
                    {
                        _sender.Send(Encoding.UTF8.GetBytes(buffer));
                        Logger.Record("TCP", "Send:" + buffer);
                    }
                    else
                    {
                        Console.WriteLine("Result:" + buffer);
                        Logger.Record("SYS", "Result:" + buffer);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Record("ERR", ex.StackTrace);
                }
            }
        }
        /// <summary>
        /// release port, stop listen, and close plugin
        /// </summary>
        public void Stop()
        {
            IsStopRunning = true;
            TMFlow.Stop();
            TFFlow.Stop();
            Logger.Record("SYS", "Plugin stop " + (PluginSF.Close() ? "success" : "fail"));
        }
    }

    class Logger
    {
        private static object locker = new object();
        public static void Record(string type,string log)
        {
            lock (locker)
            {
                string path = Path.Combine(".\\", "ShadowTM-" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(string.Format("[{0}][{1}]{2}", type, DateTime.Now.ToString("HH:mm:ss:fff"), log));
                }
            }
        }
    }
}
