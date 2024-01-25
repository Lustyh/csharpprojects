using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShadowTM
{
    class HolderStatus
    {
        public string IP = "";
        public Object obj = null;
        public string slot = "-";
        public string SN = "null";
        public int    status;
        public string Fix_ID = "0";
        public int    Fail_Count = 0;
        public string Fail_Fix = "";
        public string upload_cli = "OK";
        public bool   new_dev = true;
        public int    group_id = 0;
    }

    class Roboot
    {
        public string SN = "null";
        public int    status = 0;
        public int count = 0;
        public int Fail_Count = 0;
        public string Fail_Fix = "";
    }

    class UserCommand
    {
        public static List<Roboot> Roboots = new List<Roboot>();
        public static List<int> slot_num_by_group = new List<int>();
        public static int holder_count = 1;
        public static int fix_count = 1;
        public static int group_count = 1;
        public static string test_type = "";
        public static List<HolderStatus> holders = new List<HolderStatus>();

        /// <summary>
        /// 初始化列表,打开程序时定义列表长度和进行初始化
        /// </summary>
        /// <param name="num">治具数</param>
        /// <param name="fix_holder">每台治具的holder数</param>
        /// <param name="test_Mode">测试模式{single_up, async_test, multiple_up}</param>
        public static void Init(int num, int fix_holder, int group_num, string test_Mode)
        {
            
            fix_count = num;
            holder_count = fix_holder;
            group_count = group_num;
            test_type = test_Mode;
            Console.WriteLine(num * fix_holder * group_num);
            for (int i = 0; i < num * fix_holder * group_num; i++)
            {
                holders.Add(new HolderStatus());
            }
            
            for (int i = 0; i < 2; i++)
            {
                Roboots.Add(new Roboot());
            }
            
            Console.WriteLine($"Init holder list length:{holders.Count}");
        }

        /// <summary>
        /// 处理Client端发送的信息并回传信息给client端
        /// </summary>
        /// <param name="param">client发送的信息</param>
        /// <param name="IP">Client端的IP及其端口</param>
        /// <param name="socket">socket 对象</param>
        /// <returns>回传给client端的信息</returns>
        public static string ProcessCommand(string param, string IP = "", Object socket = null)
        {
            //try
            //{
            //判断是robot发送的信息还是clifford发送的信息  true为clifford   false为robot
            if (param.Contains("^") && param.Contains("{"))
            {
                param = param.Replace("^", "");
                JObject jObject = JObject.Parse(param);
                string message_id = jObject.GetValue("message_id").ToString().ToLower();
                if (message_id != "ack")
                {
                    Ack(jObject, message_id, (Socket)socket);
                    Thread.Sleep(100);
                }
                switch (message_id)
                {
                    case "test_complete":
                        return Test_Complete(jObject);
                    case "station_ready":
                        return StationReady(jObject);
                    case "station_online":
                        return StationVerify(jObject, IP, socket);
                }
                return "n/a";
            }
            else
            {
                param = param.ToUpper();
                int num = holders.Count();

                //初始化某个fix的值
                if (param.ToUpper().Contains("TYPE=RESET"))
                {
                    for (int i = 0; i < holders.Count; i++)
                    {
                        holders[i].slot = "-";
                        holders[i].SN = "null";
                        holders[i].status = 0;
                        holders[i].Fail_Fix = "";
                        holders[i].Fail_Count = 0;
                        holders[i].upload_cli = "OK";
                        holders[i].new_dev = true;
                    }
                }
                //检查站别状态
                if (param.ToUpper().Contains("TYPE=SFC"))
                {
                    string[] args = param.Split(';');
                    string SN = args[1].Substring(3);
                    string retstr = PluginSF.Query(SN);
                    if (PluginSF.Line_type == "SMT")
                    {
                        if (retstr.Contains("RESULT=PASS"))
                        {
                            Roboots[0].SN = SN;
                            Roboots[0].Fail_Count = 0;
                            Roboots[0].Fail_Fix = "";
                            return SN + "+PASS";
                        }
                        else
                        {
                            return SN + "+FAIL";
                        }
                    }
                    if (retstr.Contains("SF_CFG_CHK=PASS"))
                    {
                        string StatusStart = retstr.Substring(retstr.IndexOf("STATUS"));
                        string Status = Get_str("SET STATUS=", ";$;", retstr);
                        string value = PluginSF.get_status(Status);
                        string test_count = Get_str("FailCount=", ";$;", retstr);
                        string fix_id_1 = Get_str("1FixtureID=", ";$;", retstr);
                        Roboots[0].SN = SN;
                        if (Roboots[0].SN.StartsWith("BWIP"))
                            Roboots[0].SN = Roboots[0].SN.Substring(1);
                        if (test_count == "")
                        {
                            Roboots[0].Fail_Count = 0;
                            Roboots[0].Fail_Fix = "";
                        }
                        else
                        {
                            Roboots[0].Fail_Count = int.Parse(test_count);
                            Roboots[0].Fail_Fix = fix_id_1;
                        }
                        string defau = PluginSF.defau;
                        if (retstr.Contains("SF_Routing_CHK=PASS"))
                        {
                            return SN + "+PASS";
                        }
                        return SN + "+" + ((value.Length > 0) ? value : defau);
                    }
                    else
                    {
                        return SN + "+FAIL";
                    }
                }
                if (param.Contains("TYPE=START"))
                {
                    string[] args = param.Split(';');
                    int hold_id = int.Parse(args[2].Substring(4));
                    Console.WriteLine("hold_id:" + hold_id.ToString());
                    string SN = args[1].Substring(3);
                    if (SN.StartsWith("BWIP"))
                        SN = SN.Substring(1);
                    Console.WriteLine(hold_id * holder_count - holder_count);
                    Console.WriteLine(holders[hold_id * holder_count - holder_count].Fix_ID);
                    if (SN == Roboots[0].SN) //new dut
                    {
                        holders[hold_id * holder_count - holder_count].SN = SN;
                        holders[hold_id * holder_count - holder_count].status = 0;
                        holders[hold_id * holder_count - holder_count].Fail_Count = Roboots[0].Fail_Count;
                    }
                    else if (SN == holders[hold_id * holder_count - holder_count].SN) //same dut in same slot
                    {
                        holders[hold_id * holder_count - holder_count].status = 0;
                    }
                    else
                    {
                        holders[hold_id * holder_count - holder_count].SN = SN;
                        holders[hold_id * holder_count - holder_count].status = 0;
                        holders[hold_id * holder_count - holder_count].Fail_Count = 0;
                    }
                    test_start(hold_id);
                    return "PASS";
                }
                if (param.Contains("TYPE=STATUS") || param.Contains("TYPE=RESULT"))
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

                return "null";
            }
        }
        //TYPE=START;SN=WIP1331105GN0002X;FIX=1
        public static void Ack(JObject jObject,string message_id, Socket socket)
        {
            string response = string.Empty;

            string sub = string.Empty;
            foreach (JProperty j_v in jObject.Children())
            {
                if (j_v.Name != "message_id")
                {
                    int o = 0;
                    string v = j_v.Value.ToString().Trim();
                    if (int.TryParse(v, out o) || v.StartsWith("{") || v.StartsWith("["))
                    {
                        sub += $", \"{j_v.Name}\": " + j_v.Value.ToString().Replace("\r", "").Replace("\n", "").Replace("  ", " ");
                    }
                    else
                    {
                        sub += $", \"{j_v.Name}\": \"{j_v.Value.ToString().Replace("\r", "").Replace("\n", "")}\"";
                    }
                }
            }
            response += "{\"message_id\": \"ack\", \"ack_id\": \"" + message_id + "\"" + sub + "}^";

            byte[] bytes = Encoding.Default.GetBytes(response);
            ((Socket)socket).Send(bytes);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Holder_id"></param>
        public static void test_start(int Holder_id)
        {
            string list_sn = "";
            string station_id = holders[Holder_id * holder_count - holder_count].Fix_ID;
            if (PluginSF.test_mode == "multiple_up")
            {
                string sn = holders[Holder_id * holder_count - holder_count].SN;
                string sn_head = sn.Substring(0, sn.Length - 5);
                string sn_vari = sn.Substring(sn.Length - 5);

                int base_num = Convert.ToInt32(sn_vari, 16);
                //以当前SN为基数,填充SN,slot信息
                for (int j = 0; j < PluginSF.Fix_Holder_No; j++)
                {
                    string nSN = sn_head + (base_num + j).ToString("X").PadLeft(5, '0');
                    holders[Holder_id + j - 1].SN = nSN;
                    //holders[Holder_id + j - 1].status = 5;
                    list_sn += "\"" + nSN + "\",";
                }
                list_sn = list_sn.Substring(0, list_sn.Length - 1);
            }
            else
            {
                list_sn = "\"" + holders[Holder_id - 1].SN + "\"";
                if (PluginSF.Send_sn == "N")
                {
                    list_sn = "\"\"";
                }
            }

            // 用余数获取group_index. 
            // 生成一个Group_List 表示每个治具总共group 且列表应该为递减。 {1 , 0} 用group_index 获得group_id
            // 如果group总数为2， 那么余数余数只有两种情况 1 或 0。 得0则为group0，1与之相反

            List<int> Group_List = Enumerable.Range(0, slot_num_by_group.Count() + 1)
                .Select(i => slot_num_by_group.Count() - 1 - i)
                .ToList();
            int group_index = Holder_id % slot_num_by_group.Count();
            Console.WriteLine(group_index);
            string response = "{\"message_id\": \"test_start\",\"station_id\": \"" + station_id + "\", \"group_id\": " + Group_List[group_index].ToString() + ", \"dut_id\": [" + list_sn + "]}^";
            SocketServer.Send(response, holders[Holder_id * holder_count - holder_count].IP, holders[Holder_id * holder_count - holder_count].obj);
            holders[Holder_id * holder_count - holder_count].status = 5;
        }

        /// <summary>
        /// Test_Complete API   From Clifford to central controller
        /// </summary>
        /// <param name="jObject">request connect for json type</param>
        /// <returns> reture the result of Test_Complete </returns>
        public static string Test_Complete(JObject jObject, Socket socket = null)
        {
            string station_id = jObject.GetValue("station_id").ToString();
            string slot_id = jObject.GetValue("slot_id").ToString();
            string group_id = jObject.GetValue("group_id").ToString();
            string dut_id = jObject.GetValue("dut_id").ToString();
            string test_result = jObject.GetValue("result").ToString();
            //Console.WriteLine(holders.Count);
            for (int i = 0; i < holders.Count; i++)
            {
                // 
                //Console.WriteLine("group_id: " + group_id);
                //Console.WriteLine("holders[i].slot: " + holders[i].slot);
                if (station_id == holders[i].Fix_ID && group_id == holders[i].slot)
                {
                    if (test_result.ToUpper() != "PASS")
                    {
                        int fail_count = holders[i].Fail_Count + 1;
                        fail_count = fail_count < 3 ? fail_count : 3;
                        holders[i].status = 3;
                        holders[i].Fail_Count = fail_count;
                        if(fail_count < 2)
                        {
                            if (PluginSF.Retest_Rule == "AAB")
                            {
                                //auto retest
                                holders[i].status = 0;
                                Thread.Sleep(100);
                                test_start(i + 1);
                            }
                        }
                    }
                    else
                    {
                        holders[i].status = 4;
                    }
                    return "n/a";
                }
            }
            return "n/a";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        public static string StationReady(JObject jObject)
        {
            string station_id = jObject.GetValue("station_id").ToString();
            string project_id = jObject.GetValue("project_id").ToString();
            string group_id = jObject.GetValue("group_id").ToString();
            return "n/a";
        }

        /// <summary>
        /// online API  From Clifford to central controller
        /// </summary>
        /// <param name="jObject">request connect for json type </param>
        /// <param name="IP">clent ip</param>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static string StationVerify(JObject jObject, string IP, Object socket)
        {
            string project_id = jObject.GetValue("project_id").ToString();
            string station_id = jObject.GetValue("station_id").ToString();
            slot_num_by_group = jObject.GetValue("slot_num_by_group").ToObject<List<int>>();
            
            if (IP.Contains(":"))
            {
                IP = IP.Split(':')[0];
            }
            Console.WriteLine(IP);
            string[] fixturename = station_id.Split('_');
            string fixture_fullno = fixturename[fixturename.Length - 1];
            int fix_no = 0;
            if (int.TryParse(fixture_fullno, out fix_no))
            {
                fix_no = (int.Parse(fixture_fullno) - 1) % fix_count;
                Console.WriteLine("fix_no: " + fix_no.ToString());
                for (int i = 0; i < holder_count * group_count; i++)
                {
                    //Console.WriteLine(i);
                    Console.WriteLine(station_id);
                    Console.WriteLine(fix_no * holder_count * group_count + i);
                    holders[fix_no * holder_count * group_count + i].Fix_ID = station_id;
                    holders[fix_no * holder_count * group_count + i].IP = IP;
                    holders[fix_no * holder_count * group_count + i].slot = i.ToString();
                    holders[fix_no * holder_count * group_count + i].status = 0;
                    holders[fix_no * holder_count * group_count + i].SN = "";
                    holders[fix_no * holder_count * group_count + i].Fail_Fix = "";
                    holders[fix_no * holder_count * group_count + i].Fail_Count = 0;
                    holders[fix_no * holder_count * group_count + i].upload_cli = "OK";
                    holders[fix_no * holder_count * group_count + i].obj = socket;
                    holders[fix_no * holder_count * group_count + i].new_dev = false;
                }
                return "{\"message_id\": \"station_verify_result\",\"station_id\": \"" + station_id + "\", \"result\": \"OK\"}^";
            }
            else
            {
                return "{\"message_id\": \"station_verify_result\",\"station_id\": \"" + station_id + "\", \"result\": \"NG\"}^";
            }
        }

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
    }
}
