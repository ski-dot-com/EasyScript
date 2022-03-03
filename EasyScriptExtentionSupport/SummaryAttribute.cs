// See https://aka.ms/new-console-template for more information

namespace EasyScript
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class SummaryAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string description;

        // This is a positional argument
        public SummaryAttribute(string description)
        {
            this.description = description;

            // TODO: Implement code here

        }

        public string Description
        {
            get { return description; }
        }

    }
}