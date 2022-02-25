// See https://aka.ms/new-console-template for more information

global using System.Diagnostics;

namespace EasyScript
{
    [Serializable]
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class ExecutionException : Exception
    {
        public ExecutionException() { }
        public ExecutionException(string message) : base(message) { }
        public ExecutionException(string message, Exception inner) : base(message, inner) { }
        protected ExecutionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}