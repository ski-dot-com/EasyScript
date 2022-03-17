namespace EasyScriptGUI
{
    partial class REPL
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
            this.runbutton = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.consOut1 = new EasyScriptGUI.ConsOut();
            this.SuspendLayout();
            // 
            // runbutton
            // 
            this.runbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.runbutton.Location = new System.Drawing.Point(896, 479);
            this.runbutton.Name = "runbutton";
            this.runbutton.Size = new System.Drawing.Size(112, 31);
            this.runbutton.TabIndex = 1;
            this.runbutton.Text = "実行";
            this.runbutton.UseVisualStyleBackColor = true;
            this.runbutton.Click += new System.EventHandler(this.runbutton_Click);
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(12, 479);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(878, 31);
            this.textBox2.TabIndex = 2;
            // 
            // consOut1
            // 
            this.consOut1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.consOut1.Location = new System.Drawing.Point(12, 12);
            this.consOut1.Name = "consOut1";
            this.consOut1.Size = new System.Drawing.Size(996, 461);
            this.consOut1.TabIndex = 3;
            // 
            // REPL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 522);
            this.Controls.Add(this.consOut1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.runbutton);
            this.Name = "REPL";
            this.Text = "REPL";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Button runbutton;
        private TextBox textBox2;
        private ConsOut consOut1;
    }
}