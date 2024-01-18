namespace Hdv72CEC
{
    partial class FormCEC
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnSet = new System.Windows.Forms.Button();
            this.BtnSend = new System.Windows.Forms.Button();
            this.ToGo = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.BtnRead = new System.Windows.Forms.Button();
            this.ToGet = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.Addr = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Err = new System.Windows.Forms.TextBox();
            this.ErrTxt = new System.Windows.Forms.Label();
            this.BtnTest = new System.Windows.Forms.Button();
            this.TimRead = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.BtnSet, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.BtnSend, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ToGo, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.richTextBox1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.BtnRead, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.ToGet, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 5);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(646, 251);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // BtnSet
            // 
            this.BtnSet.Location = new System.Drawing.Point(3, 3);
            this.BtnSet.Name = "BtnSet";
            this.BtnSet.Size = new System.Drawing.Size(54, 23);
            this.BtnSet.TabIndex = 1;
            this.BtnSet.Text = "Set";
            this.BtnSet.UseVisualStyleBackColor = true;
            this.BtnSet.Click += new System.EventHandler(this.BtnSet_Click);
            // 
            // BtnSend
            // 
            this.BtnSend.Location = new System.Drawing.Point(3, 33);
            this.BtnSend.Name = "BtnSend";
            this.BtnSend.Size = new System.Drawing.Size(54, 23);
            this.BtnSend.TabIndex = 3;
            this.BtnSend.Text = "Send";
            this.BtnSend.UseVisualStyleBackColor = true;
            this.BtnSend.Click += new System.EventHandler(this.BtnSend_Click);
            // 
            // ToGo
            // 
            this.ToGo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ToGo.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ToGo.Location = new System.Drawing.Point(63, 33);
            this.ToGo.MaxLength = 50;
            this.ToGo.Name = "ToGo";
            this.ToGo.Size = new System.Drawing.Size(580, 23);
            this.ToGo.TabIndex = 4;
            // 
            // richTextBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.richTextBox1, 2);
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 93);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(640, 155);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // BtnRead
            // 
            this.BtnRead.Location = new System.Drawing.Point(3, 63);
            this.BtnRead.Name = "BtnRead";
            this.BtnRead.Size = new System.Drawing.Size(54, 23);
            this.BtnRead.TabIndex = 5;
            this.BtnRead.Text = "Read";
            this.BtnRead.UseVisualStyleBackColor = true;
            this.BtnRead.Click += new System.EventHandler(this.BtnRead_Click);
            // 
            // ToGet
            // 
            this.ToGet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ToGet.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ToGet.Location = new System.Drawing.Point(63, 63);
            this.ToGet.Name = "ToGet";
            this.ToGet.ReadOnly = true;
            this.ToGet.Size = new System.Drawing.Size(580, 23);
            this.ToGet.TabIndex = 6;
            this.ToGet.DoubleClick += new System.EventHandler(this.ToGet_DoubleClick);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 6;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel2.Controls.Add(this.Addr, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.Err, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.ErrTxt, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.BtnTest, 5, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(60, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(586, 30);
            this.tableLayoutPanel2.TabIndex = 7;
            // 
            // Addr
            // 
            this.Addr.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Addr.Location = new System.Drawing.Point(48, 3);
            this.Addr.Name = "Addr";
            this.Addr.Size = new System.Drawing.Size(24, 23);
            this.Addr.TabIndex = 0;
            this.Addr.Text = "f0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 30);
            this.label1.TabIndex = 1;
            this.label1.Text = "Addr:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(78, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 30);
            this.label2.TabIndex = 2;
            this.label2.Text = "Err:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Err
            // 
            this.Err.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Err.Location = new System.Drawing.Point(118, 3);
            this.Err.Name = "Err";
            this.Err.ReadOnly = true;
            this.Err.Size = new System.Drawing.Size(24, 21);
            this.Err.TabIndex = 3;
            // 
            // ErrTxt
            // 
            this.ErrTxt.AutoSize = true;
            this.ErrTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ErrTxt.Location = new System.Drawing.Point(148, 0);
            this.ErrTxt.Name = "ErrTxt";
            this.ErrTxt.Size = new System.Drawing.Size(390, 30);
            this.ErrTxt.TabIndex = 4;
            this.ErrTxt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BtnTest
            // 
            this.BtnTest.Location = new System.Drawing.Point(544, 3);
            this.BtnTest.Name = "BtnTest";
            this.BtnTest.Size = new System.Drawing.Size(39, 23);
            this.BtnTest.TabIndex = 5;
            this.BtnTest.Text = "Test";
            this.BtnTest.UseVisualStyleBackColor = true;
            this.BtnTest.Click += new System.EventHandler(this.BtnTest_Click);
            // 
            // TimRead
            // 
            this.TimRead.Interval = 1000;
            this.TimRead.Tick += new System.EventHandler(this.TimRead_Tick);
            // 
            // FormCEC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 261);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormCEC";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "CEC Tool";
            this.Load += new System.EventHandler(this.FormCEC_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button BtnSet;
        private System.Windows.Forms.TextBox Addr;
        private System.Windows.Forms.Button BtnSend;
        private System.Windows.Forms.TextBox ToGo;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button BtnRead;
        private System.Windows.Forms.TextBox ToGet;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Err;
        private System.Windows.Forms.Label ErrTxt;
        private System.Windows.Forms.Timer TimRead;
        private System.Windows.Forms.Button BtnTest;
    }
}

