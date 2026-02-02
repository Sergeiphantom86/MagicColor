Shader "Custom/WebGL_SpotGlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        [HDR] _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _EmissionStrength ("Emission Strength", Range(0, 10)) = 1
        
        // Основная пульсация
        _PulseSpeed ("Pulse Speed", Float) = 1.0
        _PulseMin ("Pulse Min", Range(0, 1)) = 0.5
        _PulseMax ("Pulse Max", Range(0, 3)) = 1.5
        
        // Пятна (оптимизировано для WebGL)
        _SpotSpeed ("Spot Speed", Float) = 0.5
        _SpotScale ("Spot Scale", Float) = 10.0
        _SpotIntensity ("Spot Intensity", Range(0, 2)) = 1.0
        _SpotDensity ("Spot Density", Range(0, 1)) = 0.3
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        // Для WebGL лучше использовать Mobile/VertexLit если не нужны сложные эффекты
        // Но здесь оставим Lambert для свечения
        
        CGPROGRAM
        // Используем самый простой surface шейдер
        #pragma surface surf Lambert noforwardadd
        
        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _EmissionColor;
        float _EmissionStrength;
        
        // Параметры пульсации
        float _PulseSpeed;
        float _PulseMin;
        float _PulseMax;
        
        // Параметры пятен
        float _SpotSpeed;
        float _SpotScale;
        float _SpotIntensity;
        float _SpotDensity;
        
        // ОПТИМИЗИРОВАННАЯ функция шума для WebGL
        // Используем простейший hash для производительности
        float simpleHash(float2 p)
        {
            return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
        }
        
        struct Input
        {
            float2 uv_MainTex;
        };
        
        void surf (Input IN, inout SurfaceOutput o)
        {
            // Базовый цвет текстуры
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = tex.rgb;
            o.Alpha = tex.a;
            
            // 1. Основная пульсация (дешевая операция)
            float pulse = (_PulseMax - _PulseMin) * 
                         (sin(_Time.y * _PulseSpeed) * 0.5 + 0.5) + _PulseMin;
            
            // 2. Пятна (оптимизированный расчет)
            // Используем только один слой шума для производительности
            float time = _Time.y * _SpotSpeed;
            
            // Вычисляем шум для пятен
            float noise1 = simpleHash(IN.uv_MainTex * _SpotScale + time);
            float noise2 = simpleHash(IN.uv_MainTex * _SpotScale * 1.7 - time * 1.3);
            
            // Простой threshold-based пятна (дешевле чем smoothstep)
            float spots = 0;
            if (noise1 > 1.0 - _SpotDensity) spots += 0.5;
            if (noise2 > 1.0 - _SpotDensity * 0.7) spots += 0.3;
            
            // 3. Финальное свечение
            float baseEmission = pulse * _EmissionStrength;
            float spotEmission = spots * _SpotIntensity;
            
            o.Emission = _EmissionColor.rgb * (baseEmission + spotEmission);
        }
        ENDCG
    }
    FallBack "Mobile/VertexLit"
}