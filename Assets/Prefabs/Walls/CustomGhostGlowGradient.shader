Shader "Custom/GhostGlow_HDR_StaticSphere_NoMagicNumbers"
{
    Properties
    {
        [HDR] _ColorA ("Start Color", Color) = (0.2, 0.8, 1, 1)
        [HDR] _ColorB ("End Color", Color) = (1, 0.4, 1, 1)

        _Color ("Tint (used for alpha)", Color) = (1,1,1,1)

        _EmissionIntensity ("Emission Intensity", Float) = 3

        _TweenStart ("Gradient Start", Range(0,1)) = 0
        _TweenEnd ("Gradient End", Range(0,1)) = 1

        _Softness ("Glow Softness", Range(0.1,5)) = 2
        _DepthPower ("Depth Power", Range(0.1,5)) = 1.5

        _GradientSharpness ("Gradient Sharpness", Range(0.5,5)) = 1

        _GradientOffset ("Gradient Offset", Range(0,1)) = 0.5
        _FresnelMultiplier ("Fresnel Multiplier", Float) = 1.0
        _DepthMultiplier ("Depth Multiplier", Float) = 0.5
        _AlphaBase ("Base Alpha", Range(0,1)) = 0.35
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _ColorA;
            float4 _ColorB;
            float4 _Color;

            float _EmissionIntensity;

            float _TweenStart;
            float _TweenEnd;

            float _Softness;
            float _DepthPower;
            float _GradientSharpness;

            float _GradientOffset;
            float _FresnelMultiplier;
            float _DepthMultiplier;
            float _AlphaBase;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 normal = normalize(i.worldNormal);

                // градиент по сфере с настраиваемым оффсетом
                float gradient =
                    pow(
                        normal.y * _GradientOffset + (1 - _GradientOffset),
                        _GradientSharpness
                    );

                float t =
                    lerp(
                        _TweenStart,
                        _TweenEnd,
                        gradient
                    );

                float3 gradientColor =
                    lerp(
                        _ColorA.rgb,
                        _ColorB.rgb,
                        t
                    );

                float3 viewDir =
                    normalize(_WorldSpaceCameraPos - i.worldPos);

                float fresnel =
                    pow(
                        1 - saturate(dot(normal, viewDir)),
                        _Softness
                    ) * _FresnelMultiplier;

                float depth =
                    pow(
                        saturate(dot(normal, viewDir)),
                        _DepthPower
                    ) * _DepthMultiplier;

                float3 emission =
                    gradientColor *
                    _EmissionIntensity *
                    (fresnel + depth);

                float alpha =
                    _Color.a * (_AlphaBase + fresnel);

                return float4(emission, alpha);
            }

            ENDHLSL
        }
    }
}