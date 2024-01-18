namespace Socket_Server_Robot
{
    partial class Socket_Server
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Socket_Server));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtSend = new System.Windows.Forms.TextBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_printstatus = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.richTextBoxLogs = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label_Holders = new System.Windows.Forms.Label();
            this.label_AutoFillSN = new System.Windows.Forms.Label();
            this.label_Station = new System.Windows.Forms.Label();
            this.label_Port = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.labelPort = new System.Windows.Forms.Label();
            this.labelStation = new System.Windows.Forms.Label();
            this.labelIP = new System.Windows.Forms.Label();
            this.labelHolderNum = new System.Windows.Forms.Label();
            this.labelAutoFillSN = new System.Windows.Forms.Label();
            this.labelFixtureList = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.labelVersion = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer_logPrint = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.panel4.SuspendLayout();
            this.panel8.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel5);
            this.groupBox1.Controls.Add(this.panel4);
            this.groupBox1.Controls.Add(this.panel3);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1090, 592);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Robot Socket Server";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.dgvData);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel5.Location = new System.Drawing.Point(217, 21);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(659, 337);
            this.panel5.TabIndex = 3;
            // 
            // dgvData
            // 
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvData.Location = new System.Drawing.Point(0, 0);
            this.dgvData.Name = "dgvData";
            this.dgvData.RowTemplate.Height = 23;
            this.dgvData.Size = new System.Drawing.Size(659, 337);
            this.dgvData.TabIndex = 0;
            //this.dgvData.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvData_RowPostPaint);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.panel8);
            this.panel4.Controls.Add(this.btnReset);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(876, 21);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(211, 337);
            this.panel4.TabIndex = 2;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.groupBox3);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(211, 294);
            this.panel8.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtSend);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(211, 294);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Send Message";
            // 
            // txtSend
            // 
            this.txtSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSend.Location = new System.Drawing.Point(3, 21);
            this.txtSend.Multiline = true;
            this.txtSend.Name = "txtSend";
            this.txtSend.Size = new System.Drawing.Size(205, 270);
            this.txtSend.TabIndex = 0;
            // 
            // btnReset
            // 
            this.btnReset.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnReset.Location = new System.Drawing.Point(0, 294);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(211, 43);
            this.btnReset.TabIndex = 0;
            this.btnReset.Text = "Send";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.groupBox2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(217, 358);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(870, 231);
            this.panel3.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_printstatus);
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Controls.Add(this.richTextBoxLogs);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(870, 231);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Message Log";
            // 
            // checkBox_printstatus
            // 
            this.checkBox_printstatus.AutoSize = true;
            this.checkBox_printstatus.Location = new System.Drawing.Point(160, 1);
            this.checkBox_printstatus.Name = "checkBox_printstatus";
            this.checkBox_printstatus.Size = new System.Drawing.Size(93, 21);
            this.checkBox_printstatus.TabIndex = 2;
            this.checkBox_printstatus.Text = "PrintStatus";
            this.checkBox_printstatus.UseVisualStyleBackColor = true;
            this.checkBox_printstatus.CheckedChanged += new System.EventHandler(this.checkBox_printstatus_CheckedChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(96, 1);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(55, 21);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "Print";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // richTextBoxLogs
            // 
            this.richTextBoxLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxLogs.Location = new System.Drawing.Point(3, 21);
            this.richTextBoxLogs.Name = "richTextBoxLogs";
            this.richTextBoxLogs.Size = new System.Drawing.Size(864, 207);
            this.richTextBoxLogs.TabIndex = 0;
            this.richTextBoxLogs.Text = "";
            this.richTextBoxLogs.TextChanged += new System.EventHandler(this.richTextBoxLogs_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel7);
            this.panel1.Controls.Add(this.panel6);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(3, 21);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(214, 568);
            this.panel1.TabIndex = 0;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.label_Holders);
            this.panel7.Controls.Add(this.label_AutoFillSN);
            this.panel7.Controls.Add(this.label_Station);
            this.panel7.Controls.Add(this.label_Port);
            this.panel7.Controls.Add(this.txtIP);
            this.panel7.Controls.Add(this.radioButton4);
            this.panel7.Controls.Add(this.radioButton3);
            this.panel7.Controls.Add(this.radioButton2);
            this.panel7.Controls.Add(this.radioButton1);
            this.panel7.Controls.Add(this.labelPort);
            this.panel7.Controls.Add(this.labelStation);
            this.panel7.Controls.Add(this.labelIP);
            this.panel7.Controls.Add(this.labelHolderNum);
            this.panel7.Controls.Add(this.labelAutoFillSN);
            this.panel7.Controls.Add(this.labelFixtureList);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(0, 130);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(214, 411);
            this.panel7.TabIndex = 8;
            // 
            // label_Holders
            // 
            this.label_Holders.AutoSize = true;
            this.label_Holders.Location = new System.Drawing.Point(87, 186);
            this.label_Holders.Name = "label_Holders";
            this.label_Holders.Size = new System.Drawing.Size(15, 17);
            this.label_Holders.TabIndex = 11;
            this.label_Holders.Text = "8";
            // 
            // label_AutoFillSN
            // 
            this.label_AutoFillSN.AutoSize = true;
            this.label_AutoFillSN.Location = new System.Drawing.Point(87, 143);
            this.label_AutoFillSN.Name = "label_AutoFillSN";
            this.label_AutoFillSN.Size = new System.Drawing.Size(34, 17);
            this.label_AutoFillSN.TabIndex = 11;
            this.label_AutoFillSN.Text = "True";
            // 
            // label_Station
            // 
            this.label_Station.AutoSize = true;
            this.label_Station.Location = new System.Drawing.Point(87, 102);
            this.label_Station.Name = "label_Station";
            this.label_Station.Size = new System.Drawing.Size(42, 17);
            this.label_Station.TabIndex = 11;
            this.label_Station.Text = "HDMI";
            // 
            // label_Port
            // 
            this.label_Port.AutoSize = true;
            this.label_Port.Location = new System.Drawing.Point(87, 61);
            this.label_Port.Name = "label_Port";
            this.label_Port.Size = new System.Drawing.Size(36, 17);
            this.label_Port.TabIndex = 11;
            this.label_Port.Text = "9090";
            // 
            // txtIP
            // 
            this.txtIP.Enabled = false;
            this.txtIP.Location = new System.Drawing.Point(87, 17);
            this.txtIP.Name = "txtIP";
            this.txtIP.ReadOnly = true;
            this.txtIP.Size = new System.Drawing.Size(121, 25);
            this.txtIP.TabIndex = 10;
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Location = new System.Drawing.Point(44, 366);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(105, 21);
            this.radioButton4.TabIndex = 9;
            this.radioButton4.Text = "radioButton4";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(44, 331);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(105, 21);
            this.radioButton3.TabIndex = 8;
            this.radioButton3.Text = "radioButton3";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(44, 296);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(105, 21);
            this.radioButton2.TabIndex = 7;
            this.radioButton2.Text = "radioButton2";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(44, 261);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(105, 21);
            this.radioButton1.TabIndex = 6;
            this.radioButton1.Text = "radioButton1";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // labelPort
            // 
            this.labelPort.AutoSize = true;
            this.labelPort.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPort.Location = new System.Drawing.Point(41, 61);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(40, 17);
            this.labelPort.TabIndex = 2;
            this.labelPort.Text = "Port :";
            // 
            // labelStation
            // 
            this.labelStation.AutoSize = true;
            this.labelStation.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStation.Location = new System.Drawing.Point(23, 102);
            this.labelStation.Name = "labelStation";
            this.labelStation.Size = new System.Drawing.Size(58, 17);
            this.labelStation.TabIndex = 3;
            this.labelStation.Text = "Station :";
            // 
            // labelIP
            // 
            this.labelIP.AutoSize = true;
            this.labelIP.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelIP.Location = new System.Drawing.Point(55, 20);
            this.labelIP.Name = "labelIP";
            this.labelIP.Size = new System.Drawing.Size(26, 17);
            this.labelIP.TabIndex = 1;
            this.labelIP.Text = "IP :";
            // 
            // labelHolderNum
            // 
            this.labelHolderNum.AutoSize = true;
            this.labelHolderNum.Location = new System.Drawing.Point(20, 184);
            this.labelHolderNum.Name = "labelHolderNum";
            this.labelHolderNum.Size = new System.Drawing.Size(61, 17);
            this.labelHolderNum.TabIndex = 5;
            this.labelHolderNum.Text = "Holders :";
            // 
            // labelAutoFillSN
            // 
            this.labelAutoFillSN.AutoSize = true;
            this.labelAutoFillSN.Location = new System.Drawing.Point(6, 143);
            this.labelAutoFillSN.Name = "labelAutoFillSN";
            this.labelAutoFillSN.Size = new System.Drawing.Size(75, 17);
            this.labelAutoFillSN.TabIndex = 4;
            this.labelAutoFillSN.Text = "AutoFillSN :";
            // 
            // labelFixtureList
            // 
            this.labelFixtureList.AutoSize = true;
            this.labelFixtureList.Location = new System.Drawing.Point(6, 225);
            this.labelFixtureList.Name = "labelFixtureList";
            this.labelFixtureList.Size = new System.Drawing.Size(75, 17);
            this.labelFixtureList.TabIndex = 4;
            this.labelFixtureList.Text = "FixtureList :";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.labelVersion);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 541);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(214, 27);
            this.panel6.TabIndex = 7;
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(1, 5);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(62, 17);
            this.labelVersion.TabIndex = 0;
            this.labelVersion.Text = "Version : ";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(214, 130);
            this.panel2.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(214, 130);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // timer_logPrint
            // 
            this.timer_logPrint.Interval = 60000;
            this.timer_logPrint.Tick += new System.EventHandler(this.timer_logPrint_Tick);
            // 
            // Socket_Server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1090, 592);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Socket_Server";
            this.Text = "Socket Server";
            this.Load += new System.EventHandler(this.Socket_Server_Load);
            this.groupBox1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelHolderNum;
        private System.Windows.Forms.Label labelFixtureList;
        private System.Windows.Forms.Label labelStation;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.Label labelIP;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtSend;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox richTextBoxLogs;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label_Holders;
        private System.Windows.Forms.Label label_AutoFillSN;
        private System.Windows.Forms.Label label_Station;
        private System.Windows.Forms.Label label_Port;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label labelAutoFillSN;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Timer timer_logPrint;
        private System.Windows.Forms.CheckBox checkBox_printstatus;
    }
}

