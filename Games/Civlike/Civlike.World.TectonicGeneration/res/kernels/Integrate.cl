#include "particle.h"

__kernel void Integrate(__global const Particle* src,
                        __global       Particle* dst,
                        float dt, int n)
{
    int i = get_global_id(0);
    if (i >= n) return;
    Particle p = src[i];
    p.pos.xyz += p.vel.xyz * dt;   // simple Euler step
    dst[i] = p;
}
