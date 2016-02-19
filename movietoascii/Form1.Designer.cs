namespace movietoascii
{
    partial class ASCIIConverter
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btGetFrames = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btConvert = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txCharacters = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.chInColor = new System.Windows.Forms.CheckBox();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.btAsciiCharacters = new System.Windows.Forms.Button();
            this.btUpdateCharacters = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pictureBox1.Location = new System.Drawing.Point(-5, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1280, 720);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // btGetFrames
            // 
            this.btGetFrames.Location = new System.Drawing.Point(8, 761);
            this.btGetFrames.Name = "btGetFrames";
            this.btGetFrames.Size = new System.Drawing.Size(392, 23);
            this.btGetFrames.TabIndex = 1;
            this.btGetFrames.Text = "part 1";
            this.btGetFrames.UseVisualStyleBackColor = true;
            this.btGetFrames.Click += new System.EventHandler(this.button1_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(8, 732);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(392, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // btConvert
            // 
            this.btConvert.Location = new System.Drawing.Point(8, 819);
            this.btConvert.Name = "btConvert";
            this.btConvert.Size = new System.Drawing.Size(392, 23);
            this.btConvert.TabIndex = 3;
            this.btConvert.Text = "part 2";
            this.btConvert.UseVisualStyleBackColor = true;
            this.btConvert.Click += new System.EventHandler(this.btConvertClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1315, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "label1";
            // 
            // txCharacters
            // 
            this.txCharacters.Location = new System.Drawing.Point(1318, 42);
            this.txCharacters.Name = "txCharacters";
            this.txCharacters.Size = new System.Drawing.Size(268, 20);
            this.txCharacters.TabIndex = 5;
            this.txCharacters.Text = "☺ ☻ ♥ ♦ ♣ ♠ •◘○ ◙  4 ♂ 3 ♀ 2 ♪ ♫☼  ►";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(1318, 142);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(268, 264);
            this.listBox1.TabIndex = 7;
            // 
            // chInColor
            // 
            this.chInColor.AutoSize = true;
            this.chInColor.Checked = true;
            this.chInColor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chInColor.Location = new System.Drawing.Point(324, 848);
            this.chInColor.Name = "chInColor";
            this.chInColor.Size = new System.Drawing.Size(76, 17);
            this.chInColor.TabIndex = 8;
            this.chInColor.Text = "Ik wil kleur";
            this.chInColor.UseVisualStyleBackColor = true;
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(8, 790);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(392, 23);
            this.progressBar2.TabIndex = 9;
            // 
            // btAsciiCharacters
            // 
            this.btAsciiCharacters.Location = new System.Drawing.Point(1318, 68);
            this.btAsciiCharacters.Name = "btAsciiCharacters";
            this.btAsciiCharacters.Size = new System.Drawing.Size(268, 39);
            this.btAsciiCharacters.TabIndex = 10;
            this.btAsciiCharacters.Text = "ASCII CHARACTERS";
            this.btAsciiCharacters.UseVisualStyleBackColor = true;
            this.btAsciiCharacters.Click += new System.EventHandler(this.button2_Click);
            // 
            // btUpdateCharacters
            // 
            this.btUpdateCharacters.Location = new System.Drawing.Point(1511, 113);
            this.btUpdateCharacters.Name = "btUpdateCharacters";
            this.btUpdateCharacters.Size = new System.Drawing.Size(75, 23);
            this.btUpdateCharacters.TabIndex = 11;
            this.btUpdateCharacters.Text = "update";
            this.btUpdateCharacters.UseVisualStyleBackColor = true;
            this.btUpdateCharacters.Click += new System.EventHandler(this.btUpdateCharacters_Click);
            // 
            // ASCIIConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1676, 1023);
            this.Controls.Add(this.btUpdateCharacters);
            this.Controls.Add(this.btAsciiCharacters);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.chInColor);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.txCharacters);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btConvert);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btGetFrames);
            this.Controls.Add(this.pictureBox1);
            this.Name = "ASCIIConverter";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ASCIIConverter_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btGetFrames;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btConvert;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txCharacters;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.CheckBox chInColor;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.Button btAsciiCharacters;
        private System.Windows.Forms.Button btUpdateCharacters;
    }
}

