// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.Json;

namespace EasyScript
{
    public record struct Node(string text, NodeType type, List<Node> Child)
    {
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append(type.ToString());
            sb.Append(": \"");
            sb.Append(text);
            sb.Append("\" {");
            for (var i=0;i<Child.Count-1; i++)
            {
                var item=Child[i];
                sb.Append(item.ToString());
                sb.Append(", ");
            }
            if (Child.Count > 0)
            {
                var item = Child[Child.Count-1];
                sb.Append(item.ToString());
            }
            sb.Append('}');
            return sb.ToString();
        }
    }
}