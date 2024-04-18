#ifndef SNORRI_UTILITIES
#define SNORRI_UTILITIES


// DEFINES.
#define PI (3.1415926536)

uniform float _LightIntensity;

uniform float3 _PlayerPosition;
uniform float3 _PlayerDirection;

uniform float _BendAmount;
uniform float _BendAmountClouds;

uniform float3 _ColorEnvironment;

uniform int _DoBend = 1;

uniform float _InteriorHideAmount = 1.0;

uniform sampler2D _TerrainTex;
uniform sampler2D _TerrainEffectTex;

uniform float3 _CameraPosition;
uniform float3 _Position;
uniform float _CameraSize;
uniform float _OrthographicCamSize;

uniform sampler2D _GlobalEffectRT;
uniform float _OrthographicWaterCamSize;
uniform float3 _PositionWaterCam;
uniform sampler2D _WaterEffectRT;

uniform sampler2D _TerrainHoleTex;

uniform float _SnowHeight;
uniform float _SnowOpacity;

float BendFactor(float3 inPosWorld)
{
    if (_DoBend == 0)
    {
        return 1.0;
    }
    float3 playerPos = _PlayerPosition;
    float3 vertWorldPos = inPosWorld;
    
    float d = abs(playerPos.z-vertWorldPos.z);
    d = length(playerPos-vertWorldPos);
    

    float factor = pow(d, 2)/_BendAmount;

    return factor;
}
float3 Bend(float3 inPosWorld, float3 inPosVert)
{ 
    if (_DoBend == 0)
    {
        return inPosVert;
    }

    float factor = BendFactor(inPosWorld);
    
    float3 vertPosOut = inPosVert;
    vertPosOut.y-=factor;

    return vertPosOut;
}
float3 TerrainEffect(float3 worldPos)
{
    float2 uv = worldPos.xz - _Position.xz;
    uv = uv / (_CameraSize * 2);
    uv += 0.5;			
    half4 terrain = tex2D(_TerrainTex, float4(uv, 0, 0));

    return terrain.rgb;
}
float3 InteriorPass(float3 colorIn)
{
    return colorIn*_InteriorHideAmount;
}

#endif  // SNORRI_UTILITIES