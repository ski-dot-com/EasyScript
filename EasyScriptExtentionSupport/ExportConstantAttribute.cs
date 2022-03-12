// See https://aka.ms/new-console-template for more information

namespace EasyScript
{
    /// <summary>
    /// 属するクラスが登録されるとき、この属性をつけたフィールドかプロパティは関数として追加される。
    /// <code>
    /// [ExportConstant]
    /// public int i;
    /// </code>と使用。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class ExportConstantAttribute : Attribute
    {
        readonly string? name;

        /// <param name="name">名前。無ければ、フィールド名かプロパティ名になる。</param>
        public ExportConstantAttribute(string? name = null)
        {
            this.name = name;

        }
        public string? Name => name;
    }
}