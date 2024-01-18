namespace CalculateSalary
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double base_salary = Convert.ToDouble(textBox1.Text);
            double One_dot_five = Convert.ToDouble(textBox2.Text);
            double Double_hours = Convert.ToDouble(textBox3.Text);
            double Thrible_hours = Convert.ToDouble(textBox4.Text);
            double salary_cal = (base_salary / 21.75) / 8;
            //MessageBox.Show(salary_cal.ToString());
            textBox5.Text = ((float)(
                (base_salary + 240) + 
                (One_dot_five*(1.5*salary_cal)) +
                (Double_hours*(2*salary_cal)) + 
                (Thrible_hours*(3*salary_cal)) -
                Convert.ToDouble(textBox6.Text)
                )).ToString();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            textBox6.Text = "1000.1";
        }
    }
}