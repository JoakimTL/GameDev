﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Utilities.Data.Boxing {
	public class MutableTrio<T1, T2, T3> {

		public T1 ValueA { get; private set; }
		public T2 ValueB { get; private set; }
		public T3 ValueC { get; private set; }

		public delegate void MutableTrioValueChange<_T1, _T2, _T3>( MutableTrio<_T1, _T2, _T3> trio );
		public event MutableTrioValueChange<T1, T2, T3> ValueAChanged;
		public event MutableTrioValueChange<T1, T2, T3> ValueBChanged;
		public event MutableTrioValueChange<T1, T2, T3> ValueCChanged;
		public event MutableTrioValueChange<T1, T2, T3> ValuesChanged;
		public event MutableTrioValueChange<T1, T2, T3> AnyChanged;

		public MutableTrio( T1 a, T2 b, T3 c ) {
			ValueA = a;
			ValueB = b;
			ValueC = c;
		}

		public void SetA( T1 a ) {
			ValueA = a;
			ValueAChanged?.Invoke( this );
			AnyChanged?.Invoke( this );
		}

		public void SetB( T2 b ) {
			ValueB = b;
			ValueBChanged?.Invoke( this );
			AnyChanged?.Invoke( this );
		}

		public void SetC( T3 c ) {
			ValueC = c;
			ValueCChanged?.Invoke( this );
			AnyChanged?.Invoke( this );
		}

		public void Set( T1 a, T2 b, T3 c ) {
			ValueA = a;
			ValueB = b;
			ValueC = c;
			ValuesChanged?.Invoke( this );
			AnyChanged?.Invoke( this );
		}

		public override bool Equals( object o ) {
			if( o is null )
				return false;
			ImmutableTrio<T1, T2, T3> no = o as ImmutableTrio<T1, T2, T3>;
			if( no is null )
				return false;
			return Equals( no );
		}

		public bool Equals( ImmutableTrio<T1, T2, T3> o ) {
			if( ValueA is null ) {
				if( !( o.ValueA is null ) )
					return false;
			} else
				if( !ValueA.Equals( o.ValueA ) )
				return false;
			if( ValueB is null ) {
				if( !( o.ValueB is null ) )
					return false;
			} else
				if( !ValueB.Equals( o.ValueB ) )
				return false;
			if( ValueC is null ) {
				if( !( o.ValueC is null ) )
					return false;
			} else
				if( !ValueC.Equals( o.ValueC ) )
				return false;
			return true;
		}

		public override int GetHashCode() {
			return ValueA.GetHashCode() ^ ValueB.GetHashCode() ^ ValueC.GetHashCode();
		}

	}
}