// See https://aka.ms/new-console-template for more information

using System.Reflection;
using System.Text;

namespace EasyScript
{
	public class ParserAndExecuter
	{

		[Serializable]
		public class PackingException : Exception
		{
			public PackingException() { }
			public PackingException(string message) : base(message) { }
			public PackingException(string message, Exception inner) : base(message, inner) { }
			protected PackingException(
			  System.Runtime.Serialization.SerializationInfo info,
			  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
		}
		public class ReferenceBuilder
		{
			string methods = "", constants = "";
			Dictionary<string,MethodInfo> methoddatas = new();
			Dictionary<string, (Type type, string? summary)> constantdatas=new();
			public void AddMethod(MethodInfo methodInfo, string name)
			{
				methoddatas[name]= methodInfo;
			}
			public void AddConstants(string name,Type type, string? summary = null)
			{
				constantdatas[name] = (type,summary);
			}
			public void AddConstantstring(string name, Type type, string? summary)
			{
				constants += $"{type.FullName} {name}\r\n";
				if(summary is not null) constants += "\t説明:\r\n\t\t" + summary.Replace("\r\n", "\r\n\t\t") + "\r\n";
			}
			public void AddMethodstring(MethodInfo methodInfo, string name)
			{
				var opt = methodInfo.GetParameters().Where(p => p.IsOptional);
				methods += $"{methodInfo.ReturnType.FullName} {name} ({ string.Join(", ", methodInfo.GetParameters().Where(p=>!p.IsOptional).ToList().ConvertAll(p => $"{(p.GetCustomAttributes<ParamArrayAttribute>().Any() ? "..." : "")}{p.ParameterType.FullName} {p.Name}"))+(opt.Any()?$"[, {string.Join(", ", opt.ToList().ConvertAll(p => $"{(p.GetCustomAttributes<ParamArrayAttribute>().Any() ? "..." : "")}{p.ParameterType.FullName} {p.Name}"))}]":"") })" + "\r\n";
				var sum = methodInfo.GetCustomAttributes(typeof(SummaryAttribute));
				if (sum.Any())
				{
					methods += "\t説明:\r\n\t\t" + ((SummaryAttribute)sum.First()).Description.Replace("\r\n", "\r\n\t\t") + "\r\n";
				}

				foreach (var item in methodInfo.GetParameters())
				{
					var psum = item.GetCustomAttributes(typeof(SummaryAttribute));
					if (psum.Any())
					{
						methods += $"\t{item.Name}:\r\n\t\t" + ((SummaryAttribute)psum.First()).Description.Replace("\r\n", "\r\n\t\t") + "\r\n";
					}
				}
				var rsum = methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(typeof(SummaryAttribute),false).Concat(methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(typeof(SummaryAttribute), true));
				if (rsum.Any())
				{
					methods += $"\t返り値:\r\n\t\t" + ((SummaryAttribute)rsum.First()).Description.Replace("\r\n", "\r\n\t\t") + "\r\n";
				}
			}
			public static explicit operator string (ReferenceBuilder self)
			{
				self.methods = "";
				self.constants = "";
				foreach (var (name, methodInfo) in self.methoddatas)
				{
					self.AddMethodstring(methodInfo, name);
				}
				foreach (var (name, (type, summary)) in self.constantdatas)
				{
					self.AddConstantstring(name, type, summary);
				}
				string res = "";
				res += "メソッド:\r\n" + self.methods;
				res += "定数:\r\n" + self.constants;
				return res;
			}
			public override string ToString()
			{
				return (string)this;
			}
		}
		public readonly ReferenceBuilder referenceBuilder = new();
		record InternalExtention(ParserAndExecuter This)
		{

			[ExportConstant("true"),Summary("真偽値の「真」を表す値。")]
			public bool t = true;
			[ExportConstant("false"), Summary("真偽値の「偽」を表す値。")]
			public bool f = false;
			[ExportConstant, Summary("何もないことを表す「null」という特別な値。")]
			public Null @null = Null;
			[ExportMethod("if"), Summary("条件分岐を行うための関数。")]
			[return:Summary("ブロックが計算した値か「null」")]
			public static object If([Summary("条件")] bool con, [Summary("条件が真のときに計算するブロック。")] BlockType t, [Summary("条件が偽のときに計算するブロック。")] BlockType? f = null)
			{
				f ??= new(_ => Null);
				return con ? t() : f();
			}
			[ExportMethod("while"), Summary("繰り返しを行うための関数。条件が真の間繰り返す。")]
			[return: Summary("ブロックが最後に計算した値か「null」")]
			public object While([Summary("繰り返しの条件の式")] Node con, [Summary("繰り返し計算するブロック。")] BlockType t)
			{
				object res = Null;
				while (This.Run(con) as bool? ?? false)
				{
					res = t();
				}
				return res;
			}
			[ExportMethod, Summary("右と左を足す関数。")]
			[return:Summary("答え")]
			public long add(long l, long r)
			{
				return l + r;
			}
			[ExportMethod, Summary("右と左を足す関数。")]
			[return: Summary("答え")]
			public double addf(double l, double r)
			{
				return l + r;
			}
			[ExportMethod, Summary("右から左を引く関数。")]
			[return: Summary("答え")]
			public long sub(long l, long r)
			{
				return l - r;
			}
			[ExportMethod, Summary("右から左を引く関数。")]
			[return: Summary("答え")]
			public double subf(double l, double r)
			{
				return l - r;
			}
			[ExportMethod, Summary("引数の真偽の反転を行う関数。")]
			[return: Summary("答え")]
			public bool not(bool @in) => !@in;
			[ExportMethod, Summary("ブロックを計算する。")]
			[return: Summary("計算された値")]
			public object run([Summary("計算するブロック")] BlockType block)
			{
				return block();
			}
			[ExportMethod, Summary("式を計算する。")]
			[return: Summary("計算された値")]
			public object eval([Summary("計算する式")] Node node)
			{
				return This.Run(node);
			}
			[ExportMethod, Summary("データを文字列にする。")]
			[return: Summary("文字列になったデータ。")]
			public string serialize([Summary("文字列にしたいデータ")] object obj)
			{
				return This.Serialize(obj);
			}
			[ExportMethod, Summary("文字列にしたデータをもとに戻す。")]
			[return: Summary("もとに戻ったデータ。")]
			public object deserialize([Summary("もとに戻したいデータ")] string s)
			{
				return This.Deserialize(s);
			}
            [ExportMethod, Summary("関数を定義する関数。")]
            [return: Summary("定義された関数。")]
            public MethodType func([Summary(@"関数の名前。「""""」とすると、無名関数になる。")] string name, [Summary("関数の本体。引数を変数として使用できる。")] BlockType blockType, [Summary("引数。何個でも入れることができる。「argdata」で得られた値でないと、エラーになる。")] params object[] datas)
			{
				List<ArgData> t = datas.ToList().ConvertAll(t => (ArgData)t);
				var res= new MethodType((args) =>
				{
					This.funccallstackdepthes.Push(This.Variables.Count);
					Dictionary<string, object> dic = new();
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
				}).checkParamCount(t.FindIndex(x=>x.@default is not null), t.Count,name);
				if (name == "")
				{
					return res;
				}
				else return This.Methods[name] = res;
			}
		}
		public ParserAndExecuter()
		{

			Methods["//get"] = new MethodType(lst =>
			{
				try
				{
					return Variables.Peek()[(string)lst[0]];
				}
				catch (KeyNotFoundException)
				{
					throw new ExecutionException($"変数\"{(string)lst[0]}\"は、一度も使われていません。スコープ外の変数を参照するには、「gets」や「getg」をお使いください。");
				}
			}).checkParamCount(1, name:"//get");
			Methods["//set"] = new MethodType(lst =>
			{
				return Variables.Peek()[(string)lst[0]] = lst[1];
			}).checkParamCount(2, name: "//set");
			Methods["//getv"] = 
				new MethodType(lst => { 
					try { 
						Stack<Dictionary<string, object>> stk = new(Variables.Reverse()); 
						Dictionary<string, object> dict = stk.Pop(); 
						for (; stk.Count > funccallstackdepthes.Peek() && !dict.ContainsKey((string)lst[0]); dict = stk.Pop()) ; 
						if (!dict.ContainsKey((string)lst[0])) { 
							dict = Variables.Last();
						} 
						if (!dict.ContainsKey((string)lst[0])) { 
							dict = Constants; 
						} 
						return 
						dict[(string)lst[0]]; 
					} catch (KeyNotFoundException) { 
						throw new ExecutionException($"変数\"{(string)lst[0]}\"は、定義されていません。ラムダ関数では、グローバル関数以外の外の変数を参照できません。"); 
					} 
				}).checkParamCount(1, name: "//getv");
			Methods["//setv"] = 
				new MethodType(lst => { try { Stack<Dictionary<string, object>> stk = new(Variables); Dictionary<string, object> dict = stk.Pop(); for (; stk.Count > funccallstackdepthes.Peek() && !dict.ContainsKey((string)lst[0]); dict = stk.Pop()) ; if (!dict.ContainsKey((string)lst[0])) { dict = Variables.Last(); } if (!dict.ContainsKey((string)lst[0])) { throw new ExecutionException($"変数\"{(string)lst[0]}\"は、定義されていません。ラムダ関数では、グローバル関数以外の外の変数を設定できません。"); } return dict[(string)lst[0]] = lst[1]; } catch (KeyNotFoundException) { throw new ExecutionException($"変数\"{(string)lst[0]}\"は、定義されていません。ラムダ関数では、グローバル関数以外の外の変数を設定できません。"); } }).checkParamCount(2, name: "//setv");
			Methods["//setg"] = 
				new MethodType(lst => { return Variables.Last()[(string)lst[0]] = lst[1]; }).checkParamCount(2, name: "//setg");
			Methods["func"] = new MethodType(lst => new MethodType((args) =>
			{
				funccallstackdepthes.Push(Variables.Count);
				List<ArgData> t = lst.ToArray()[1..].ToList().ConvertAll(o => (ArgData)o);
				Dictionary<string, object> dic = new();
				for (int i = 0; i < t.Count; i++)
				{
					var data = t[i];
					if (i< args.Count)
					{
						var arg = args[i];
						dic[data.name] = arg ?? data.@default ?? throw new Exception();
					}
					else
					{
						dic[data.name] = data.@default ?? throw new Exception();
					}
				}
				var res= ((BlockType)lst[0])(dic);
				funccallstackdepthes.Pop();
				return res;
			})).checkParamCount(1,-1, name: "func");
			Methods["deffunc"] = new MethodType(lst => Methods[(string)lst[0]]=(MethodType)lst[1]).checkParamCount(2, name: "deffunc");
			Methods["while"] = new MethodType(lst =>
			{
				object res = new();
				while ((bool)Run((Node)lst[0])) res = (lst[1] as BlockType ?? ((_) => new()))();
				return res;
			}).checkParamCount(2, name: "while");
			Import(new InternalExtention(this), true);
			Serializers.Add(new InternalSerializer());
			//Methods["//getargs"] = lst =>;
		}
		public class InternalSerializer : ISerializer
		{
			public bool CanDeserialize(string s)
			{
				return s.StartsWith("#") || s.StartsWith("%") || (s.StartsWith('"') && s.EndsWith('"')) || (s.StartsWith('[') && s.EndsWith(']'));

			}

			public bool CanSerialize(object obj)
			{
				return obj is long or double or string or List<object>;
			}

			public object Deserialize(string s)
			{
				if (s.StartsWith("#"))
				{
					return int.Parse(s[1..]);
				}
				else if (s.StartsWith("%"))
				{
					return float.Parse(s[1..]);
				}
				else if ((s.StartsWith('"') && s.EndsWith('"')))
				{
					s = s[1..^1];
					char c = '\0';
					bool Next()
					{
						c = s[0];
						s = s[1..];
						return s.Length > 0;
					}
					string cur = "";
					while (Next())
					{
						if (c=='\\')
						{
							Next();
						}
						cur += c;
					}
					return cur;
				}
				else if ((s.StartsWith('[') && s.EndsWith(']')))
				{
					s=s[1..^1];
					string cur = "";
					char c = '\0';
					List<string> list = new();
					bool Next()
					{
						c = s[0];
						s = s[1..];
						return s.Length > 0;
					}
					int depth = 0;
					bool instring=false;
					while (Next())
					{
						if (depth == 0 && c == ',')
						{
							list.Add(cur);
							cur = "";
						}
						else if (!instring && c == '[')
						{
							depth++;
						}
						else if (!instring && c == ']')
						{
							depth--;
						}
						else if (c=='"')
						{
							instring = !instring;
						}
						else if (c == '\\')
						{
							Next();
							cur += c;
						}
						else
						{
							cur += c;
						}
					}
					return list.ConvertAll(s=>Deserialize(s));
				}
				else throw new NotSupportedException();
			}

			public string Serialize(object obj)
			{
				if (obj is long l)
				{
					return $"#{l}";
				}
				else if (obj is double d)
				{
					return $"%{d}";
				}
				else if (obj is string s)
				{
					return $"\"{s.Replace("\"", @"\""").Replace("\\", @"\\")}\"";
				}
				else if (obj is List<object> lst)
				{
					return $"[{string.Join(",", lst.ConvertAll(Serialize))}]";
				}
				else throw new NotSupportedException();
			}
		}
		#region Configs
		public readonly string[] separators = new string[] { 
			" ",
			"(",
			")",
			"{",
			"}",
			",",
			"\"",
			"=",
			":",
		};
		public readonly Dictionary<string, MethodType> Methods = new()
		{
			["if"] = new MethodType(lst => 
			(bool)lst[0] 
			? 
			(lst[1] as BlockType ?? ((_) => Null))() 
			: 
			((lst.Count < 3 ? null: lst[2] as BlockType) 
			?? 
			((_) => Null))()).checkParamCount(2,3,"if"),
			
			["run"] = new MethodType(lst => (lst[0] as BlockType ?? ((_) => Null))()).checkParamCount(1, name: "run"),
			["equals"] = new MethodType(lst => lst[0].Equals(lst[1])).checkParamCount(2, name: "equals"),
			["argdata"] = new MethodType(lst => new ArgData((string)lst[0], lst.Count < 2 ? null : lst[1])).checkParamCount(1, 2, "argdata"),
			["list"] = new MethodType(lst => lst).checkParamCount(0,-1, "list"),
		};
		public List<string> doesnotMakeScope = new()
		{
			"//get",
			"//set",
			"//getv",
			"//setv",
		};
		public Dictionary<string, object> Constants = new()
		{
			["true"] = true,
			["false"] = false,
			["null"] = Null.Instance,
		};
		#endregion
		#region Parser
		public List<Node> Parse(string s)
		{
			var GetSpliter = (string sep) => (List<string> lst, string s) =>
			{
				var tmp = s.Split(sep);
				for (int i = 0; i < tmp.Length - 1; i++)
				{
					lst.Add(tmp[i]);
					lst.Add(sep);
				}
				lst.Add(tmp[^1]);
				return lst;
			};
			var GetSpliters = (string[] seps) => (IEnumerable<string> ie) =>
			{
				var lst=new List<string>(ie);
				foreach (var item in seps)
				{
					lst=lst.Aggregate(new List<string>(), GetSpliter(item));
				}
				return lst;
			};
			var tokens = new Deq<string>(new string[]
			{
				s
				.Replace("$", "$_")
				.Replace("\\\\", "$bs")
				.Replace("\\ ", "$sp")
				.Replace("\\(", "$lp")
				.Replace("\\)", "$rp")
				.Replace("\\{", "$lb")
				.Replace("\\}", "$rb")
				.Replace("\\,", "$cm")
				.Replace("\\.", "$pr")
				.Replace("\\\"", "$dq")
			}
				.Apply(GetSpliters(separators))
				.ConvertAll(s => s
				.Replace("$sp", "\\ ")
				.Replace("$lp", "\\(")
				.Replace("$rp", "\\)")
				.Replace("$lb", "\\{")
				.Replace("$rb", "\\}")
				.Replace("$cm", "\\,")
				.Replace("$pr", "\\.")
				.Replace("$dq", "\\\"")
				.Replace("$bs", "\\\\")
				.Replace("$_", "$")));
			var newtokens = new List<string>();
			for (; tokens.Count != 0;)
			{
				var tok = tokens.Dequeue();
				if (tok == "\"")
				{
					var newtok = tok;
					tok = tokens.Dequeue();
					while (tok != "\"")
					{
						newtok += tok;
						tok = tokens.Dequeue();
					}
					newtok += tok;
					newtokens.Add(newtok
					);
				}
				else
				{
					if(tok!="")newtokens.Add(tok);
				}
			}
			tokens = new(new string[] { "," }.Concat(newtokens));
			List<Node> nodes = new();
			try
			{
				for (; tokens.Count != 0;)
				{
					while (tokens.Count != 0 && tokens.Peek() == " ") tokens.Dequeue();
					if (tokens.Count == 0) break;
					if (tokens.Dequeue() != ",") throw new SyntaxException("文を続ける場合は、「,」が必要です。");
					while (tokens.Count != 0 && tokens.Peek() == " ") tokens.Dequeue();
					if (tokens.Count == 0) break;
					nodes.Add(parseLoop(tokens));
				}
			}
			catch (Exception ex)
			{
				throw new SyntaxException("構文が間違っています。", ex);
			}
			return nodes;
		}
		#region old
		Node parseLoop_old(Deq<string> tokens)
		{
		start:;
			string tok = tokens.Dequeue();
			if (tok == " ") goto start;
			else if (tok.StartsWith("\""))
			{
				StringBuilder stringBuilder = new();
				char c;
				for (var ie = tok[1..^1].GetEnumerator(); ie.MoveNext();)
				{
					c = ie.Current;
					if (c == '\\')
					{
						if (!ie.MoveNext()) throw new SyntaxException("「\\」の後には文字が来なければなりません。");
						c = ie.Current;
						switch (c)
						{
							default:
								stringBuilder.Append(c);
								break;
						}
					}
					else
					{
						stringBuilder.Append(c);
					}
				}
				return new Node(stringBuilder.ToString(), NodeType.lit, new());
			}
			else if (tok == "{")
			{
				var @params = new List<Node>();
				while (tokens.Peek() == " ") tokens.Dequeue();
				if (tokens.Peek() == "}")
				{
					tokens.Dequeue();
					return new Node(" ", NodeType.call, @params);
				}
			blockstart:;
				@params.Add(parseLoop(tokens));
				string s;
				while (tokens.Peek() == " ") tokens.Dequeue();
				if ((s = tokens.Dequeue()) == ",") goto blockstart;
				else if (s == "}") return new Node(" ", NodeType.block, @params);
				throw new SyntaxException("ブロックは、「,」で区切られた式である必要があります。");
			}
			else if (tok.StartsWith("#"))
			{
				return new Node(tok[1..], NodeType.@int, new());
			}
			else if (tok.StartsWith("%"))
			{
				return new Node(tok[1..], NodeType.flo, new());
			}
			else if (tok.StartsWith("!"))
			{
				return new Node(tok[1..], NodeType.cst, new());
			}
			else if (tok.StartsWith("?"))
			{
				tok = tok[1..];
				if (tok.StartsWith("::")) return new("//getg", NodeType.call, new() { new(tok[2..], NodeType.lit, new()) });
				if (tok.Contains("::")) return new("//gets", NodeType.call, new() { new(tok[..tok.IndexOf("::")], NodeType.@int, new()), new(tok[(tok.IndexOf("::") + 2)..], NodeType.lit, new()) });
				if (tok.Contains(':')) return new("//getsb", NodeType.call, new() { new(tok[..tok.IndexOf(':')], NodeType.@int, new()), new(tok[(tok.IndexOf(':') + 1)..], NodeType.lit, new()) });
				else
					return new Node("//get", NodeType.call, new() { new(tok, NodeType.lit, new()) });
			}
			else
			{
				var name = tok;
				while (tokens.Peek() == " ") tokens.Dequeue();
				string next = tokens.Dequeue();
				if (next == "(")
				{
					var @params = new List<Node>();
					while (tokens.Peek() == " ") tokens.Dequeue();
					if (tokens.Peek() == ")")
					{
						tokens.Dequeue();
						return new Node(name, NodeType.call, @params);
					}
				paramstart:;
					@params.Add(parseLoop(tokens));
					string s;
					while (tokens.Peek() == " ") tokens.Dequeue();
					if ((s = tokens.Dequeue()) == ",") goto paramstart;
					else if (s == ")") return new Node(name, NodeType.call, @params);
					throw new SyntaxException("パラメーターは「,」で区切られた式である必要があります。");
				}
				else if (next == ("="))
				{
					if (tok.StartsWith("::")) return new("//setg", NodeType.call, new() { new(tok[2..], NodeType.lit, new()), parseLoop(tokens) });
					if (tok.Contains("::")) return new("//sets", NodeType.call, new() { new(tok[..tok.IndexOf("::")], NodeType.@int, new()), new(tok[(tok.IndexOf("::") + 2)..], NodeType.lit, new()), parseLoop(tokens) });
					if (tok.Contains(':')) return new("//setsb", NodeType.call, new() { new(tok[..tok.IndexOf(':')], NodeType.@int, new()), new(tok[(tok.IndexOf(':') + 1)..], NodeType.lit, new()), parseLoop(tokens) });
					else
						return new("//set", NodeType.call, new() { new(tok, NodeType.lit, new()), parseLoop(tokens) });
				}
				else throw new SyntaxException("関数名の後にはパラメーターが来るべきです。");
			}
		}
		#endregion
		Node parseLoop(Deq<string> tokens)
		{
		start:;
			string tok = tokens.Dequeue();
			if (tok == " ") goto start;
			else if (('0' <= tok[0] && '9' >= tok[0]) || (tok[0] == '-' && ('0' <= tok[1] && '9' >= tok[1])))
				return (tok.Contains('.') || tok.Contains('e')) ? new Node(tok, NodeType.flo, new()) : new(tok, NodeType.@int, new());
			else if (tok.StartsWith("\""))
			{
				StringBuilder stringBuilder = new();
				char c;
				for (var ie = tok[1..^1].GetEnumerator(); ie.MoveNext();)
				{
					c = ie.Current;
					if (c == '\\')
					{
						if (!ie.MoveNext()) throw new SyntaxException("「\\」の後には文字が来なければなりません。");
						c = ie.Current;
						switch (c)
						{
							default:
								stringBuilder.Append(c);
								break;
						}
					}
					else
					{
						stringBuilder.Append(c);
					}
				}
				return new Node(stringBuilder.ToString(), NodeType.lit, new());
			}
			else if (tok == "{")
			{
				var @params = new List<Node>();
				while (tokens.Peek() == " ") tokens.Dequeue();
				if (tokens.Peek() == "}")
				{
					tokens.Dequeue();
					return new Node(" ", NodeType.block, @params);
				}
			blockstart:;
				@params.Add(parseLoop(tokens));
				string s;
				while (tokens.Peek() == " ") tokens.Dequeue();
				if ((s = tokens.Dequeue()) == ",") goto blockstart;
				else if (s == "}") return new Node(" ", NodeType.block, @params);
				throw new SyntaxException("ブロックは、「,」で区切られた式である必要があります。");
			}
			else if (tok.StartsWith("#"))
			{
				return new Node(tok[1..], NodeType.@int, new());
			}
			else if (tok.StartsWith("%"))
			{
				return new Node(tok[1..], NodeType.flo, new());
			}
			else if (tok == "@")
			{
				string next = tokens.Dequeue();
				if (next == "(")
				{
					var @params = new List<Node>();
					while (tokens.Peek() == " ") tokens.Dequeue();
					if (tokens.Peek() == ")")
					{
						throw new SyntaxException("「@」の後には、「(」と「)」で囲まれた式が来なければなりません。");
					}
					@params.Add(parseLoop(tokens));
					while (tokens.Peek() == " ") tokens.Dequeue();
					if (tokens.Dequeue() == ")") return new Node("", NodeType.expression, @params);
					throw new SyntaxException("「@」の後には、「(」と「)」で囲まれた式が来なければなりません。");
				}
				else throw new SyntaxException("「@」の後には、「(」と「)」で囲まれた式が来なければなりません。"); ;
			}
			else if (tok == "$")
			{
				string next = tokens.Dequeue();
				if (next == "(")
				{
					var @params = new List<Node>();
					while (tokens.Peek() == " ") tokens.Dequeue();
					if (tokens.Peek() == ")")
					{
						throw new SyntaxException("「$」の後には、「(」と「)」で囲まれた式が来なければなりません。");
					}
					@params.Add(parseLoop(tokens));
					while (tokens.Peek() == " ") tokens.Dequeue();
					if (tokens.Dequeue() == ")") return new Node("", NodeType.precalc, @params);
					throw new SyntaxException("「$」の後には、「(」と「)」で囲まれた式が来なければなりません。");
				}
				else throw new SyntaxException("「$」の後には、「(」と「)」で囲まれた式が来なければなりません。"); ;
			}
			else
			{
				var name = tok;
				while (tokens.Count>0 && tokens.Peek() == " ") tokens.Dequeue();
				if (tokens.Count > 0)
				{
					string next = tokens.Dequeue();
					if (next == "(")
					{
						var @params = new List<Node>();
						while (tokens.Peek() == " ") tokens.Dequeue();
						if (tokens.Peek() == ")")
						{
							tokens.Dequeue();
							return new Node(name, NodeType.call, @params);
						}
					paramstart:;
						@params.Add(parseLoop(tokens));
						string s;
						while (tokens.Peek() == " ") tokens.Dequeue();
						if ((s = tokens.Dequeue()) == ",") goto paramstart;
						else if (s == ")") return new Node(name, NodeType.call, @params);
						throw new SyntaxException("パラメーターは「,」で区切られた式である必要があります。");
					}
					else if (next == ("="))
					{
						return new("//setv", NodeType.call, new() { new(tok, NodeType.lit, new()), parseLoop(tokens) });
					}
					else if (next == (":"))
					{
						if(tokens.Peek() == ("="))
						{
							tokens.Dequeue();
							return new("//set", NodeType.call, new() { new(tok, NodeType.lit, new()), parseLoop(tokens) });
						}
						else if(tokens.Peek() == (":")&& tokens.ElementAt(1)=="=")
						{
							tokens.Dequeue();
							tokens.Dequeue();
							return new("//setg", NodeType.call, new() { new(tok, NodeType.lit, new()), parseLoop(tokens) });
						}
						else
						{
							tokens.REnqueue(next);
							goto elsebr;
						}
					}
					else
					{
						tokens.REnqueue(next);
						goto elsebr;
					}
				}
				else goto elsebr;
				elsebr:;
				return new("//getv", NodeType.call, new() { new(tok, NodeType.lit, new()) }); ;
			}
		}
		#endregion
		#region Executer
		public Stack<int> blockcallstackdepthes = new();
		public Stack<int> funccallstackdepthes = new(new[] { 0 });
		public static MethodType retjunk(Action<List<object>> action) => (t) =>
		{
			action(t);
			return Null;
		};
		public static Null Null => Null.Instance;

		public List<ISerializer> Serializers => serializers;
		public readonly Stack<Dictionary<string, object>> Variables = new();
		public object Run(Node node)
		{
			if(node.type == NodeType.call && !doesnotMakeScope.Contains(node.text)) Variables.Push(new());
			try
			{
				var res = node.type switch
				{
					NodeType.call => Methods.ContainsKey(node.text)? Methods[node.text](node.Child.ConvertAll(n => Run(n))) : throw new ExecutionException($"関数\"{node.text}\"は、定義されていません。"),
					NodeType.block => new BlockType((dict) => {
						blockcallstackdepthes.Push(Variables.Count);
						Variables.Push(new(dict??new()));
						var res_ = node.Child.ConvertAll(n => Run(n)).LastOrDefault(Null);
						Variables.Pop(); 
						blockcallstackdepthes.Pop();
						return res_;
					}),
					NodeType.lit => node.text,
					NodeType.@int => Convert.ToInt64(node.text),
					NodeType.flo => Convert.ToDouble(node.text),
					NodeType.cst => Constants[node.text],
					NodeType.value => Deserialize(node.text),
					NodeType.expression => check(node.Child[0],1),
					NodeType.precalc => throw new ExecutionException("「$(...)」は、「@(...)」の中でしか使えません。")/*Run(node.Child[0])*/,
					_ => throw new NotSupportedException(nameof(node.type)),
				};
				if(node.type == NodeType.call && !doesnotMakeScope.Contains(node.text)) Variables.Pop();
				return res;
			}
			catch (ExecutionException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ExecutionException("実行時に例外が起きました。", ex);
			}
		}
		Node check(Node node,int depth)
		{
			switch (node.type)
			{
				case NodeType.expression:
					depth++;
					break;
				case NodeType.precalc:
					depth--;
					break;
				default:
					break;
			}
			var res = new Node(node.text, node.type, new());
			if (depth==0)
			{
				res = new(Serialize(Run(node.Child.First())), NodeType.value, new());
			}
			else
			{
				foreach (var item in node.Child)
				{
					res.Child.Add(check(item, depth));
				}
			}
			return res;
		}
		public string Serialize(object obj)
		{
			bool b = false;
			string? s = null;

			for (int i = 0; !b && i < serializers.Count; i++)
			{
				b = serializers[i].TrySerialize(obj, out s);
			}
			if (!b)
			{
				s = null;
			}
			return s ?? throw new InvalidOperationException($"「{obj.GetType().FullName}」型のデータは、文字列にできず、定数にもできません。");
		}
		public object Deserialize(string s)
		{
			bool b = false;
			object? obj = null;

			for (int i = 0; !b && i < serializers.Capacity; i++)
			{
				b = serializers[i].TryDeserialize(out obj, s);
			}
			if (!b)
			{
				obj = null;
			}
			return obj ?? throw new InvalidOperationException();

		}

		private readonly List<ISerializer> serializers = new();
		public interface ISerializer
		{
			string Serialize(object obj);
			object Deserialize(string s);
			bool CanSerialize(object obj);
			bool CanDeserialize(string s);
			bool TrySerialize(in object obj, out string? s)
			{
				if (CanSerialize(obj))
				{
					s = Serialize(obj);
					return true;
				}
				else
				{
					s =null;
					return false;
				}
			}
			bool TryDeserialize(out object? obj, in string s)
			{
				if (CanDeserialize(s))
				{
					obj = Deserialize(s);
					return true;
				}
				else
				{
					obj = null;
					return false;
				}
			}
		}
		public List<object> Run(IEnumerable<Node> nodes)
		{
			while (Variables.Count > 1) Variables.Pop();
			if (Variables.Count < 1) Variables.Push(new());
			blockcallstackdepthes.Clear();
			funccallstackdepthes.Clear();
			funccallstackdepthes.Push(0);

			return nodes.Aggregate(Array.Empty<object>() as IEnumerable<object>, (current, node) => current.Concat(new object[] { Run(node) })).ToList();
		}
		#endregion
		public List<object> Execute(string s) => Run(Parse(s));
		public uint Import(object o, bool allowoverride = false)
		{
			uint count = 0;
			foreach (var item in o.GetType().GetMethods())
			{
				var tmp = item.GetCustomAttributes(typeof(ExportMethodAttribute), false).Concat(item.GetCustomAttributes(typeof(ExportMethodAttribute), true));
				if (tmp.Any())
				{
					ExportMethodAttribute attr = tmp.First() as ExportMethodAttribute ?? throw new();
					if (!allowoverride)
					{
						if (Methods.ContainsKey(attr.Name ?? item.Name)) continue;
					}
					System.Reflection.ParameterInfo[] @params = item.GetParameters();
					int min = 0, max= @params.Length;
					for (; min < @params.Length; min++)
					{
						if(@params[min].IsOptional) break;
					}
					var defs= @params.ToList().ConvertAll(p => p.DefaultValue);
					Methods[attr.Name ?? item.Name] = (@params.Any()&&@params.Last().GetCustomAttributes(typeof(ParamArrayAttribute), false).Any()) ?new MethodType(lst_ =>
					{
						object?[] defaultargs = defs.ToArray();
						Array.Resize(ref defaultargs, Math.Max(defaultargs.Length, lst_.Count));
						lst_.CastNullable().CopyTo(defaultargs);
						List<object?> lst =defaultargs.ToList();
						return item.Invoke(o, lst.ToArray()[..(max - 1)].Concat(new[] { lst.ToArray()[(max - 1)..] }).ToArray()) ?? Null;
					}).checkParamCount((min <= max - 1?min:max-1), -1, attr.Name ?? item.Name) : new MethodType(lst_ =>
					{
						object?[] defaultargs = defs.ToArray();
						Array.Resize(ref defaultargs, Math.Max(defaultargs.Length, lst_.Count));
						lst_.CastNullable().CopyTo(defaultargs);
						List<object?> lst = defaultargs.ToList();
						return item.Invoke(o, lst.ToArray()) ?? Null;
					}).checkParamCount(min, max, attr.Name ?? item.Name);
					referenceBuilder.AddMethod(item, attr.Name ?? item.Name);
					count++;
				}
			}
			foreach (var item in o.GetType().GetProperties())
			{
				var tmp = item.GetCustomAttributes(typeof(ExportConstantAttribute), false).Concat(item.GetCustomAttributes(typeof(ExportMethodAttribute), true));
				if (tmp.Any())
				{
					ExportConstantAttribute attr = tmp.First() as ExportConstantAttribute ?? throw new();
					if (!allowoverride)
					{
						if (Constants.ContainsKey(attr.Name ?? item.Name)) continue;
					}
					var tmp__ = item.GetValue(o);
					if (tmp__ is not null)
					{
						Constants[attr.Name ?? item.Name] = tmp__;
						var ien = item.GetCustomAttributes(typeof(SummaryAttribute));
						if (ien.Any())
						{
							referenceBuilder.AddConstants(attr.Name ?? item.Name, item.PropertyType,((SummaryAttribute)ien.First()).Description);
						}
						else
						{
							referenceBuilder.AddConstants(attr.Name ?? item.Name, item.PropertyType);
						}
						count++;
					}
				}
			}
			foreach (var item in o.GetType().GetFields())
			{
				var tmp = item.GetCustomAttributes(typeof(ExportConstantAttribute), false).Concat(item.GetCustomAttributes(typeof(ExportMethodAttribute), true));
				if (tmp.Any())
				{
					ExportConstantAttribute attr = tmp.First() as ExportConstantAttribute ?? throw new();
					if (!allowoverride)
					{
						if (Constants.ContainsKey(attr.Name ?? item.Name)) continue;
					}
					var tmp__ = item.GetValue(o);
					if (tmp__ is not null)
					{
						Constants[attr.Name ?? item.Name] = tmp__;
						var ien = item.GetCustomAttributes(typeof(SummaryAttribute));
						if (ien.Any())
						{
							referenceBuilder.AddConstants(attr.Name ?? item.Name, item.FieldType, ((SummaryAttribute)ien.First()).Description);
						}
						else
						{
							referenceBuilder.AddConstants(attr.Name ?? item.Name, item.FieldType);
						}
						count++;
					}
				}
			}
			return count;
		}
	}
}