namespace QueryDataPractice
{
    partial class QueryTest
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.SN = new System.Windows.Forms.TextBox();
            this.ParsedData = new System.Windows.Forms.RichTextBox();
            this.OriginData = new System.Windows.Forms.RichTextBox();
            this.FactoryBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ServerList = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.queryStation = new System.Windows.Forms.ComboBox();
            this.cmdText = new System.Windows.Forms.ComboBox();
            this.queryStep = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.sendStr = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(609, 45);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Query";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SN
            // 
            this.SN.Location = new System.Drawing.Point(287, 45);
            this.SN.Name = "SN";
            this.SN.Size = new System.Drawing.Size(316, 23);
            this.SN.TabIndex = 1;
            // 
            // ParsedData
            // 
            this.ParsedData.Font = new System.Drawing.Font("Microsoft New Tai Lue", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ParsedData.Location = new System.Drawing.Point(793, 45);
            this.ParsedData.Name = "ParsedData";
            this.ParsedData.Size = new System.Drawing.Size(354, 494);
            this.ParsedData.TabIndex = 2;
            this.ParsedData.Text = "";
            // 
            // OriginData
            // 
            this.OriginData.Font = new System.Drawing.Font("Microsoft New Tai Lue", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.OriginData.Location = new System.Drawing.Point(92, 196);
            this.OriginData.Name = "OriginData";
            this.OriginData.Size = new System.Drawing.Size(654, 343);
            this.OriginData.TabIndex = 3;
            this.OriginData.Text = "";
            // 
            // FactoryBox
            // 
            this.FactoryBox.FormattingEnabled = true;
            this.FactoryBox.Location = new System.Drawing.Point(310, 91);
            this.FactoryBox.Name = "FactoryBox";
            this.FactoryBox.Size = new System.Drawing.Size(86, 25);
            this.FactoryBox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(253, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "SN:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.Location = new System.Drawing.Point(251, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "Factory:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // ServerList
            // 
            this.ServerList.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ServerList.FormattingEnabled = true;
            this.ServerList.Location = new System.Drawing.Point(599, 91);
            this.ServerList.Name = "ServerList";
            this.ServerList.Size = new System.Drawing.Size(121, 25);
            this.ServerList.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(545, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "Server:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(253, 133);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "Station:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(235, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 17);
            this.label6.TabIndex = 11;
            this.label6.Text = "querystep:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(533, 128);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 17);
            this.label7.TabIndex = 12;
            this.label7.Text = "cmdText:";
            // 
            // queryStation
            // 
            this.queryStation.FormattingEnabled = true;
            this.queryStation.Location = new System.Drawing.Point(310, 126);
            this.queryStation.Name = "queryStation";
            this.queryStation.Size = new System.Drawing.Size(87, 25);
            this.queryStation.TabIndex = 13;
            // 
            // cmdText
            // 
            this.cmdText.FormattingEnabled = true;
            this.cmdText.Location = new System.Drawing.Point(599, 125);
            this.cmdText.Name = "cmdText";
            this.cmdText.Size = new System.Drawing.Size(121, 25);
            this.cmdText.TabIndex = 14;
            // 
            // queryStep
            // 
            this.queryStep.FormattingEnabled = true;
            this.queryStep.Location = new System.Drawing.Point(310, 165);
            this.queryStep.Name = "queryStep";
            this.queryStep.Size = new System.Drawing.Size(87, 25);
            this.queryStep.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(537, 168);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 17);
            this.label5.TabIndex = 16;
            this.label5.Text = "SendStr:";
            // 
            // sendStr
            // 
            this.sendStr.FormattingEnabled = true;
            this.sendStr.Location = new System.Drawing.Point(599, 160);
            this.sendStr.Name = "sendStr";
            this.sendStr.Size = new System.Drawing.Size(121, 25);
            this.sendStr.TabIndex = 17;
            // 
            // QueryTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1181, 586);
            this.Controls.Add(this.sendStr);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.queryStep);
            this.Controls.Add(this.cmdText);
            this.Controls.Add(this.queryStation);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ServerList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FactoryBox);
            this.Controls.Add(this.OriginData);
            this.Controls.Add(this.ParsedData);
            this.Controls.Add(this.SN);
            this.Controls.Add(this.button1);
            this.Name = "QueryTest";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.QueryTest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button button1;
        private TextBox SN;
        private RichTextBox ParsedData;
        private RichTextBox OriginData;
        private ComboBox FactoryBox;
        private Label label1;
        private Label label2;
        private ComboBox ServerList;
        private Label label3;
        private Label label4;
        private Label label6;
        private Label label7;
        private ComboBox queryStation;
        private ComboBox cmdText;
        private ComboBox queryStep;
        private Label label5;
        private ComboBox sendStr;
    }
}