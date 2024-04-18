Shader "snorri/lamb_base"
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

		[Toggle] _DoInterior ("Do Interior", Int) = 0
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Cull off
        
        CGPROGRAM

        #pragma surface surf CustomLambert addshadow fullforwardshadows exclude_path:deferred exclude_path:prepass vertex:vertexDataFunc
        #pragma target 3.0

        #include "shader_func_lamb.cginc"
        #include "shader_func_utils.cginc"
                
        sampler2D _MainTex;
        int _DoInterior;

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
			v.vertex.xyz = Bend(worldPosition, v.vertex.xyz);
		}
        
        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
            float3 color  = mainTex.rgb * _Color.rgb;

            if (_DoInterior == 1)
            {
                color = InteriorPass(color);
            }
            
            o.Albedo.rgb = color;
            
            o.Alpha = mainTex.a * _Color.a;
        }
        
        ENDCG
        
    }
    FallBack "Diffuse"
}
