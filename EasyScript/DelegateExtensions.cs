namespace EasyScript
{
    public static class DelegateExtensions
	{
		public static MethodType checkParamCount(this MethodType methodType, int min, int? max = null, string name="")
			=>
			(lst) => (lst.Count >= min && (max==-1||lst.Count <= (max is null ? min : max)))?methodType(lst):throw new ParamaterMissmutchException($"関数「{name}」の引数は、{(max is null ?$"{min}個": $"{min}個以上{(max==-1?"":$"、{max}個以下")}")}である必要があります。");
	}
}