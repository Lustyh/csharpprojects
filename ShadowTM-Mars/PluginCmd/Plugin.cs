using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PluginCmd
{
    public class Plugin
    {
        private static string ServerIP= "10.18.6.52";
        private static string DBName  = "QMS";
        private static string BU      = "NB4";
        private static string Portal  = "MonitorPortal";
        private static string Station = "SWDL";
        private static string Step    = "querydata";
        private static SqlConnection sf = new SqlConnection();

        private class Initializer
        {
            private const string PLUGINI = ".\\Plugin.ini";
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
            /// <summary>
            /// 读取配置
            /// </summary>
            /// <param name="section"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            public static string ReadItem(string section, string key)
            {
                StringBuilder value = new StringBuilder(255);
                int i = GetPrivateProfileString(section, key, "", value, 255, PLUGINI);
                return value.ToString();
            }
        }

        private class Logger
        {
            private static object locker = new object();
            public static void Record(string type, string log)
            {
                lock (locker)
                {
                    string path = Path.Combine(".\\", "PluginTM-" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                    using (StreamWriter writer = new StreamWriter(path, true))
                    {
                        writer.WriteLine(string.Format("[{0}][{1}]{2}", type, DateTime.Now.ToString("HH:mm:ss:fff"), log));
                    }
                }
            }
        }

        public static bool Init()
        {
            try
            {
                ServerIP= Initializer.ReadItem("DataBase", "ServerIP");
                DBName  = Initializer.ReadItem("DataBase", "DBName");
                BU      = Initializer.ReadItem("DataBase", "BU");
                Portal  = Initializer.ReadItem("DataBase", "Portal");
                Station = Initializer.ReadItem("DataBase", "Station");
                Step    = Initializer.ReadItem("DataBase", "Step");
                string info = string.Format(
                    "ServerIP={0} DBName={1} BU={2} Portal={3} Station={4} Step={5}", 
                    ServerIP, 
                    DBName, 
                    BU, 
                    Portal, 
                    Station, 
                    Step);
                Logger.Record("INT", info);
                Console.WriteLine(info);
                sf.ConnectionString = string.Format(
                    "server={0};Database={1};Uid=sdt;Pwd=SDT#7;Integrated Security=False",
                    ServerIP,
                    DBName
                    );
                Console.WriteLine(sf.ConnectionString);
            }
            catch (Exception x)
            {
                Logger.Record("INT", "ERROR:" + x.Message);
                return false;
            }
            return true;
        }
        public static string ProcessCommand(string param)
        {
            const string help = "Please Send With Format:SN=WIPXXXXXXX";
            string[] args = param.Split(' ');
            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    string[] cmd = args[i].Split('=');
                    if (cmd.Length != 2) return help;
                    if (cmd[0].Trim().Equals("SN"))
                    {
                        string barcode = cmd[1].Trim();
                        if (!barcode.Contains("WIP"))
                            return help;
                        string query = string.Format(
                            "SN={0};$;station=QueryData;$;MonitorAgentVer=VW20151102.01;$;",
                            barcode.Substring(barcode.IndexOf("WIP"))
                            );
                        try
                        {
                            if (sf.State == ConnectionState.Open) sf.Close();
                            sf.Open();
                        }
                        catch (Exception x)
                        {
                            Logger.Record("SQL", "Open Error:" + x.Message);
                            return "Open Sqlconnection Error!";
                        }
                        StringBuilder OutPutStr = new StringBuilder();
                        if (sf.State == ConnectionState.Open)
                        {
                            Console.WriteLine(query);
                            try
                            {
                                SqlCommand sqlcmd = new SqlCommand(Portal, sf);
                                sqlcmd.CommandType = CommandType.StoredProcedure;
                                sqlcmd.Parameters.Add("@BU", SqlDbType.VarChar).Value = BU;
                                sqlcmd.Parameters.Add("@Station", SqlDbType.VarChar).Value = Station;
                                sqlcmd.Parameters.Add("@Step", SqlDbType.VarChar).Value = Step;
                                sqlcmd.Parameters.Add("@InPutStr", SqlDbType.VarChar).Value = query;
                                sqlcmd.Parameters.Add("@OutPutStr", SqlDbType.VarChar, 8000);
                                sqlcmd.Parameters["@BU"].Direction = ParameterDirection.Input;
                                sqlcmd.Parameters["@BU"].DbType = DbType.String;
                                sqlcmd.Parameters["@Station"].Direction = ParameterDirection.Input;
                                sqlcmd.Parameters["@Station"].DbType = DbType.String;
                                sqlcmd.Parameters["@Step"].Direction = ParameterDirection.Input;
                                sqlcmd.Parameters["@Step"].DbType = DbType.String;
                                sqlcmd.Parameters["@InPutStr"].Direction = ParameterDirection.Input;
                                sqlcmd.Parameters["@InPutStr"].DbType = DbType.String;
                                sqlcmd.Parameters["@OutPutStr"].DbType = DbType.String;
                                sqlcmd.Parameters["@OutPutStr"].Direction = ParameterDirection.Output;
                                sqlcmd.ExecuteScalar();
                                OutPutStr.Append(sqlcmd.Parameters["@OutPutStr"].Value.ToString());
                                if (sf.State == ConnectionState.Open) sf.Close();
                                string retstr = OutPutStr.ToString().ToUpper();
                                Console.WriteLine(retstr);
                                Logger.Record("SQL", "OutPut:" + retstr);
                                if (retstr.Contains("SF_CFG_CHK=PASS"))
                                {
                                    string StatusStart = retstr.Substring(retstr.IndexOf("STATUS"));
                                    string Status = StatusStart.Substring(0, StatusStart.IndexOf(";"));
                                    string value = Initializer.ReadItem("STATION", "STATUS_" + Status.Split('=')[1].Trim());
                                    string defau = Initializer.ReadItem("STATION", "STATUS_XX");
                                    //20190221 change return format to barcode&result
                                    return barcode + "+" + ((value.Length > 0) ? value : defau);
                                    //20190221 change return format to barcode&result
                                    //#return (value.Length > 0) ? value : defau;
                                }
                                else
                                {
                                    return barcode + "+FAIL";
                                }
                            }
                            catch (Exception x)
                            {
                                if (sf.State == ConnectionState.Open) sf.Close();
                                Logger.Record("SQL", "Query:" + x.Message);
                            }
                        }
                        else
                        {
                            sf.Close();
                        }
                    }
                }
                catch (Exception x)
                {
                    Logger.Record("ERR", "Process Command Error:" + x.Message);
                }
            }
            return  "Empty";
        }

        public static bool Close()
        {
            Console.WriteLine("Close shopfloor");
            return true;
        }
    }
}
