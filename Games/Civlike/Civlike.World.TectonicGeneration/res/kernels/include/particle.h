// Use float4 to avoid float3 alignment surprises across hosts.
typedef struct {
    float4 pos;   // .w unused
    float4 vel;   // .w unused
    float  mass;
    int    state;
    float  _pad0, _pad1; // pad to 16-byte multiple
} Particle;