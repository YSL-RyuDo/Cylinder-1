Shader "UI/CircleMaskDark"
{
    Properties
    {
        _Radius("Radius", Range(0, 1)) = 0.3
        _Smoothness("Smoothness", Range(0.001, 0.5)) = 0.1
        _Darkness("Darkness", Range(0, 1)) = 0.7
    }

    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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

            float _Radius;
            float _Smoothness;
            float _Darkness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);

                float alpha = smoothstep(_Radius, _Radius + _Smoothness, dist);
                return float4(0, 0, 0, alpha * _Darkness);
            }
            ENDCG
        }
    }
}
