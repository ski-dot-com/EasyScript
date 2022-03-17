using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScriptGUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void コンパイラーToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Thread(()=>Application.Run(new Compiler())).Start();
        }

        private void rEPLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Thread(() => Application.Run(new REPL())).Start();
        }
    }
}
