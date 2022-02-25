// See https://aka.ms/new-console-template for more information

namespace EasyScript
{
    [Serializable]
    public class MethodNotFoundException : ExecutionException
    {
        public MethodNotFoundException() { }
        public MethodNotFoundException(string message) : base(message) { }
        public MethodNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected MethodNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    [Serializable]
    public class VariableNotFoundException : ExecutionException
    {
        public VariableNotFoundException() { }
        public VariableNotFoundException(string message) : base(message) { }
        public VariableNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected VariableNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}