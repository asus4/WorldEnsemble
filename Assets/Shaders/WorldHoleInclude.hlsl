#ifndef WORLD_HOLE_INCLUDE
#define WORLD_HOLE_INCLUDE

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"

// Parallax from custom height pixels instaed of texture
void ParallaxOffset_half(
    half Height,
    half3 ViewDirTS,
    half Scale,
    out half2 OffsetUv)
{
    OffsetUv = ParallaxOffset1Step(Height, Scale, ViewDirTS);
}

#endif // WORLD_HOLE_INCLUDE
