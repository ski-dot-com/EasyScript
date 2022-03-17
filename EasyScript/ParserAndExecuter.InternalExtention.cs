// See https://aka.ms/new-console-template for more information

namespace EasyScript
{
    public partial class ParserAndExecuter
    {
        record ReplInternalExtention(ParserAndExecuter This)
		{
			[ExportMethod, Explanation("関数を定義する関数。")]
			[return: Explanation("定義された関数。")]
			public FunctionType func([Explanation(@"関数の名前。「""""」とすると、無名関数になる。")] string name, [Explanation("関数の本体。引数を変数として使用できる。")] BlockType blockType, [Explanation("引数。何個でも入れることができる。「argdata」で得られた値でないと、エラーになる。")] params object[] datas)
			{
				List<ArgData> t = datas.ToList().ConvertAll(t => (ArgData)t);
				var res = new FunctionType((args) =>
				{
					This.funccallstackdepthes.Push(This.Variables.Count);
					TypedDictionary<string, object> dic = new();
					for (int i = 0; i < t.Count; i++)
					{
						var data = t[i];
						if (i < args.Count)
						{
							var arg = args[i];
							dic[data.name] = arg ?? data.@default ?? throw new Exception();
						}
						else
						{
							dic[data.name] = data.@default ?? throw new Exception();
						}
					}
					var res = blockType(dic);
					This.funccallstackdepthes.Pop();
					return res;
				}).checkParamCount(t.FindIndex(x => x.@default is not null), t.Count, name);
				if (name == "")
				{
					return res;
				}
				else return This.Functions[name] = res;
			}
			[ExportMethod, Explanation("メソッドを定義する関数。")]
			public void method([Explanation(@"メソッドを定義したいオブジェクト。")] UserDefinedObject obj, [Explanation(@"メソッドの名前。")] string name, [Explanation("メソッドの本体。引数を変数として使用できる。親のオブジェクトを「this」として参照できる。")] BlockType blockType, [Explanation("引数。何個でも入れることができる。「argdata」で得られた値でないと、エラーになる。")] params object[] datas)
			{
				List<ArgData> t = datas.ToList().ConvertAll(t => (ArgData)t);
				var res = new MethodType((self, args) =>
				{
					This.funccallstackdepthes.Push(This.Variables.Count);
					TypedDictionary<string, object> dic = new();
					for (int i = 0; i < t.Count; i++)
					{
						var data = t[i];
						if (i < args.Count)
						{
							var arg = args[i];
							dic[data.name] = arg ?? data.@default ?? throw new Exception();
						}
						else
						{
							dic[data.name] = data.@default ?? throw new Exception();
						}
					}
					dic["this"] = self;
					var res = blockType(dic);
					This.funccallstackdepthes.Pop();
					return res;
				}).checkParamCount(t.FindIndex(x => x.@default is not null), t.Count, name);
				obj.methods[name] = res;
			}
			[ExportMethod]
			[Explanation("変数に型を設定できる。")]
			public void settype([Explanation("変数の名前")] string name, [Explanation("変数につける型")] Type type)
			{
				try
				{
					Stack<TypedDictionary<string, object>> stk = new(This.Variables.Reverse());
					TypedDictionary<string, object> dict = stk.Pop();
					for (; stk.Count > This.funccallstackdepthes.Peek() && !dict.ContainsKey(name); dict = stk.Pop()) ;
					if (!dict.ContainsKey(name))
					{
						dict = This.Variables.Last();
					}
					if (!dict.ContainsKey(name))
					{
						dict = This.Constants;
					}
					dict.SetType(name, type);
				}
				catch (KeyNotFoundException)
				{
					throw new ExecutionException($"変数\"{name}\"は、定義されていません。ラムダ関数では、グローバル関数以外の外の変数を参照できません。");
				}
			}
		}
        record InternalExtention(ExecuterBase This)
		{

			[ExportConstant("true"), Explanation("真偽値の「真」を表す値。")]
			public bool t = true;
			[ExportConstant("false"), Explanation("真偽値の「偽」を表す値。")]
			public bool f = false;
			[ExportConstant, Explanation("何もないことを表す「null」という特別な値。")]
			public Null @null = Null;
			[ExportConstant, Explanation("整数が属する型。小数として扱われている整数は入らない。")]
			public Type inttype = Type.FromSystemType<long>();
			[ExportConstant, Explanation("小数が属する型。「inttype」に属するものは入らない。")]
			public Type floattype = Type.FromSystemType<double>();
			[ExportConstant, Explanation("「true」と「false」が属する型。")]
			public Type booltype = Type.FromSystemType<bool>();
			[ExportMethod("if"), Explanation("条件分岐を行うための関数。")]
			[return: Explanation("ブロックが計算した値か「null」")]
			public static object If([Explanation("条件")] bool con, [Explanation("条件が真のときに計算するブロック。")] BlockType t, [Explanation("条件が偽のときに計算するブロック。")] BlockType? f = null)
			{
				f ??= new(_ => Null);
				return con ? t() : f();
			}
			[ExportMethod("while"), Explanation("繰り返しを行うための関数。条件が真の間繰り返す。")]
			[return: Explanation("ブロックが最後に計算した値か「null」")]
			public object While([Explanation("繰り返しの条件の式")] Node con, [Explanation("繰り返し計算するブロック。")] BlockType t)
			{
				object res = Null;
				while (This.Run(con) as bool? ?? false)
				{
					res = t();
				}
				return res;
			}
			[ExportMethod, Explanation("右と左を足す関数。")]
			[return: Explanation("答え")]
			public long add(long l, long r)
			{
				return l + r;
			}
			[ExportMethod, Explanation("右と左を足す関数。")]
			[return: Explanation("答え")]
			public double addf(double l, double r)
			{
				return l + r;
			}
			[ExportMethod, Explanation("右から左を引く関数。")]
			[return: Explanation("答え")]
			public long sub(long l, long r)
			{
				return l - r;
			}
			[ExportMethod, Explanation("右から左を引く関数。")]
			[return: Explanation("答え")]
			public double subf(double l, double r)
			{
				return l - r;
			}
			[ExportMethod, Explanation("引数の真偽の反転を行う関数。")]
			[return: Explanation("答え")]
			public bool not(bool @in) => !@in;
			[ExportMethod, Explanation("ブロックを計算する。")]
			[return: Explanation("計算された値")]
			public object run([Explanation("計算するブロック")] BlockType block)
			{
				return block();
			}
			[ExportMethod, Explanation("式を計算する。")]
			[return: Explanation("計算された値")]
			public object eval([Explanation("計算する式")] Node node)
			{
				return This.Run(node);
			}
			[ExportMethod, Explanation("データを文字列にする。")]
			[return: Explanation("文字列になったデータ。")]
			public string serialize([Explanation("文字列にしたいデータ")] object obj)
			{
				return This.Serialize(obj);
			}
			[ExportMethod, Explanation("文字列にしたデータをもとに戻す。")]
			[return: Explanation("もとに戻ったデータ。")]
			public object deserialize([Explanation("もとに戻したいデータ")] string s)
			{
				return This.Deserialize(s);
			}
			[ExportMethod]
			[Explanation("引数からリストを作る関数。")]
			[return: Explanation("できたリスト。")]
			public List<object> list([Explanation("リストに含むデータ。")] params object[] vs) => vs.ToList();

			[ExportMethod]
			[Explanation("引数データを作る関数。")]
			[return: Explanation("できた引数データ。")]
			public ArgData argdata([Explanation("引数の名前。")] string s, [Explanation("引数のデフォルト。無ければ必須引数。")] object? def = null) => new(s, def);
			[ExportMethod]
			[Explanation("等しいか調べる関数。数学でいうところの「=」。「equals(a,b)」は、数学の「a=b」と同じようなもの。")]
			[return: Explanation("等しいなら「true」。でなければ、「false」。")]
			public bool equals([Explanation("左の引数。")] object a, [Explanation("右の引数。左の引数と比べられる。")] object b)
			{
				return a.Equals(b);
			}
			[ExportMethod]
			[Explanation("「a」の方が大きいか調べる関数。数学でいうところの「>」。「greater_than(a,b)」は、数学の「a>b」と同じようなもの。")]
			[return: Explanation("「a」の方が大きいなら「true」。でなければ、「false」。")]
			public bool greater_than([Explanation("左の引数。")] IComparable a, [Explanation("右の引数。左の引数と比べられる。")] object b)
			{
				return a.CompareTo(b) > 0;
			}
			[ExportMethod]
			[Explanation("「a」の方が小さいか調べる関数。数学でいうところの「<」。「less_than(a,b)」は、数学の「a<b」と同じようなもの。")]
			[return: Explanation("「a」の方が小さいなら「true」。でなければ、「false」。")]
			public bool less_than([Explanation("左の引数。")] IComparable a, [Explanation("右の引数。左の引数と比べられる。")] object b)
			{
				return a.CompareTo(b) < 0;
			}
			[ExportMethod]
			[Explanation("「a」の方が大きいか調べる関数。数学でいうところの「≥」。「greater_than(a,b)」は、数学の「a≥b」と同じようなもの。")]
			[return: Explanation("「a」の方が大きいなら「true」。でなければ、「false」。")]
			public bool greater_equals([Explanation("左の引数。")] IComparable a, [Explanation("右の引数。左の引数と比べられる。")] object b)
			{
				return a.CompareTo(b) > 0;
			}
			[ExportMethod]
			[Explanation("「a」の方が小さいか調べる関数。数学でいうところの「≤」。「less_than(a,b)」は、数学の「a≤b」と同じようなもの。")]
			[return: Explanation("「a」の方が小さいなら「true」。でなければ、「false」。")]
			public bool less_equals([Explanation("左の引数。")] IComparable a, [Explanation("右の引数。左の引数と比べられる。")] object b)
			{
				return a.CompareTo(b) < 0;
			}
			
			[ExportMethod]
			[Explanation("合併型を作る関数。")]
			[return: Explanation("AまたはBに含まれるならばそれを含み、また、そうでないものは含まない型")]
			public Type uniontype(Type A, Type B) => A | B;
			[ExportMethod]
			[Explanation("交差型を作る関数。")]
			[return: Explanation("Aに含まれて、Bに含まれるならばそれを含み、また、そうでないものは含まない型")]
			public Type intersectiontype(Type A, Type B) => A & B;
			[ExportMethod]
			[Explanation("Aに含まれるものはすべてBに含まれるか調べる関数。")]
			[return: Explanation("Aに含まれるものはすべてBに含まれるならば「true」、でなければ「false」。")]
			public bool extends(Type A, Type B) => B.isSupertypeof(A);
			[ExportMethod]
			[Explanation("新しいオブジェクトを作る。")]
			public UserDefinedObject newobj() => new();
		}
	}
}