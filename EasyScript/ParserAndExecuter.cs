// See https://aka.ms/new-console-template for more information

using System.Reflection;
using System.Text;

namespace EasyScript
{
    public partial class ParserAndExecuter : Executer
    {
        public ParserAndExecuter()
		{

			Functions["//get"] = new FunctionType(lst =>
			{
				try
				{
					return Variables.Peek()[(string)lst[0]];
				}
				catch (KeyNotFoundException)
				{
					throw new ExecutionException($"変数\"{(string)lst[0]}\"は、一度も使われていません。スコープ外の変数を参照するには、「gets」や「getg」をお使いください。");
				}
			}).checkParamCount(1, name: "//get");
			Functions["//set"] = new FunctionType(lst =>
			{
				return Variables.Peek()[(string)lst[0]] = lst[1];
			}).checkParamCount(2, name: "//set");
			Functions["//getv"] =
				new FunctionType(lst => {
					try
					{
						Stack<TypedDictionary<string, object>> stk = new(Variables.Reverse());
						TypedDictionary<string, object> dict = stk.Pop();
						for (; stk.Count > funccallstackdepthes.Peek() && !dict.ContainsKey((string)lst[0]); dict = stk.Pop()) ;
						if (!dict.ContainsKey((string)lst[0]))
						{
							dict = Variables.Last();
						}
						if (!dict.ContainsKey((string)lst[0]))
						{
							dict = Constants;
						}
						return
						dict[(string)lst[0]];
					}
					catch (KeyNotFoundException)
					{
						throw new ExecutionException($"変数\"{(string)lst[0]}\"は、定義されていません。ラムダ関数では、グローバル関数以外の外の変数を参照できません。");
					}
				}).checkParamCount(1, name: "//getv");
			Functions["//setv"] =
				new FunctionType(lst => { try { Stack<TypedDictionary<string, object>> stk = new(Variables); TypedDictionary<string, object> dict = stk.Pop(); for (; stk.Count > funccallstackdepthes.Peek() && !dict.ContainsKey((string)lst[0]); dict = stk.Pop()) ; if (!dict.ContainsKey((string)lst[0])) { dict = Variables.Last(); } if (!dict.ContainsKey((string)lst[0])) { throw new ExecutionException($"変数\"{(string)lst[0]}\"は、定義されていません。ラムダ関数では、グローバル関数以外の外の変数を設定できません。"); } return dict[(string)lst[0]] = lst[1]; } catch (KeyNotFoundException) { throw new ExecutionException($"変数\"{(string)lst[0]}\"は、定義されていません。ラムダ関数では、グローバル関数以外の外の変数を設定できません。"); } }).checkParamCount(2, name: "//setv");
			Functions["//setg"] =
				new FunctionType(lst => { return Variables.Last()[(string)lst[0]] = lst[1]; }).checkParamCount(2, name: "//setg");
			Functions["callm"] = new FunctionType(lst =>
			{
				return ((UserDefinedObject)lst[0]).CallMethod((string)lst[1], (List<object>)lst[2]);
			});
			Serializers.Add(new InternalSerializer());
			Import(new InternalExtention(this));
			Import(new ReplInternalExtention(this));
			//Methods["//getargs"] = lst =>;
		}
		#region Configs
		private readonly string[] separators = new string[] {
			" ",
			"(",
			")",
			"{",
			"}",
			",",
			"\"",
			"\r\n",
			"=",
			":",
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
				var lst = new List<string>(ie);
				foreach (var item in seps)
				{
					lst = lst.Aggregate(new List<string>(), GetSpliter(item));
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
					if (tok != "") newtokens.Add(tok);
				}
			}
			tokens = new(new string[] { "," }.Concat(newtokens));
			List<Node> nodes = new();
			try
			{
				for (; tokens.Count != 0;)
				{
					while (tokens.Count != 0 && tokens.Peek()  is " " or "\r\n") tokens.Dequeue();
					if (tokens.Count == 0) break;
					if (tokens.Dequeue() != ",") throw new SyntaxException("文を続ける場合は、「,」が必要です。");
					while (tokens.Count != 0 && tokens.Peek()  is " " or "\r\n") tokens.Dequeue();
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
			if (tok  is " " or "\r\n") goto start;
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
				while (tokens.Peek()  is " " or "\r\n") tokens.Dequeue();
				if (tokens.Peek() == "}")
				{
					tokens.Dequeue();
					return new Node(" ", NodeType.call, @params);
				}
			blockstart:;
				@params.Add(parseLoop(tokens));
				string s;
				while (tokens.Peek()  is " " or "\r\n") tokens.Dequeue();
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
				while (tokens.Peek()  is " " or "\r\n") tokens.Dequeue();
				string next = tokens.Dequeue();
				if (next == "(")
				{
					var @params = new List<Node>();
					while (tokens.Peek()  is " " or "\r\n") tokens.Dequeue();
					if (tokens.Peek() == ")")
					{
						tokens.Dequeue();
						return new Node(name, NodeType.call, @params);
					}
				paramstart:;
					@params.Add(parseLoop(tokens));
					string s;
					while (tokens.Peek()  is " " or "\r\n") tokens.Dequeue();
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
			var func = (Deq<string> tokens) =>
			{
			start:;
				Node res;
				string tok = tokens.Dequeue();
				if (tok is " " or "\r\n") goto start;
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
								case 'r':
									stringBuilder.Append('\r');
									break;
								case 'n':
									stringBuilder.Append('\n');
									break;
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
					while (tokens.Peek() is " " or "\r\n") tokens.Dequeue();
					if (tokens.Peek() == "}")
					{
						tokens.Dequeue();
						return new Node(" ", NodeType.block, @params);
					}
				blockstart:;
					@params.Add(parseLoop(tokens));
					string s;
					while (tokens.Peek() is " " or "\r\n") tokens.Dequeue();
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
						while (tokens.Peek()  is " " or "\r\n") tokens.Dequeue();
						if (tokens.Peek() == ")")
						{
							throw new SyntaxException("「@」の後には、「(」と「)」で囲まれた式が来なければなりません。");
						}
						@params.Add(parseLoop(tokens));
						while (tokens.Peek()  is " " or "\r\n") tokens.Dequeue();
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
						while (tokens.Peek()  is " " or "\r\n") tokens.Dequeue();
						if (tokens.Peek() == ")")
						{
							throw new SyntaxException("「$」の後には、「(」と「)」で囲まれた式が来なければなりません。");
						}
						@params.Add(parseLoop(tokens));
						while (tokens.Peek()  is " " or "\r\n") tokens.Dequeue();
						if (tokens.Dequeue() == ")") return new Node("", NodeType.precalc, @params);
						throw new SyntaxException("「$」の後には、「(」と「)」で囲まれた式が来なければなりません。");
					}
					else throw new SyntaxException("「$」の後には、「(」と「)」で囲まれた式が来なければなりません。"); ;
				}
				else
				{
					var name = tok;
					while (tokens.Count > 0 && tokens.Peek()  is " " or "\r\n") tokens.Dequeue();
					if (tokens.Count > 0)
					{
						string next = tokens.Dequeue();
						if (next == "(")
						{
							var @params = new List<Node>();
							while (tokens.Peek()  is " " or "\r\n") tokens.Dequeue();
							if (tokens.Peek() == ")")
							{
								tokens.Dequeue();
								return new Node(name, NodeType.call, @params);
							}
						paramstart:;
							@params.Add(parseLoop(tokens));
							string s;
							while (tokens.Peek()  is " " or "\r\n") tokens.Dequeue();
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
							if (tokens.Peek() == ("="))
							{
								tokens.Dequeue();
								return new("//set", NodeType.call, new() { new(tok, NodeType.lit, new()), parseLoop(tokens) });
							}
							else if (tokens.Peek() == (":") && tokens.ElementAt(1) == "=")
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
				};

			};
			var res = func(tokens);
			return res;
		}

        #endregion
        public virtual List<object> Execute(string s) => Run(Parse(s));
    }
}