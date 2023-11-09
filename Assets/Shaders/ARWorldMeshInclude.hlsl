#ifndef AR_WORLD_MESH_INCLUDE
#define AR_WORLD_MESH_INCLUDE

void PeriodicLine_float(
    float3 Pos,
    float3 CameraPos,
    float LineWidth,
    float PeriodicScale,
    float DspTime,
    out float Alpha)
{
    float distance = length(Pos - CameraPos);
    float time = DspTime / PeriodicScale;
    float edge = frac((distance - time) * PeriodicScale);

    const float k = 0.5;
    float w = LineWidth;
    Alpha = smoothstep(k - w, k, edge) * smoothstep(k + w, k, edge);
}

#endif // AR_WORLD_MESH_INCLUDE
