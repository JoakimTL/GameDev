using System;
using Engine.MemLib;
using Engine.Graphics.Objects.Default.Shaders;
using Engine.LinearAlgebra;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual.Text;
using Engine.Graphics.Objects.Default.Cameras.Views;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual {
	public class TextLabel : UIElement {

		private bool updated, reloadRequired;
		private TextMesh instanceMesh;
		public TextAttributeData Attributes { get; private set; }
		public TextData TextData { get; private set; }

		public string Content { get => TextData.Content; set => TextData.Set( value ); }

		public bool GenerateCollisionData;

		public event Action TextUpdated;

		private TextLabel( TextAttributeData attribs, TextData textData ) {
			Mesh = instanceMesh = new TextMesh( $"label[{ID}]", attribs.MaxStringLength );
			ShaderBundle = Mem.ShaderBundles.Text;
			Material = attribs.Font.Material;
			Attributes = attribs;
			Attributes.TextAttributesChanged += Changed;
			Attributes.StringLengthChanged += StringLengthChanged;
			Normalized = Attributes.NormalizeScale;
			TextData = textData;
			UpdatedThirdActive += UpdateEvent;
			Activated += ActivationEvent;
		}

		public TextLabel( TextAttributeData attribs, ColorCodedContent content ) : this( attribs, new TextData( attribs, content ) ) { }
		public TextLabel( TextAttributeData attribs, string content ) : this( attribs, new TextData( attribs, content ) ) { }
		public TextLabel( Font font, string content ) : this( new TextAttributeData( font ), content ) { }
		public TextLabel( Font font ) : this( new TextAttributeData( font ), "" ) { }
		public TextLabel() : this( new TextAttributeData( Mem.Font.Get( "default" ) ), "" ) { }

		private void StringLengthChanged() {
			Mesh.Dispose();
			Mesh = instanceMesh = new TextMesh( $"label[{ ID }]", Attributes.MaxStringLength );
			reloadRequired = true;
		}

		private void Changed() {
			updated = true;
		}

		private void ActivationEvent() {
			updated = true;
		}

		public int GetIndexAbsolute( Vector2 positionNDCA, View2 uiView ) {
			return TextData.GetIndex( positionNDCA * uiView.Projection.Scale * 0.5f + uiView.TranformInterface.Translation, Data.Transform.GlobalTranslation, Data.Transform.GlobalScale );
		}

		public int GetIndexRelative( Vector2 relativePosition ) {
			return TextData.GetIndex( relativePosition, 0, 1 );
		}

		/// <summary>
		/// Gets the position of an indexed letter, relative to the label.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Vector2 GetPosition( int index ) {
			return TextData.GetPosition( index, 1 );
		}

		private void UpdateEvent( MouseInputEventData data ) {
			if( updated ) {
				Normalized = Attributes.NormalizeScale;
				Material = Attributes.Font.Material;
				updated = false;
			}

			if( TextData.Update() || reloadRequired ) {
				UpdateMesh();
				reloadRequired = false;
				TextUpdated?.Invoke();
			}
		}

		public void UpdateMesh() {
			TextData.CreateLetters();
			//CollisionModel = Data.CollisionShape.CreateCollection( GJK.CheckCollision2D, transform );
			TextData.UpdateInstanceData();
			instanceMesh.FillData( TextData.GetInstanceData() );
		}
	}
}
