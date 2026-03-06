Shader "Custom/Path"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PathColor ("Path Color", Color) = (1,0,0,1) // 路径颜色
        _PathWidth ("Path Width", Range(0, 0.1)) = 0.02 // 路径宽度
        _PointsCount ("Points Count", Int) = 5 // 路径点数量
        _Points ("Points", Vector)= (0.2,0.2,0)

        [HideInInspector] _StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil("Stencil ID", Float) = 1
        [HideInInspector] _StencilOp("Stencil Operation", Float) = 2
        [HideInInspector] _StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask("Stencil Read Mask", Float) = 255
    }

    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Stencil
        {
        Ref [_Stencil]
        Comp [_StencilComp]
        Pass [_StencilOp]
        ReadMask [_StencilReadMask]
        WriteMask [_StencilWriteMask]
        }

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

            sampler2D _MainTex;
            float4 _PathColor;
            float _PathWidth;
            int _PointsCount;
            float3 _Points[100]; // 最大支持100个路径点

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // 计算点到线段的最短距离
            float DistanceToSegment(float2 p, float2 a, float2 b)
            {
                float2 pa = p - a;
                float2 ba = b - a;
                float t = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
                float2 projection = a + t * ba;
                return length(p - projection);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float minDistance = 9999.0;
                // 遍历所有线段，计算当前像素到路径的最小距离
                for (int idx = 0; idx < _PointsCount - 1; idx++)
                {
                    float2 start = _Points[idx].xy;
                    float2 end = _Points[idx + 1].xy;
                    float d = DistanceToSegment(i.uv, start, end);
                    minDistance = min(minDistance, d);
                }
                // 根据距离场生成路径颜色
                float smoothWidth = _PathWidth * 0.5;
                float alpha = smoothstep(_PathWidth + smoothWidth, _PathWidth - smoothWidth, minDistance);
                return fixed4(_PathColor.rgb, alpha * _PathColor.a);
            }
            ENDCG
        }
    }
}
