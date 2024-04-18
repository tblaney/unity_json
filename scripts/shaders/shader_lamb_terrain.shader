Shader "snorri/lamb_terrain" {
    Properties {
        // texture
        [Space]
		[Header(Color)]
        _MainTexSide("Side/Bottom Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)

        // terrain
        [Space]
		[Header(Terrain Settings)]
        _Scale("Top Scale", Range(-2,2)) = 1
		_SideScale("Side Scale", Range(-2,2)) = 1
		_TopSpread("TopSpread", Range(0,1)) = 1

        _TerrainHoleSize ("Terrain Hole Size", Range(0.1, 2.0)) = 1.0
        _HoleTex ("Terrain Hole Tex", 2D) = "black" {}

        [Space]
		[Header(Shadow)]
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

		[Toggle] _DoInterior ("Do Interior", Int) = 0
    }

    CGINCLUDE
        #pragma surface surf CustomLambert vertex:SplatmapVert finalcolor:SplatmapFinalColor finalprepass:SplatmapFinalPrepass finalgbuffer:SplatmapFinalGBuffer addshadow fullforwardshadows
        #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        #pragma multi_compile_fog
        
        #include "shader_func_terrain.cginc"

        float3 _Color;
        
        sampler2D _MainTex;
        sampler _MainTexSide;
        float _Scale, _SideScale;
        float _TopSpread;

        float _ShadowStrength;
        float _Steps;
        fixed4 _ShadowColor;
        
		int _DoInterior;
        
        void surf(Input IN, inout SurfaceOutput o)
        {
            half4 splat_control;
            half weight;
            fixed4 mixedDiffuse;
            SplatmapMix(IN, splat_control, weight, mixedDiffuse, o.Normal);

            float3 toptexture = mixedDiffuse.rgb * _Color;
			float3 blendNormal = saturate(pow(IN.worldNormal * 1.4, 4));
            // triplanar for side and bottom texture, x,y,z sides

			float3 x = tex2D(_MainTexSide, IN.localPos.zy * _SideScale);
			float3 y = tex2D(_MainTexSide, IN.localPos.zx * _SideScale);
			float3 z = tex2D(_MainTexSide, IN.localPos.xy * _SideScale);

            float3 sidetexture = z;
			sidetexture = lerp(sidetexture, x, blendNormal.x);
			sidetexture = lerp(sidetexture, y, blendNormal.y);

            x = tex2D(_HoleTex, IN.localPos.zy * _SideScale);
			y = tex2D(_HoleTex, IN.localPos.zx * _SideScale);
			z = tex2D(_HoleTex, IN.localPos.xy * _SideScale);

            float3 holetexture = z;
			holetexture = lerp(holetexture, x, blendNormal.x);
			holetexture = lerp(holetexture, y, blendNormal.y);

            //toptexture = CorrectColorForHoles(IN.worldPos.xyz, toptexture, holetexture);

            float worldNormalDotNoise = dot(o.Normal, blendNormal.y);
            float3 topTextureResult = step(_TopSpread, worldNormalDotNoise) * toptexture;
            
			float3 sideTextureResult = step(worldNormalDotNoise, _TopSpread) * sidetexture;

			float3 diffuseSide = topTextureResult + sideTextureResult;
            float3 colorEnd = diffuseSide;

            float3 snow = CorrectColorForSnow(IN.worldPos, colorEnd);

            if (_DoInterior == 1)
            {
                snow = InteriorPass(snow);
            }

            o.Albedo = snow;
            o.Alpha = weight;
        }
    ENDCG

    Category 
    {
        Tags {
            "Queue" = "Geometry-99"
            "RenderType" = "Opaque"
			"TerrainCompatible" = "True"
        }

        // TODO: Seems like "#pragma target 3.0 _NORMALMAP" can't fallback correctly on less capable devices?
        // Use two sub-shaders to simulate different features for different targets and still fallback correctly.
        SubShader { // for sm3.0+ targets
            CGPROGRAM
                #pragma target 3.0
                #pragma multi_compile_local __ _ALPHATEST_ON
                #pragma multi_compile_local __ _NORMALMAP

                
            inline fixed4 LightingCustomLambert(SurfaceOutput s, fixed3 lightDir, fixed atten)
            {
                fixed NdotL = max(0, dot(s.Normal, lightDir));

                // Apply shadow strength
                NdotL = pow(NdotL, _ShadowStrength);

                // Apply ramping for hard steps
                NdotL = floor(NdotL * _Steps) / _Steps;

                fixed4 c;
                c.rgb = s.Albedo * _Color.rgb * NdotL * atten;

                fixed3 lightColor = _LightColor0.rgb * 0.6;

                // Mix in shadow color based on the absence of light
                c.rgb = lerp(_ShadowColor.rgb, c.rgb, NdotL)*lightColor;

                c.a = s.Alpha;
                return c;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
