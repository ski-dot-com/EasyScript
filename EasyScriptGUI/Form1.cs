using System.Text.Json;
namespace EasyScriptGUI
{
	public partial class Compiler : Form
	{
		public Compiler()
		{
			InitializeComponent();
		}
        string? filename = null;
		string txt = "";
		private void button2_Click(object sender, EventArgs e)
		{
            var of = new OpenFileDialog
            {
                FileName = filename??"*.ess",
                Filter = "EasyScriptファイル(*.ess)|*.ess|すべてのファイル(*.*)|*.*",
                FilterIndex = 1,
                Title = "ファイルを読み込む...",
                RestoreDirectory = true
            };
            if (of.ShowDialog() == DialogResult.OK)
            {
				txt = File.ReadAllText(of.FileName);
                FileName.Text = of.FileName;
                filename=Path.GetFileName(of.FileName);

                FileContents.Text = txt;
			}
		}
		readonly EasyScript.ParserAndExecuter parserAndExecuter = new();
        private void Compile_Click(object sender, EventArgs e)
		{
            var sf = new SaveFileDialog
            {
                FileName = (filename ?? "*.ess")+"c",
                Filter = "EasyScriptCodeファイル(*.essc)|*.essc|すべてのファイル(*.*)|*.*",
                FilterIndex = 1,
                Title = "コードを保存する...",
                RestoreDirectory = true
            };
            if (sf.ShowDialog() != DialogResult.OK) 
                return;
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.ForeColor = System.Drawing.SystemColors.ControlText;
            toolStripStatusLabel1.Text = "コンパイル中...";
            toolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.Highlight;
            try
            {
                var tmp = ((EasyScript.Node)parserAndExecuter.Execute($"@({{{txt}}})")[0]).Child;
                toolStripProgressBar1.PerformStep();
                using (var fs = File.Create(sf.FileName))
                {
                    using var bw = new BinaryWriter(fs);
                    foreach(var item in tmp)
                    {

                    }
                    bw.Flush();
                }
                toolStripProgressBar1.PerformStep();
                toolStripStatusLabel1.Text = "コンパイル成功！";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                toolStripStatusLabel1.Text = ex.Message;
                toolStripStatusLabel1.ForeColor = Color.Red;
            }
		}

        private void Save_Click(object sender, EventArgs e)
        {
            if (FileName.Text == "") return;
            using var fs = File.CreateText(FileName.Text);
            fs.Write(FileContents.Text);
            fs.Flush();
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}