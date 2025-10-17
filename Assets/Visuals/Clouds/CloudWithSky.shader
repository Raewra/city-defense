Shader "Custom/CloudPlaneWithSky"
{
    Properties
    {
        _ShapeNoise ("Shape Noise", 2D) = "white" {}
        _DetailNoise ("Detail Noise", 2D) = "gray" {}
        _Color ("Cloud Color", Color) = (1,1,1,1)
        _Alpha ("Cloud Alpha", Range(0,1)) = 0.5
        _CloudScale ("Cloud Scale", Float) = 1.0
        _DetailNoiseScale ("Detail Noise Scale", Float) = 2.0
        _Wind ("Wind Direction", Vector) = (0.1, 0.0, 0.0, 0.0)
        _DetailNoiseWind ("Detail Noise Wind", Vector) = (0.0, 0.1, 0.0, 0.0)
        _Speed ("Cloud Speed", Range(0,2)) = 0.2

        _SkyTop ("Sky Top Color", Color) = (0.2, 0.4, 0.7, 1)     // darker blue
        _SkyBottom ("Sky Bottom Color", Color) = (0.5, 0.7, 1.0, 1) // lighter blue
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
            float _Speed;
            float4 _SkyTop;
            float4 _SkyBottom;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Gradient background
                float t = i.uv.y;
                float3 skyCol = lerp(_SkyBottom.rgb, _SkyTop.rgb, t);

                // Scrolling UVs for clouds
                float2 uvShape = i.uv * _CloudScale + _Wind.xy * (_Time.y * _Speed);
                float2 uvDetail = i.uv * _DetailNoiseScale + _DetailNoiseWind.xy * (_Time.y * _Speed);

                // Sample noises
                float shape = tex2D(_ShapeNoise, uvShape).r;
                float detail = tex2D(_DetailNoise, uvDetail).r;

                // Density controls how strong the clouds appear
                float density = saturate(lerp(shape, detail, 0.5));

                // Mix clouds on top of sky
                float3 cloudCol = lerp(skyCol, _Color.rgb, density);
                float finalAlpha = saturate(density * _Alpha);

                return float4(cloudCol, 1.0); // fully opaque (sky + clouds)
            }
            ENDCG
        }
    }
}
