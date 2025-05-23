﻿//using Engine.Module.Entities.Container;
//using Engine.Module.Render.Entities;
//using Engine.Standard.Entities.Components.Rendering;
//using Engine.Standard.Render.Text.Services;
//using Engine.Standard.Render.Text.Typesetting;
//using Engine.Transforms;

//namespace Engine.Standard.Render.Entities.Behaviours;

//public sealed class TextRendering2Behaviour : SynchronizedRenderBehaviourBase<TextRendering2Archetype> {

//	private string _desyncFontName = string.Empty;
//	private string _desyncText = string.Empty;
//	private Alignment _desyncHorizontalAlignment = Alignment.Negative;
//	private Alignment _desyncVerticalAlignment = Alignment.Positive;

//	private Transform2Behaviour? _subscribedTransformBehaviour;
//	private TextLayout? _textLayout;

//	protected override void OnRenderEntitySet() {
//		base.OnRenderEntitySet();
//		_textLayout = RenderEntity.ServiceAccess.Get<TextLayoutProvider>().CreateLayout( "test", 0 );
//		_textLayout.Text = Archetype.RenderedTextComponent.Text;
//		_textLayout.FontName = Archetype.RenderedTextComponent.FontName;
//		_desyncText = _textLayout.Text;
//		_desyncFontName = _textLayout.FontName;
//		this.RenderEntity.OnBehaviourRemoved += OnBehaviourRemoved;
//	}

//	private void OnBehaviourRemoved( RenderBehaviourBase @base ) {
//		if (@base == this._subscribedTransformBehaviour) {
//			_subscribedTransformBehaviour.Transform.OnMatrixChanged -= OnMatrixChanged;
//			this._subscribedTransformBehaviour = null;
//		}
//	}

//	protected override void OnUpdate( double time, double deltaTime ) {
//		if (this._subscribedTransformBehaviour is null) {
//			if (!this.RenderEntity.TryGetBehaviour( out this._subscribedTransformBehaviour ))
//				return;
//			_subscribedTransformBehaviour.Transform.OnMatrixChanged += OnMatrixChanged;
//			if (_textLayout is null)
//				return;
//			_textLayout.TextArea = AABB.Create( [
//				_subscribedTransformBehaviour.Transform.GlobalTranslation - _subscribedTransformBehaviour.Transform.GlobalScale,
//				_subscribedTransformBehaviour.Transform.GlobalTranslation + _subscribedTransformBehaviour.Transform.GlobalScale
//			] );
//			_textLayout.TextRotation = _subscribedTransformBehaviour.Transform.GlobalRotation;
//		}
//		_textLayout?.Update( time, deltaTime );
//	}

//	private void OnMatrixChanged( IMatrixProvider<float> provider ) {
//		if (_textLayout is null || _subscribedTransformBehaviour is null)
//			return;
//		_textLayout.TextArea = AABB.Create( [
//			_subscribedTransformBehaviour.Transform.GlobalTranslation - _subscribedTransformBehaviour.Transform.GlobalScale,
//			_subscribedTransformBehaviour.Transform.GlobalTranslation + _subscribedTransformBehaviour.Transform.GlobalScale
//		] );
//		_textLayout.TextRotation = _subscribedTransformBehaviour.Transform.GlobalRotation;
//	}

//	protected override bool InternalDispose() {
//		return true;
//	}

//	protected override bool PrepareSynchronization( ComponentBase component ) {
//		if (component is RenderedTextComponent rtc) {
//			this._desyncFontName = rtc.FontName;
//			this._desyncText = rtc.Text;
//			return _textLayout is not null && (_desyncFontName != _textLayout.FontName || _desyncText != _textLayout.Text);
//		}
//		return false;
//	}

//	protected override void Synchronize() {
//		if (_textLayout is null)
//			return;
//		_textLayout.FontName = _desyncFontName;
//		_textLayout.Text = _desyncText;
//	}
//}
