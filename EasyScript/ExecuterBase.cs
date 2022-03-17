// See https://aka.ms/new-console-template for more information
using static EasyScript.ParserAndExecuter;
namespace EasyScript
{
    public abstract class ExecuterBase
    {

        private readonly List<ISerializer> serializers = new();

        public List<ISerializer> Serializers => serializers;

        public abstract object CallMethod(string name, List<object> list);

        public object Deserialize(string s)
        {
            bool b = false;
            object? obj = null;

            for (int i = 0; !b && i < serializers.Capacity; i++)
            {
                b = serializers[i].TryDeserialize(out obj, s);
            }
            if (!b)
            {
                obj = null;
            }
            return obj ?? throw new InvalidOperationException();

        }

        public abstract object Run(Node node);
        public abstract List<object> Run(IEnumerable<Node> nodes);

        public string Serialize(object obj)
        {
            bool b = false;
            string? s = null;

            for (int i = 0; !b && i < serializers.Count; i++)
            {
                b = serializers[i].TrySerialize(obj, out s);
            }
            if (!b)
            {
                s = null;
            }
            return s ?? throw new InvalidOperationException($"「{obj.GetType().FullName}」型のデータは、文字列にできず、定数にもできません。");
        }
    }
}