Shader "Hsinpa/AreaDebugger"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _Lint("Lint", Color) = (1, 1, 1, 1)
        _BoxPosition("Position", Vector) = (0, 0, 0, 0)
        _Width("Width", Float) = 0
        _Height("Height", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
            float4 _MainTex_ST;

            float2 _BoxPosition;
            float _Width;
            float _Height;
            fixed4 _Lint;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                fixed2 bl = step(fixed2(0.1, 0.1), i.uv);       // bottom-left
                fixed2 tr = step(fixed2(0.1, 0.1), 1.0 - i.uv);   // top-right

                fixed borderCol = bl.x * bl.y * tr.x * tr.y;

                fixed3 color = fixed3(borderCol, borderCol, borderCol);

                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
}
