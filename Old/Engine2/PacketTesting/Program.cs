using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace PacketTesting {
	class Program {
		static void Main( string[] args ) {
			List<Type> packetList = new List<Type>();

			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach( Assembly assembly in assemblies )
				foreach( Type type in assembly.GetTypes() )
					if( type.IsSubclassOf( typeof( Packet ) ) && !type.IsAbstract )
						packetList.Add( type );

			packetList.Sort( PacketTypeSorter );

			List<ConstructorInfo> aaa = new List<ConstructorInfo>();
			for( int i = 0; i < packetList.Count; i++ ) {
				var c = packetList[ i ].GetConstructors();

				foreach( var item in c ) {
					Console.WriteLine( packetList[ i ].FullName + ": " );
					// Make a NewExpression that calls the ctor with the args we just created
					if (TryCreatePacketCreator( packetList[ i ], out Func<byte[], Packet> function ) ) {
						Console.WriteLine( function );
					}else
						Console.WriteLine("lol...");
				}
			}

			/*var listOfBs = 
				(from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
				// alternative: from domainAssembly in domainAssembly.GetExportedTypes()
				from assemblyType in domainAssembly.GetTypes()
				where typeof( B ).IsAssignableFrom( assemblyType )
				// alternative: where assemblyType.IsSubclassOf(typeof(B))
				// alternative: && ! assemblyType.IsAbstract
				select assemblyType ).ToArray();*/
		}

		//https://stackoverflow.com/questions/55137619/pre-compiled-lambda-expression-to-create-class-that-has-a-constructor-with-a-par
		private static bool TryCreatePacketCreator( Type packetType, out Func<byte[], Packet> function ) {
			function = null;
			ConstructorInfo constructor = packetType.GetConstructor( new Type[] { typeof( byte[] ) } );
			if( constructor is null )
				return false;
			ParameterExpression parameter = Expression.Parameter( typeof( byte[] ), "data" );
			Expression<Func<byte[], Packet>> creatorExpression = Expression.Lambda<Func<byte[], Packet>>( Expression.New( constructor, new Expression[] { parameter } ), parameter );
			function = creatorExpression.Compile();
			return true;
		}

		private static int PacketTypeSorter( Type x, Type y ) {
			return string.Compare( x.FullName, y.FullName, StringComparison.Ordinal );
		}
	}
}
