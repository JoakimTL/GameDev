namespace Engine.Rendering;
public interface IMesh {
	void SetElementData<T>( T[] data, uint offset = 0 ) where T : unmanaged;
	void SetVertexData<T>( T[] data, uint offset = 0 ) where T : unmanaged;

}
