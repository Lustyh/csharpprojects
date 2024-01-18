using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LearnListBox
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            JObject v = Readjson();
            /*MessageBox.Show(v["phases"].ToString());*/
            foreach (var item in v["phases"].ToArray())
            {
                //*listBox1.Items.Add(item.Key+":"+item.Value);*//
                
                foreach (var j in item["measurements"])
                {
                    listBox1.Items.Add(item["name"]+"---"+j["name"]+"\t:\t"+j["outcome"]);
                }
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        public JObject Readjson()
        {
            JObject v = new JObject();
            /*E:\Projects\T6\evt\MMI\fqc_battery.json*/
            if (comboBox1.Text.Equals("Mars"))
            {
                string Jsonfile = textBox1.Text;
                using (System.IO.StreamReader file = System.IO.File.OpenText(Jsonfile))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        JObject o = (JObject)JToken.ReadFrom(reader);
                        return o;
                        /*var value = o[key].ToString();
                        return value;*/
                    }
                }
            }
            else
            {
                return v;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            /*DialogResult dr = openFileDialog1.ShowDialog();*/
            //获取所打开文件的文件名
            textBox1.Text = openFileDialog1.FileName;
            /*if (dr == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(filename))
            {
                StreamReader sr = new StreamReader(filename);
                textBox1.Text = sr.ReadToEnd();
                sr.Close();
            }*/
            /*MessageBox.Show(openFileDialog1.ToString());*/
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}

