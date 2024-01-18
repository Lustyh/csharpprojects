

namespace LoginWindow
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ("Lusty".Equals(textBox1.Text) && "123456".Equals(textBox2.Text))
            {
                MessageBox.Show("Login success");
            }
            else
            {
                MessageBox.Show("Login fail password wrong.");
            }
            Form2 mainForm = new Form2(textBox1.Text, textBox2.Text);
            mainForm.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}