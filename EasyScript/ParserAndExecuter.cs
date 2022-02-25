// See https://aka.ms/new-console-template for more information

using System.Text;

namespace EasyScript
{
	public class Null
	{
		private Null() { }
		private readonly static Null instance = new();
		public static Null Instance => instance;
		public override string ToString()
		{
			return "null";
		}
	}
	public class ParserAndExecuter
	{
		public ParserAndExecuter()
		{
			Methods["get"] = lst =>
			{
				try
				{
					return Variables.Peek()[(string)lst[0]];
				}
				catch (KeyNotFoundException)
				{
					throw new ExecutionException($"変数\"{(string)lst[0]}\"は、一度も使われていません。スコープ外の変数を参照するには、「gets」や「getg」をお使いください。");
				}
			};
			Methods["set"] = lst =>
			{
				return Variables.Peek()[(string)lst[0]] = lst[1];
			};
			Methods["getv"] = lst =>
			{
				try
				{
					Stack<Dictionary<string, object>> stk = new(Variables);
					Dictionary<string, object> dict= stk.Pop();
					for (; stk.Count > funccallstackdepthes.Peek() && !dict.ContainsKey((string)lst[0]); dict = stk.Pop()) ;
					if (!dict.ContainsKey((string)lst[0]))
					{
						dict = stk.Last();
					}
					if (!dict.ContainsKey((string)lst[0]))
					{
						dict = Constants;
					}
					return dict[(string)lst[0]];
				}
				catch (KeyNotFoundException)
				{
					throw new ExecutionException($"変数\"{(string)lst[0]}\"は、一度も使われていません。ラムダ関数では、外の変数を参照できません。");
				}
			};
			Methods["setv"] = lst =>
			{
				try
				{
					Stack<Dictionary<string, object>> stk = new(Variables);
					Dictionary<string, object> dict = stk.Pop();
					for (; stk.Count > funccallstackdepthes.Peek() && dict.ContainsKey((string)lst[0]); dict = stk.Pop()) ;
					if (!dict.ContainsKey((string)lst[0]))
					{
						throw new ExecutionException($"変数\"{(string)lst[0]}\"は、一度も使われていません。ラムダ関数では、外の変数を参照できません。");
					}
					return dict[(string)lst[0]] = lst[1];
				}
				catch (KeyNotFoundException)
				{
					throw new ExecutionException($"変数\"{(string)lst[0]}\"は、一度も使われていません。ラムダ関数では、外の変数を参照できません。");
				}
			};
			Methods["gets"] = lst =>
			{
				try
				{
					return Variables.Reverse().ToArray()[(long)lst[0]][(string)lst[1]];
				}
				catch (KeyNotFoundException)
				{
					throw new ExecutionException($"変数\"{(long)lst[0]}::{(string)lst[0]}\"は、一度も使われていません。スコープの数が間違ってないかどうか、確認してください。");
				}
			};
			Methods["sets"] = lst =>
			{
				return Variables.Reverse().ToArray()[(long)lst[0]][(string)lst[1]] = lst[2];
			};
			Methods["getsb"] = lst =>
			{
				try
				{
					return Variables.Reverse().ToArray()[(long)lst[0]+blockcallstackdepthes.Peek()][(string)lst[1]];
				}
				catch (KeyNotFoundException)
				{
					throw new ExecutionException($"変数\"{(long)lst[0]}:{(string)lst[0]}\"は、一度も使われていません。スコープの数が間違ってないかどうか、確認してください。");
				}
			};
			Methods["setsb"] = lst =>
			{
				return Variables.Reverse().ToArray()[(long)lst[0] + blockcallstackdepthes.Peek()][(string)lst[1]] = lst[2];
			};
			Methods["getg"] = lst =>
			{
				try
				{
					return Variables.Last()[(string)lst[0]];
				}
				catch (KeyNotFoundException)
				{
					throw new ExecutionException($"変数\"::{(string)lst[0]}\"は、一度も使われていません。グローバルスコープにあるかどうか、確認してください。");
				}
			};
			Methods["setg"] = lst =>
			{
				return Variables.Last()[(string)lst[0]] = lst[1];
			};
			Methods["deffunc"] = lst => Methods[(string)lst[0]] = new((arg) =>
			{
				return ((BlockType)lst[0])();
			});
			Methods["argdata"] = lst => new ArgData((string)lst[0], lst.Count<2?null:lst[1]); 
			//Methods["getargs"] = lst =>;
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
			["if"] = lst => 
			(bool)lst[0] ? (lst[1] as BlockType ?? (() => new()))() : ((lst.Count < 2 ? null: lst[2] as BlockType) ?? (() => new()))(),
			["while"] = lst =>
			{
				object res = new();
				while((bool)lst[0]) res = (lst[1] as BlockType ?? (() => new()))();
				return res;
			},
			["run"] = lst=> (lst[0] as BlockType ?? (() => new()))(),
			["equals"] = lst => lst[0].Equals(lst[1]),
		};
		public List<string> doesnotMakeScope = new()
		{
			"get",
			"set",
			"getv",
			"setv",
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
				if (tok.StartsWith("::")) return new("getg", NodeType.call, new() { new(tok[2..], NodeType.lit, new()) });
				if (tok.Contains("::")) return new("gets", NodeType.call, new() { new(tok[..tok.IndexOf("::")], NodeType.@int, new()), new(tok[(tok.IndexOf("::") + 2)..], NodeType.lit, new()) });
				if (tok.Contains(':')) return new("getsb", NodeType.call, new() { new(tok[..tok.IndexOf(':')], NodeType.@int, new()), new(tok[(tok.IndexOf(':') + 1)..], NodeType.lit, new()) });
				else
					return new Node("get", NodeType.call, new() { new(tok, NodeType.lit, new()) });
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
					if (tok.StartsWith("::")) return new("setg", NodeType.call, new() { new(tok[2..], NodeType.lit, new()), parseLoop(tokens) });
					if (tok.Contains("::")) return new("sets", NodeType.call, new() { new(tok[..tok.IndexOf("::")], NodeType.@int, new()), new(tok[(tok.IndexOf("::") + 2)..], NodeType.lit, new()), parseLoop(tokens) });
					if (tok.Contains(':')) return new("setsb", NodeType.call, new() { new(tok[..tok.IndexOf(':')], NodeType.@int, new()), new(tok[(tok.IndexOf(':') + 1)..], NodeType.lit, new()), parseLoop(tokens) });
					else
						return new("set", NodeType.call, new() { new(tok, NodeType.lit, new()), parseLoop(tokens) });
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
						return new("setv", NodeType.call, new() { new(tok, NodeType.lit, new()), parseLoop(tokens) });
					}
					else if (next == (":"))
					{
						if(tokens.Peek() == ("="))
						{
							tokens.Dequeue();
							return new("set", NodeType.call, new() { new(tok, NodeType.lit, new()), parseLoop(tokens) });
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
				if (('0' <= tok[0] && '9' >= tok[0]) || (tok[0] == '-' && ('0' <= tok[1] && '9' >= tok[1])))
					return tok.Contains('.') ? new Node(tok, NodeType.flo, new()) : new(tok, NodeType.@int, new());
				return new("getv", NodeType.call, new() { new(tok, NodeType.lit, new()) }); ;
			}
		}
		#endregion
		#region Executer
		public Stack<int> blockcallstackdepthes = new();
		public Stack<int> funccallstackdepthes = new(new[] { 0 });
		public static MethodType retjunk(Action<List<object>> action) => (t) =>
		{
			action(t);
			return Null.Instance;
		};
		public readonly Stack<Dictionary<string, object>> Variables = new();
		public object Run(Node node)
		{
			if(node.type == NodeType.call && !doesnotMakeScope.Contains(node.text)) Variables.Push(new());
			try
			{
				var res = node.type switch
				{
					NodeType.call => Methods.ContainsKey(node.text)? Methods[node.text](node.Child.ConvertAll(n => Run(n))) : throw new ExecutionException($"関数\"{node.text}\"は、定義されていません。"),
					NodeType.block => new BlockType(() => {
						blockcallstackdepthes.Push(Variables.Count);
						Variables.Push(new());
						var res_ = node.Child.ConvertAll(n => Run(n)).Last();
						Variables.Pop(); 
						blockcallstackdepthes.Pop();
						return res_;
					}),
					NodeType.lit => node.text,
					NodeType.@int => Convert.ToInt64(node.text),
					NodeType.flo => Convert.ToDouble(node.text),
					NodeType.cst => Constants[node.text],
					_ => throw new NotSupportedException(nameof(node.type)),
				};
				if(node.type == NodeType.call && !doesnotMakeScope.Contains(node.text)) Variables.Pop();
				return res;
			}
			catch (Exception ex)
			{
				throw new ExecutionException("実行時に例外が起きました。", ex);
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
					Methods[attr.Name ?? item.Name] = lst => item.Invoke(o, new[] { lst }) ?? new();
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
						count++;
					}
				}
			}
			return count;
		}
	}
}