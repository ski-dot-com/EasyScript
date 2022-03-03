namespace EasyScript
{
    public delegate object MethodType(List<object> @params);
	public delegate object BlockType(TypedDictionary<string, object>? @params = null);
	public class Delegates{ }
}