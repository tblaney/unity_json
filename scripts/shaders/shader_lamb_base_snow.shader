Shader "snorri/lamb_base_snow"
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

        [Space]
		[Header(Snow)]
        _NoiseTex("Noise", 2D) = "gray" {}	
        _NoiseScale("Noise Scale", Range(0,2)) = 0.1
		_NoiseWeight("Noise Weight", Range(0,2)) = 0.1

        _SnowTex("Snow Texture", 2D) = "white" {}	
		_SnowTextureScale("Snow Texture Scale", Range(0,2)) = 0.3
        _SnowColor ("Snow Color", Color) = (0, 0, 0, 0)
        _SnowStart ("Snow Start", Range(0, 500)) = 100
        _SnowPathBlending("Path Color Blending", Range(0,3)) = 2
		_SnowPathStrength("Snow Path Smoothness", Range(0,4)) = 2
        [HDR]_SnowPathColorIn("Snow Path Color", Color) = (1,1,1,1)
		[HDR]_SnowPathColorOut("Snow Path Color2", Color) = (0.5,0.5,1,1)
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Cull off
        
        CGPROGRAM

        #pragma surface surf CustomLambert addshadow fullforwardshadows exclude_path:deferred exclude_path:prepass vertex:vertexDataFunc
        #pragma target 3.0

        #include "shader_func_snow.cginc"
        #include "shader_func_lamb.cginc"
        #include "shader_func_utils.cginc"
                
        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldPos;
            float3 normal;
        };

        float3 _ColorMain;

        void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			//v.vertex.y = 0;
			float3 worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
			//v.vertex.xyz = Bend(worldPosition, v.vertex.xyz);
            o.worldPos = worldPosition;
            v.vertex.xyz = CorrectVertForSnowObject(v.vertex.xyz, worldPosition, v.normal);

            v.vertex.xyz = Bend(worldPosition, v.vertex.xyz);

            o.normal = v.normal;
		}
        
        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
            float3 color  = mainTex.rgb * _Color.rgb;

            color = CorrectColorForSnowObject(IN.worldPos, color, IN.normal);
            
            o.Albedo.rgb = color;
            
            o.Alpha = mainTex.a * _Color.a;
        }
        
        ENDCG
        
    }
    FallBack "Diffuse"
}
