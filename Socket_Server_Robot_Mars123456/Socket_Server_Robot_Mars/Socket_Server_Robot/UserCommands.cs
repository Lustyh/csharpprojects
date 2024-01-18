using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Socket_Server_Robot
{
    public class HolderStatus
    {
        public string IP = "";
        public Object obj = null;
        public string slot = "-";
        public string SN = "";
        public int status;
        public string Fix_ID = "0";
        public int Fail_Count = 0;
        public string Fail_Fix = "";
        public string upload_cli = "OK";
        public bool new_dev = true;
    }

    public class Robot
    {
        public string IP = "";
        public string slot = "-";
        public string SN = "";
        public int status;
        public string Fix_ID = "0";
        public int count = 0;
        public int Fail_Count = 0;
        public string Fail_Fix = "";
    }
    public class UserCommands
    {
        public List<Robot> Robots = new List<Robot>();
        public static int fix_Num = 1;
        public static int fix_count = 1;
        public static string test_type = "";
        public List<HolderStatus> holders = new List<HolderStatus>();

        public delegate bool UploadData(int row, int cell, string txt);
        public UploadData uploadData;
        public delegate void PrintMes(string Type, string Txt);
        public PrintMes printMes;
        public delegate void SendToClifford(string send_mes, string ip, object obj);
        public SendToClifford sendToClifford;
        /// <summary>
        /// 初始化列表,打开程序时定义列表长度和进行初始化
        /// </summary>
        /// <param name="num">治具数</param>
        /// <param name="fix_holder">每台治具的holder数</param>
        /// <param name="test_Mode">测试模式{single_up, async_test, multiple_up}</param>
        public void Init(int num, int fix_holder, string test_Mode)
        {
            fix_count = num;
            fix_Num = fix_holder;
            test_type = test_Mode;
            for (int i = 0; i < num * fix_holder; i++)
            {
                holders.Add(new HolderStatus());
            }
            for (int i = 0; i < 2; i++)
            {
                Robots.Add(new Robot());
            }
        }

        /// <summary>
        /// 处理Client端发送的信息并回传信息给client端
        /// </summary>
        /// <param name="param">client发送的信息</param>
        /// <param name="IP">Client端的IP及其端口</param>
        /// <param name="socket">socket 对象</param>
        /// <returns>回传给client端的信息</returns>
        public string ProcessCommand(string param, string IP = "", Object socket = null)
        {
            try
            {
                //判断是robot发送的信息还是clifford发送的信息  true为clifford   false为robot
                if (param.Contains("^") && param.Contains("{"))
                {
                    param = param.Replace("^", "");
                    JObject jObject = JObject.Parse(param);
                    string message_id = jObject.GetValue("message_id").ToString().ToLower();
                    if (Socket_Server.test_environment.ToUpper() == "MARS" && message_id != "ack")
                    {
                        Ack_Mars(jObject, message_id, (Socket)socket);
                        Thread.Sleep(300);
                    }
                    switch (message_id)
                    {
                        case "ack":
                            return Ack(jObject);
                        case "test_complete":
                            return Test_Complete(param);
                        case "error":
                            return Error_handing(jObject);
                        case "online":
                            return Online(jObject, IP, socket);
                        case "station_online":
                            return Online(jObject, IP, socket);
                        case "ready":
                            return Ready(jObject, IP, socket);
                        case "station_ready":
                            return Ready(jObject, IP, socket);
                        default:
                            return "null";
                    }
                }
                else
                {
                    param = param.ToUpper();
                    int num = holders.Count();
                    string req_type = GetKeyValue(param, "TYPE");
                    string SN = GetKeyValue(param, "SN");
                    string result = GetKeyValue(param, "RESULT");
                    if (SN.Length>5)
                    {
                        if (SN.Substring(0,4)=="BWIP")
                        {
                            SN = SN.Substring(1);
                        }
                    }
                    switch (req_type)
                    {
                        case "SFC":
                            return CheckStation(SN);
                        case "RESET":
                            return RESET();
                        case "STATUS":
                            return Status(param, num);
                        case "START":
                            int fix = int.Parse(GetKeyValue(param, "FIX"));
                            return Robot_Start(SN, fix);
                        case "RESULT":
                            return Status(param, num);
                        case "UPLOAD":
                            return Upload_status(int.Parse(GetKeyValue(param, "FIX")), int.Parse(GetKeyValue(param, "STATUS")));
                        case "UPLOAD1":
                            return Upload_failcount(SN,int.Parse(GetKeyValue(param, "FIX")), int.Parse(GetKeyValue(param, "FCOUNT")));
                        default:
                            return "No request Type";
                    }

                    //初始化某个fix的值
                    /*if (param.ToUpper().Contains("TYPE=RESET"))
                    {
                        for (int i = 0; i < holders.Count; i++)
                        {
                            holders[i].slot = "-";
                            holders[i].SN = "";
                            holders[i].status = 0;
                            holders[i].Fail_Fix = "";
                            holders[i].Fail_Count = 0;
                            holders[i].upload_cli = "OK";
                            holders[i].new_dev = true;

                        }
                    }*/
                    //检查站别状态
                    /*if (param.ToUpper().Contains("TYPE=SFC"))
                    {
                        string[] args = param.Split(';');
                        string SN = args[1].Substring(3);
                        string retstr = Shopfloor.QueryData(SN, Socket_Server.Station);
                        if (retstr.Contains("SF_CFG_CHK=PASS"))
                        {
                            string StatusStart = retstr.Substring(retstr.IndexOf("STATUS"));
                            string Status = Get_str("SET STATUS=", ";$;", retstr);
                            string test_count = Get_str("FailCount=", ";$;", retstr);
                            string fix_id_1 = Get_str("1FixtureID=", ";$;", retstr);
                            Robots[0].SN = SN;
                            if (test_count == "")
                            {
                                Robots[0].Fail_Count = 0;
                                Robots[0].Fail_Fix = "";
                            }
                            else
                            {
                                Robots[0].Fail_Count = int.Parse(test_count);
                                Robots[0].Fail_Fix = fix_id_1;
                            }
                            return SN + "+" + ((Status.Length > 0) ? Status : Socket_Server.defau);
                        }
                        else
                        {
                            return SN + "+FAIL";
                        }
                    }
                    //Robot给 ready 信号
                    if (param.Contains("TYPE=START"))
                    {
                        string[] args = param.Split(';');
                        int hold_id = int.Parse(args[2].Substring(4));
                        string SN = args[1].Substring(3);
                        if (Socket_Server.auto_fill == "Y")
                        {
                            Auto_Fill_SN(SN, hold_id);
                        }
                        else
                        {
                            holders[hold_id - 1].SN = SN;
                            holders[hold_id - 1].status = 0;
                            holders[hold_id - 1].Fail_Count = Robots[0].Fail_Count;
                            holders[hold_id - 1].Fail_Fix = Robots[0].Fail_Fix;
                            holders[hold_id - 1].upload_cli = "NG";
                            holders[hold_id - 1].new_dev = true;
                        }
                        test_start(hold_id, socket);
                        return "PASS";
                    }*/
                    //查询holder的状态  一个或者所有
                    /*if (param.Contains("TYPE=STATUS") || param.Contains("TYPE=RESULT"))
                    {
                        if (param.Contains("FIX=ALL"))
                        {
                            string all = string.Empty;
                            for (int i = 0; i < num; i++)
                            {
                                all += holders[i].status.ToString();
                            }
                            return all.Trim();
                        }
                        else
                        {
                            string[] args = param.Split(';');
                            return holders[int.Parse(args[1].Substring(4)) - 1].status.ToString();
                        }
                    }
                    //查询所有holder的状态
                    if (param.Contains("TYPE=GET"))
                    {
                        string Response = "";
                        for (int i = 0; i < holders.Count; i++)
                        {
                            Response += string.Format("No={0};IP={1};slot={2};SN={3};status={4};Fix_ID={5};Fail_Count={6};Fail_Fix={7};upload_cli={8};new_dev={9}\r\n",
                                i.ToString(), holders[i].IP, holders[i].slot, holders[i].SN, holders[i].status.ToString(), holders[i].Fix_ID, holders[i].Fail_Count.ToString(),
                                holders[i].Fail_Fix, holders[i].upload_cli, holders[i].new_dev.ToString());
                        }
                        return Response;
                    }
                    return "null";*/
                }
            }
            catch (Exception x)
            {
                return param + "\nError:" + x.Message;
            }
        }
        public string Upload_status(int fixture_no, int holder_status)
        {
            if (Socket_Server.auto_fill=="Y")
            {
                for (int i = 0; i < fix_Num; i++)
                {
                    holders[(fixture_no - 1) * fix_Num + i].status = holder_status;
                    uploadData((fixture_no - 1) * fix_Num + i + 3, 4, holder_status.ToString());
                }
            }
            else
            {
                holders[fixture_no - 1].status = holder_status;
                uploadData(fixture_no + 2, 4, holder_status.ToString());
            }
            return "OK";
        }
        public string Upload_failcount(string SN,int fixture_no, int holder_failcount)
        {
            if (holders[fixture_no - 1].SN == SN)
            {
                holders[fixture_no - 1].Fail_Count = holder_failcount;
                uploadData(fixture_no + 2, 5, holder_failcount.ToString());
            }
            return "OK";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RequestStr"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public string Status(string RequestStr, int num)
        {
            if (RequestStr.Contains("FIX=ALL"))
            {
                string all = string.Empty;
                for (int i = 0; i < num; i++)
                {
                    all += holders[i].status.ToString();
                }
                return all.Trim();
            }
            else
            {
                string[] args = RequestStr.Split(';');
                return holders[int.Parse(args[1].Substring(4)) - 1].status.ToString();
            }
        }
        /// <summary>
        /// reset holders status
        /// </summary>
        /// <returns></returns>
        public string RESET()
        {
            for (int i = 0; i < holders.Count; i++)
            {
                holders[i].slot = "-";
                holders[i].SN = "";
                holders[i].status = 0;
                holders[i].Fix_ID = "";
                holders[i].Fail_Fix = "";
                holders[i].Fail_Count = 0;
                holders[i].upload_cli = "OK";
                holders[i].new_dev = true;
            }
            return "PASS";
        }
        /// <summary>
        /// 解析手臂信息，获取value
        /// </summary>
        /// <param name="Str">原始信息</param>
        /// <param name="Key">Key</param>
        /// <returns>key的value</returns>
        public string GetKeyValue(string Str, string Key)
        {
            try
            {
                string[] strlist = Str.Split(';');
                foreach (var item in strlist)
                {
                    if (item.Contains(string.Format("{0}=", Key)))
                    {
                        return item.Substring(Key.Length + 1);
                    }
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }
        
        /// <summary>
        /// 自动补全SN,只要用于SMT传入一个SN,自动补全后面的SN,16进制增加.
        /// </summary>
        /// <param name="first_SN">panel板上第一个SN</param>
        /// <param name="hold_id">fix_num</param>
        /// <returns>PASS or FAIL</returns>
        public string Auto_Fill_SN(string first_SN, int hold_id)
        {
            if (first_SN.Length < 5)
            {
                return "FAIL";
            }
            else
            {
                string befor_5 = first_SN.Substring(0, first_SN.Length - 5);
                string last_5 = first_SN.Substring(first_SN.Length - 5, 5);
                try
                {
                    for (int m = 0; m < fix_Num; m++)
                    {
                        string A = Convert.ToString(Convert.ToInt32(last_5, 16) + m, 16);
                        string B = "";
                        for (int i = 0; i < 5 - A.Length; i++)
                        { B += "0"; }
                        B += A.ToUpper();
                        if (B.Length != 5)
                        { return "FAIL"; }
                        holders[hold_id - 1 + m].SN = befor_5 + B;
                        holders[hold_id - 1 + m].status = 0;
                        holders[hold_id - 1 + m].upload_cli = "NG";
                        holders[hold_id - 1 + m].new_dev = true;
                    }
                }
                catch (Exception)
                {

                    //SocketServer.save_log("Auto_Fill_SN", ex.Message);
                }
            }
            return "PASS";
        }
        /// <summary>
        /// 检查SF站别信息
        /// </summary>
        /// <param name="SN">device serial</param>
        /// <returns>Check Result</returns>
        public string CheckStation(string SN)
        {
            string retstr = Shopfloor.QueryData(SN, Socket_Server.SFStation);
            // printMes("DEBUG", retstr);
            if (retstr.Contains("SF_CFG_CHK=PASS"))
            {
                string Status = Get_str("SET STATUS=", ";$;", retstr);
                string test_count = Get_str("FailCount=", ";$;", retstr);
                string fix_id_1 = Get_str("1FixtureID=", ";$;", retstr);
                if (Status == "9K")
                {
                    Robots[0].SN = SN;
                    uploadData(0, 1, SN);
                    return SN + "+PASS";
                }
                foreach (string status in Socket_Server.defau.Split(';'))
                {
                    if (status == Status)
                    {
                        return SN + "+PASS1";
                    }
                }
                if (retstr.Contains("SF_Routing_CHK=PASS"))
                {
                    Robots[0].SN = SN;
                    uploadData(0, 1, SN);
                    if (test_count == "")
                    {
                        Robots[0].Fail_Count = 0;
                        uploadData(0, 5, "0");
                        Robots[0].Fail_Fix = "";
                        uploadData(0, 6, "");
                    }
                    else
                    {
                        Robots[0].Fail_Count = int.Parse(test_count);
                        uploadData(0, 5, test_count);
                        Robots[0].Fail_Fix = fix_id_1;
                        uploadData(0, 6, fix_id_1);
                    }
                    return SN + "+PASS";
                }
                return SN + "+FAIL+" + Status;
            }
            else
            {
                return SN + "+FAIL";
            }
            
        }
        /// <summary>
        /// robot send start test to server controller
        /// </summary>
        /// <param name="SN">device serial</param>
        /// <param name="fixture_no">fixrure no.</param>
        /// <returns></returns>
        public string Robot_Start(string SN, int fixture_no)
        {
            if (SN.Length > 5)
            {
                if (SN.Substring(0,4)=="BWIP")
                {
                    SN = SN.Substring(1);
                }
            }
            switch (Socket_Server.test_mode)
            {
                case "single_up":
                    return single_up(SN, fixture_no);
                case "multiple_up":
                    return multiple_up(SN, fixture_no);
                case "async_test":
                    return async_test(SN, fixture_no);
                default:
                    break;
            }
            return "FAIL";
        }
        public string single_up(string SN, int fixture_no)
        {
            try
            {
                new Thread(() =>
                {
                    int FailCount = 0;
                    holders[fixture_no - 1].SN = SN;
                    uploadData(fixture_no + 2, 1, SN);
                    holders[fixture_no - 1].status = 0;
                    uploadData(fixture_no + 2, 4, "0");
                    holders[fixture_no - 1].upload_cli = "NG";
                    uploadData(fixture_no + 2, 7, "NG");
                    if (!Socket_Server.Send_SN) { SN = ""; }
                    string response = "{\"message_id\": \"test_start\", \"test_type\": \"single_up\", \"slot_id\": " + holders[fixture_no - 1].slot.ToString() + ", \"dut_id\": \"" + SN + "\"}^";
                    if (Socket_Server.test_environment.ToUpper() == "MARS")
                    {
                        response = "{\"message_id\": \"test_start\",\"station_id\": \"" + holders[fixture_no - 1].Fix_ID + "\", \"group_id\": " + holders[fixture_no - 1].slot.ToString() + ", \"dut_id\": [\"" + SN + "\"]}^";
                    }
                    sendToClifford(response, holders[fixture_no - 1].IP, holders[fixture_no - 1].obj);

                    if (SN != "WIP12345678")
                    {
                        string SFCInfo = Shopfloor.QueryData(SN, Socket_Server.Station);
                        //int FailCount = 0;
                        try
                        {
                            FailCount = int.Parse(Get_str("FailCount=", ";$;", SFCInfo));
                        }
                        catch (Exception)
                        {
                        }
                    }
                    holders[fixture_no - 1].Fail_Count = FailCount;
                    uploadData(fixture_no + 2, 5, FailCount.ToString());
                }).Start();

                return "PASS";
            }
            catch (Exception ee)
            {
                return "458:" + ee.Message;
            }
        }
        public string multiple_up(string SN, int fixture_no)
        {
            string list_sn = "[";
            int hodler_No = fixture_no % fix_Num;
            int fix_No = fixture_no / fix_Num;
            if (Socket_Server.auto_fill == "Y")
            {
                string befor_5 = SN.Substring(0, SN.Length - 5);
                string last_5 = SN.Substring(SN.Length - 5, 5);
                
                try
                {
                    for (int m = 0; m < fix_Num; m++)
                    {
                        string A = Convert.ToString(Convert.ToInt32(last_5, 16) + m, 16);
                        string B = "";
                        for (int i = 0; i < 5 - A.Length; i++)
                        { B += "0"; }
                        B += A.ToUpper();
                        if (B.Length != 5)
                        { return "FAIL"; }
                        holders[(fixture_no - 1) * fix_Num + m].SN = befor_5 + B;
                        uploadData((fixture_no - 1) * fix_Num + m + 3, 1, befor_5 + B);
                        holders[(fixture_no - 1) * fix_Num + m].status = 0;
                        uploadData((fixture_no - 1) * fix_Num + m + 3, 4, "0");
                        holders[(fixture_no - 1) * fix_Num + m].upload_cli = "NG";
                        uploadData((fixture_no - 1) * fix_Num + m + 3, 7, "NG");
                        holders[((fixture_no - 1) - 1) * fix_Num + m].new_dev = true;
                        if (!Socket_Server.Send_SN)
                        {
                            list_sn += "\"\", ";
                        }
                        else
                        {
                            list_sn += "\"" + befor_5 + B + "\", ";
                        }
                    }
                    /*if (!Socket_Server.Send_SN)
                    {
                        for (int i = 0; i < fix_Num; i++)
                        {
                            list_sn += "\"\", ";
                        }
                    }*/
                    list_sn = list_sn.Substring(0, list_sn.Length - 2) + "]";
                    string response = "{\"message_id\": \"test_start\", \"test_type\": \"multiple_up\", \"slot_id\": " + "0" + ", \"dut_id\": " + list_sn + "}^";
                    if (Socket_Server.test_environment.ToUpper() == "MARS")
                    {
                        response = "{\"message_id\": \"test_start\",\"station_id\": \"" + holders[(fixture_no - 1) * fix_Num].Fix_ID + "\", \"group_id\": " + holders[(fixture_no - 1) * fix_Num].slot.ToString() + ", \"dut_id\": " + list_sn + "}^";
                    }
                    sendToClifford(response, holders[(fixture_no - 1) * fix_Num].IP, holders[(fixture_no - 1) * fix_Num].obj);
                }
                catch (Exception ex)
                {
                    printMes("multiple_up function1", ex.Message);
                    //SocketServer.save_log("Auto_Fill_SN", ex.Message);
                }
            }
            else
            {
                new Thread(() =>
                {
                    try
                    {
                        int FailCount = 0;
                        
                        holders[fix_No * fix_Num + hodler_No - 1].SN = SN;
                        uploadData(fix_No * fix_Num + hodler_No + 2, 1, SN);
                        holders[fix_No * fix_Num + hodler_No - 1].status = 0;
                        uploadData(fix_No * fix_Num + hodler_No + 2, 4, "0");
                        holders[fix_No * fix_Num + hodler_No - 1].upload_cli = "NG";
                        uploadData(fix_No * fix_Num + hodler_No + 2, 7, "NG");
                        holders[fix_No * fix_Num + hodler_No - 1].new_dev = true;
                        for (int i = 0; i < Socket_Server.Count; i++)
                        {
                            bool start_upload = true;
                            for (int m = 0; m < fix_Num - 1; m++)
                            {
                                if (Socket_Server.RetestRule == "AAB")
                                {
                                    if ((holders[i * fix_Num + m].Fix_ID != holders[i * fix_Num + m + 1].Fix_ID) ||
                                        (holders[i * fix_Num + m].status != 0 && holders[i * fix_Num + m].status != 1) ||
                                        (holders[i * fix_Num + m + 1].status != 0 && holders[i * fix_Num + m + 1].status != 1) ||
                                        holders[i * fix_Num + m].SN == "" || holders[i * fix_Num + m + 1].SN == "")
                                    {
                                        start_upload = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    if ((holders[i * fix_Num + m].Fix_ID != holders[i * fix_Num + m + 1].Fix_ID) ||
                                        holders[i * fix_Num + m].status != 0 || holders[i * fix_Num + m + 1].status != 0 ||
                                        holders[i * fix_Num + m].SN == "" || holders[i * fix_Num + m + 1].SN == "")
                                    {
                                        start_upload = false;
                                        break;
                                    }
                                }
                            }
                            if (start_upload)
                            {
                                for (int n = 0; n < fix_Num; n++)
                                {
                                    if (!Socket_Server.Send_SN)
                                    {
                                        list_sn += "\"\", ";
                                    }
                                    else
                                    {
                                        list_sn += "\"" + holders[i * fix_Num + n].SN + "\", ";
                                    }
                                }
                                /*if (!Socket_Server.Send_SN)
                                {
                                    for (int m = 0; m < fix_Num; m++)
                                    {
                                        list_sn += "\"\", ";
                                    }
                                }*/
                                list_sn = list_sn.Substring(0, list_sn.Length - 2) + "]";
                                string response = "{\"message_id\": \"test_start\", \"test_type\": \"multiple_up\", \"slot_id\": " + "0" + ", \"dut_id\": " + list_sn + "}^";
                                if (Socket_Server.test_environment.ToUpper() == "MARS")
                                {
                                    response = "{\"message_id\": \"test_start\",\"station_id\": \"" + holders[i * fix_Num].Fix_ID + "\", \"group_id\": " + holders[i * fix_Num].slot.ToString() + ", \"dut_id\": " + list_sn + "}^";
                                }
                                sendToClifford(response, holders[i * fix_Num].IP, holders[i * fix_Num].obj);
                            }
                            if (!SN.Contains("WIP12345678"))
                            {
                                string SFCInfo = Shopfloor.QueryData(SN, Socket_Server.Station);

                                try
                                {
                                    FailCount = int.Parse(Get_str("FailCount=", ";$;", SFCInfo));
                                }
                                catch (Exception)
                                {
                                }
                            }
                            holders[fix_No * fix_Num + hodler_No - 1].Fail_Count = FailCount;
                            uploadData(fix_No * fix_Num + hodler_No + 2, 5, FailCount.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        printMes("multiple_up function2", ex.Message);
                    }
                }).Start();
            }
            return "PASS";
        }

        public string async_test(string SN, int fixture_no)
        {
            int FailCount = 0;
            if (holders[0].SN == SN)
            {
                FailCount = holders[0].Fail_Count;
            }

            new Thread(() => {
                int hodler_No = (fixture_no - 1) % fix_Num;
                int fix_No = (fixture_no - 1) / fix_Num;
                holders[fix_No * fix_Num + hodler_No].SN = SN;
                uploadData(fix_No * fix_Num + hodler_No + 3, 1, SN);
                holders[fix_No * fix_Num + hodler_No].status = 0;
                uploadData(fix_No * fix_Num + hodler_No + 3, 4, "0");
                holders[fix_No * fix_Num + hodler_No].upload_cli = "NG";
                uploadData(fix_No * fix_Num + hodler_No + 3, 7, "NG");

                holders[fix_No * fix_Num + hodler_No].new_dev = true;
                if (!Socket_Server.Send_SN) { SN = ""; }
                string response = "{\"message_id\": \"test_start\", \"test_type\": \"async_test\", \"slot_id\": " + hodler_No.ToString() + ", \"dut_id\": \"" + SN + "\"}^";
                if (Socket_Server.test_environment.ToUpper() == "MARS")
                {
                    response = "{\"message_id\": \"test_start\",\"station_id\": \"" + holders[fix_No * fix_Num + hodler_No].Fix_ID + "\", \"group_id\": " + holders[fix_No * fix_Num + hodler_No].slot.ToString() + ", \"dut_id\": [\"" + SN + "\"]}^";
                }
                sendToClifford(response, holders[fix_No * fix_Num].IP, holders[fix_No * fix_Num].obj);

                if (!SN.Contains("WIP12345678"))
                {
                    string SFCInfo = Shopfloor.QueryData(SN, Socket_Server.Station);
                    try
                    {
                        FailCount = int.Parse(Get_str("FailCount=", ";$;", SFCInfo));
                    }
                    catch (Exception)
                    {
                    }
                }
                holders[fix_No * fix_Num + hodler_No].Fail_Count = FailCount;
                uploadData(fix_No * fix_Num + hodler_No + 3, 5, FailCount.ToString());
            }).Start();
            return "PASS";
        }

        /// <summary>
        /// 生成test_start字符串,并发送
        /// </summary>
        /// <param name="Holder_id">holder 的编号</param>
        /// <param name="socket">socket</param>
        public void test_start(int Holder_id, Object socket)
        {
            string list_sn = "";
            string station_id = holders[Holder_id - 1].Fix_ID;
            if (test_type.ToLower() == "multiple_up")
            {
                List<string> dut_list = new List<string>();
                dut_list.Clear();
                list_sn += "[";
                for (int i = 0; i < holders.Count; i++)
                {
                    if (holders[i].Fix_ID == station_id && holders[i].new_dev)
                    {
                        dut_list.Add(holders[i].SN);
                        //Console.WriteLine(holders[i].SN);
                    }
                }
                if (dut_list.Count != fix_Num)
                { return; }
                else
                {
                    for (int i = 0; i < dut_list.Count; i++)
                    {
                        list_sn += "\"" + dut_list[i] + "\", ";
                    }
                    list_sn = list_sn.Substring(0, list_sn.Length - 2) + "]";
                }
            }
            else
            {
                list_sn = "\"" + holders[Holder_id - 1].SN + "\"";
            }
            string response = "{\"message_id\": \"test_start\", \"test_type\": \"" + test_type + "\", \"slot_id\": " + holders[Holder_id - 1].slot.ToString() + ", \"dut_id\": " + list_sn + "}^";
            //SocketServer.Send(response, holders[Holder_id - 1].IP, holders[Holder_id - 1].obj);
        }

        /// <summary>
        /// online API  From Clifford to central controller
        /// </summary>
        /// <param name="jObject">request connect for json type </param>
        /// <param name="IP">clent ip</param>
        /// <param name="socket"></param>
        /// <returns></returns>
        public string Online(JObject jObject, string IP, Object socket)
        {
            string station_id = jObject.GetValue("station_id").ToString();
            try
            {
                if (IP.Contains(":"))
                {
                    IP = IP.Split(':')[0];
                }
                int fix_no = (int.Parse(station_id.Substring(station_id.Length - 1, 1)) - 1) % fix_count;
                for (int i = 0; i < fix_Num; i++)
                {
                    holders[fix_no * fix_Num + i].Fix_ID = station_id;
                    uploadData(fix_no * fix_Num + i + 3, 2, station_id);
                    holders[fix_no * fix_Num + i].IP = IP;
                    uploadData(fix_no * fix_Num + i + 3, 0, IP);
                    holders[fix_no * fix_Num + i].slot = i.ToString();
                    uploadData(fix_no * fix_Num + i + 3, 3, i.ToString());
                    holders[fix_no * fix_Num + i].status = 0;
                    uploadData(fix_no * fix_Num + i + 3, 4, "0");
                    holders[fix_no * fix_Num + i].SN = "";
                    uploadData(fix_no * fix_Num + i + 3, 1, "");
                    holders[fix_no * fix_Num + i].Fail_Fix = "";
                    uploadData(fix_no * fix_Num + i + 3, 6, "");
                    holders[fix_no * fix_Num + i].Fail_Count = 0;
                    uploadData(fix_no * fix_Num + i + 3, 5, "0");
                    holders[fix_no * fix_Num + i].upload_cli = "ready";
                    uploadData(fix_no * fix_Num + i + 3, 7, "ready");
                    holders[fix_no * fix_Num + i].obj = socket;
                    holders[fix_no * fix_Num + i].new_dev = false;
                }
                if (Socket_Server.test_environment.ToUpper() == "MARS")
                {
                    return "{\"message_id\": \"station_verify_result\",\"station_id\": \"" + station_id + "\", \"result\": \"OK\"}^";
                }
                return "{\"message_id\": \"ack\",  \"ack_id\": \"" + "online" + "\",\"station_id\": \"" + station_id + "\", \"result\": \"OK\"}^";    // "{ \"result\": \"PASS\", \"message\": \"OK\"}^";
            }
            catch (Exception)
            {
                if (Socket_Server.test_environment.ToUpper() == "MARS")
                {
                    return "{\"message_id\": \"station_verify_result\",\"station_id\": \"" + station_id + "\", \"result\": \"NG\"}^";
                }
                return "{\"message_id\": \"ack\",  \"ack_id\": \"" + "online" + "\",\"station_id\": \"" + station_id + "\", \"result\": \"NG\"}^";
            }
        }



        /// <summary>
        /// Clifford
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        public string Error_handing(JObject jObject)
        {
            string error_code = jObject.GetValue("error_code").ToString();
            string station_id = jObject.GetValue("station_id").ToString();
            string dut_slot = jObject.GetValue("slot_id").ToString().ToLower();
            try
            {
                switch (error_code)
                {
                    case "test_start":
                        break;
                    case "get_info":
                        break;
                    case "sfc_connect":
                        for (int index = 0; index < holders.Count; ++index)
                        {
                            if (holders[index].Fix_ID == station_id && holders[index].slot == dut_slot)
                            {
                                holders[index].upload_cli = "ready";
                                int num1 = uploadData(index + 3, 7, "ready") ? 1 : 0;
                                holders[index].status = 4;
                                int num2 = uploadData(index + 3, 4, "4") ? 1 : 0;
                                break;
                            }
                        }
                        break;
                    case "dup_sn":
                        break;
                    case "sfc_connectfinal":
                        break;
                    case "cmd_validate":
                        break;
                    case "auto_test":
                        break;
                    default:
                        break;
                }
                return "{\"message_id\": \"ack\",  \"ack_id\": \"error\",\"station_id\": \"" + station_id + "\", \"result\": \"OK\", \"slot_id\": " + dut_slot + "}^";
            }
            catch (Exception)
            {
                return "{\"message_id\": \"ack\",  \"ack_id\": \"error\",\"station_id\": \"" + station_id + "\", \"result\": \"NG\", \"slot_id\": " + dut_slot + "}^";
            }
        }
        /// <summary>
        /// 处理ack API的字符
        /// </summary>
        /// <param name="param">ack的字符串</param>
        /// <returns>'n/a'</returns>
        public string Ack(JObject jObject)
        {
            try
            {
                /*if (Socket_Server.test_environment.ToUpper() == "MARS")
                {
                    return "n/a";
                }*/
                string station_id = jObject.GetValue("station_id").ToString();
                string ack_id = jObject.GetValue("ack_id").ToString().ToLower();
                //string result = jObject.GetValue("result").ToString();
                switch (ack_id)
                {
                    case "test_start":
                        switch (Socket_Server.test_mode)
                        {
                            case "single_up":
                                for (int i = 0; i < holders.Count; i++)
                                {
                                    if (holders[i].Fix_ID==station_id)
                                    {
                                        holders[i].upload_cli = "Testing";
                                        uploadData(i + 3, 7, "Testing");
                                        holders[i].status = 5;
                                        uploadData(i + 3, 4, "5");
                                        break;
                                    }
                                }
                                break;
                            case "multiple_up":
                                for (int i = 0; i < holders.Count; i++)
                                {
                                    if (holders[i].Fix_ID == station_id)
                                    {
                                        holders[i].upload_cli = "Testing";
                                        uploadData(i + 3, 7, "Testing");
                                        holders[i].status = 5;
                                        uploadData(i + 3, 4, "5");
                                    }
                                }
                                break;
                            case "async_test":
                                string dut_slot = null;
                                if (Socket_Server.test_environment.ToUpper() == "MARS")
                                {
                                    dut_slot = jObject.GetValue("group_id").ToString().ToLower();
                                }
                                else
                                {
                                    dut_slot = jObject.GetValue("slot_id").ToString().ToLower();
                                }
                                for (int i = 0; i < holders.Count; i++)
                                {
                                    if (holders[i].Fix_ID == station_id && holders[i].slot == dut_slot)
                                    {
                                        holders[i].upload_cli = "Testing";
                                        uploadData(i + 3, 7, "Testing");
                                        holders[i].status = 5;
                                        uploadData(i + 3, 4, "5");
                                        break;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "station_verify":
                        break;
                    default:
                        break;
                }
                return "n/a";
            }
            catch (Exception ack_error)
            {
                printMes("ERR", ack_error.Message);
                return "n/a";
            }
        }
        /// <summary>
        /// 处理 Test_Complete API的数据
        /// </summary>
        /// <param name="param">Test_Complete的字符串</param>
        /// <returns>Test_Complete处理的结果 需要回传给client端的字符串</returns>
        public string Test_Complete(string param)
        {
            JObject jObject = JObject.Parse(param);
            string station_id = jObject.GetValue("station_id").ToString();
            string slot_id = null;
            string group_id = "";
            try
            {
                string dut_id = jObject.GetValue("dut_id").ToString();

                string test_result=null;
                if (Socket_Server.test_environment.ToUpper() == "MARS")
                {
                    test_result = jObject.GetValue("result").ToString();
                    slot_id = jObject.GetValue("group_id").ToString();
                    // slot_id = jObject.GetValue("slot_id").ToString();
                }
                else
                {
                    test_result = jObject.GetValue("test_result").ToString();
                    slot_id = jObject.GetValue("slot_id").ToString();
                }
                for (int i = 0; i < holders.Count; i++)
                {
                    if (station_id == holders[i].Fix_ID && (dut_id == holders[i].SN || dut_id == "") && slot_id == holders[i].slot)
                    //if (station_id == holders[i].Fix_ID && slot_id == holders[i].slot && dut_id == holders[i].SN)
                    {
                        if (test_result.ToUpper() == "FAIL" || test_result.ToUpper() == "ERROR" || test_result.ToUpper() == "TIMEOUT" || test_result.ToUpper() == "FAIL_STOP" || 
                            test_result.ToUpper() == "NO_SN" || test_result.ToUpper() == "SFC_GET_INFO_FAIL" || test_result.ToUpper() == "SFC_CONNECT_FAIL" || test_result.ToUpper() == "SFC_CONNECT_FINAL_FAIL")
                        {
                            switch (holders[i].Fail_Count)
                            {
                                case 0:
                                    holders[i].status = 1;
                                    uploadData(i + 3, 4, "1");
                                    break;
                                case 1:
                                    holders[i].status = 2;
                                    uploadData(i + 3, 4, "2");
                                    break;
                                case 2:
                                    holders[i].status = 3;
                                    uploadData(i + 3, 4, "3");
                                    break;
                                default:
                                    holders[i].status = 3;
                                    uploadData(i + 3, 4, "3");
                                    break;
                            }
                        }
                        else
                        {
                            holders[i].status = 4;
                            uploadData(i + 3, 4, "4");
                        }
                        holders[i].upload_cli = "ready";
                        uploadData(i + 3, 7, "ready");
                    }
                }
                if (Socket_Server.test_environment.ToUpper() == "MARS")
                {
                    return "n/a";
                }
                return "{\"message_id\": \"ack\",  \"ack_id\": \"test_complete\",\"station_id\": \"" + station_id + "\", \"result\": \"OK\", \"slot_id\": " + slot_id + "}^";
            }
            catch (Exception)
            {
                if (Socket_Server.test_environment.ToUpper() == "MARS")
                {
                    return "n/a";
                }
                return "{\"message_id\": \"ack\",  \"ack_id\": \"test_complete\",\"station_id\": \"" + station_id + "\", \"result\": \"NG\", \"slot_id\": " + slot_id + "}^";
            }
        }
        /// <summary>
        /// 处理ready API的数据
        /// </summary>
        /// <param name="param">ready API的字符串</param>
        /// <param name="IP">Client端的IP</param>
        /// <param name="socket">socket</param>
        /// <returns>ready处理的结果 需要回传给client端的字符串</returns>
        public string Ready(JObject jObject, string IP, Object socket)
        {
            //JObject jObject = JObject.Parse(param);
            string station_id = jObject.GetValue("station_id").ToString();
            try
            {
                if (IP.Contains(":"))
                {
                    IP = IP.Split(':')[0];
                }
                for (int i = 0; i < holders.Count; i++)
                {
                    if (holders[i].Fix_ID == station_id)
                    {
                        holders[i].IP = IP;
                        uploadData(i + 3, 0, IP);
                        holders[i].obj = socket;
                        uploadData(i + 3, 7, "Ready");
                    }
                }
                if (Socket_Server.test_environment.ToUpper() == "MARS")
                {
                    return "n/a";
                }
                return "{\"message_id\": \"ack\",  \"ack_id\": \""+ "ready" + "\",\"station_id\": \"" + station_id + "\", \"result\": \"OK\"}^";    // "{ \"result\": \"PASS\", \"message\": \"OK\"}^";
            }
            catch (Exception)
            {
                if (Socket_Server.test_environment.ToUpper() == "MARS")
                {
                    return "n/a";
                }
                return "{\"message_id\": \"ack\",  \"ack_id\": \"" + "ready" + "\",\"station_id\": \"" + station_id + "\", \"result\": \"NG\"}^";
            }
        }
        /// <summary>
        /// 截取querydata里面的变量 
        /// </summary>
        /// <param name="name">querydata里面的key</param>
        /// <param name="separation">分隔符</param>
        /// <param name="str_sample">querydata字符串</param>
        /// <returns>key的value</returns>
        private static string Get_str(string name, string separation, string str_sample)
        {
            try
            {
                string Get_1, Get_2, Get_3, Get_4, Get_5;
                int Get_n_1, Get_n_2, Get_n_3, Get_n_4, Get_n_5;
                Get_1 = name.ToUpper();
                Get_2 = separation.ToUpper();
                Get_3 = str_sample;
                Get_n_4 = Get_3.IndexOf(Get_1);
                Get_n_5 = Get_3.IndexOf(Get_2);
                if (Get_n_4 > -1)
                { }

                if (Get_n_5 > -1) { } else { }

                Get_4 = Get_3.Substring(Get_3.ToUpper().IndexOf(Get_1));

                Get_n_1 = Get_1.Length;
                Get_n_2 = Get_2.Length;
                Get_n_3 = Get_4.ToUpper().IndexOf(Get_2);
                Get_5 = Get_3.Substring(Get_3.ToUpper().IndexOf(Get_1) + Get_n_1, Get_n_3 - Get_n_1);
                return Get_5;
            }
            catch (Exception)
            {

                return "";
            }
        }
        public void Ack_Mars(JObject jObject, string message_id, Socket socket)
        {
            string response = string.Empty;

            string sub = string.Empty;
            foreach (JProperty j_v in jObject.Children())
            {
                if (j_v.Name != "message_id")
                {
                    int o = 0;
                    string v = j_v.Value.ToString().Trim();
                    if (j_v.Name != "test_profile")
                    {
                        if (int.TryParse(v, out o) || v.StartsWith("{") || v.StartsWith("["))
                        {
                            sub += $", \"{j_v.Name}\": " + j_v.Value.ToString().Replace("\r", "").Replace("\n", "").Replace("  ", " ");
                        }
                        else
                        {
                            sub += $", \"{j_v.Name}\": \"{j_v.Value.ToString().Replace("\r", "").Replace("\n", "")}\"";
                        }
                    }
                    else
                    {
                        sub += $", \"{j_v.Name}\": \"{j_v.Value.ToString().Replace("\r", "").Replace("\n", "")}\"";
                    }
                }
            }
            response += "{\"message_id\": \"ack\", \"ack_id\": \"" + message_id + "\"" + sub + "}^";
            printMes("TCP send", response);
            byte[] bytes = Encoding.Default.GetBytes(response);
            ((Socket)socket).Send(bytes);

        }

        private bool CheckSta(string station_id)
        {
            for (int i = 0; i < holders.Count; i++)
            {
                if (holders[i].Fix_ID == station_id)
                { return true; }
            }
            return false;
        }
    }
}
