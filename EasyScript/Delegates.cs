namespace EasyScript
{
	public delegate object FunctionType(List<object> @params);
	public delegate object MethodType(UserDefinedObject @this, List<object> @params);
	public delegate object BlockType(TypedDictionary<string, object>? @params = null);
	class Delegates{ 
	}
}