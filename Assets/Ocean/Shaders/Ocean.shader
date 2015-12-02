Shader "Mobile/Ocean" {
	Properties {
	    _SurfaceColor ("SurfaceColor", Color) = (1,1,1,1)
	    _WaterColor ("WaterColor", Color) = (1,1,1,1)

		_Specularity ("Specularity", Range(0.01,1)) = 0.3

		_SunColor ("SunColor", Color) = (1,1,0.901,1)

		_Refraction ("Refraction (RGB)", 2D) = "white" {}
		_Reflection ("Reflection (RGB)", 2D) = "white" {}
		_Bump ("Bump (RGB)", 2D) = "bump" {}
		_Foam("Foam (RGB)", 2D) = "white" {}
		_FoamBump ("Foam B(RGB)", 2D) = "bump" {}
		_FoamFactor("Foam Factor", Range(0,3)) = 1.8
		_Size ("Size", Vector) = (1, 1, 1, 1)
		_SunDir ("SunDir", Vector) = (0.3, -0.6, -1, 0)
		_WaveOffset ("Wave speed", Vector) = (19,9,-16,-7)
		
		_SurfaceColorLod1 ("Surface Color LOD1", Color) = (1,1,1,0.5)
		_WaterColorLod1 ("Water Color LOD1", Color) = (1,1,1,0.5)
		_WaterTex ("Water LOD1 (RGB)", 2D) = "white" {}
		_WaterLod1Alpha ("Water Transparency", Range(0,1)) = 0.95
	}
	SubShader {
	    Tags { "RenderType" = "Opaque" "Queue"="Geometry"}
		LOD 2
    	Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"

			struct v2f {
    			half4 pos : SV_POSITION;
    			half4  projTexCoord : TEXCOORD0;
    			half4  bumpTexCoord : TEXCOORD1;
    			float3  viewDir : TEXCOORD2;
    			float3  objSpaceNormal : TEXCOORD3;
    			float3  lightDir : TEXCOORD4;
				UNITY_FOG_COORDS(7)
    			float2  foamStrengthAndDistance : TEXCOORD5;
			};

			half4 _Size;
			half4 _SunDir;
			uniform half4 _WaveOffset;

			v2f vert (appdata_tan v) {
    			v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);

    			o.bumpTexCoord.xy = v.vertex.xz/float2(_Size.x, _Size.z)*5;
    			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
    			o.foamStrengthAndDistance.x = v.tangent.w;
    			o.foamStrengthAndDistance.y = clamp(o.pos.z, 0, 1.0);
    
  				half4 projSource = float4(v.vertex.x, 0.0, v.vertex.z, 1.0);
    			half4 tmpProj = mul( UNITY_MATRIX_MVP, projSource);
    			o.projTexCoord = tmpProj;

    			float3 objSpaceViewDir = ObjSpaceViewDir(v.vertex);
    			float3 binormal = cross( normalize(v.normal), normalize(v.tangent.xyz) );
				float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal );
    
    			o.objSpaceNormal = v.normal;
    			o.viewDir = mul(rotation, objSpaceViewDir);
    			o.lightDir = mul(rotation, float3(_SunDir.xyz));
                
				UNITY_TRANSFER_FOG(o, o.pos);

    			return o;
			}

			sampler2D _Refraction;
			sampler2D _Reflection;
			sampler2D _Bump;
			sampler2D _Foam;
			sampler2D _FoamBump;
			half _FoamFactor;
			half4 _SurfaceColor;
			half4 _WaterColor;
			half _Specularity;
			half4 _SunColor;

			half4 frag (v2f i) : COLOR {
				half3 normViewDir = normalize(i.viewDir);
				half4 buv = half4(i.bumpTexCoord.x + _WaveOffset.x * 0.05, i.bumpTexCoord.y + _WaveOffset.y * 0.03, i.bumpTexCoord.x + _WaveOffset.z * 0.04, i.bumpTexCoord.y - _WaveOffset.w * 0.02);

				float foamStrength = i.foamStrengthAndDistance.x * _FoamFactor;
				half foam = clamp(tex2D(_Foam, i.bumpTexCoord.xy * 1.0)  - 0.5, 0.0, 1.0) * foamStrength;
								
				half3 tangentNormal0 = ( tex2D(_Bump, buv.xy) * 2 ) - 1;
				half3 tangentNormal1 = ( tex2D(_Bump, buv.zw) * 2 ) - 1;

				half3 tangentNormalA0 = (  tex2D(_FoamBump, i.bumpTexCoord.xy) * 2  - 1)*foam;

				half3 tangentNormal = normalize(tangentNormal0 + tangentNormal1 + tangentNormalA0);


				float2 projTexCoord = 0.5 * i.projTexCoord.xy * float2(1, _ProjectionParams.x) / i.projTexCoord.w + float2(0.5, 0.5);
				half4 result = half4(0, 0, 0, 1);

				float2 bumpSampleOffset = i.objSpaceNormal.xz * 0.05 + tangentNormal.xy * 0.05 + tangentNormalA0 + projTexCoord.xy;
	
				half3 reflection = tex2D( _Reflection,  bumpSampleOffset ) * _SurfaceColor ;
				half3 refraction = tex2D( _Refraction,  bumpSampleOffset ) * _WaterColor ;

				float fresnelLookup = dot(tangentNormal, normViewDir);

				//float bias = 0.06;
				//float power = 4.0;
				float fresnelTerm = 0.06 + (1.0 - 0.06)*pow(1.0 - fresnelLookup, 4);

				float3 halfVec = normalize(normViewDir - normalize(i.lightDir));
    			float specular = pow(max(dot(halfVec,  tangentNormal) , 0.0), 250.0 * _Specularity )*(1-foam) ;


				//result.rgb = lerp(refraction, reflection, fresnelTerm)+ clamp(foam, 0.0, 1.0) + specular;
				result.rgb = lerp(refraction, reflection, fresnelTerm)*_SunColor.rgb + clamp(foam, 0.0, 1.0)*_SunColor.b + specular*_SunColor.rgb;

				UNITY_APPLY_FOG(i.fogCoord, result); 

    			return result;
			}

		

			ENDCG
		}
    }
		
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 1
    	Pass {
		    Blend One OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f {
    			float4 pos : SV_POSITION;
    			float2  bumpTexCoord : TEXCOORD1;
    			float3  viewDir : TEXCOORD2;
    			float3  objSpaceNormal : TEXCOORD3;
    			float3  lightDir : TEXCOORD4;
    			float2  foamStrengthAndDistance : TEXCOORD5;
			};

			half4 _Size;
			half4 _SunDir;
			sampler2D _WaterTex;
            uniform half4 _WaveOffset;
            
			v2f vert (appdata_tan v) {
    			v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);

    			o.bumpTexCoord.xy = v.vertex.xz/float2(_Size.x, _Size.z)*5;
    			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
    			o.foamStrengthAndDistance.x = v.tangent.w;
    			o.foamStrengthAndDistance.y = clamp(o.pos.z, 0, 1.0);
    
  				half4 projSource = float4(v.vertex.x, 0.0, v.vertex.z, 1.0);
    			half4 tmpProj = mul( UNITY_MATRIX_MVP, projSource);

    			float3 objSpaceViewDir = ObjSpaceViewDir(v.vertex);
    			float3 binormal = cross( normalize(v.normal), normalize(v.tangent.xyz) );
				float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal );
    
    			o.objSpaceNormal = v.normal;
    			o.viewDir = mul(rotation, objSpaceViewDir);
    			o.lightDir = mul(rotation, float3(_SunDir.xyz));

    			return o;
			}

			sampler2D _Bump;
			sampler2D _Foam;
			half4 _SurfaceColorLod1;
			half _FoamFactor;
			half4 _WaterColorLod1;
            half _WaterLod1Alpha;
            
			half4 frag (v2f i) : COLOR {
				half3 normViewDir = normalize(i.viewDir);
                half4 buv = half4(i.bumpTexCoord.x + _WaveOffset.x * 0.05, i.bumpTexCoord.y + _WaveOffset.y * 0.03, i.bumpTexCoord.x + _WaveOffset.z * 0.04, i.bumpTexCoord.y - _WaveOffset.w * 0.02);
                half2 buv2 = half2(i.bumpTexCoord.x - _WaveOffset.z * 0.05, i.bumpTexCoord.y - _WaveOffset.w * 0.05);
                
				half3 tangentNormal0 = (tex2D(_Bump, buv.xy) * 2.0) - 1;
				half3 tangentNormal1 = (tex2D(_Bump, buv.zw) * 2.0) - 1;
				half3 tangentNormal = normalize(tangentNormal0 + tangentNormal1);

				half4 result = half4(0, 0, 0, 1);
	            half3 tex = tex2D(_WaterTex, buv2*2) * _WaterColorLod1;
                
				float fresnelLookup = dot(tangentNormal, normViewDir);
				float bias = 0.06;
				float power = 4.0;
				float fresnelTerm = bias + (1.0-bias)*pow(1.0 - fresnelLookup, power);

				float foamStrength = i.foamStrengthAndDistance.x * _FoamFactor;
				half4 foam = clamp(tex2D(_Foam, i.bumpTexCoord.xy * 1.0)  - 0.5, 0.0, 1.0) * foamStrength;

				float3 halfVec = normalize(normViewDir - normalize(i.lightDir));
    			float specular = pow(max(dot(halfVec, tangentNormal.xyz), 0.0), 250.0);
                
                result.a = _WaterLod1Alpha;
				result.rgb = lerp(tex, _SurfaceColorLod1, fresnelTerm) + clamp(foam.r, 0.0, 1.0) + specular;
                
    			return result;
			}
			ENDCG
			

		}
    }
    FallBack "Diffuse", 1
}
