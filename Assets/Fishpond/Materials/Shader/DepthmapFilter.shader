Shader "Hsinpa/DepthmapFilter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _Threshold_Min("Threshold_Mix", Range(0,1)) = 0.0
        _Threshold_Max("Threshold_Max", Range(0,1)) = 1.0

        _Rotation("Rotation", Range(0.0 , 3.14)) = 0
        _KernelSize("Kernel Size (Performance heavy)", Int) = 1
        _Erode("Erode", Range(0.0 , 10)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100



        //Gaussian Blur
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_horizontal
            #pragma target 3.0

            #include "UnityCG.cginc"

            // Define the constants used in Gaussian calculation.
            static const float TWO_PI = 6.28319;
            static const float E = 2.71828;

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _MainTex_TexelSize;
            int	_KernelSize;

            // Two-dimensional Gaussian curve function.
            float gaussian(int x, int y)
            {
                return 1.0;
            }

            float4 frag_horizontal(v2f_img i) : SV_Target
            {
                float3 col = float3(0.0, 0.0, 0.0);
                float kernelSum = 0.0;

                fixed upper = ((_KernelSize - 1.0) / 2.0);
                fixed lower = -upper;

                for (int x = lower; x <= upper; ++x)
                {
                    for (int y = lower; y <= upper; ++y)
                    {
                        float gauss = gaussian(x, y);
                        kernelSum += gauss;

                        fixed2 offset = fixed2(_MainTex_TexelSize.x * x, _MainTex_TexelSize.y * y);
                        col += gauss * tex2D(_MainTex, i.uv + offset);
                    }
                }

                col /= kernelSum;

                return float4(col, 1.0);
            }
            ENDCG
        }

        //Erosion
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float2 _MainTex_TexelSize;
            float _Erode;

            float Erode(sampler2D tex, float2 uv) {

                float4 sum = tex2D(tex, (uv));

                for (float dist = 1.0; dist < _Erode; dist++) {
                
                    sum = min(sum, tex2D(tex, (uv + (float2(-dist, -dist) * _MainTex_TexelSize))));
                    sum = min(sum, tex2D(tex, (uv + (float2(0.0, -dist) * _MainTex_TexelSize))));
                    sum = min(sum, tex2D(tex, (uv + (float2(dist, -dist) * _MainTex_TexelSize))));

                    //Center
                    sum = min(sum, tex2D(tex, (uv + (float2(-dist, 0.0) * _MainTex_TexelSize))));
                    sum = min(sum, tex2D(tex, (uv + (float2(dist, 0.0) * _MainTex_TexelSize))));

                    //Top
                    sum = min(sum, tex2D(tex, (uv + (float2(-dist, dist) * _MainTex_TexelSize))));
                    sum = min(sum, tex2D(tex, (uv + (float2(0.0, dist) * _MainTex_TexelSize))));
                    sum = min(sum, tex2D(tex, (uv + (float2(dist, dist) * _MainTex_TexelSize))));

                }
                
                return float4(sum.xyz , 1.0);
            }

            float4 frag(v2f_img IN) : COLOR {
                float s = Erode(_MainTex, IN.uv);

                return float4(s, s, s, 1);
            }
        ENDCG
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
            float4 _MainTex_ST;

            float _Threshold_Min;
            float _Threshold_Max;

            float _Threshold;
            uniform float _Rotation;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }


            float2x2 rotate2d(float _angle) {
                return float2x2(cos(_angle), -sin(_angle),
                    sin(_angle), cos(_angle));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //Rotate
                fixed2 rotatedUV = i.uv - float2(.5, .5);
                rotatedUV = mul(rotatedUV, rotate2d(_Rotation));
                rotatedUV += float2(.5, .5);

                fixed4 col = tex2D(_MainTex, rotatedUV);
                fixed filterValue = col.x;

                if (filterValue < _Threshold_Min || filterValue >= _Threshold_Max) {
                    filterValue = 0.0;
                }
                else {
                    filterValue = 1.0;
                }

                return fixed4(filterValue, filterValue, filterValue, 1.0);
            }
            ENDCG
        }
    }
}
