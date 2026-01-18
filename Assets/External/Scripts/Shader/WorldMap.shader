Shader "Custom/WorldMap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LineColor ("Line Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _MainTex_TexelSize;

            float4 _LineColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float2 offset = _MainTex_TexelSize.xy * 5;
                
                float4 upCol = tex2D(_MainTex, i.uv + float2(0, offset.y));
                float4 downCol = tex2D(_MainTex, i.uv - float2(0, offset.y));
                float4 leftCol = tex2D(_MainTex, i.uv - float2(offset.x, 0));
                float4 rightCol = tex2D(_MainTex, i.uv + float2(offset.x, 0));
                
                float upSub = max(upCol.a - col.a, 0);
                float downSub = max(downCol.a - col.a, 0);
                float leftSub = max(leftCol.a - col.a, 0);
                float rightSub = max(rightCol.a - col.a, 0);
                
                float edgeIntensity = upSub + downSub + leftSub + rightSub;
                
                float4 finalColor = col + _LineColor * edgeIntensity;
                finalColor = saturate(finalColor);
                
                return finalColor;
            }

            ENDHLSL
        }
    }
}
