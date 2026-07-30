Shader "UI/GlowOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,1,1,1)
        _OutlineSize ("Outline Size", Float) = 1
        _GlowPower ("Glow Power", Float) = 3
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _Color;
            float4 _OutlineColor;
            float _OutlineSize;
            float _GlowPower;

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
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv) * _Color;
                float alpha = col.a;

                float outline = 0;
                float2 px = _MainTex_TexelSize.xy * _OutlineSize;

                outline = max(outline, tex2D(_MainTex, i.uv + float2(px.x, 0)).a);
                outline = max(outline, tex2D(_MainTex, i.uv - float2(px.x, 0)).a);
                outline = max(outline, tex2D(_MainTex, i.uv + float2(0, px.y)).a);
                outline = max(outline, tex2D(_MainTex, i.uv - float2(0, px.y)).a);

                outline = saturate(outline - alpha);
                float glow = pow(outline, _GlowPower);

                col.rgb += _OutlineColor.rgb * glow * _OutlineColor.a;
                col.a = max(alpha, outline);

                return col;
            }
            ENDCG
        }
    }
}