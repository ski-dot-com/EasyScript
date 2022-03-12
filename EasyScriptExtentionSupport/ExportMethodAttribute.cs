// See https://aka.ms/new-console-template for more information

namespace EasyScript
{
    /// <summary>
    /// 属するクラスが登録されるとき、この属性をつけたメソッドは関数として追加される。
    /// <code>
    /// [ExportMethod]
    /// public void testextension()
    /// {
    ///     This.Methods["print"](new() { "Hello from extension!" });
    /// }
    /// </code>と使用。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class ExportMethodAttribute : Attribute
    {
        readonly string? name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">名前。無ければ、メソッド名になる。</param>
        public ExportMethodAttribute(string? name = null)
        {
            this.name = name;

        }
        public string? Name => name;
    }
}