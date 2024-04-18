

float4 _SnowPathColorIn, _SnowPathColorOut;
float _SnowPathBlending;
float _SnowTextureScale;
float _SnowPathStrength;
float3 _SnowColor;

float _NoiseScale, _NoiseWeight;
sampler2D _NoiseTex, _SnowTex;
float _SnowStart;

#include "shader_func_utils.cginc"

float GetSnowEffect(float3 pos)
{
    return step(_SnowStart + sin(pos.x) + sin(pos.z), pos.y);
}
float GetSnowEffectSmooth(float3 pos)
{
    return saturate(pos.y/(_SnowStart + sin(pos.x) + sin(pos.z)));
}

float3 CorrectVertForSnow(float3 localPos, float3 worldPos, float3 normal)
{
    float3 localPosition = localPos;
    float3 worldPosition = worldPos;

    float2 uv = worldPosition.xz - _Position.xz;
    uv = uv / (_OrthographicCamSize * 2);
    uv += 0.5;	
    float4 RTEffect = tex2Dlod(_GlobalEffectRT, float4(uv, 0, 0));

    RTEffect *=  smoothstep(0.99, 0.9, uv.x) * smoothstep(0.99, 0.9,1- uv.x);
    RTEffect *=  smoothstep(0.99, 0.9, uv.y) * smoothstep(0.99, 0.9,1- uv.y);

    float snow_visibility = tex2Dlod(_GlobalEffectRT, float4(uv, 0, 0)).r;
    float SnowNoise = tex2Dlod(_NoiseTex, float4(worldPosition.xz * _NoiseScale, 0, 0));
    float3 vertex_addition = (_SnowOpacity + (SnowNoise * _NoiseWeight * saturate(_SnowOpacity*4)));
    vertex_addition *= (1-snow_visibility);
    float worldNormalDotNoise = dot(normal, float3(0, 1, 0));
    vertex_addition.x *= 0;
    vertex_addition.z *= 0;
    float3 vertPos = localPosition + vertex_addition*_SnowHeight*GetSnowEffect(worldPosition);

    return vertPos;
}

float3 CorrectVertForSnowObject(float3 localPos, float3 worldPos, float3 normal)
{
    float3 localPosition = localPos;
    float3 worldPosition = worldPos;

    float2 uv = worldPosition.xz - _Position.xz;
    uv = uv / (_OrthographicCamSize * 2);
    uv += 0.5;	
    float4 RTEffect = tex2Dlod(_GlobalEffectRT, float4(uv, 0, 0));

    RTEffect *=  smoothstep(0.99, 0.9, uv.x) * smoothstep(0.99, 0.9,1- uv.x);
    RTEffect *=  smoothstep(0.99, 0.9, uv.y) * smoothstep(0.99, 0.9,1- uv.y);

    float snow_visibility = tex2Dlod(_GlobalEffectRT, float4(uv, 0, 0)).r;
    float SnowNoise = tex2Dlod(_NoiseTex, float4(worldPosition.xz * _NoiseScale, 0, 0));
    float3 vertex_addition = (_SnowOpacity + (SnowNoise * _NoiseWeight * saturate(_SnowOpacity*4)));
    vertex_addition *= (1-snow_visibility);
    float worldNormalDotNoise = dot(normal, float3(0, 1, 0));
    vertex_addition.x *= 0;
    vertex_addition.z *= 0;
    float3 vertPos = localPosition + vertex_addition*_SnowHeight*GetSnowEffectSmooth(worldPosition)*saturate(localPosition.y-0.5);

    return vertPos;
}

float3 CorrectColorForSnow(float3 worldPos, float3 colorEnd)
{
    float3 snow = tex2D(_SnowTex, worldPos.xz * _SnowTextureScale);
    float2 uv = worldPos.xz - _Position.xz;
    uv /= (_OrthographicCamSize * 2);
    uv += 0.5;

    float4 effect = tex2D(_GlobalEffectRT, float2 (uv.x, uv.y));
    effect *=  smoothstep(0.99, 0.9, uv.x) * smoothstep(0.99, 0.9,1- uv.x);
    effect *=  smoothstep(0.99, 0.9, uv.y) * smoothstep(0.99, 0.9,1- uv.y);
    float3 path = lerp(_SnowPathColorOut * effect.g, _SnowPathColorIn, saturate(effect.g * _SnowPathBlending));
    snow = lerp(snow, path, saturate(effect.g));

    float3 target = lerp(colorEnd, snow, saturate(_SnowOpacity));
    float target_g = target;
    float step_g = step(0.01, _SnowOpacity);
    float3 newColor = target*(1-step_g) + target_g*(step_g);

    float3 alb = colorEnd*(1-GetSnowEffect(worldPos)) + (newColor*_SnowColor+path)*GetSnowEffect(worldPos);
    return alb;
}

float3 CorrectColorForSnowObject(float3 worldPos, float3 colorEnd, float3 normal)
{
    float3 snow = tex2D(_SnowTex, worldPos.xz * _SnowTextureScale);
    float2 uv = worldPos.xz - _Position.xz;
    uv /= (_OrthographicCamSize * 2);
    uv += 0.5;

    float4 effect = tex2D(_GlobalEffectRT, float2 (uv.x, uv.y));
    effect *=  smoothstep(0.99, 0.9, uv.x) * smoothstep(0.99, 0.9,1- uv.x);
    effect *=  smoothstep(0.99, 0.9, uv.y) * smoothstep(0.99, 0.9,1- uv.y);
    float3 path = lerp(_SnowPathColorOut * effect.g, _SnowPathColorIn, saturate(effect.g * _SnowPathBlending));
    snow = lerp(snow, path, saturate(effect.g));

    float3 target = lerp(colorEnd, snow, saturate(_SnowOpacity));
    float target_g = target;
    float step_g = step(0.01, _SnowOpacity);
    float3 newColor = target*(1-step_g) + target_g*(step_g);
    
    float worldNormalDotNoise = dot(normal, float3(0, 1, 0));
    newColor = lerp(colorEnd, newColor, saturate(worldNormalDotNoise+0.5));

    float3 alb = colorEnd*(1-GetSnowEffect(worldPos)) + (newColor*_SnowColor+path)*GetSnowEffect(worldPos);
    return alb;
}