Shader "snorri/lamb_transparent_base"
{
	Properties
	{
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Opacity ("Opacity", Range(0, 1)) = 1
		_Color ("Color", Color) = (1,1,1,1)
		
        _ShadowStrength ("Shadow Strength", Range(0,1)) = 0.5
        _Steps ("Ramp Steps", Range(1, 10)) = 5
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
		LOD 200

		Pass 
		{
			ZWrite On
			Cull Off // make double sided
			ColorMask 0 // don't draw any color

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			
	
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				clip(col.a - .97); // remove non-opaque pixels from writing to zbuffer
				return col;
			}
			ENDCG
		}

		// ---------- Start Pass 2 ----------
		ZWrite Off
		Cull Off // make double sided
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf CustomLambert fullforwardshadows alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		#include "shader_func_lamb.cginc"

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
		};

		float _Opacity;

		void surf (Input IN, inout SurfaceOutput o)
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);

			o.Albedo = c.rgb * _Color.rgb;
			float alpha = _Opacity*c.a;

			o.Alpha = alpha;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
