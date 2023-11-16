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
    float time = DspTime / PeriodicScale * 0.25;
    float edge = frac((distance - time) * PeriodicScale);

    const float k = 0.5;
    float w = LineWidth;
    Alpha = smoothstep(k - w, k, edge) * smoothstep(k + w, k, edge);
}

void ContactPoint_float(
    float3 PositionWS,
    float3 NormalWS,
    float3 PointWS,
    float3 PointNormalWS,
    float Length,
    out float Out)
{
    float distance = length(PositionWS - PointWS);
    float distanceRatio = saturate(1 - (distance / Length));
    float frensel = saturate(dot(NormalWS, NormalWS));

    Out = distanceRatio * frensel;
}

#endif // AR_WORLD_MESH_INCLUDE
