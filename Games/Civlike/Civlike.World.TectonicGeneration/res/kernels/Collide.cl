#include "particle.h"

__kernel void Collide(__global const Particle* src,
                      __global       Particle* dst,
                      float floorY, float k, int n)
{
    int i = get_global_id(0);
    if (i >= n) return;
    Particle p = src[i];
    // Branchless floor clamp on Y; simple spring-like response
    float pen = fmax(0.0f, floorY - p.pos.y);
    p.pos.y = fmax(p.pos.y, floorY);
    p.vel.y += k * pen;
    dst[i] = p;
}
