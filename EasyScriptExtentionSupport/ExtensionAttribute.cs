// See https://aka.ms/new-console-template for more information

namespace EasyScript
{
    /// <summary>
    /// 拡張機能を登録するための属性。
    /// <code>[assembly: Extension(typeof(/*登録したいクラス*/))]</code>
    /// と使用。
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    public sealed class ExtensionAttribute : Attribute
    {
        public readonly Type export;
        /// <summary>
        /// 引数に <c>ParserAndExecuter</c> を含むかどうか。
        /// </summary>
        public bool UseParserAndExecuter
        {
            get;
            init;
        } = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="export">登録したいクラス</param>
        public ExtensionAttribute(Type export)
        {
            this.export = export;
        }
    }
}