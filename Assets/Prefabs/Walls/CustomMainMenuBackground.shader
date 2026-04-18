Shader "Custom/MainMenuBackground_NoGrid_Alpha"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _ColorTop("Top Color", Color) = (0.05,0.05,0.15,1)
        _ColorBottom("Bottom Color", Color) = (0.1,0.05,0.15,1)
        _SpotColor1("Spot Color 1", Color) = (0.2,0,0.5,0.4)
        _SpotColor2("Spot Color 2", Color) = (0,0.3,0.6,0.4)
        _SpotColor3("Spot Color 3", Color) = (0.8,0.1,0.5,0.3)
        _SpotSpeed("Spots Speed", Range(0,2)) = 0.5
        _SpotScale("Spots Scale", Range(0.5,5)) = 2.0
        _SparkColor("Spark Color", Color) = (1,0.4,0.8,1)
        _SparkIntensity("Spark Intensity", Range(0,2)) = 1.2
        _SparkSpeed("Spark Speed", Range(0,3)) = 1.0
        _SparkDensity("Spark Density", Range(0.5,5)) = 2.5
        _Brightness("Brightness", Range(0.5,2)) = 1.0
        _Alpha("Alpha", Range(0,1)) = 1.0         
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _ColorTop, _ColorBottom;
            float4 _SpotColor1, _SpotColor2, _SpotColor3;
            float _SpotSpeed, _SpotScale;
            float4 _SparkColor;
            float _SparkIntensity, _SparkSpeed, _SparkDensity;
            float _Brightness;
            float _Alpha;     

            float random(float2 st)
            {
                return frac(sin(dot(st, float2(12.9898,78.233))) * 43758.5453123);
            }

            float smoothNoise(float2 st)
            {
                float2 i = floor(st);
                float2 f = frac(st);
                float2 u = f*f*(3.0-2.0*f);
                return lerp(lerp(random(i), random(i+float2(1,0)), u.x),
                            lerp(random(i+float2(0,1)), random(i+float2(1,1)), u.x), u.y);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float time = _Time.y;

                float3 gradient = lerp(_ColorTop.rgb, _ColorBottom.rgb, uv.y);

                float2 spotUV1 = uv * _SpotScale + float2(time*0.2, time*0.1);
                float2 spotUV2 = uv * (_SpotScale*1.5) - float2(time*0.15, time*0.25);
                float2 spotUV3 = uv * (_SpotScale*0.8) + float2(time*0.3, -time*0.2);
                float noise1 = smoothNoise(spotUV1);
                float noise2 = smoothNoise(spotUV2);
                float noise3 = smoothNoise(spotUV3);
                float3 spots = _SpotColor1.rgb*noise1 + _SpotColor2.rgb*noise2 + _SpotColor3.rgb*noise3;
                float spotAlpha = saturate(noise1*_SpotColor1.a + noise2*_SpotColor2.a + noise3*_SpotColor3.a);

                float2 sparkUV = uv * _SparkDensity;
                float sparkTime = time * _SparkSpeed;
                float spark1 = sin(sparkUV.x*50 - sparkTime*5) * cos(sparkUV.y*30 + sparkTime*3);
                float spark2 = sin(sparkUV.y*60 + sparkTime*4) * cos(sparkUV.x*40 - sparkTime*2);
                float spark3 = sin((sparkUV.x+sparkUV.y)*45 + sparkTime*7);
                float sparkNoise = saturate((spark1+spark2+spark3)*0.5 + 0.3);
                sparkNoise = pow(sparkNoise, 2) * _SparkIntensity;
                sparkNoise *= 0.5 + 0.5*sin(uv.x*100 + uv.y*80 + time*20);
                float3 sparkColor = _SparkColor.rgb * sparkNoise * _SparkColor.a;

                float3 finalColor = gradient + spots*spotAlpha + sparkColor;
                finalColor *= _Brightness;

                float vignette = 1.0 - length(uv-0.5)*0.4;
                finalColor *= vignette;

                float alpha = _Alpha;

                return fixed4(finalColor, alpha);
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}