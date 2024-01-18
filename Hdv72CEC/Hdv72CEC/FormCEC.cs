using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hdv72CEC
{
    public partial class FormCEC : Form
    {
        private const int DEVICE = 0;
        private bool exit_test = false;
        private Thread _test = null;
        public FormCEC()
        {
            InitializeComponent();
        }

        private void FormCEC_Load(object sender, EventArgs e)
        {
            new Thread(Init).Start();
        }

        private void Init()
        {
            Thread.Sleep(500);
            uint Count = 0;
            uint ID = 0;
            CheckErr(Hdv72.GetDeviceCount(out Count));
            info("Device Count:" + Count.ToString());
            CheckErr(Hdv72.GetDeviceID(DEVICE, out ID));
            info("Device ID:" + ID.ToString());
            CheckErr(Hdv72.DeviceOpen(DEVICE));
        }

        private void info(string msg)
        {
            richTextBox1.AppendText(
                string.Format("[{0}]{1}\n",
                DateTime.Now.ToString("HH:mm:ss"),
                msg.Trim())
                );
        }

        private void CheckErr(int hr)
        {
            Err.Text = hr.ToString();
            if (hr < 0)
            {
                ErrTxt.Text = Hdv72.GetErrorText(hr);
                info(ErrTxt.Text);
            }
            else
            {
                ErrTxt.Text = string.Empty;
            }
        }

        private void BtnSet_Click(object sender, EventArgs e)
        {
            CheckErr(Hdv72.SetCECPowerUp(DEVICE, 1));
            CheckErr(Hdv72.SetCECLogicalAddress(DEVICE, 0, Convert.ToUInt32(Addr.Text, 16)));
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            string[] source = ToGo.Text.Split(' ');
            byte[] data = new byte[source.Length];
            for(int i = 0; i < data.Length; i++)
            {
                data[i] = Convert.ToByte(source[i], 16);
            }
            info("Send:" + ToGo.Text);
            CheckErr(Hdv72.WriteCECCommand(DEVICE, data, (uint)data.Length));
        }

        private bool ReadOnce()
        {
            byte[] get = new byte[16];
            CheckErr(Hdv72.ReadCECCommand(DEVICE, 0, get, (uint)get.Length));
            string msg = string.Empty;
            for(int i = 0; i < get.Length; i++)
            {
                if (get[i] != 0x00)
                {
                    msg += get[i].ToString("X2") + " ";
                }
            }
            if (msg.Length > 2)
            {
                ToGet.Text = msg;
                info("Get:" + msg);
                return true;
            }
            return false;
        }

        private void TimRead_Tick(object sender, EventArgs e)
        {
            ReadOnce();
        }

        private void BtnRead_Click(object sender, EventArgs e)
        {
            TimRead.Stop();
            TimRead.Enabled = false;
            ReadOnce();
        }

        private void ToGet_DoubleClick(object sender, EventArgs e)
        {
            if (BtnRead.Enabled)
            {
                BtnRead.Enabled = false;
                TimRead.Enabled = true;
                TimRead.Start();
                info("Start Listen:");
            }
            else
            {
                BtnRead.Enabled = true;
                TimRead.Enabled = false;
                TimRead.Stop();
                info("End Listen:");
            }
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            if(_test == null)
            {
                exit_test = false;
                _test = new Thread(response_Test);
                _test.Start();
            }else
            {
                exit_test = true;
            }
        }

        private void response_Test()
        {
            BtnSet_Click(null, null);
            Thread.Sleep(500);
            byte[] offset = new byte[] { 0x00, 0x02, 0x04, 0x06, 0x08, 0x10, 0x12, 0x14 };
            while (!exit_test)
            {
                for(int i = 0; i < offset.Length; i++)
                {
                    if (exit_test) break;
                    ToGo.Text = "40 89 " + (0x80 + offset[i]).ToString("X2");
                    BtnSend_Click(null, null);
                    string start = DateTime.Now.ToString("HH:mm:ss:fff");
                    while (!exit_test)
                    {
                        Thread.Sleep(50);
                        if (ReadOnce())
                        {
                            string end = DateTime.Now.ToString("HH:mm:ss:fff");
                            info("response:" + start + "-" + end);
                            break;
                        }
                    }
                }
            }
            _test = null;
        }
    }
}
