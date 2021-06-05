Shader "G.H.S/Pixelate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            int _ColorStep;

            fixed4 frag(v2f i) : SV_Target
            {
                float4 oriCol = tex2D(_MainTex, i.uv);
                float brightness = floor(oriCol.rgb * _ColorStep) / (_ColorStep - 1);
                brightness = lerp(0.5, 1, ceil(brightness * 4 + 0.25) / 4);
                return oriCol * saturate(brightness);
            }
            ENDCG
        }
    }
}