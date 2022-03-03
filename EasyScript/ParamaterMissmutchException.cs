// See https://aka.ms/new-console-template for more information

namespace EasyScript
{
    [Serializable]
    public class ParamaterMissmutchException : Exception
    {
        public ParamaterMissmutchException() { }
        public ParamaterMissmutchException(string message) : base(message) { }
        public ParamaterMissmutchException(string message, Exception inner) : base(message, inner) { }
        protected ParamaterMissmutchException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}