Shader "Custom/AlphaGlowWall_URP"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _ScrollSpeed ("Scroll Speed", Float) = 0.1 // 适当减小滚动速度
        _FadeHeight ("Fade Height", Float) = 2.0
        _EmissionIntensity ("Emission Intensity", Float) = 3.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend One One // 纯加法混合，保证发光效果
        ZWrite Off
        Cull Back

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #define UNITY_TIME (_Time)

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float worldY : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _EmissionColor;
            float _ScrollSpeed;
            float _FadeHeight;
            float _EmissionIntensity;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                
                // 限制 UV 滚动速度，避免 UV 过快抖动
                float scrollOffset = frac(UNITY_TIME.y * _ScrollSpeed);
                o.uv = v.uv + float2(0, scrollOffset);
                o.worldY = v.vertex.y;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 texColor = tex2D(_MainTex, i.uv);
                
                // 透明度渐变
                float fadeFactor = saturate(1.0 - (i.worldY / _FadeHeight));
                texColor.a *= fadeFactor;

                // 计算发光
                float glow = texColor.a * _EmissionIntensity;
                half4 emission = _EmissionColor * glow;

                // 避免透明度闪烁
                clip(texColor.a - 0.01);

                return emission; // 直接返回发光颜色
            }
            ENDHLSL
        }
    }
}