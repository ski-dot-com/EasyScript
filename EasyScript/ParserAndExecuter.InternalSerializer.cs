// See https://aka.ms/new-console-template for more information

namespace EasyScript
{
    public partial class ParserAndExecuter
	{
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
					return long.Parse(s[1..]);
				}
				else if (s.StartsWith("%"))
				{
					return double.Parse(s[1..]);
				}
				else if ((s.StartsWith('"') && s.EndsWith('"')))
				{
					s = s[1..^1];
					char c = '\0';
					bool Next()
					{
						try
						{
							c = s[0];
							s = s[1..];
							return true;
						}
						catch (Exception)
						{
							return false;
						}
					}
					string cur = "";
					while (Next())
					{
						if (c == '\\')
						{
							Next();
						}
						cur += c;
					}
					return cur;
				}
				else if ((s.StartsWith('[') && s.EndsWith(']')))
				{
					s = s[1..^1];
					string cur = "";
					char c = '\0';
					List<string> list = new();
					bool Next()
					{
                        try
                        {
							c = s[0];
							s = s[1..];
							return true;
						}
                        catch (Exception)
                        {
                            return false;
                        }
					}
					int depth = 0;
					bool instring = false;
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
						else if (c == '"')
						{
							instring = !instring;
							cur += c;
						}
						else if (c == '\\')
						{
							cur += c;
							Next();
							cur += c;
						}
						else
						{
							cur += c;
						}
					}
					if (cur != "") list.Add(cur);
					return list.ConvertAll(s => Deserialize(s));
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
					return $"\"{s.Replace("\\", @"\\").Replace("\"", @"\""")}\"";
				}
				else if (obj is List<object> lst)
				{
					return $"[{string.Join(",", lst.ConvertAll(Serialize))}]";
				}
				else throw new NotSupportedException();
			}
		}
	}
}