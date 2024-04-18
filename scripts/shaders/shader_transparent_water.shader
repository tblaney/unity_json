Shader "snorri/transparent_water"
{
	Properties
	{
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_CausticsTex ("Caustics (RGB)", 2D) = "white" {}
		_Opacity ("Opacity", Range(0, 1)) = 1
		_Color ("Color", Color) = (1,1,1,1)

        _ShadowStrength ("Shadow Strength", Range(0,1)) = 0.5
        _Steps ("Ramp Steps", Range(1, 10)) = 5
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)

		_EdgeFoamColor ("Edge Color", Color) = (1,1,1,1)
		_EdgeFoamScale ("Edge Scale", Range(0.0,1.0)) = 1.0

		_CausticsScale ("Caustics Scale", Range(0, 2)) = 1.0
		_CausticsSpeed ("Caustics Speed", Range(0, 1)) = 1.0
		_CausticsThreshold ("Caustics Treshold", Range(0, 1)) = 0.3
		_CausticsOpacity ("Caustics Opacity", Range(0, 1)) = 0.3

		_WaveHeight ("Wave Height", Range(0, 30)) = 1.0
        _WaveSpeed ("Wave Speed", Range(0, 30)) = 1.0
        _WaveLength ("Wave Length", Range(0, 50)) = 1.0

        _DistanceDensity ("Distance", Range(0.0, 0.5)) = 0.1

		_SparkleTex ("Sparkles Tex", 2D) = "bump"{}
        _SparkleScale ("Sparkles Scale", float) = 10
        _SparkleSpeed ("Sparkles Speed", float) = 0.75
        _SparkleColor ("Sparkles Color", Color) = (1, 1, 1, 1)
        _SparkleExponent ("Sparkles Exponent", float) = 10000
	}

	CGINCLUDE

	float _WaveHeight;
	float _WaveSpeed;
	float _WaveLength;	

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
	float3 motionFourWaySparkle(sampler2D tex, float2 uv, float4 coordinateScale, float speed)
	{
		float2 uv1 = panner(uv * coordinateScale.x, float2( 0.1,  0.1), speed);
		float2 uv2 = panner(uv * coordinateScale.y, float2(-0.1, -0.1), speed);
		float2 uv3 = panner(uv * coordinateScale.z, float2(-0.1,  0.1), speed);
		float2 uv4 = panner(uv * coordinateScale.w, float2( 0.1, -0.1), speed);

		float3 sample1 = (tex2D(tex, uv1)).rgb;
		float3 sample2 = (tex2D(tex, uv2)).rgb;
		float3 sample3 = (tex2D(tex, uv3)).rgb;
		float3 sample4 = (tex2D(tex, uv4)).rgb;

		float3 normalA = float3(sample1.x, sample2.y, 1);
		float3 normalB = float3(sample3.x, sample4.y, 1);
		
		return normalize(float3( (normalA+normalB).xy, (normalA*normalB).z ));
	}

    ENDCG



	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
		LOD 200
		Pass 
		{
			ZWrite Off
			Cull Off // make double sided
			// ColorMask 0 // don't draw any color
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma target 4.0


			#include "UnityCG.cginc"

			float4 _EdgeFoamColor;
			float _EdgeFoamScale;
            uniform sampler2D _LastCameraDepthTexture;
			float _DepthDensity;
            float _DistanceDensity;

			float3 _Color;
			float _Opacity;

	
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 grabPosition : TEXCOORD2;
                float3 worldPosition : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert (appdata v)
			{
				v2f o;

				float3 worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;

				float y = getWave(worldPosition.xyz);
				v.vertex.y += y;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.grabPosition = ComputeScreenPos(o.vertex);

				o.worldPosition = worldPosition;

				return o;
			}

			half4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				// clip(col.a - .97); // remove non-opaque pixels from writing to zbuffer

				// return half4(0,0,0,0);

				// ------------------------------ //
                // SAMPLE DEPTH AND COLOR BUFFERS //
                // ------------------------------ //

                float2 screenCoord = i.grabPosition.xy / i.grabPosition.w;
                float fragDepth = tex2D(_LastCameraDepthTexture , screenCoord.xy).x;

				// ------------------------ //
                // DEPTH AND DISTANCE MASKS //
                // ------------------------ //

                float persp = LinearEyeDepth(fragDepth);
                float ortho = (_ProjectionParams.z-_ProjectionParams.y)*(1-fragDepth)+_ProjectionParams.y;
                float depth = lerp(persp,ortho,unity_OrthoParams.w);

                float persp2 = LinearEyeDepth(i.vertex.z);
                float ortho2 = (_ProjectionParams.z-_ProjectionParams.y)*(1-i.vertex.z)+_ProjectionParams.y;
                float depth2 = lerp(persp2,ortho2,unity_OrthoParams.w);

                float opticalDepth = abs(depth - depth2);
                float transmittance = exp(-_DepthDensity * opticalDepth);

                float distanceMask = exp(-_DistanceDensity * length(i.worldPosition - _WorldSpaceCameraPos));

				//return half4(_Color.rgb + _EdgeFoamColor.a*(1-(opticalDepth*(1-_EdgeFoamScale))), _Opacity);
				return half4(_Color.rgb + _EdgeFoamColor*saturate(_EdgeFoamColor.a*(1-(opticalDepth*(1-_EdgeFoamScale)))),_Opacity);
				//return half4(_EdgeFoamColor.rgb*_EdgeFoamColor.a, 1-opticalDepth);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
