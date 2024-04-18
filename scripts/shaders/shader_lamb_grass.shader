Shader "snorri/lamb_grass"
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

        _Cutoff( "Mask Clip Value", Float ) = 0.5
        _MaxWindStrength("MaxWindStrength", Range( 0 , 3)) = 0.1
		_WindAmplitudeMultiplier("WindAmplitudeMultiplier", Float) = 1
        _WindEffect ("Wind Effect", Range(0, 1)) = 1

        _BendingInfluence("BendingInfluence", Range( 0 , 1)) = 0
		_WindInfluenceMask ("Wind Influence", Range(0, 5)) = 0

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

        float _MaxWindStrength;
		float _WindEffect = 1;
        float _Cutoff;

		uniform sampler2D _WindVectors;
		uniform float _WindAmplitudeMultiplier;
		uniform float _WindAmplitude;
		uniform float _WindSpeed;
		uniform float4 _WindDirection;
        uniform float _WindStrength;
		uniform float _TrunkWindSpeed;
		uniform float _TrunkWindSwinging;
		uniform float _TrunkWindWeight;
		float _WindInfluenceMask;
        
		uniform float4 _ObstaclePosition;
		uniform float4 _TerrainUV;

		uniform float _PigmentMapInfluence;
		uniform sampler2D _PigmentMap;
		float3 _PigmentColor;
		uniform float _MinHeight;
		uniform float _HeightmapInfluence;
		uniform float _MaxHeight;
		float _BendingStrength;
		float _BendingRadius;
		float _BendingInfluence;
		
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

        void vertexDataFunc(inout appdata_full v, out Input o) 
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
            float2 windDir = float2(_WindDirection.x, _WindDirection.z);
            float3 windVector = UnpackNormal(tex2Dlod(_WindVectors, float4(((worldPos.xz * 0.01) * _WindAmplitudeMultiplier * _WindAmplitude) + ((_WindSpeed * 0.05) * _Time.w) * windDir, 0, 0.0)));

            float3 windEffect = lerp(float3(0, 0, 0), windVector, _TrunkWindSwinging);
            windEffect = lerp((_MaxWindStrength * _WindStrength) * windEffect, float3(0, 0, 0), 1.0 - v.color.r);

            float3 obstaclePosition = _ObstaclePosition.xyz;
            float3 toObstacleDir = normalize(obstaclePosition - worldPos);
            float3 worldToObjDir = mul(unity_WorldToObject, float4(toObstacleDir, 0)).xyz;
            float3 bending = v.color.r * -((worldToObjDir * _BendingStrength * 0.1) * (1.0 - saturate(distance(obstaclePosition, worldPos) / _BendingRadius)) * _BendingInfluence);

            float3 windBendingEffect = windEffect + bending;
            float2 terrainUV = (1.0 - _TerrainUV.zw) / _TerrainUV.x + (_TerrainUV.x / (_TerrainUV.x * _TerrainUV.x)) * worldPos.xz;
            float heightMapValue = tex2Dlod(_PigmentMap, float4(terrainUV, 0, 1.0)).a;

            float3 finalOffset = lerp(windBendingEffect, windBendingEffect * heightMapValue, _PigmentMapInfluence);
            float touchBendPos = 0.0;

            #ifdef _VS_TOUCHBEND_ON
                touchBendPos = TouchReactAdjustVertex(float4(worldPos, 0.0)).y;
            #endif

            float grassLength = lerp(_MinHeight * saturate(1.0 - heightMapValue - touchBendPos), 0.0, 1.0 - v.color.r);
            grassLength = (_HeightmapInfluence * grassLength) + lerp(_MaxHeight, 0.0, 1.0 - v.color.r);

            finalOffset.y += grassLength;
            float3 vertexOffset = finalOffset;

            // Apply grass hiding with render texture
            float3 vertexResult = v.vertex.xyz + vertexOffset * saturate(v.vertex.y - _WindInfluenceMask) * _WindEffect;
            float2 uv = worldPos.xz - _Position.xz;
            uv /= (_OrthographicCamSize * 2);
            uv += 0.5;
            float4 effect = tex2Dlod(_GlobalEffectRT, float4(uv.x, uv.y, 0, 0));

            vertexResult.y *= 1 - saturate(effect.r);
            v.vertex.xyz = vertexResult;

            o.worldPos = worldPos;
            o.localPos = v.vertex;
        }

        
        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
            float3 color = (mainTex.rgb);

            float3 terrainTex = TerrainEffect(IN.worldPos.xyz);


			//float3 new_color = lerp(_Color.rgb, color, max(1.0, IN.localPos.y));
			float3 new_color = terrainTex;

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
