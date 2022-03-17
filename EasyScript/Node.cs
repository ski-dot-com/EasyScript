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
            for (var i = 0; i < Child.Count - 1; i++)
            {
                var item = Child[i];
                sb.Append(item.ToString());
                sb.Append(", ");
            }
            if (Child.Count > 0)
            {
                var item = Child[Child.Count - 1];
                sb.Append(item.ToString());
            }
            sb.Append('}');
            return sb.ToString();
        }
        public int Selialize(List<SerializedNode> datas)
        {
            int res = datas.Count;
            datas.Add(new());
            List<int> nch = new(Child.Count);
            foreach (var item in Child)
            {
                nch.Add(item.Selialize(datas));
            }
            datas[res] = new(text, type, nch);
            return res;
        }
        public static Node Deselialize(List<SerializedNode> datas, int pos)
        {
            var cur=datas[pos];
            Node res = new(cur.text, cur.type, new(cur.Child.Count));
            foreach (var item in cur.Child)
            {
                res.Child.Add(Deselialize(datas, item));
            }
            return res;
        }
    }
    public record struct SerializedNode(string text, NodeType type, List<int> Child)
    {
    }
}