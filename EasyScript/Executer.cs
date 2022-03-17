using System.Reflection;
using static EasyScript.ParserAndExecuter;
// See https://aka.ms/new-console-template for more information

namespace EasyScript
{
    public class Executer: ExecuterBase
    {
        public readonly ReferenceBuilder referenceBuilder = new();
        protected readonly TypedDictionary<string, FunctionType> Functions = new()
        {
            ["if"] = new FunctionType(lst =>
            (bool)lst[0]
            ?
            (lst[1] as BlockType ?? ((_) => Null))()
            :
            ((lst.Count < 3 ? null : lst[2] as BlockType)
            ??
            ((_) => Null))()).checkParamCount(2, 3, "if"),

            ["run"] = new FunctionType(lst => (lst[0] as BlockType ?? ((_) => Null))()).checkParamCount(1, name: "run"),
            ["equals"] = new FunctionType(lst => lst[0].Equals(lst[1])).checkParamCount(2, name: "equals"),
            ["argdata"] = new FunctionType(lst => new ArgData((string)lst[0], lst.Count < 2 ? null : lst[1])).checkParamCount(1, 2, "argdata"),
            ["list"] = new FunctionType(lst => lst).checkParamCount(0, -1, "list"),
        };

        protected readonly Stack<TypedDictionary<string, object>> Variables = new();
        public List<string> doesnotMakeScope = new()
        {
            "//get",
            "//set",
            "//getv",
            "//setv",
        };
        protected TypedDictionary<string, object> Constants = new()
        {
            ["true"] = true,
            ["false"] = false,
            ["null"] = Null.Instance,
        };
        protected Stack<int> blockcallstackdepthes = new();
        protected Stack<int> funccallstackdepthes = new(new[] { 0 });
        public static Null Null => Null.Instance;
        public static FunctionType retjunk(Action<List<object>> action) => (t) =>
        {
            action(t);
            return Null;
        };
        public override object CallMethod(string name, List<object> list) => Functions[name](list);
        public virtual uint Import(object o, bool allowoverride = false)
        {
            uint count = 0;
            foreach (var item in o.GetType().GetMethods())
            {
                var tmp = item.GetCustomAttributes(typeof(ExportMethodAttribute), false).Concat(item.GetCustomAttributes(typeof(ExportMethodAttribute), true));
                if (tmp.Any())
                {
                    ExportMethodAttribute attr = tmp.First() as ExportMethodAttribute ?? throw new();
                    if (!allowoverride)
                    {
                        if (Functions.ContainsKey(attr.Name ?? item.Name)) continue;
                    }
                    System.Reflection.ParameterInfo[] @params = item.GetParameters();
                    int min = 0, max = @params.Length;
                    for (; min < @params.Length; min++)
                    {
                        if (@params[min].IsOptional) break;
                    }
                    var defs = @params.ToList().ConvertAll(p => p.DefaultValue);
                    Functions[attr.Name ?? item.Name] = (@params.Any() && @params.Last().GetCustomAttributes(typeof(ParamArrayAttribute), false).Any()) ? new FunctionType(lst_ =>
                    {
                        object?[] defaultargs = defs.ToArray();
                        Array.Resize(ref defaultargs, Math.Max(defaultargs.Length - 1, lst_.Count));
                        lst_.CastNullable().CopyTo(defaultargs);
                        List<object?> lst = defaultargs.ToList();
                        return item.Invoke(o, lst.ToArray()[..(max - 1)].Concat(new[] { lst.ToArray()[(max - 1)..] }).ToArray()) ?? Null;
                    }).checkParamCount((min <= max - 1 ? min : max - 1), -1, attr.Name ?? item.Name) : new FunctionType(lst_ =>
                    {
                        object?[] defaultargs = defs.ToArray();
                        Array.Resize(ref defaultargs, Math.Max(defaultargs.Length, lst_.Count));
                        lst_.CastNullable().CopyTo(defaultargs);
                        List<object?> lst = defaultargs.ToList();
                        return item.Invoke(o, lst.ToArray()) ?? Null;
                    }).checkParamCount(min, max, attr.Name ?? item.Name);
                    referenceBuilder.AddMethod(item, attr.Name ?? item.Name);
                    count++;
                }
            }
            foreach (var item in o.GetType().GetProperties())
            {
                var tmp = item.GetCustomAttributes(typeof(ExportConstantAttribute), false).Concat(item.GetCustomAttributes(typeof(ExportMethodAttribute), true));
                if (tmp.Any())
                {
                    ExportConstantAttribute attr = tmp.First() as ExportConstantAttribute ?? throw new();
                    if (!allowoverride)
                    {
                        if (Constants.ContainsKey(attr.Name ?? item.Name)) continue;
                    }
                    var tmp__ = item.GetValue(o);
                    if (tmp__ is not null)
                    {
                        Constants[attr.Name ?? item.Name] = tmp__;
                        var ien = item.GetCustomAttributes(typeof(ExplanationAttribute));
                        if (ien.Any())
                        {
                            referenceBuilder.AddConstants(attr.Name ?? item.Name, item.PropertyType, ((ExplanationAttribute)ien.First()).Description);
                        }
                        else
                        {
                            referenceBuilder.AddConstants(attr.Name ?? item.Name, item.PropertyType);
                        }
                        count++;
                    }
                }
            }
            foreach (var item in o.GetType().GetFields())
            {
                var tmp = item.GetCustomAttributes(typeof(ExportConstantAttribute), false).Concat(item.GetCustomAttributes(typeof(ExportMethodAttribute), true));
                if (tmp.Any())
                {
                    ExportConstantAttribute attr = tmp.First() as ExportConstantAttribute ?? throw new();
                    if (!allowoverride)
                    {
                        if (Constants.ContainsKey(attr.Name ?? item.Name)) continue;
                    }
                    var tmp__ = item.GetValue(o);
                    if (tmp__ is not null)
                    {
                        Constants[attr.Name ?? item.Name] = tmp__;
                        var ien = item.GetCustomAttributes(typeof(ExplanationAttribute));
                        if (ien.Any())
                        {
                            referenceBuilder.AddConstants(attr.Name ?? item.Name, item.FieldType, ((ExplanationAttribute)ien.First()).Description);
                        }
                        else
                        {
                            referenceBuilder.AddConstants(attr.Name ?? item.Name, item.FieldType);
                        }
                        count++;
                    }
                }
            }
            return count;
        }
        public override object Run(Node node)
        {
            if (node.type == NodeType.call && !doesnotMakeScope.Contains(node.text)) Variables.Push(new());
            try
            {
                var res = node.type switch
                {
                    NodeType.call => Functions.ContainsKey(node.text) ? Functions[node.text](node.Child.ConvertAll(n => Run(n))) : throw new ExecutionException($"関数\"{node.text}\"は、定義されていません。"),
                    NodeType.block => new BlockType((dict) =>
                    {
                        blockcallstackdepthes.Push(Variables.Count);
                        Variables.Push(new(dict ?? new()));
                        var res_ = node.Child.ConvertAll(n => Run(n)).LastOrDefault(Null);
                        Variables.Pop();
                        blockcallstackdepthes.Pop();
                        return res_;
                    }),
                    NodeType.lit => node.text,
                    NodeType.@int => Convert.ToInt64(node.text),
                    NodeType.flo => Convert.ToDouble(node.text),
                    NodeType.cst => Constants[node.text],
                    NodeType.value => Deserialize(node.text),
                    NodeType.expression => check(node.Child[0], 1),
                    NodeType.precalc => throw new ExecutionException("「$(...)」は、「@(...)」の中でしか使えません。")/*Run(node.Child[0])*/,
                    _ => throw new NotSupportedException(nameof(node.type)),
                };
                if (node.type == NodeType.call && !doesnotMakeScope.Contains(node.text)) Variables.Pop();
                return res;
            }
            catch (ExecutionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ExecutionException("実行時に例外が起きました。", ex);
            }
        }

        public override List<object> Run(IEnumerable<Node> nodes)
        {
            while (Variables.Count > 1) Variables.Pop();
            if (Variables.Count < 1) Variables.Push(new());
            blockcallstackdepthes.Clear();
            funccallstackdepthes.Clear();
            funccallstackdepthes.Push(0);

            return nodes.Aggregate(Array.Empty<object>() as IEnumerable<object>, (current, node) => current.Concat(new object[] { Run(node) })).ToList();
        }
        protected Node check(Node node, int depth)
        {
            switch (node.type)
            {
                case NodeType.expression:
                    depth++;
                    break;
                case NodeType.precalc:
                    depth--;
                    break;
                default:
                    break;
            }
            var res = new Node(node.text, node.type, new());
            if (depth == 0)
            {
                res = new(Serialize(Run(node.Child.First())), NodeType.value, new());
            }
            else
            {
                foreach (var item in node.Child)
                {
                    res.Child.Add(check(item, depth));
                }
            }
            return res;
        }
    }
}