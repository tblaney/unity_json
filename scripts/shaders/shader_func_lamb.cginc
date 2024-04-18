// DEFINES.
#define PI (3.1415926536)

float _ShadowStrength;
float _Steps;
fixed4 _ShadowColor;
fixed4 _Color;

inline fixed4 LightingCustomLambert(SurfaceOutput s, fixed3 lightDir, fixed atten)
{
    fixed NdotL = max(0, dot(s.Normal, lightDir));

    // Apply shadow strength
    NdotL = pow(NdotL, _ShadowStrength);

    // Apply ramping for hard steps
    NdotL = floor(NdotL * _Steps) / _Steps;

    fixed4 c;
    c.rgb = s.Albedo * _Color.rgb * NdotL * atten;

    fixed3 lightColor = _LightColor0.rgb * 0.6;

    // Mix in shadow color based on the absence of light
    c.rgb = lerp(_ShadowColor.rgb, c.rgb, NdotL)*lightColor;

    c.a = s.Alpha;
    return c;
}
