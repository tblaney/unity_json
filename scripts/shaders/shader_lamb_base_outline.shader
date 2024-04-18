Shader "snorri/lamb_base_outline"
{
    Properties
    {
        // Colors
        _Color ("Color", Color) = (0, 0, 0, 1)

        // texture
        _MainTex ("Main Texture", 2D) = "white" { }

        _ShadowStrength ("Shadow Strength", Range(0,1)) = 0.5
        _Steps ("Ramp Steps", Range(1, 10)) = 5
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)

        _OutlineColor("Outline color", Color) = (1,0,0,0.5)
		_OutlineWidth("Outlines width", Range(0.0, 2.0)) = 0.15
		_OutlineAngle("Switch shader on angle", Range(0.0, 180.0)) = 89
    }
    
    CGINCLUDE

	#include "UnityCG.cginc"

	struct appdata 
    {
		float4 vertex : POSITION;
		float4 normal : NORMAL;
	};

    float4 _OutlineColor;
    float _OutlineWidth;
    float _OutlineAngle;

    ENDCG
    
    SubShader
    {
        //First outline
		Pass{
			Tags{ "Queue" = "Geometry" }
			Cull Front
			CGPROGRAM

			struct v2f {
				float4 pos : SV_POSITION;
			};

			#pragma vertex vert
			#pragma fragment frag

			v2f vert(appdata v)
            {
				appdata original = v;

				float3 scaleDir = normalize(v.vertex.xyz - float4(0,0,0,1));
				if (degrees(acos(dot(scaleDir.xyz, v.normal.xyz))) > _OutlineAngle) 
                {
					v.vertex.xyz += normalize(v.normal.xyz) * _OutlineWidth;
				} else
                {
					v.vertex.xyz += scaleDir * _OutlineWidth;
				}

                float3 worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;

				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			half4 frag(v2f i) : COLOR
            {
				float4 color = _OutlineColor;
				color.a = 1;
				return color;
			}

			ENDCG
		}

        Tags { "RenderType" = "Opaque" }
        Cull off
        
        CGPROGRAM

        #pragma surface surf CustomLambert addshadow fullforwardshadows exclude_path:deferred exclude_path:prepass vertex:vertexDataFunc
        #pragma target 3.0

        #include "shader_func_lamb.cginc"
                
        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldPos;
        };

        void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			//v.vertex.y = 0;
			float3 worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
			//v.vertex.xyz = Bend(worldPosition, v.vertex.xyz);
		}
        
        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
            float3 color  = mainTex.rgb * _Color.rgb;
            
            o.Albedo.rgb = color;
            
            o.Alpha = mainTex.a * _Color.a;
        }
        
        ENDCG
        
    }
    FallBack "Diffuse"
}
