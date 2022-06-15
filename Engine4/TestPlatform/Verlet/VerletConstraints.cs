using System.Numerics;

namespace TestPlatform.Verlet;

public static class VerletConstraints {
	private static readonly Random r = new();

	internal static void GlobalUpdate( VerletParticle[] particles, uint activeParticles, float time, float dt ) {
		for ( int i = 0; i < activeParticles; i++ ) {
			if ( !particles[ i ].Alive ) {
				particles[ i ].Reset( new Vector3( r.NextSingle() * 2 - 1, r.NextSingle() * 2 - 1, r.NextSingle() * 2 - 1 ) + new Vector3( 0, 2, 0 ), ( r.NextSingle() * .5f + .5f ) * 0.125f );
				break;
			}
		}
	}

	internal static void AddGravity( VerletParticle[] particles, uint activeParticles, float time, float dt ) {
		for ( int i = 0; i < activeParticles; i++ ) {
			if ( particles[ i ].Alive ) {
				particles[ i ].Acceleration += new Vector3( 0, -9.81f, 0 );
			}
		}
	}

	internal static void KeepInScene( VerletParticle[] particles, uint activeParticles, float time, float dt ) {
		for ( int i = 0; i < activeParticles; i++ ) {
			if ( particles[ i ].Alive ) {
				//-cos(x*x*pi/2)
				Vector2 xz = new( particles[ i ].Position.X, particles[ i ].Position.Z );
				float lensq = xz.LengthSquared();
				if ( lensq < 1 ) {
					float h = -MathF.Cos( lensq * MathF.PI * 0.5f );
					if ( particles[ i ].Position.Y <= h ) {
						float derivativeX = MathF.PI * particles[ i ].Position.X * MathF.Sin( lensq * MathF.PI * 0.5f );
						float derivativeZ = MathF.PI * particles[ i ].Position.Z * MathF.Sin( lensq * MathF.PI * 0.5f );
						particles[ i ].Position -= new Vector3( derivativeX * dt, particles[ i ].Position.Y - h, derivativeZ * dt );
					}
				} else {
					float h = lensq - 1;
					if ( particles[ i ].Position.Y <= h ) {
						float derivativeX = 2 * particles[ i ].Position.X;
						float derivativeZ = 2 * particles[ i ].Position.Z;
						particles[ i ].Position -= new Vector3( derivativeX * dt, particles[ i ].Position.Y - h, derivativeZ * dt );
					}
				}
			}
		}
	}

	internal static void IncreaseTemperature( VerletParticle[] particles, uint activeParticles, float time, float dt ) {
		for ( int i = 0; i < activeParticles; i++ ) {
			if ( particles[ i ].Alive ) {
				if ( Vector3.Distance( particles[ i ].Position, new Vector3( 0, -1, 0 ) ) < 0.5f )
					particles[ i ].Temperature += 1600 * dt;
			}
		}
	}
	internal static void TemperatureBleed( VerletParticle[] particles, uint activeParticles, float time, float dt ) {
		for ( int i = 0; i < activeParticles; i++ ) {
			if ( particles[ i ].Alive ) {
				particles[ i ].Temperature *= 1 - ( 0.14f * dt );
			}
		}
	}

	internal static void ForceFromTemperature( VerletParticle[] particles, uint activeParticles, float time, float dt ) {
		for ( int i = 0; i < activeParticles; i++ ) {
			if ( particles[ i ].Alive ) {
				particles[ i ].Acceleration += new Vector3( 0, particles[ i ].Temperature * 0.005f, 0 );
			}
		}
	}

	internal static void Collision( VerletParticle[] particles, uint activeParticles, float time, float dt ) {
		for ( int i = 0; i < activeParticles; i++ ) {
			if ( particles[ i ].Alive ) {
				for ( int j = i + 1; j < activeParticles; j++ ) {
					if ( particles[ j ].Alive ) {
						Vector3 collisionAxis = particles[ i ].Position - particles[ j ].Position;
						float combinedRadius = particles[ i ].Radius + particles[ j ].Radius;
						float collisionLength = collisionAxis.Length();
						if ( collisionLength <= combinedRadius ) {
							Vector3 n = collisionAxis / collisionLength;
							float delta = combinedRadius - collisionLength;
							particles[ i ].Position += .5f * delta * n;
							particles[ j ].Position -= .5f * delta * n;
							float tempDiff = particles[ i ].Temperature - particles[ j ].Temperature;
							particles[ i ].Temperature -= tempDiff * 5 * dt;
							particles[ j ].Temperature += tempDiff * 5 * dt;
						}
					}
				}
			}
		}
	}

	internal static void Resolve( VerletParticle[] particles, uint activeParticles, float time, float dt ) {
		for ( int i = 0; i < activeParticles; i++ ) {
			if ( particles[ i ].Alive ) {
				VerletParticle p = particles[ i ];
				Vector3 vel = p.Position - p.LastPosition;
				p.LastPosition = p.Position;
				p.Position += vel + p.Acceleration * dt * dt;
				p.Acceleration = new();
			}
		}
	}
}