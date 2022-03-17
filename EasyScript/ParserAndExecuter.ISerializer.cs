// See https://aka.ms/new-console-template for more information

namespace EasyScript
{
    public partial class ParserAndExecuter
	{
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
					s = null;
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
	}
}