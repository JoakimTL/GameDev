using Engine.Standard.Render.UserInterface.Standard;
using Engine.Standard.Render.UserInterface;
using Engine;
using Engine.Module.Render.Ogl.OOP.Textures;

namespace Civlike.Client.Render.Ui.Components;

public sealed class MenuBackground : TexturedBackground {
	public MenuBackground( UserInterfaceElementBase element, OglTexture texture ) : base( element, texture ) {
		Placement.Set( new( 0, 0, 1 ), Alignment.Center, Alignment.Center, true, true );
	}

	public void AddChild<T>( T child ) where T : UserInterfaceComponentBase => base.AddChild( child );
}