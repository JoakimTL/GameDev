using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects {
	public interface ITransform {
		Matrix4 Matrix { get; }
		event Action OnSetChanged;
		event Action OnChangedEvent;
	}

	public abstract class Transform : ITransform {

		protected Matrix4 _matrix;
		protected bool _changed; 
		
		public event Action OnSetChanged;
		public event Action OnChangedEvent;

		protected abstract Matrix4 CreateMatrix();
		public abstract Transform Parent { get; }

		public Matrix4 Matrix {
			get {
				CheckChanged();
				return _matrix;
			}
		}

		protected void CheckChanged() {
			if( !(Parent is null) )
				Parent.CheckChanged();
			if( _changed ) {
				OnChangedEvent?.Invoke();
				if( !( Parent is null ) )
					_matrix = CreateMatrix() * Parent.Matrix;
				else
					_matrix = CreateMatrix();
				_changed = false;
			}
		}

		protected void SetChanged() {
			_changed = true;
			OnSetChanged?.Invoke();
			OnChangedEvent?.Invoke();
		}
	}

	public abstract class Transform<T1, T2, T3> : Transform where T1 : struct where T2 : struct where T3 : struct {

		public override Transform Parent => parent;
		protected Transform<T1, T2, T3> parent;

		protected abstract Matrix4 TranslationMatrix { get; }
		protected abstract Matrix4 RotationMatrix { get; }
		protected abstract Matrix4 ScaleMatrix { get; }

		public readonly TransformInterface<T1, T2, T3> Interface;
		public readonly TransformReadonlyInterface<T1, T2, T3> Readonly;

		public Transform() {
			_matrix = Matrix4.Identity;
			_changed = true;
			parent = null;
			Interface = new TransformInterface<T1, T2, T3>( this );
			Readonly = new TransformReadonlyInterface<T1, T2, T3>( this );
		}

		protected override Matrix4 CreateMatrix() {
			return ScaleMatrix * RotationMatrix * TranslationMatrix;
		}

		public abstract T1 Translation { get; set; }
		public abstract T2 Rotation { get; set; }
		public abstract T3 Scale { get; set; }
		public abstract T1 GlobalTranslation { get; }
		public abstract T2 GlobalRotation { get; }
		public abstract T3 GlobalScale { get; }

		public void SetParent( Transform<T1, T2, T3> transform ) {
			if( transform == Parent )
				return;

			if( parent != null ) {
				parent.OnChangedEvent -= SetChanged;
			}
			parent = transform;
			if( parent != null ) { 
				parent.OnChangedEvent += SetChanged;
			}

			_changed = true;
		}

		public void SetParentFromInterface( TransformInterface<T1, T2, T3> transform ) {
			transform.SetParent( this );
		}

		public void SetParentFromReadonly( TransformReadonlyInterface<T1, T2, T3> transform ) {
			transform.SetParent( this );
		}

		public override string ToString() {
			return $"Transform:[{Translation}][{Rotation}][{Scale}]";
		}

	}

	public sealed class TransformInterface<T1, T2, T3> where T1 : struct where T2 : struct where T3 : struct {

		private readonly Transform<T1, T2, T3> transform;

		public TransformInterface( Transform<T1, T2, T3> transform ) {
			this.transform = transform;
		}

		public T1 Translation { get { return transform.Translation; } set { transform.Translation = value; } }
		public T2 Rotation { get { return transform.Rotation; } set { transform.Rotation = value; } }
		public T3 Scale { get { return transform.Scale; } set { transform.Scale = value; } }
		public T1 GlobalTranslation { get { return transform.GlobalTranslation; } }
		public T2 GlobalRotation { get { return transform.GlobalRotation; } }
		public T3 GlobalScale { get { return transform.GlobalScale; } }
		public Matrix4 Matrix => transform.Matrix;

		internal void SetParent( Transform<T1, T2, T3> transform ) {
			transform.SetParent( this.transform );
		}

	}

	public sealed class TransformReadonlyInterface<T1, T2, T3> where T1 : struct where T2 : struct where T3 : struct {

		private readonly Transform<T1, T2, T3> transform;

		public TransformReadonlyInterface( Transform<T1, T2, T3> transform ) {
			this.transform = transform;
		}

		public T1 Translation { get { return transform.Translation; } }
		public T2 Rotation { get { return transform.Rotation; } }
		public T3 Scale { get { return transform.Scale; } }
		public T1 GlobalTranslation { get { return transform.GlobalTranslation; } }
		public T2 GlobalRotation { get { return transform.GlobalRotation; } }
		public T3 GlobalScale { get { return transform.GlobalScale; } }
		public Matrix4 Matrix => transform.Matrix;

		internal void SetParent( Transform<T1, T2, T3> transform ) {
			transform.SetParent( this.transform );
		}
	}
}
