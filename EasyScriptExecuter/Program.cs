// See https://aka.ms/new-console-template for more information
using EasyScript;
using System.Text.Json;
if(args.Length !=1)
{
	Console.WriteLine("使い方:");
	Console.WriteLine("(これ) (EasyScriptCodeファイル(*.essc))");
	return;
}
var nodes = JsonSerializer.Deserialize<List<Node>>(File.ReadAllText(args[0]));
ParserAndExecuter parserAndExecuter = new();
parserAndExecuter.Import(new Test());
parserAndExecuter.Run(nodes??new());
record Test()
{
	[ExportMethod, Explanation("引数のオブジェクトを表示します。")]
	[return: Explanation("どんな時でも「null」")]
	public void print([Explanation("表示したいオブジェクト")] object obj) => Console.WriteLine(string.Join(",", obj));
}
