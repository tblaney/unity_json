#ifndef WATER_UTILITIES
#define WATER_UTILITIES



// DEFINES.
#define PI (3.1415926536)

#include "shader_func_utils.cginc"

float3 CorrectColorForWaterRings(float3 worldPosition, float3 colorIn, float opacity)
{
    float2 uv = worldPosition.xz - _PositionWaterCam.xz;
    uv = uv / (_OrthographicWaterCamSize * 2);
    uv += 0.5;			
    float3 waterRingVisibility = tex2Dlod(_WaterEffectRT, float4(uv, 0, 0));
    waterRingVisibility = waterRingVisibility*opacity;

    return colorIn + waterRingVisibility;
}


#endif  // WATER_UTILITIES