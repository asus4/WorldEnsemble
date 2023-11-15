#ifndef GET_MAIN_LIGHT_INCLUDE
#define GET_MAIN_LIGHT_INCLUDE

// See Unity blog post:
// https://blog.unity.com/engine-platform/custom-lighting-in-shader-graph-expanding-your-graphs-in-2019


void MainLight_half(float3 WorldPos, out half3 Direction, out half3 Color, out half DistanceAtten, out half ShadowAtten)
{
#if SHADERGRAPH_PREVIEW
   Direction = half3(0.5, 0.5, 0);
   Color = 1;
   DistanceAtten = 1;
   ShadowAtten = 1;
#else

#if defined(_MAIN_LIGHT_SHADOWS_SCREEN) && !defined(_SURFACE_TYPE_TRANSPARENT)
   half4 clipPos = TransformWorldToHClip(WorldPos);
   half4 shadowCoord = ComputeScreenPos(clipPos);
#else
   half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif // SHADOWS_SCREEN

   Light mainLight = GetMainLight(shadowCoord);
   Direction = mainLight.direction;
   Color = mainLight.color;
   DistanceAtten = mainLight.distanceAttenuation;

   //ShadowAtten = mainLight.shadowAttenuation;
   ShadowAtten = MainLightRealtimeShadow(shadowCoord);
#endif // SHADERGRAPH_PREVIEW
}

#endif // GET_MAIN_LIGHT_INCLUDE
