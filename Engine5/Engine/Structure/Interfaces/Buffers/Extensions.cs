namespace Engine.Structure.Interfaces.Buffers;
public static class Extensions {
	internal static bool IsInsideSegment( this IBufferSegment segment, ulong offsetBytes, ulong sizeBytes ) 
		=> segment.SizeBytes >= offsetBytes + sizeBytes;
}
