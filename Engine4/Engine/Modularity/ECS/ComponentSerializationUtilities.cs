using System.Diagnostics.CodeAnalysis;
using Engine.Data;

namespace Engine.Modularity.ECS;

public static class ComponentSerializationUtilities {
	public static bool GetFromSerializedData( byte[] data,
		[NotNullWhen( true )] out Guid? parentGuid,
		[NotNullWhen( true )] out Guid? typeGuid,
		[NotNullWhen( true )] out byte[]? componentData ) {
		byte[][]? segments = Segmentation.Parse( data );
		parentGuid = null;
		typeGuid = null;
		componentData = null;
		if ( segments is null || segments.Length != 3 )
			return false;
		parentGuid = new Guid( segments[ 0 ] );
		typeGuid = new Guid( segments[ 1 ] );
		componentData = segments[ 2 ];
		return true;
	}
}
