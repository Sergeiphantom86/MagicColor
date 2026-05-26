Shader "Custom/CrumbledPaper"
{
    Properties
    {
        [MainTexture] _BaseMap ("Paper Texture", 2D) = "white" {}
        [MainColor] _BaseColor ("Color", Color) = (1,1,1,1)
        
        _NormalMap ("Crinkle Normals", 2D) = "bump" {}
        _NormalStrength ("Crinkle Strength", Range(0, 2)) = 0.8
        
        _Glossiness ("Smoothness", Range(0, 1)) = 0.2
        _Metallic ("Metallic", Range(0, 1)) = 0.0
        
        _RimPower ("Rim Light", Range(0, 4)) = 1.2
        _RimColor ("Rim Color", Color) = (0.8,0.7,0.5,1)
        
        _Bumpiness ("Micro Bumpiness", Range(0, 0.5)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 300
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 tangentWS : TEXCOORD3;
                float3 viewDirWS : TEXCOORD4;
            };
            
            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float4 _NormalMap_ST;
                float _NormalStrength;
                float _Glossiness;
                float _Metallic;
                float _RimPower;
                float4 _RimColor;
                float _Bumpiness;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.tangentWS = float4(TransformObjectToWorldDir(input.tangentOS.xyz), input.tangentOS.w);
                output.viewDirWS = GetWorldSpaceViewDir(output.positionWS);
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                // 1. Базовая текстура бумаги
                half4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                
                // 2. Карта нормалей (складки)
                half4 normalSample = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv);
                half3 tangentNormal = UnpackNormal(normalSample);
                tangentNormal.xy *= _NormalStrength;
                tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));
                
                // 3. Микро-шум (доп. шероховатость)
                float micro = sin(input.uv.x * 150) * cos(input.uv.y * 150) * _Bumpiness;
                tangentNormal.xy += micro;
                tangentNormal = normalize(tangentNormal);
                
                // 4. TBN матрица (правильное преобразование)
                float3 normalWS = normalize(input.normalWS);
                float3 tangentWS = normalize(input.tangentWS.xyz);
                float3 bitangentWS = normalize(cross(normalWS, tangentWS) * input.tangentWS.w);
                float3x3 TBN = float3x3(tangentWS, bitangentWS, normalWS);
                
                // Преобразуем нормаль из tangent space в world space
                float3 worldNormal = normalize(mul(TBN, tangentNormal)); // <--- правильный порядок: матрица * вектор
                
                // 5. Освещение (стандартный URP Lit)
                InputData lightingInput = (InputData)0;
                lightingInput.positionWS = input.positionWS;
                lightingInput.normalWS = worldNormal;
                lightingInput.viewDirectionWS = normalize(input.viewDirWS);
                lightingInput.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = albedo.rgb;
                surfaceData.specular = half3(0, 0, 0);  // бумага не металлик
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _Glossiness;
                surfaceData.occlusion = 1.0;
                surfaceData.emission = 0.0;
                surfaceData.alpha = albedo.a;
                
                half4 color = UniversalFragmentPBR(lightingInput, surfaceData);
                
                // 6. Rim-эффект (подсветка краёв складок)
                float3 viewDir = normalize(input.viewDirWS);
                float ndv = saturate(dot(worldNormal, viewDir));
                float rim = pow(1.0 - ndv, _RimPower);
                color.rgb += rim * _RimColor.rgb * albedo.rgb;
                
                // 7. Защита от NaN/розового (если что-то пошло не так)
                if (any(isnan(color.rgb)) || any(isinf(color.rgb)))
                    color.rgb = half3(0.5, 0.5, 0.5);
                
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}