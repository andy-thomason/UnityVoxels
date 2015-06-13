Shader "Custom/chunk_shader" {
    Properties {
        //_Color ("Main Color", Color) = (1,1,1,0.5)
        _Texture ("Texture", 2D) = "white" { }
    }
    SubShader {
        Pass {

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f {
                float4 pos : SV_POSITION;
                float3 color : COLOR0;
                float2 uv : TEXCOORD0;
            };
            
            sampler2D _Texture;

            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                o.color = v.color;
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
            	return fixed4(tex2D(_Texture, i.uv) * float4(i.color, 1));
            	//return fixed4(1, 0, 0, 1);
            }
            ENDCG

        }
    }
}
