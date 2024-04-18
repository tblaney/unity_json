Shader "snorri/lamb_base_all"
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
        
        _OutlineColor("Outline color", Color) = (1,1,1,0.5)
		_OutlineWidth("Outlines width", Range(0.0, 1.0)) = 0.1
		_OutlineAngle("Switch shader on angle", Range(0.0, 180.0)) = 89

        _Scale("Dissolve Scale", Range(1.0, 100.0)) = 32
        _AlphaTex("Alpha Tex", 2D) = "white" {}
		_Cutoff( "Mask Clip Value", Range(0, 1.1) ) = 0.5

        [Toggle] _DoInterior ("Do Interior", Int) = 0
    }

    CGINCLUDE

	#include "UnityCG.cginc"
    #include "shader_func_utils.cginc"

	struct appdata 
    {
		float4 vertex : POSITION;
		float4 normal : NORMAL;
	};

    float4 _OutlineColor;
    float _OutlineWidth;
    float _OutlineAngle;

    float _DoInterior;

    sampler2D _AlphaTex;
    float _Cutoff = 0.5;
    float _Scale;

    ENDCG
    
    SubShader
    {
        //First outline
		Pass
        {
			Tags{ "Queue" = "Transparent" "RenderType"="Transparent"  }
			Cull Front
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
			CGPROGRAM

			struct v2f {
				float4 pos : SV_POSITION;
                float3 vertex : TEXCOORD2; //vertex position
			};

			#pragma vertex vert
			#pragma fragment frag

			v2f vert(appdata v)
            {
				appdata original = v;

                v2f o;


				float3 scaleDir = normalize(v.vertex.xyz - float4(0,0,0,1));
				if (degrees(acos(dot(scaleDir.xyz, v.normal.xyz))) > _OutlineAngle) 
                {
					v.vertex.xyz += normalize(v.normal.xyz) * _OutlineWidth;
				} else
                {
					v.vertex.xyz += scaleDir * _OutlineWidth;
				}

                float3 worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;

                v.vertex.xyz = Bend(worldPosition, v.vertex.xyz);

				o.pos = UnityObjectToClipPos(v.vertex);
                o.vertex.xyz = v.vertex.xyz;

				return o;
			}

			half4 frag(v2f i) : COLOR
            {
				float4 color = _OutlineColor;
                
                fixed4 alphaTexX = tex2D(_AlphaTex, i.vertex.xz/_Scale);
                fixed4 alphaTexY = tex2D(_AlphaTex, i.vertex.xy/_Scale);
                fixed4 alphaTexZ = tex2D(_AlphaTex, i.vertex.yz/_Scale);

                float a = (alphaTexX.r+alphaTexY.r+alphaTexZ.r)/3;

				// color.a = step(a+_OutlineWidth, _Cutoff);
                //color.a = 0;

                float cutoff = 1 - _Cutoff;
                color.a = step(0, a- cutoff) * _OutlineColor.a;

                if (_DoInterior == 1)
                {
                    color.rgb = InteriorPass(color.rgb);
                }
                
				return color;
			}

			ENDCG
		}

		Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
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
            float3 localPos;
        };

        void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			//v.vertex.y = 0;
			float3 worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
			v.vertex.xyz = Bend(worldPosition, v.vertex.xyz);
            o.localPos = v.vertex.xyz;
		}
        
        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
            float3 color  = mainTex.rgb * _Color.rgb;

            fixed4 alphaTexX = tex2D(_AlphaTex, IN.localPos.xz/_Scale);
            fixed4 alphaTexY = tex2D(_AlphaTex, IN.localPos.xy/_Scale);
            fixed4 alphaTexZ = tex2D(_AlphaTex, IN.localPos.yz/_Scale);

            float a = (alphaTexX.r+alphaTexY.r+alphaTexZ.r)/3;

            if (_DoInterior)
            {
                color = InteriorPass(color);
            }
            
            o.Albedo.rgb = color;

            float cutoff = 1-_Cutoff;
			clip( a - cutoff );
        }
        
        ENDCG
        
    }
    FallBack "Diffuse"
}
