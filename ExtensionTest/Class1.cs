using EasyScript;
[assembly: Extension(typeof(ExtensionTest.Class1), UseParserAndExecuter =true)]
namespace ExtensionTest
{
    /// <summary>
    /// 拡張機能の例。
    /// 注意：
    /// 可変長引数は、object[]限定です。(今後変わるかも)
    /// </summary>
    public class Class1
    {

        ParserAndExecuter This;
        public Class1(ParserAndExecuter @this)
        {
            This = @this;
        }
        [ExportConstant("testextensioni")]
        [Explanation("説明テスト。testextensioni")]
        public int i = 0;
        [ExportMethod]
        [Explanation("説明テスト。testextension")]
        [return: Explanation("説明テスト。testextension::return")]
        public void testextension([Explanation("説明テスト。testextension::arg::a")] object a)
        {
            This.CallMethod("print",new() { $"Hello from extension! the arg is {a}" });
        }
    }
}