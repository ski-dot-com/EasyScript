// See https://aka.ms/new-console-template for more information
using EasyScript;
ParserAndExecuter parserAndExecuter = new();
Console.WriteLine($"{parserAndExecuter.Import(new Test())}個のデータがインポートされました。");
while (true)
{
	Console.Write(">");
	string s = Console.ReadLine()??"";
	if (false) ;
	else if (s == "//exit")
	{
		return;
	}
	else if (s == "//help")
	{
		Console.WriteLine("使い方:  ");
		Console.WriteLine("//help: この画面を出します。");
		//Console.WriteLine("//reference: 主な関数の説明を出します。");
		Console.WriteLine("//beginnershelp: 簡単な説明を出します。");
	}
	else if (s == "//beginnershelp")
	{
		Console.WriteLine("使い方:  (この画面は、キーを押すと進みます。)");
		Console.ReadKey();
		Console.WriteLine("ここには、式を打ち込めます。");
		Console.WriteLine("ex) (式)");
		Console.ReadKey();
		Console.WriteLine("また、複数の式を打ち込みたいときは、式を「,」でつなぎます。");
		Console.WriteLine("ex) (式),(式)");
		Console.ReadKey();
		Console.WriteLine("式は、七種類あります。(これから増えるかもしれません。)");
		Console.ReadKey();
		Console.WriteLine("一種類目の式は、「\"」で囲った文字列です。");
		Console.WriteLine("ex) \"hello\"");
		Console.ReadKey();
		Console.WriteLine("" +
			"「\"」、" +
			"「\\」の" +
			"文字を使いたいときは、前に「\\」を付けます。");
		Console.WriteLine("ex) \"\\\"I love you,\\\" I said.\"");
		Console.ReadKey();
		Console.WriteLine("二種類目の式は、関数呼び出しです。");
		Console.WriteLine("ex) print(\"0\")");
		Console.ReadKey();
		Console.WriteLine("関数呼び出しは、関数名から始まります。これは、「\"」で囲ってはいけません。");
		Console.WriteLine("ex) print...");
		Console.ReadKey();
		Console.WriteLine("そのつぎに、「(」と「)」で囲まれた引数があります。");
		Console.WriteLine("ex) print(...)");
		Console.ReadKey();
		Console.WriteLine("引数は、一個の式であるときもあります。");
		Console.WriteLine("ex) print(\"0\")");
		Console.ReadKey();
		Console.WriteLine("複数の引数を渡す時は、式を「,」でつなぎます。");
		Console.WriteLine("ex) set(\"x\", \"0\")");
		Console.ReadKey();
		Console.WriteLine("そして、、、引数がない時もあります！");
		Console.WriteLine("ex) null()");
		Console.ReadKey();
		Console.WriteLine("関数は呼び出されるとき、スコープを作ります。");
		Console.WriteLine("ex) (グローバルスコープ)=>(グローバルスコープ(新しいスコープ))");
		Console.ReadKey();
		Console.WriteLine("呼び出しが終わると、スコープを閉じます。");
		Console.WriteLine("ex) (グローバルスコープ(関数のスコープ))=>(グローバルスコープ)");
		Console.ReadKey();
		Console.WriteLine("呼び出されるときにスコープを作らず、呼び出しが終わってもスコープを閉じない関数もあります。");
		Console.WriteLine("ex) (グローバルスコープ)=>(グローバルスコープ)=>(グローバルスコープ)");
		Console.ReadKey();
		Console.WriteLine("具体的に言うと、、、");
		Console.WriteLine("「"+string.Join("」、　「", parserAndExecuter.doesnotMakeScope)+"」");
		Console.WriteLine("が相当します。");
		Console.ReadKey();
		Console.WriteLine("三種類目の式は、ブロックです。");
		Console.WriteLine("ex) {print(\"hello\")}");
		Console.ReadKey();
		Console.WriteLine("ブロックは、「{」と「}」で囲まれた式の塊です。");
		Console.ReadKey();
		Console.WriteLine("ブロックの中身は、一個の式であるときもあります。");
		Console.WriteLine("ex) {print(\"hello\")}");
		Console.ReadKey();
		Console.WriteLine("複数の式を書きたい時は、式を「,」でつなぎます。");
		Console.WriteLine("ex) {print(\"hello\"),print(\"world\")}");
		Console.ReadKey();
		Console.WriteLine("ブロックの中身がいらないときは、中身は書かなくて大丈夫です。");
		Console.WriteLine("ex) {}");
		Console.ReadKey();
		Console.WriteLine("ブロックは計算されるときスコープを作ります。");
		Console.WriteLine("ex) (グローバルスコープ)=>(グローバルスコープ(新しいスコープ))");
		Console.ReadKey();
		Console.WriteLine("計算が終わるとスコープを閉じます。");
		Console.WriteLine("ex) (グローバルスコープ(ブロックのスコープ))=>(グローバルスコープ)");
		Console.ReadKey();
		Console.WriteLine("四種類目の式は、「#」か「%」から始まる数字です。「#」は整数、「%」は小数です。");
		Console.WriteLine("ex) #10");
		Console.WriteLine(" %10.0");
		Console.WriteLine(" %1e1");
		Console.WriteLine(" %-1e1");
		Console.ReadKey();
		Console.WriteLine("「#」と「%」を省略することもできるようになりました。");
		Console.WriteLine("ex) 10");
		Console.WriteLine(" 10.0");
		Console.WriteLine(" 1e1");
		Console.WriteLine(" -1e1");
		Console.ReadKey();
		//Console.WriteLine("五種類目の式は、「!」から始まる文字列です。定数を参照することができます。");
		//Console.WriteLine("ex) !true");
		//Console.WriteLine(" !false");
		//Console.ReadKey();
		Console.WriteLine("五種類目の式は、「(式(=を含まない)):=(式)」です。変数を定義することができます。");
		Console.WriteLine("ex) x:=0");
		Console.ReadKey();
		Console.WriteLine("「(式(=を含まない))::=(式)」で、グローバル変数(どこからでも参照できる変数)を定義することができます。");
		Console.WriteLine("ex) x::=0");
		Console.ReadKey();
		Console.WriteLine("六種類目の式は、「(式(=を含まない))=(式)」です。定義した変数を設定することができます。");
		Console.WriteLine("ex) x=0");
		Console.ReadKey();
		//Console.WriteLine("頭に「::」をつけると、グローバル変数（グローバルスコープ(関数を呼び出す前に定義した変数が属するスコープ)に属する変数）を設定することができます。");
		//Console.WriteLine("ex) ::x=#0");
		//Console.ReadKey();
		//Console.WriteLine("その頭に、数字をつけると、外側から何番目のスコープに属する変数かを指定することができます。(グローバルスコープは0番目です。n番目のスコープ内で開かれたスコープはn+1番目です。)");
		//Console.WriteLine("ex) 0::x=#0");
		//Console.ReadKey();
		//Console.WriteLine("「::」を「:」にすると、0番目のスコープが、一番最後にブロックが開いたスコープになります。(n番目のスコープ内で開かれたスコープはn+1番目です。)");
		//Console.WriteLine("ex) 0:x=#0");
		//Console.ReadKey();
		//Console.WriteLine("頭に何もついていないと、一番最後に開いたスコープに属する変数を設定することができます。");
		//Console.WriteLine("ex) x=#0");
		//Console.ReadKey();
		Console.WriteLine("七種類目の式は、ただの文字列です。変数を参照することができます。");
		Console.WriteLine("ex) x");
		Console.ReadKey();
		//Console.WriteLine("変数の設定の時と同じようなことができます。");
		//Console.WriteLine("ex) ?x");
		//Console.WriteLine("ex) ?::x");
		//Console.WriteLine("ex) ?0::x");
		//Console.WriteLine("ex) ?0:x");
		//Console.ReadKey();
		Console.WriteLine("「//help」と打つと、短いヘルプが出ます。");
		Console.ReadKey();
		//Console.WriteLine("「//reference」と打つと、主な関数の使い方が出ます。");
		//Console.ReadKey();
		Console.WriteLine("(終わり)");
	}
	else if (s.StartsWith("//"));
	else
	{
		try
		{
			var trees= parserAndExecuter.Parse(s);
			System.Diagnostics.Debug.WriteLine(String.Join("\r\n-----\r\n", trees));
			var reses = parserAndExecuter.Run(trees);
			if(reses.Any()) Console.WriteLine(reses.Last().ToString());
		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}
	}
}
class Test
{
	[ExportMethod]
	public object print(List<object> list) => ParserAndExecuter.retjunk(_ => Console.WriteLine(_.First().ToString()))(list);
	[ExportConstant("zero")]
	public int i = 0;
	[ExportConstant()]
	public string text => "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
}