using Engine.Utilities.Data;
using Engine.Utilities.Time;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs {
	public class ConstraintManager {

		private readonly UIElement e;
		public ConstraintTransform.ReadonlyInterface Transform { get => transform.Interface; }
		private readonly ConstraintTransform transform;
		public InterpolationMethod Interpolation { get; set; }
		public ConstraintAnimation Animation { get; private set; }
		private readonly Queue<QueuedAnimation> animationQueue;
		private bool queueEngaged;
		private float queueEngageTime;

		public bool RepeatQueue;

		public ConstraintManager( UIElement e ) {
			this.e = e;
			transform = new ConstraintTransform();
			Interpolation = InterpolationMethods.CosineInterpolation;
			animationQueue = new Queue<QueuedAnimation>();
		}

		internal void Update() {
			if( Animation is null )
				if( !HandleAnimationQueue() )
					return;

			Animation.Update( e.Manager.Time, transform, e, e.Manager.Window );
			if( !Animation.Animating && animationQueue.Count > 0 )
				Animation = null;
		}

		private bool HandleAnimationQueue() {
			if( animationQueue.Count == 0 )
				return false;

			QueuedAnimation qAnim = animationQueue.Peek();
			if( !queueEngaged ) {
				queueEngaged = true;
				queueEngageTime = e.Manager.Time;
			}
			if( e.Manager.Time < queueEngageTime + qAnim.delay )
				return false;
			queueEngaged = false;
			qAnim = animationQueue.Dequeue();
			if( !( Animation is null ) )
				Animation.FinalizeAnimation( e.Manager.Time, transform, e, e.Manager.Window );
			Animation = qAnim.animation;
			Animation.Start( transform, e.Manager.Time );
			if( RepeatQueue )
				animationQueue.Enqueue( qAnim );
			return true;
		}

		public void Enqueue( ConstraintAnimation animation, float delay = 0 ) {
			animationQueue.Enqueue( new QueuedAnimation( animation, delay ) );
		}

		public void Enqueue( ConstraintBundle bundle, float duration, float delay = 0 ) {
			animationQueue.Enqueue( new QueuedAnimation( new ConstraintAnimation( bundle, duration, Interpolation ), delay ) );
		}

		public void Enqueue( ConstraintBundle bundle, float duration, InterpolationMethod interpolation, float delay = 0 ) {
			animationQueue.Enqueue( new QueuedAnimation( new ConstraintAnimation( bundle, duration, interpolation ), delay ) );
		}

		public void Start() {
			if( e.Manager is null )
				return;
			Animation?.Start( transform, e.Manager.Time );
		}

		public void Set( ConstraintBundle bundle, float duration = 0 ) {
			Animation = new ConstraintAnimation( bundle, duration, Interpolation );
		}

		private class QueuedAnimation {
			public ConstraintAnimation animation;
			public float delay;

			public QueuedAnimation( ConstraintAnimation animation, float delay ) {
				this.animation = animation;
				this.delay = delay;
			}
		}

	}
}
