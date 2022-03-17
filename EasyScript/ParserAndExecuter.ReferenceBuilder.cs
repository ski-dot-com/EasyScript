// See https://aka.ms/new-console-template for more information

using System.Reflection;

namespace EasyScript
{
    public partial class ParserAndExecuter
	{
        public class ReferenceBuilder
		{
			string methods = "", constants = "";
			TypedDictionary<string, MethodInfo> methoddatas = new();
			TypedDictionary<string, (System.Type type, string? summary)> constantdatas = new();
			public void AddMethod(MethodInfo methodInfo, string name)
			{
				methoddatas[name] = methodInfo;
			}
			public void AddConstants(string name, System.Type type, string? summary = null)
			{
				constantdatas[name] = (type, summary);
			}
			public void AddConstantstring(string name, System.Type type, string? summary)
			{
				constants += $"{type.FullName} {name}\r\n";
				if (summary is not null) constants += "\t説明:\r\n\t\t" + summary.Replace("\r\n", "\r\n\t\t") + "\r\n";
			}
			public void AddMethodstring(MethodInfo methodInfo, string name)
			{
				var opt = methodInfo.GetParameters().Where(p => p.IsOptional);
				methods += $"{methodInfo.ReturnType.FullName} {name} ({ string.Join(", ", methodInfo.GetParameters().Where(p => !p.IsOptional).ToList().ConvertAll(p => $"{(p.GetCustomAttributes<ParamArrayAttribute>().Any() ? "..." : "")}{p.ParameterType.FullName} {p.Name}")) + (opt.Any() ? $"[, {string.Join(", ", opt.ToList().ConvertAll(p => $"{(p.GetCustomAttributes<ParamArrayAttribute>().Any() ? "..." : "")}{p.ParameterType.FullName} {p.Name}"))}]" : "") })" + "\r\n";
				var sum = methodInfo.GetCustomAttributes(typeof(ExplanationAttribute));
				if (sum.Any())
				{
					methods += "\t説明:\r\n\t\t" + ((ExplanationAttribute)sum.First()).Description.Replace("\r\n", "\r\n\t\t") + "\r\n";
				}

				foreach (var item in methodInfo.GetParameters())
				{
					var psum = item.GetCustomAttributes(typeof(ExplanationAttribute));
					if (psum.Any())
					{
						methods += $"\t{item.Name}:\r\n\t\t" + ((ExplanationAttribute)psum.First()).Description.Replace("\r\n", "\r\n\t\t") + "\r\n";
					}
				}
				var rsum = methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(typeof(ExplanationAttribute), false).Concat(methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(typeof(ExplanationAttribute), true));
				if (rsum.Any())
				{
					methods += $"\t返り値:\r\n\t\t" + ((ExplanationAttribute)rsum.First()).Description.Replace("\r\n", "\r\n\t\t") + "\r\n";
				}
			}
			public static explicit operator string(ReferenceBuilder self)
			{
				self.methods = "";
				self.constants = "";
				foreach (var (name, methodInfo) in self.methoddatas)
				{
					self.AddMethodstring(methodInfo, name);
				}
				foreach (var (name, (type, summary)) in self.constantdatas)
				{
					self.AddConstantstring(name, type, summary);
				}
				string res = "";
				res += "メソッド:\r\n" + self.methods;
				res += "定数:\r\n" + self.constants;
				return res;
			}
			public override string ToString()
			{
				return (string)this;
			}
		}
	}
}