// See https://aka.ms/new-console-template for more information

namespace EasyScript
{
    [Serializable]
    public class SyntaxException : Exception
    {
        public SyntaxException() { }
        public SyntaxException(string message) : base(message) { }
        public SyntaxException(string message, Exception inner) : base(message, inner) { }
        protected SyntaxException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}