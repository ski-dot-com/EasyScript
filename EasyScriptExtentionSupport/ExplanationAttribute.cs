// See https://aka.ms/new-console-template for more information

namespace EasyScript
{
    /// <summary>
    /// 説明をつけるための属性。
    /// <code>
    /// [Explanation("{説明}")]
    /// public void testextension()
    /// {
    ///     This.Methods["print"](new() { "Hello from extension!" });
    /// }
    /// </code>と使用。
    /// </summary>
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class ExplanationAttribute : Attribute
    {
        readonly string description;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description">説明</param>
        public ExplanationAttribute(string description)
        {
            this.description = description;
        }
        public string Description => description;

    }
}