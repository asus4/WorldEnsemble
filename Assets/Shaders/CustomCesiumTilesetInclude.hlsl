#ifndef CUSTOM_CESIUM_TILESET_INCLUDE
#define CUSTOM_CESIUM_TILESET_INCLUDE

/*
Based on CesiumUnlitTilesetShader.
https://github.com/CesiumGS/cesium-unity/blob/main/LICENSE

Modified by asus4
*/

float4 CesiumRasterOverlay(in UnityTexture2D tex, float2 uv, float4 texTS)
{
   uv = uv * texTS.zw+ texTS.xy;
   uv.y = 1.0 - uv.y;
   float4 color = SAMPLE_TEXTURE2D(tex.tex, tex.samplerstate, tex.GetTransformedUV(uv));
   return color;
}

half4 CesiumRasterOverlay(in UnityTexture2D tex, half2 uv, half4 texTS)
{
   uv = uv * texTS.zw+ texTS.xy;
   uv.y = 1.0 - uv.y;
   half4 color = SAMPLE_TEXTURE2D(tex.tex, tex.samplerstate, tex.GetTransformedUV(uv));
   return color;
}

void CesiumSampleRasterOverlays_float(
   float2 uv0, float2 uv1, float2 uv2, float2 uv3,
   float4 vertexColor,
   in UnityTexture2D baseTex, int baseTexUvIndex, float4 baseColorFactor,
   in UnityTexture2D overlay0Tex, int overlay0UvIndex, float4 overlay0TexTS,
   in UnityTexture2D overlay1Tex, int overlay1UvIndex, float4 overlay1TexTS,
   in UnityTexture2D overlay2Tex, int overlay2UvIndex, float4 overlay2TexTS,
   out float4 OUT)
{
   float2 texCoords[4] = {uv0, uv1, uv2, uv3};

   // Sample base texture
   float4 color = SAMPLE_TEXTURE2D(baseTex.tex, baseTex.samplerstate, baseTex.GetTransformedUV(texCoords[baseTexUvIndex]));
   color *= vertexColor * baseColorFactor;

   // Overlay 0
   float4 overlay = CesiumRasterOverlay(overlay0Tex, texCoords[overlay0UvIndex], overlay0TexTS);
   color = lerp(color, overlay, overlay.a);
   // Overlay 1
   overlay = CesiumRasterOverlay(overlay1Tex, texCoords[overlay1UvIndex], overlay1TexTS);
   color = lerp(color, overlay, overlay.a);
   // Overlay 2
   overlay = CesiumRasterOverlay(overlay2Tex, texCoords[overlay2UvIndex], overlay2TexTS);
   color = lerp(color, overlay, overlay.a);

   OUT = color;
}

void CesiumSampleRasterOverlays_half(
   half2 uv0, half2 uv1, half2 uv2, half2 uv3,
   half4 vertexColor,
   in UnityTexture2D baseTex, int baseTexUvIndex, half4 baseColorFactor,
   in UnityTexture2D overlay0Tex, int overlay0UvIndex, half4 overlay0TexTS,
   in UnityTexture2D overlay1Tex, int overlay1UvIndex, half4 overlay1TexTS,
   in UnityTexture2D overlay2Tex, int overlay2UvIndex, half4 overlay2TexTS,
   out half4 OUT)
{
   half2 texCoords[4] = {uv0, uv1, uv2, uv3};

   // Sample base texture
   half4 color = SAMPLE_TEXTURE2D(baseTex.tex, baseTex.samplerstate, baseTex.GetTransformedUV(texCoords[baseTexUvIndex]));
   color *= vertexColor * baseColorFactor;

   // Overlay 0
   half4 overlay = CesiumRasterOverlay(overlay0Tex, texCoords[overlay0UvIndex], overlay0TexTS);
   color = lerp(color, overlay, overlay.a);
   // Overlay 1
   overlay = CesiumRasterOverlay(overlay1Tex, texCoords[overlay1UvIndex], overlay1TexTS);
   color = lerp(color, overlay, overlay.a);
   // Overlay 2
   overlay = CesiumRasterOverlay(overlay2Tex, texCoords[overlay2UvIndex], overlay2TexTS);
   color = lerp(color, overlay, overlay.a);

   OUT = color;
}

#endif // CUSTOM_CESIUM_TILESET_INCLUDE
