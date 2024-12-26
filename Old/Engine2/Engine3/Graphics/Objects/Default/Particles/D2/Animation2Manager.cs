using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Graphics.Objects.Default.Particles.D2 {
	public class Animation2Manager {
		private readonly Particle2 p;
		private AnimationTarget source, target;
		private float animStartTime;
		private Queue<AnimationTarget> queue;
		private bool queueEngaged;
		private float queueEngageTime;
		public bool RepeatQueue;

		public Animation2Manager( Particle2 p ) {
			this.p = p;
			queue = new Queue<AnimationTarget>();
		}

		public void Update() {
			if( target is null || p.System.Time > target.duration + animStartTime ) {
				int ret = HandleAnimationQueue();
				if( ret <= 0 ) {
					if( ret == -1 )
						if( p.TerminateFromQueue )
							p.Alive = false;
					return;
				}
			}

			if( target is null )
				return;
			float interp = target.interpolation.Invoke( ( p.System.Time - animStartTime ) / target.duration );
			p.Data.Blend = interp;
			Vector4b sourceColor = 0;
			if( !( source is null ) )
				sourceColor = source.targetDiffuse;
			p.Data.Color = target.targetDiffuse * interp + sourceColor * ( 1 - interp );
		}

		private int HandleAnimationQueue() {
			if( queue.Count == 0 )
				return -1;

			AnimationTarget qAnim = queue.Peek();
			if( !queueEngaged ) {
				queueEngaged = true;
				queueEngageTime = p.System.Time;
			}
			if( p.System.Time < queueEngageTime + qAnim.delay )
				return 0;
			queueEngaged = false;
			qAnim = queue.Dequeue();
			source = target;
			Vector2 off1 = 0;
			Vector2 off2 = 0;
			if( !( source is null ) )
				off1 = p.System.ParticleMaterial.GetPointFromIndex( source.textureIndex );
			target = qAnim;
			if( !( target is null ) )
				off2 = p.System.ParticleMaterial.GetPointFromIndex( target.textureIndex );
			p.Data.TextureOffset1 = off1;
			p.Data.TextureOffset2 = off2;
			animStartTime = p.System.Time;
			if( RepeatQueue )
				queue.Enqueue( qAnim );
			return 1;
		}

		public void Enqueue( AnimationTarget target ) {
			queue.Enqueue( target );
		}

		public void Set( AnimationTarget target ) {
			source = target;
			this.target = target;
			p.Data.TextureOffset1 = p.System.ParticleMaterial.GetPointFromIndex( target.textureIndex );
			p.Data.TextureOffset2 = p.System.ParticleMaterial.GetPointFromIndex( target.textureIndex );
			animStartTime = p.System.Time;
		}

}
}
