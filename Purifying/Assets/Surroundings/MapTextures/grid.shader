Shader "Custom/GridShader"
{
    Properties
    {
        _BackgroundColor("Background Color", Color) = (0, 0, 0, 0.5)
        _GridColor("Grid Color", Color) = (1, 1, 1, 1)
        _DotColor("Dot Color", Color) = (1, 0, 0, 1)
        _GridSize("Grid Size", Float) = 10
        _LineWidth("Line Width", Range(0.01, 0.5)) = 0.495
        _DotSize("Dot Size", Range(0.001, 0.2)) = 0.05
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _BackgroundColor;
            float4 _GridColor;
            float4 _DotColor;
            float _GridSize;
            float _LineWidth;
            float _DotSize;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // **手动实现 TransformObjectToHClip**
            float4 TransformObjectToHClip(float3 pos)
            {
                return UnityObjectToClipPos(float4(pos, 1.0));
            }

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv * _GridSize;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // 计算网格线
                float2 gridUV = frac(i.uv);
                float2 distanceToLine = abs(gridUV - 0.5);
                float gridLine = max(distanceToLine.x, distanceToLine.y) < _LineWidth ? 1.0 : 0.0;

                // 计算网格交点，并让 dot 变成圆形
                float2 gridCenter = round(i.uv);
                float2 uvOffset = i.uv - gridCenter;
                float dotPoint = distance(uvOffset, float2(0, 0)) < _DotSize ? 1.0 : 0.0;

                // 颜色混合
                float4 color = _BackgroundColor;
                color = lerp(color, _GridColor, gridLine);
                color = lerp(color, _DotColor, dotPoint);
                return color;
            }
            ENDCG
        }
    }
}