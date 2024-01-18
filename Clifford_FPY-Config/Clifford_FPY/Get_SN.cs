using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Clifford_FPY
{
    public partial class Get_SN : Form
    {
        public Get_SN()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] SNs = textBox1.Text.Replace(" ", "").Replace("\t", "").Replace("\r", "").Split('\n');
            Form1.get_sn = SNs;
            this.Close();
        }
    }
}
