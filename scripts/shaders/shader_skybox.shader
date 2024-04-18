Shader "snorri/skybox" {
    Properties {
        _TopColorDay ("Top Color Day", Color) = (0,0,1,1)
        _BottomColorDay ("Bottom Color Day", Color) = (0,1,1,1)
        _TopColorNight ("Top Color Night", Color) = (0,0,1,1)
        _BottomColorNight ("Bottom Color Night", Color) = (0,1,1,1)

        _TimeDay ("Time of Day", Range(0.0, 1.0)) = 0.0

        _SunRadius ("Sun Radius", Range(0.0, 1.0)) = 1.0
        _MoonRadius ("Moon Radius", Range(0.0, 1.0)) = 1.0
        _MoonOffset ("Moon Offset", Range(0.0, 1.0)) = 1.0
        _MoonColor ("Moon Color", Color) = (0,1,1,1)
        _SunColor ("Sun Color", Color) = (0,1,1,1)

        _Stars("Stars Texture", 2D) = "black" {}
        _StarsCutoff("Stars Cutoff",  Range(0, 1)) = 0.08
        _StarsScale("Stars Scale",  Range(0.1, 10.0)) = 1.0
        [Toggle] _DoInterior ("Do Interior", Int) = 0

    }

    SubShader {
        Tags { "Queue" = "Background" }
        Pass {
            Name "SKYBOX"
            Tags { "LightMode" = "Always" }
            Cull Off 
            ZWrite Off 
            Fog { Mode Off }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"


            half3 _TopColorDay;
            half3 _BottomColorDay;

            half3 _TopColorNight;
            half3 _BottomColorNight;

            float _SunRadius;
            float _MoonRadius;
            float _MoonOffset;
            float3 _SunColor;
            float3 _MoonColor;

            int _DoInterior;

            sampler2D _Stars;
            float _StarsCutoff;
            float _StarsScale;

            uniform float _LightIntensity;
            uniform float3 _ColorEnvironment;
            uniform float _InteriorHideAmount = 1.0;

            float _TimeDay;

            float3 InteriorPass(float3 colorIn)
            {
                return colorIn*_InteriorHideAmount;
            }



            struct appdata_t {
                float4 vertex : POSITION;
                    float3 uv : TEXCOORD0;
            };
            
            struct v2f {
                float3 uv : TEXCOORD0;
                float3 pos : TEXCOORD1;
                float4 vertex : POSITION;
                float3 worldPos : TEXCOORD2;
            };
            
            v2f vert (appdata_t v) {
                v2f o;
                o.pos = v.vertex.xyz;
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : COLOR {
                float3 dir = normalize(i.pos);
            
                // Ensure horizon is centered.
                float f = saturate(0.5 * (dir.y + 1.0));

                //half3 color = lerp(_BottomColorDay, _TopColorDay*2.0, f) * max(0.25, _LightIntensity);

                // form colors based on main environment color
                float3 topColorDay = _ColorEnvironment;
                float3 topColorNight = _ColorEnvironment*0.2;

                float3 bottomColorDay = _ColorEnvironment*0.4;
                float3 bottomColorNight = _ColorEnvironment*0.1;


                //half3 color = lerp(_BottomColorDay, _TopColorDay*2.0, f);
                //half3 colorNight = lerp(_BottomColorNight, _TopColorNight*2.0, f);

                half3 color = lerp(bottomColorDay, topColorDay*2.0, f);
                half3 colorNight = lerp(bottomColorNight, topColorNight*2.0, f);

                color = lerp(color, colorNight*max(0.8, _LightIntensity), (1-saturate(_LightIntensity-0.35)));

                // sun
                float sun = distance(i.uv.xyz, _WorldSpaceLightPos0);
                float sunDisc = 1 - (sun / _SunRadius);
                sunDisc = saturate(sunDisc * 50);
 
                // (crescent) moon
                float moon = distance(i.uv.xyz, _WorldSpaceLightPos0);
                float crescentMoon = distance(float3(i.uv.x + _MoonOffset, i.uv.yz), _WorldSpaceLightPos0);
                float crescentMoonDisc = 1 - (crescentMoon / _MoonRadius);
                crescentMoonDisc = saturate(crescentMoonDisc * 50);
                float moonDisc = 1 - (moon / _MoonRadius);
                moonDisc = saturate(moonDisc * 50);
                moonDisc = saturate(moonDisc - crescentMoonDisc);

                int dayStep = step(0.3, _LightIntensity);

                float dayFactor = saturate(_LightIntensity-0.25);
 
                //float3 sunAndMoon = (sunDisc * _SunColor) + (moonDisc * _MoonColor);
                float3 sunAndMoon = (sunDisc * _SunColor)*dayFactor +  (moonDisc * _MoonColor)*(1-dayFactor);

                float2 skyUV = i.worldPos.xz / i.worldPos.y;
                float3 stars = tex2D(_Stars, abs(skyUV)*_StarsScale);
                //stars *= saturate(_WorldSpaceLightPos0.y);
                stars = step(_StarsCutoff, stars);

                stars = stars*(1-saturate(_LightIntensity-0.35));

                color = color + sunAndMoon + stars*2;
                //color = color + stars*2;

                if (_DoInterior == 1)
                {
                    color.rgb = InteriorPass(color.rgb);
                }
                

                return half4(color, 1);
            }
            ENDCG
        }
    }
    FallBack "Skybox"
}
