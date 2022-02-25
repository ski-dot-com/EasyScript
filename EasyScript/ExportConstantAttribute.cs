// See https://aka.ms/new-console-template for more information

namespace EasyScript
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class ExportConstantAttribute : Attribute
    {
        readonly string? name;

        // This is a positional argument
        public ExportConstantAttribute(string? name = null)
        {
            this.name = name;

        }
        public string? Name => name;
    }
}