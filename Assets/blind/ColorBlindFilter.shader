Shader "Custom/ColorBlindFilter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorBlindType ("Color Blind Type", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
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
            float _ColorBlindType;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);

                // 色盲模式
                if (_ColorBlindType == 1) // Deuteranopia
                {
                    col.rgb = float3(col.r * 0.8, col.g * 0.5, col.b);
                }
                else if (_ColorBlindType == 2) // Protanopia
                {
                    col.rgb = float3(col.r * 0.5, col.g * 0.8, col.b);
                }
                else if (_ColorBlindType == 3) // Tritanopia
                {
                    col.rgb = float3(col.r, col.g * 0.8, col.b * 0.5);
                }

                return col;
            }
            ENDCG
        }
    }
}
