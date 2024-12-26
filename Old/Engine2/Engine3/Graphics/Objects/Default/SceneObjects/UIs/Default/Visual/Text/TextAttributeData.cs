using Engine.MemLib;
using Engine.Utilities.Data;
using Engine.Utilities.Data.Boxing;
using System;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual.Text {
	public class TextAttributeData {

		public const int DEFAULT_MAX_STRING_LENGTH = 128;

		private MutableSinglet<Font> _font;
		public Font Font {
			get => _font.Value;
			set => _font.Value = value;
		}

		private MutableSinglet<bool> _normalized;
		public bool NormalizeScale {
			get => _normalized.Value;
			set => _normalized.Value = value;
		}

		private MutableSinglet<bool> _oneline;
		public bool OneLine {
			get => _oneline.Value;
			set => _oneline.Value = value;
		}

		private MutableSinglet<bool> _breakline;
		public bool BreakLine {
			get => _breakline.Value;
			set => _breakline.Value = value;
		}

		private MutableSinglet<bool> _expanding;
		public bool Expanding {
			get => _expanding.Value;
			set => _expanding.Value = value;
		}

		private MutableSinglet<bool> _block;
		public bool Block {
			get => _block.Value;
			set => _block.Value = value;
		}

		private MutableSinglet<float> _maxlength;
		public float MaxLength {
			get => _maxlength.Value;
			set => _maxlength.Value = value;
		}

		private MutableSinglet<HorizontalAlignment> _horali;
		public HorizontalAlignment HorizontalAlignment {
			get => _horali.Value;
			set => _horali.Value = value;
		}

		private MutableSinglet<VerticalAlignment> _verali;
		public VerticalAlignment VerticalAlignment {
			get => _verali.Value;
			set => _verali.Value = value;
		}

		private MutableSinglet<int> _maxStringLength;
		public int MaxStringLength {
			get => _maxStringLength.Value;
			set => _maxStringLength.Value = value;
		}

		public event Action TextAttributesChanged;
		public event Action StringLengthChanged;
		public event Action ExpansionAttitudeChanged;
		private bool _changing;

		public TextAttributeData( Font font, float maxLength = 10.0f, bool normalize = true, bool block = false, bool oneLine = false, bool breakLine = true, bool expanding = true, HorizontalAlignment horizontal = HorizontalAlignment.LEFT, VerticalAlignment vertical = VerticalAlignment.BOTTOM ) {
			_font = new MutableSinglet<Font>( font, delegate ( Font f ) { return f != null; } );
			_font.Changed += AttributeChanged;
			_normalized = new MutableSinglet<bool>( normalize );
			_normalized.Changed += AttributeChanged;
			_oneline = new MutableSinglet<bool>( oneLine );
			_oneline.Changed += AttributeChanged;
			_breakline = new MutableSinglet<bool>( breakLine );
			_breakline.Changed += AttributeChanged;
			_block = new MutableSinglet<bool>( block );
			_block.Changed += AttributeChanged;
			_maxlength = new MutableSinglet<float>( maxLength, delegate ( float v ) { return v > 0; } );
			_maxlength.Changed += AttributeChanged;
			_horali = new MutableSinglet<HorizontalAlignment>( horizontal );
			_horali.Changed += AttributeChanged;
			_verali = new MutableSinglet<VerticalAlignment>( vertical );
			_verali.Changed += AttributeChanged;
			_verali = new MutableSinglet<VerticalAlignment>( vertical );
			_verali.Changed += AttributeChanged;
			_expanding = new MutableSinglet<bool>( expanding );
			_expanding.Changed += ExpandingChanged;
			_maxStringLength = new MutableSinglet<int>( DEFAULT_MAX_STRING_LENGTH );
			_maxStringLength.Changed += MaxStringLengthChanged;
			_changing = false;
		}

		public TextAttributeData( float maxLength = 10.0f, bool normalize = true, bool block = false, bool oneLine = false, bool breakLine = true, bool expanding = true, HorizontalAlignment horizontal = HorizontalAlignment.LEFT, VerticalAlignment vertical = VerticalAlignment.BOTTOM ) : this( Mem.Font.Get( "default" ), maxLength, normalize, block, oneLine, breakLine, expanding, horizontal, vertical ) { }

		private void ExpandingChanged( bool oldValue ) {
			if( !_changing )
				ExpansionAttitudeChanged?.Invoke();
		}

		private void MaxStringLengthChanged( int oldValue ) {
			if( !_changing )
				StringLengthChanged?.Invoke();
		}

		private void AttributeChanged( Font oldValue ) {
			if( !_changing )
				TextAttributesChanged?.Invoke();
		}

		private void AttributeChanged( bool oldValue ) {
			if( !_changing )
				TextAttributesChanged?.Invoke();
		}

		private void AttributeChanged( float oldValue ) {
			if( !_changing )
				TextAttributesChanged?.Invoke();
		}

		private void AttributeChanged( HorizontalAlignment oldValue ) {
			if( !_changing )
				TextAttributesChanged?.Invoke();
		}

		private void AttributeChanged( VerticalAlignment oldValue ) {
			if( !_changing )
				TextAttributesChanged?.Invoke();
		}

		public TextAttributeData SetMaxLength( float maxLength ) {
			MaxLength = maxLength;
			return this;
		}

		public TextAttributeData SetNormalization( bool normalize ) {
			NormalizeScale = normalize;
			return this;
		}

		public TextAttributeData SetOneLine( bool oneLine ) {
			OneLine = oneLine;
			return this;
		}

		public TextAttributeData SetBreakLine( bool breakLine ) {
			BreakLine = breakLine;
			return this;
		}

		public TextAttributeData SetAlignment( HorizontalAlignment horizontal ) {
			HorizontalAlignment = horizontal;
			return this;
		}

		public TextAttributeData SetAlignment( VerticalAlignment vertical ) {
			VerticalAlignment = vertical;
			return this;
		}

		public void Set( TextAttributeData data ) {
			_changing = true;
			Font = data.Font;
			NormalizeScale = data.NormalizeScale;
			OneLine = data.OneLine;
			BreakLine = data.BreakLine;
			MaxLength = data.MaxLength;
			Block = data.Block;
			Expanding = data.Expanding;
			Font = data.Font;
			HorizontalAlignment = data.HorizontalAlignment;
			VerticalAlignment = data.VerticalAlignment;
			MaxStringLength = data.MaxStringLength;
			_changing = false;
			TextAttributesChanged?.Invoke();
		}

		public TextAttributeData Clone() {
			return new TextAttributeData( Font ) {
				NormalizeScale = NormalizeScale,
				OneLine = OneLine,
				BreakLine = BreakLine,
				MaxLength = MaxLength,
				Block = Block,
				Expanding = Expanding,
				Font = Font,
				HorizontalAlignment = HorizontalAlignment,
				VerticalAlignment = VerticalAlignment,
				MaxStringLength = MaxStringLength
			};
		}
	}
}
