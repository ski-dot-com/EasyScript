namespace EasyScript
{
    public abstract class Type
	{
		public abstract bool isInstance(object o);
		public abstract bool isSupertypeof(Type o);
		class AllType : Type
		{
			public override bool isInstance(object o)
			{
				return true;
			}

			public override bool isSupertypeof(Type o)
			{
				return true;
			}
		}
		class UnionType : Type
		{
			Type l, r;

			public UnionType(Type l, Type r)
			{
				this.l = l;
				this.r = r;
			}

			public override bool isInstance(object o)
			{
				return l.isInstance(o) || r.isInstance(o);
			}

			public override bool isSupertypeof(Type o)
			{
				if(o is IntersectionType i&&(isSupertypeof(i.l)|| isSupertypeof(i.r)))return true;
				return l.isSupertypeof(o) || r.isSupertypeof(o);
			}
		}
		class IntersectionType : Type
		{
			internal Type l, r;

			public IntersectionType(Type l, Type r)
			{
				this.l = l;
				this.r = r;
			}

			public override bool isInstance(object o)
			{
				return l.isInstance(o) && r.isInstance(o);
			}

			public override bool isSupertypeof(Type o)
			{
				if (o is IntersectionType i && (isSupertypeof(i.l) || isSupertypeof(i.r))) return true;
				return l.isSupertypeof(o) && r.isSupertypeof(o);
			}
		}
		class SystemType<T> : Type
		{
			private SystemType(){}
			public static readonly SystemType<T> instance = new();
			public override bool isInstance(object o)
			{
				return o is T;
			}

			public override bool isSupertypeof(Type o)
			{
				if (o is IntersectionType i && (isSupertypeof(i.l) || isSupertypeof(i.r))) return true;
				return isocase((dynamic)this, (dynamic)o);
			}
			public static bool isocase(Type type, Type type1) => false;
			public static bool isocase<L,R>(SystemType<L> A, SystemType<R> _) where R:L=> true;
		}

		private static readonly Type all = new AllType();

		public static Type All => all;
		public static Type FromSystemType<From>() => SystemType<From>.instance;
		public static Type FromPredicate(Predicate<object> predicate) => new PredType(predicate);
		public static Type operator |(Type l, Type r) => new UnionType(l, r);
		public static Type operator &(Type l, Type r) => new IntersectionType(l, r);
		class PredType :Type { 
			Predicate<object> predicate;

            public PredType(Predicate<object> predicate)
            {
                this.predicate = predicate;
            }

            public override bool isInstance(object o)=>predicate(o);

			public override bool isSupertypeof(Type o) => this == o||(o is PredType p&&predicate==p.predicate);
        }

    }
    public class ClassType
    {
    }
	public class UserDefinedObject
    {
		public readonly TypedDictionary<string, object> datas=new();
		public readonly TypedDictionary<string,MethodType> methods=new();
		public object CallMethod(string name, List<object> @params)=>(methods[name])(this, @params);
		
		public UserDefinedObject(UserDefinedObject from)
		{
			datas = new(from.datas);
			methods = new(from.methods);
		}
		public UserDefinedObject()
        {
        }
	}
}
