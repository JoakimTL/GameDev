using Engine.Module.Render.Ogl.OOP.Buffers;

namespace Engine.Module.Render.Ogl.Services;

public sealed class ShaderStorageBlockService( OglBufferService oglBufferService ) {

	public bool Create( uint size ) {
		if (!oglBufferService.UniformBuffer.TryAllocate( size, out OglBufferSegment? segment ))
			return false;
		// ...
		return true;
	}

}