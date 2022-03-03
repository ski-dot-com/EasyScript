// See https://aka.ms/new-console-template for more information

namespace EasyScript
{
    [System.AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    public sealed class ExtensionAttribute : Attribute
    {
        public Type export;
        public bool UseParserAndExecuter
        {
            get;
            init;
        } = false;
        public ExtensionAttribute(Type export)
        {
            this.export = export;
        }
    }
}