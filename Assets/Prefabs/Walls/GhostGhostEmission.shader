Shader "Ghost/GhostEmission"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _EmissionStrength ("Emission Strength", Float) = 1

        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 2
        _NoiseSpeed ("Noise Speed", Vector) = (0.1, 0.1, 0, 0)
        _SpotIntensity ("Spot Intensity", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

            float4 _BaseColor;
            float _EmissionStrength;
            float _NoiseScale;
            float4 _NoiseSpeed;
            float _SpotIntensity;

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
                o.uv = v.uv * _NoiseScale + _Time.y * _NoiseSpeed.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float noise = tex2D(_NoiseTex, i.uv).r;

                float glow = 1 + (noise - 0.5) * 2 * _SpotIntensity;

                fixed3 emission = _BaseColor.rgb * glow * _EmissionStrength;

                return fixed4(emission, 1);
            }
            ENDCG
        }
    }
}