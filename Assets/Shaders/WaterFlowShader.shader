Shader "Custom/URPWaterFlow"
{
    Properties
    {
        _BaseMap ("Water Texture", 2D) = "white" {}
        _FlowSpeed ("Flow Speed", Float) = 0.2
        _FlowDirection ("Flow Direction (XY)", Vector) = (1, 0, 0, 0)
        _Color ("Tint Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // URP texture macros
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _BaseMap_ST;
            float _FlowSpeed;
            float4 _FlowDirection;
            float4 _Color;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Flow direction
                float2 flowDir = normalize(_FlowDirection.xy);
                float2 uvOffset = flowDir * _FlowSpeed * _Time.y;

                // Scroll UVs
                float2 uv = IN.uv + uvOffset;

               
                half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);

                return texColor * _Color;
            }
            ENDHLSL
        }
    }
}
