// See https://aka.ms/new-console-template for more information

namespace EasyScript
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class ExportMethodAttribute : Attribute
    {
        readonly string? name;

        // This is a positional argument
        public ExportMethodAttribute(string? name = null)
        {
            this.name = name;

        }
        public string? Name => name;
    }
}