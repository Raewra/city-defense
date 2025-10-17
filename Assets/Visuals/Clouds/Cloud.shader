Shader "Custom/CloudPlane"
{
    Properties
    {
        _ShapeNoise ("Shape Noise", 2D) = "white" {}
        _DetailNoise ("Detail Noise", 2D) = "gray" {}
        _Color ("Cloud Color", Color) = (1,1,1,1)
        _Alpha ("Alpha", Range(0,1)) = 0.5
        _CloudScale ("Cloud Scale", Float) = 1.0
        _DetailNoiseScale ("Detail Noise Scale", Float) = 2.0
        _Wind ("Wind Direction", Vector) = (0.1, 0.0, 0.0, 0.0)
        _DetailNoiseWind ("Detail Noise Wind", Vector) = (0.0, 0.1, 0.0, 0.0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

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

            sampler2D _ShapeNoise;
            sampler2D _DetailNoise;
            float4 _ShapeNoise_ST;
            float4 _DetailNoise_ST;
            float4 _Color;
            float _Alpha;
            float _CloudScale;
            float _DetailNoiseScale;
            float4 _Wind;
            float4 _DetailNoiseWind;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Scroll UVs with time
                float2 uvShape = i.uv * _CloudScale + _Wind.xy * _Time.y;
                float2 uvDetail = i.uv * _DetailNoiseScale + _DetailNoiseWind.xy * _Time.y;

                float shape = tex2D(_ShapeNoise, uvShape).r;
                float detail = tex2D(_DetailNoise, uvDetail).r;

                float density = saturate(lerp(shape, detail, 0.5));
                fixed4 col = _Color;
                col.a = density * _Alpha;

                return col;
            }
            ENDCG
        }
    }
}
