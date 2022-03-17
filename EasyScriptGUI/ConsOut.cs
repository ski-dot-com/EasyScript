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
	public partial class ConsOut : UserControl
	{
		public ConsOut()
		{
			InitializeComponent();
		}

		private void Out_ContentsResized(object sender, ContentsResizedEventArgs e)
		{
			Out.Size = new(Width - 26, e.NewRectangle.Height + 6);
			panel1.AutoScrollPosition = new Point(panel1.AutoScrollPosition.X, panel1.Height + panel1.VerticalScroll.Value);
		}

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
			Out.Size = new(Width - 26, Out.Size.Height);
		}
    }
}
