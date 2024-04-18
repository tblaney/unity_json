Shader "snorri/lamb_vegetation"
{
    Properties
    {
        // Colors
        _Color ("Color", Color) = (0, 0, 0, 1)
        _NoiseColor ("Noise Color", Color) = (0, 0, 0, 1)
        _NoiseScale ("Noise Scale", Range(0, 1)) = 1

        // texture
        _MainTex ("Main Texture", 2D) = "white" { }

        _ShadowStrength ("Shadow Strength", Range(0,1)) = 0.5
        _Steps ("Ramp Steps", Range(1, 10)) = 5
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)

        _Cutoff( "Mask Clip Value", Float ) = 0.5
        _MaxWindStrength("MaxWindStrength", Range( 0 , 1)) = 0.1164738
		_WindAmplitudeMultiplier("WindAmplitudeMultiplier", Float) = 1
        _WindEffect ("Wind Effect", Range(0, 1)) = 1
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Cull off
        
        CGPROGRAM

        #pragma surface surf CustomLambert addshadow fullforwardshadows exclude_path:deferred exclude_path:prepass vertex:vertexDataFunc
        #pragma target 3.0

        #include "shader_func_lamb.cginc"
        #include "noise.cginc"
                
        sampler2D _MainTex;

        float _MaxWindStrength;
		float _WindEffect = 1;
        float _Cutoff;
        
        float3 _NoiseColor;
        float _NoiseScale;

		uniform sampler2D _WindVectors;
		uniform float _WindAmplitudeMultiplier;
		uniform float _WindAmplitude;
		uniform float _WindSpeed;
		uniform float4 _WindDirection;
        uniform float _WindStrength;
		uniform float _TrunkWindSpeed;
		uniform float _TrunkWindSwinging;
		uniform float _TrunkWindWeight;

		
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float3 localPos;
            float2 uv_MainTex;
			float2 uv_texcoord;
			float2 uv4_texcoord4;
			float2 uv2_texcoord2;
			float4 vertexColor : COLOR;
		};

        void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

            // Wind effect calculations
            float windPhase = (_WindSpeed * 0.05) * _Time.w;
            float2 windDirection = float2(_WindDirection.x, _WindDirection.z)*max(0.2, saturate(noise(float4(worldPos, 1))));
            float3 WindVectors = UnpackNormal(tex2Dlod(_WindVectors, float4((_WindAmplitudeMultiplier * _WindAmplitude * (worldPos.xz * 0.05) + (windPhase * windDirection)), 0, 0.0)));

            float3 objectScale = float3(length(unity_ObjectToWorld[0].xyz), length(unity_ObjectToWorld[1].xyz), length(unity_ObjectToWorld[2].xyz));
            float3 extendedWindDirection = float3(windDirection, 0.0); // Extend windDirection to float3
            float3 trunkWindOffset = sin((windPhase * (_TrunkWindSpeed * 50 / objectScale)) * extendedWindDirection) - (_TrunkWindSwinging - 1);
            float3 trunkWindEffect = trunkWindOffset * (1 / (_TrunkWindSwinging - 1)) * _TrunkWindWeight * (saturate(v.vertex.y) * 0.05);

            // Apply wind effects
            float3 windEffect = (WindVectors * (0.25 * saturate(v.vertex.y)) * _MaxWindStrength * _WindStrength) + trunkWindEffect;
            windEffect.z = 0;

            windEffect = windEffect*max(0.2, saturate(noise(float4(worldPos*_NoiseScale, 1))));

            v.vertex.xyz += windEffect * _WindEffect;

            // Normal modification (smoothing normals towards up vector)
            v.normal = lerp(v.normal.xyz, float3(0, 1, 0), 0.2);

            // Final position calculations
            o.localPos = v.vertex.xyz;
            o.worldPos = worldPos;
		}
        
        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
            float3 color = (mainTex.rgb);


			//float3 new_color = lerp(_Color.rgb, color, max(1.0, IN.localPos.y));
			float3 new_color = color*_Color + _NoiseColor*noise(float4(IN.worldPos*_NoiseScale, 1));

			o.Albedo = new_color;

            o.Alpha = 1;

			float alpha = mainTex.a;
			float cutoff = _Cutoff;
			clip( alpha - cutoff );
        }
        
        ENDCG
        
    }
    FallBack "Diffuse"
}
