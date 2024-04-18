Shader "snorri/clouds" {
    Properties {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _ColorDay ("Color Day", Color) = (1.0,1.0,1.0,1.0)
        _ColorNight ("Color Night", Color) = (1.0,1.0,1.0,1.0)

        _TexClouds ("Clouds", 2D) = "white" {}
        _TexClouds02 ("Clouds Secondary", 2D) = "white" {}
        _TexClouds03 ("Clouds Third", 2D) = "white" {}

        _CloudsSpeed ("Clouds Speed", Range(0.0, 20.0)) = 0.1
        _Scale ("Scale", Range(0.1, 1000.0)) = 1.0
        _AlphaClip ("Alpha Clip", Range(0.0, 1.0)) = 0.1
        _NoiseClip ("Noise Clip", Range(0.0, 300.0)) = 5.0

        _WaveHeight ("Wave Height", Range(0, 5)) = 1.0
        _WaveSpeed ("Wave Speed", Range(0, 20)) = 1.0
        _WaveLength ("Wave Length", Range(0, 20)) = 1.0

        _Opacity ("Opacity", Range(0.0, 1.0)) = 1.0
        _OffsetHorizonOpacity ("Horizon Offset Opacity", Range(-50.0, 50.0)) = 1.0

        [Toggle] _DoInterior ("Do Interior", Int) = 0

    }

    SubShader {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 100

        Cull Off

        Blend SrcAlpha OneMinusSrcAlpha // Enable transparency
        ZWrite Off // Disable depth write
        ZTest LEqual // Enable depth test

        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "noise.cginc"
            #include "shader_func_utils.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _TexClouds;
            sampler2D _TexClouds02;
            sampler2D _TexClouds03;

            float _CloudsSpeed;
            float4 _MainTex_ST;
            float _Scale;
            float _AlphaClip;
            float _NoiseClip;

            float3 _ColorDay;
            float3 _ColorNight;

            float _WaveHeight;
			float _WaveSpeed;
			float _WaveLength;

            int _DoInterior;

            float _OffsetHorizonOpacity;

            float _Opacity;

            float getWave(float3 v)
			{
				float xWave = _WaveHeight * sin(v.x * _WaveLength + _Time * _WaveSpeed);
				float zWave = _WaveHeight * cos(v.z * _WaveLength + _Time * _WaveSpeed);
				return xWave+zWave;
			}
			float2 panner(float2 uv, float2 direction, float speed)
			{
				return uv + normalize(direction)*speed*_Time;
			}
			float3 motionChaos(sampler2D tex, float2 uv, float speed, bool unpackNormal)
			{
				float2 uv1 = panner(uv + float2(0.000, 0.000), float2( 0.1,  0.1), speed);
				float2 uv2 = panner(uv + float2(0.418, 0.355), float2(-0.1, -0.1), speed);
				float2 uv3 = panner(uv + float2(0.865, 0.148), float2(-0.1,  0.1), speed);
				float2 uv4 = panner(uv + float2(0.651, 0.752), float2( 0.1, -0.1), speed);

				float3 sample1;
				float3 sample2;
				float3 sample3;
				float3 sample4;

				if (unpackNormal)
				{
					sample1 = UnpackNormal(tex2D(tex, uv1)).rgb;
					sample2 = UnpackNormal(tex2D(tex, uv2)).rgb;
					sample3 = UnpackNormal(tex2D(tex, uv3)).rgb;
					sample4 = UnpackNormal(tex2D(tex, uv4)).rgb;

					return normalize(sample1 + sample2 + sample3 + sample4);
				}
				else
				{
					sample1 = tex2D(tex, uv1).rgb;
					sample2 = tex2D(tex, uv2).rgb;
					sample3 = tex2D(tex, uv3).rgb;
					sample4 = tex2D(tex, uv4).rgb;

					return (sample1 + sample2 + sample3 + sample4) / 4.0;
				}
			}
			float3 motionChaosRound(sampler2D tex, float2 uv, float speed)
			{
				float2 uv1 = panner(uv + float2(0.000, 0.000), float2( 0.1,  0.1), speed);
				float2 uv2 = panner(uv + float2(0.418, 0.355), float2(-0.1, -0.1), speed);
				float2 uv3 = panner(uv + float2(0.865, 0.148), float2(-0.1,  0.1), speed);
				float2 uv4 = panner(uv + float2(0.651, 0.752), float2( 0.1, -0.1), speed);

				float3 sample1;
				float3 sample2;
				float3 sample3;
				float3 sample4;

				sample1 = step(0.2, tex2D(tex, uv1).rgb);
				sample2 = step(0.2, tex2D(tex, uv2).rgb);
				sample3 = step(0.2, tex2D(tex, uv3).rgb);
				sample4 = step(0.2, tex2D(tex, uv4).rgb);

				return (sample1 + sample2 + sample3 + sample4) / 4.0;
				
			}
            
            v2f vert (appdata v) 
            {
                v2f o;

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float y = getWave(worldPos.xyz);
                v.vertex.y += y;

                v.vertex.y -= BendFactor    (worldPos);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target 
            {
                float3 localPos = i.worldPosition - mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;
                localPos.y += BendFactor(i.worldPosition.xyz);

                float3 worldPosShifted = (localPos.xyz) - _Time*_CloudsSpeed;
                fixed4 clouds = tex2D(_TexClouds, (worldPosShifted.xz)/_Scale);
                float alpha = step(clouds.r, _AlphaClip);

                if (noise(float4(worldPosShifted.x/_NoiseClip,worldPosShifted.z/_NoiseClip,0,localPos.y/_NoiseClip)) > 0.3f)
                {
                    alpha = 1;
                }

                float lerpFactor = max(0.0, (_PlayerPosition.y + _OffsetHorizonOpacity) - i.worldPosition.y);
                lerpFactor = saturate(lerpFactor/(abs(_OffsetHorizonOpacity)+0.0000001));

                alpha = lerp(alpha, 1, lerpFactor);

                float3 color = clouds.rgb*lerp(_ColorNight, _ColorDay, saturate(_LightIntensity-0.2));

				color = color*max(0.4, min(1.0, saturate(_LightIntensity)));

                if (_DoInterior == 1)
                {
                    color.rgb = InteriorPass(color.rgb);
                }

                return fixed4(color,(1-alpha)*_Opacity*max(0.4, min(_LightIntensity, 1.0)));
            }
            ENDCG
        }
    }
}
