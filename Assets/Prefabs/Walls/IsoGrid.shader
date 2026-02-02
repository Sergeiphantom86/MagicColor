Shader "Custom/RoundedGridUV"
{
    Properties
    {
        _GridSize ("Grid Size (X,Y cells)", Vector) = (10,10,0,0)

        _Radius ("Corner Radius", Range(0,0.5)) = 0.08
        _Outline ("Outline Width", Range(0,0.2)) = 0.03

        _BaseColor ("Base Color", Color) = (0.85,0.85,0.85,1)
        _ActiveColor ("Active Color", Color) = (1,0.6,0.2,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)

        _FillAlpha ("Fill Alpha", Range(0,1)) = 1.0
        _OutlineAlpha ("Outline Alpha", Range(0,1)) = 1.0
        _ActiveAlpha ("Active Alpha Mult", Range(0,1)) = 1.0

        _ActiveCell ("Active Cell (x,y)", Vector) = (-1,-1,0,0)
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _GridSize;
            float _Radius;
            float _Outline;

            float4 _BaseColor;
            float4 _ActiveColor;
            float4 _OutlineColor;

            float _FillAlpha;
            float _OutlineAlpha;
            float _ActiveAlpha;

            float4 _ActiveCell;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float sdRoundedBox(float2 p, float2 b, float r)
            {
                float2 q = abs(p) - b + r;
                return length(max(q, 0)) + min(max(q.x, q.y), 0) - r;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 grid = i.uv * _GridSize.xy;

                float2 cellId = floor(grid);
                float2 cellUV = frac(grid) - 0.5;

                float2 boxSize = float2(0.45, 0.45);

                float dOuter = sdRoundedBox(cellUV, boxSize, _Radius);
                float dInner = sdRoundedBox(cellUV, boxSize - _Outline, _Radius);

                float aa = fwidth(dOuter) * 1.5;

                float fillMask = 1.0 - smoothstep(0, aa, dOuter);
                float outlineMask = smoothstep(0, aa, dOuter) -
                                    smoothstep(0, aa, dInner);

                float isActive =
                    (cellId.x == _ActiveCell.x &&
                     cellId.y == _ActiveCell.y) ? 1.0 : 0.0;

                float3 fillColor =
                    lerp(_BaseColor.rgb, _ActiveColor.rgb, isActive);

                float fillAlpha =
                    fillMask * _FillAlpha * lerp(1.0, _ActiveAlpha, isActive);

                float outlineAlpha = outlineMask * _OutlineAlpha;

                float3 color =
                    lerp(fillColor, _OutlineColor.rgb, outlineMask);

                float alpha = max(fillAlpha, outlineAlpha);

                return float4(color, alpha);
            }
            ENDCG
        }
    }
}