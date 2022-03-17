// See https://aka.ms/new-console-template for more information

namespace EasyScript
{
    public partial class ParserAndExecuter
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
	}
}