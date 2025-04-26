Shader "Hidden/Custom/ColorBlind"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Mode ("Color Blind Mode", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Mode;

            fixed4 frag(v2f_img i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 rgb = col.rgb;
                float3 result;

                if (_Mode > 0.5 && _Mode < 1.5) {
                    // Protanopia（红色盲）
                    result = mul(rgb, float3x3(
                        0.567, 0.433, 0.0,
                        0.558, 0.442, 0.0,
                        0.0,   0.242, 0.758));
                }
                else if (_Mode > 1.5 && _Mode < 2.5) {
                    // Deuteranopia（绿色盲）
                    result = mul(rgb, float3x3(
                        0.625, 0.375, 0.0,
                        0.7,   0.3,   0.0,
                        0.0,   0.3,   0.7));
                }
                else if (_Mode > 2.5 && _Mode < 3.5) {
                    // Tritanopia（蓝色盲）
                    result = mul(rgb, float3x3(
                        0.95, 0.05,  0.0,
                        0.0,  0.433, 0.567,
                        0.0,  0.475, 0.525));
                }
                else {
                    // Normal vision
                    result = rgb;
                }

                return fixed4(result, col.a);
            }
            ENDCG
        }
    }
}
