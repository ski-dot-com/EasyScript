// See https://aka.ms/new-console-template for more information

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
}