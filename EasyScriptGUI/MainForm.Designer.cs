namespace EasyScriptGUI
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ウィンドウToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.コンパイラーToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rEPLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ウィンドウToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 33);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ウィンドウToolStripMenuItem
            // 
            this.ウィンドウToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.コンパイラーToolStripMenuItem,
            this.rEPLToolStripMenuItem});
            this.ウィンドウToolStripMenuItem.Name = "ウィンドウToolStripMenuItem";
            this.ウィンドウToolStripMenuItem.Size = new System.Drawing.Size(91, 29);
            this.ウィンドウToolStripMenuItem.Text = "ウィンドウ";
            // 
            // コンパイラーToolStripMenuItem
            // 
            this.コンパイラーToolStripMenuItem.Name = "コンパイラーToolStripMenuItem";
            this.コンパイラーToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.コンパイラーToolStripMenuItem.Text = "コンパイラ";
            this.コンパイラーToolStripMenuItem.Click += new System.EventHandler(this.コンパイラーToolStripMenuItem_Click);
            // 
            // rEPLToolStripMenuItem
            // 
            this.rEPLToolStripMenuItem.Name = "rEPLToolStripMenuItem";
            this.rEPLToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.rEPLToolStripMenuItem.Text = "REPL";
            this.rEPLToolStripMenuItem.Click += new System.EventHandler(this.rEPLToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem ウィンドウToolStripMenuItem;
        private ToolStripMenuItem コンパイラーToolStripMenuItem;
        private ToolStripMenuItem rEPLToolStripMenuItem;
    }
}