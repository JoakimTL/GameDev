namespace Engine.Rendering.Standard.VertexArrayObjects;

public class VertexLayoutBindingAttribute : Attribute {
	public readonly Type BindingType;

	public VertexLayoutBindingAttribute( Type bindingType ) {
		this.BindingType = bindingType ?? throw new ArgumentNullException( nameof( bindingType ) );
	}
}
