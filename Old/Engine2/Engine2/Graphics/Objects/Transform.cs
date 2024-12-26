using Engine.LMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects {
	public interface ITransform {
		Matrix4 TMatrix { get; }
	}

	public sealed class TransformInterface<T1, T2, T3> where T1 : struct where T2 : struct where T3 : struct {

		private Transform<T1, T2, T3> transform;

		public TransformInterface( Transform<T1, T2, T3> transform ) {
			this.transform = transform;
		}

		public T1 Translation { get { return transform.Translation; } set { transform.Translation = value; } }
		public T2 Rotation { get { return transform.Rotation; } set { transform.Rotation = value; } }
		public T3 Scale { get { return transform.Scale; } set { transform.Scale = value; } }
		public T1 GlobalTranslation { get { return transform.GlobalTranslation; } }
		public T2 GlobalRotation { get { return transform.GlobalRotation; } }
		public T3 GlobalScale { get { return transform.GlobalScale; } }
		public Matrix4 TMatrix => transform.TMatrix;

		public void SetParent( Transform<T1, T2, T3> transform ) {
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
		public Matrix4 TMatrix => transform.TMatrix;

		public void SetParent( Transform<T1, T2, T3> transform ) {
			transform.SetParent( this.transform );
		}
	}

	public abstract class Transform<T1, T2, T3> : ITransform where T1 : struct where T2 : struct where T3 : struct {

		protected Matrix4 _matrix;
		protected bool _changed;
		protected abstract Matrix4 TranslationMatrix { get; }
		protected abstract Matrix4 RotationMatrix { get; }
		protected abstract Matrix4 ScaleMatrix { get; }

		protected Transform<T1, T2, T3> parent;

		public event TransformChangeHandler Changed;
		public delegate void TransformChangeHandler();

		public readonly TransformInterface<T1, T2, T3> Interface;
		public readonly TransformReadonlyInterface<T1, T2, T3> Readonly;

		public Transform() {
			_matrix = Matrix4.Identity;
			_changed = true;
			parent = null;
			Interface = new TransformInterface<T1, T2, T3>( this );
			Readonly = new TransformReadonlyInterface<T1, T2, T3>( this );
		}

		public virtual Matrix4 TMatrix {
			get {
				if( parent != null )
					parent.CheckChanged();
				if( _changed ) {
					Changed?.Invoke();
					if( parent != null )
						_matrix = CreateMatrix() * parent.TMatrix;
					else
						_matrix = CreateMatrix();
					_changed = false;
				}

				return _matrix;
			}
		}

		protected virtual Matrix4 CreateMatrix() {
			return ScaleMatrix * RotationMatrix * TranslationMatrix;
		}

		protected void CheckChanged() {
			if( parent != null )
				parent.CheckChanged();
			if( _changed ) {
				Changed?.Invoke();
				if( parent != null )
					_matrix = ScaleMatrix * RotationMatrix * TranslationMatrix * parent.TMatrix;
				else
					_matrix = ScaleMatrix * RotationMatrix * TranslationMatrix;
				_changed = false;
			}
		}

		public abstract T1 Translation { get; set; }
		public abstract T2 Rotation { get; set; }
		public abstract T3 Scale { get; set; }
		public abstract T1 GlobalTranslation { get; }
		public abstract T2 GlobalRotation { get; }
		public abstract T3 GlobalScale { get; }

		public TransformReadonlyInterface<T1, T2, T3> Parent {
			get {
				return parent.Readonly;
			}
		}

		public void SetParent( Transform<T1, T2, T3> transform ) {
			if( transform == parent )
				return;

			if( parent != null )
				parent.Changed -= SetChanged;
			this.parent = transform;
			if( parent != null )
				parent.Changed += SetChanged;

			_changed = true;
		}

		public void SetParentFromInterface( TransformInterface<T1, T2, T3> transform ) {
			transform.SetParent( this );
		}

		public void SetParentFromReadonly( TransformReadonlyInterface<T1, T2, T3> transform ) {
			transform.SetParent( this );
		}

		protected void SetChanged() {
			_changed = true;
		}

		public override string ToString() {
			return $"Transform:[{Translation}][{Rotation}][{Scale}]";
		}

	}
}
