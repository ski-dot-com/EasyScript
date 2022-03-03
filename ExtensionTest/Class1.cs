using EasyScript;
[assembly: Extension(typeof(ExtensionTest.Class1), UseParserAndExecuter =true)]
namespace ExtensionTest
{
    public class Class1
    {

        ParserAndExecuter This;
        public Class1(ParserAndExecuter @this)
        {
            This = @this;
        }
        [ExportMethod]
        public void testextension()
        {
            This.Methods["print"](new() { "Hello from extension!" });
        }
    }
}