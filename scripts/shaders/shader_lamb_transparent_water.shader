Shader "snorri/lamb_transparent_water"
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

		[Toggle] _DoInterior ("Do Interior", Int) = 0
	}

	CGINCLUDE

	float _WaveHeight;
	float _WaveSpeed;
	float _WaveLength;	

	#include "shader_func_utils.cginc"

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

	float correctAlphaForHoles(float3 worldPosition, float alpha)
	{
		float2 uvW = worldPosition.xz - _Position.xz;
		uvW = uvW / (_OrthographicCamSize * 2);
		uvW += 0.5;			
		float3 vis = tex2Dlod(_TerrainHoleTex, float4(uvW, 0, 0));

		return alpha*(1-step(0.5, vis.r));
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
			ColorMask 0 // don't draw any color
			// Blend SrcAlpha OneMinusSrcAlpha

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

				v.vertex.xyz = Bend(o.worldPosition, v.vertex.xyz);

				return o;
			}

			half4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				// clip(col.a - .97); // remove non-opaque pixels from writing to zbuffer

				return half4(0,0,0,0);
			}
			ENDCG
		}

		// ---------- Start Pass 2 ----------
		ZWrite Off
		Cull Off // make double sided
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf CustomLambert fullforwardshadows alpha:fade vertex:vertexDataFunc

		// Use shader model 3.0 target, to get nicer looking lighting 
		#pragma target 4.0

		#include "shader_func_lamb.cginc"
		#include "shader_func_water.cginc"

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
            float3 worldPos;
			float4 screenPos;
			float3 vertex;
			float3 worldNormal;
		};

		float _Opacity;

		sampler2D _CausticsTex;
		float _CausticsScale;
		float _CausticsThreshold;
		float _CausticsOpacity;
		float _CausticsSpeed;

		float4 _EdgeFoamColor;
		float _EdgeFoamScale;
		float _DistanceDensity;

		sampler2D _CameraDepthTexture;
		
		sampler2D _SparkleTex;
		float _SparkleScale;	
		float _SparkleSpeed;
		float _SparkleExponent;
		float3 _SparkleColor;

		int _DoInterior;


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			float3 worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
			o.worldPos = worldPosition;

			o.screenPos = ComputeScreenPos(UnityObjectToClipPos(v.vertex));

			float y = getWave(o.worldPos.xyz);
			v.vertex.y += y;

			o.worldNormal = UnityObjectToWorldNormal(v.normal);

			v.vertex.xyz = Bend(worldPosition, v.vertex.xyz);

			o.vertex = UnityObjectToClipPos(v.vertex);
		}
		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);

			float foamSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, IN.screenPos);
			float foamDepth = abs(LinearEyeDepth(foamSample).r - IN.screenPos.w);

			float distanceMask = exp(-_DistanceDensity * length(IN.worldPos - _WorldSpaceCameraPos));

			float3 caustics = motionChaosRound(_CausticsTex, IN.worldPos.xz*_CausticsScale, _CausticsSpeed)*distanceMask;

			float3 blendNormal = saturate(pow(IN.worldNormal * 1.4, 4));
			float2 uvX = IN.worldPos.zy/_SparkleScale;
			float2 uvY = IN.worldPos.zx/_SparkleScale;
			float2 uvZ = IN.worldPos.xy/_SparkleScale;
			float2 uvNormal = uvZ;
			uvNormal = lerp(uvNormal, uvX, blendNormal.x);
			uvNormal = lerp(uvNormal, uvY, blendNormal.y);

			float3 sparkly1 = motionFourWaySparkle(_SparkleTex, uvNormal, float4(1,2,3,4), _SparkleSpeed);
			float3 sparkly2 = motionFourWaySparkle(_SparkleTex, uvNormal, float4(1,0.5,2.5,2), _SparkleSpeed);
			
			float sparkleMask = dot(sparkly1, sparkly2) * saturate(3.0 * sqrt(saturate(dot(sparkly1.x, sparkly2.x))));
			sparkleMask = ceil(saturate(pow(sparkleMask, _SparkleExponent))) * distanceMask;

			float3 sparkleColor = lerp(0, _SparkleColor, sparkleMask);

			o.Albedo = (c.rgb * _Color.rgb + caustics*step(_CausticsThreshold, caustics.r)*_CausticsOpacity);
			o.Albedo += _EdgeFoamColor*(1-saturate(foamDepth-_EdgeFoamScale))*_LightColor0;
			o.Albedo += sparkleColor;
			o.Albedo = CorrectColorForWaterRings(IN.worldPos, o.Albedo.rgb, _Opacity);
			
			if (_DoInterior == 1)
			{ 
				o.Albedo.rgb = InteriorPass(o.Albedo.rgb);
			}

			float alpha = correctAlphaForHoles(IN.worldPos.xyz, _Opacity*c.a);
			o.Alpha = alpha;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
