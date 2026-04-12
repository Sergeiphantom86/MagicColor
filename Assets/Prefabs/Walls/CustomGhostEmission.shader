Shader "Custom/GhostTwoColorEmission"
{
    Properties
    {
        _ColorA ("Color A", Color) = (0,1,1,1)
        _ColorB ("Color B", Color) = (1,0,1,1)

        _ColorSpeed ("Color Speed", Range(0,5)) = 1

        _EmissionPower ("Emission Power", Range(0,10)) = 3

        _Alpha ("Alpha", Range(0,1)) = 0.5
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
        Cull Back

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _ColorA;
            float4 _ColorB;

            float _ColorSpeed;
            float _EmissionPower;
            float _Alpha;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = sin(_Time.y * _ColorSpeed) * 0.5 + 0.5;

                float3 color = lerp(_ColorA.rgb, _ColorB.rgb, t);

                float4 tex = tex2D(_MainTex, i.uv);

                float3 emission = color * _EmissionPower;

                float3 finalColor = tex.rgb + emission;

                return float4(finalColor, tex.a * _Alpha);
            }

            ENDCG
        }
    }
}