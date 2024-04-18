Shader "snorri/lamb_base_dissolve"
{
    Properties
    {
        // Colors
        _Color ("Color", Color) = (0, 0, 0, 1)

        // texture
        _MainTex ("Main Texture", 2D) = "white" { }
		_AlphaTex("Alpha Tex", 2D) = "white" {}
		_Cutoff( "Mask Clip Value", Range(0, 1.1) ) = 0.5

        _ShadowStrength ("Shadow Strength", Range(0,1)) = 0.5
        _Steps ("Ramp Steps", Range(1, 10)) = 5
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)
        _Scale("Scale", Range(1.0, 100.0)) = 32

		[Toggle] _DoInterior ("Do Interior", Int) = 0
    }
    
    SubShader
    {
		Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
        Cull off
        
        CGPROGRAM

        #pragma surface surf CustomLambert fullforwardshadows exclude_path:deferred exclude_path:prepass vertex:vertexDataFunc
        #pragma target 3.0

        #include "shader_func_lamb.cginc"
        #include "shader_func_utils.cginc"
                
        sampler2D _MainTex;
        sampler2D _AlphaTex;
        float _Cutoff = 0.5;
        float _Scale;

        int _DoInterior;

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

            if (_DoInterior == 1)
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
