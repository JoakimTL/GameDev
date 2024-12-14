using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;
using Engine.Standard.Entities.Components;
using Engine.Standard.Render.Text;
using Engine.Standard.Render.Text.Typesetting;

namespace Engine.Standard.Render.Entities.Behaviours;

public sealed class TextRenderingBehaviour : SynchronizedRenderBehaviourBase<TextRendering2Archetype> {

	private string _desyncFontName = string.Empty;
	private string _desyncText = string.Empty;

	private Transform2Behaviour? _subscribedTransformBehaviour;
	private TextLayout? _textLayout;

	protected override void OnRenderEntitySet() {
		base.OnRenderEntitySet();
		_textLayout = RenderEntity.ServiceAccess.Get<TextLayoutProvider>().CreateLayout( "test", 0 );
		_textLayout.Text = Archetype.RenderedTextComponent.Text;
		_textLayout.FontName = Archetype.RenderedTextComponent.FontName;
		_desyncText = _textLayout.Text;
		_desyncFontName = _textLayout.FontName;
		this.RenderEntity.OnBehaviourRemoved += OnBehaviourRemoved;
	}

	private void OnBehaviourRemoved( RenderBehaviourBase @base ) {
		if (@base == this._subscribedTransformBehaviour) {
			if (_textLayout is not null)
				_textLayout.BaseMatrix = null;
			this._subscribedTransformBehaviour = null;
		}
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		if (this._subscribedTransformBehaviour is null) {
			if (!this.RenderEntity.TryGetBehaviour( out this._subscribedTransformBehaviour ))
				return;
			if (_textLayout is not null)
				_textLayout.BaseMatrix = _subscribedTransformBehaviour.Transform;
		}
		_textLayout?.Update( time, deltaTime );
	}

	protected override bool InternalDispose() {
		_textLayout?.Dispose();
		return true;
	}

	protected override bool PrepareSynchronization( ComponentBase component ) {
		if (component is RenderedTextComponent rtc) {
			this._desyncFontName = rtc.FontName;
			this._desyncText = rtc.Text;
			return _textLayout is not null && (_desyncFontName != _textLayout.FontName || _desyncText != _textLayout.Text);
		}
		return false;
	}

	protected override void Synchronize() {
		if (_textLayout is null)
			return;
		_textLayout.FontName = _desyncFontName;
		_textLayout.Text = _desyncText;
	}
}
