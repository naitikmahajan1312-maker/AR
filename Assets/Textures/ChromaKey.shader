Shader "Unlit/ChromaKey"
{
    Properties
    {
        _MainTex ("Video", 2D) = "white" {}
        _KeyColor ("Key Color", Color) = (0,1,0,1)
        _Threshold ("Threshold", Range(0,1)) = 0.30
        _Smoothing ("Smoothing", Range(0,1)) = 0.12
        _Spill ("Spill Remove", Range(0,1)) = 0.75
        _Erode ("Matte Erode", Range(0,0.2)) = 0.02
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Cull Off

        // premultiplied alpha blending
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _KeyColor;
            float _Threshold, _Smoothing, _Spill, _Erode;

            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // Compare in chroma space (less sensitive to brightness)
            float2 CbCr(float3 rgb)
            {
                float cb = 0.5 + (-0.169*rgb.r - 0.331*rgb.g + 0.5*rgb.b);
                float cr = 0.5 + ( 0.5*rgb.r - 0.419*rgb.g - 0.081*rgb.b);
                return float2(cb, cr);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float diff = distance(CbCr(col.rgb), CbCr(_KeyColor.rgb));

                // Matte (erode helps remove thin green edge)
                float a = smoothstep(_Threshold + _Erode, _Threshold + _Erode + _Smoothing, diff);

                // Spill suppression near edges
                float edge = 1.0 - a; // 1 at edge/transparent
                float maxRB = max(col.r, col.b);
                col.g = lerp(col.g, maxRB, edge * _Spill);

                // Premultiply to remove color fringes on edges
                col.rgb *= a;
                col.a = a;

                return col;
            }
            ENDCG
        }
    }
}
