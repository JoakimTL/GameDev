using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Reflection;
public static class TypeUtilities {

	public static IReadOnlyList<Type> AllLoadedTypes { get; } = AppDomain.CurrentDomain.GetAssemblies().SelectMany( p => p.GetTypes() ).ToList().AsReadOnly();

	public static IEnumerable<Type> GetAllSubtypes<T>( bool allowAbstract = false, bool allowInterface = false )
		=> AllLoadedTypes.Where( p => (!p.IsAbstract || p.IsAbstract == allowAbstract) && (!p.IsInterface || p.IsInterface == allowInterface) && p.IsAssignableTo( typeof( T ) ) );

	public static bool TryInstantiate<T>( this Type t, [NotNullWhen( true )] out T? instance ) where T : class
		=> (instance = t.GetConstructor( Type.EmptyTypes )?.Invoke( null ) as T) is not null;
}
