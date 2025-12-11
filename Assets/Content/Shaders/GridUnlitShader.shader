Shader "Custom/GridUnlitShader"
{
    Properties
    {
        _Offset ("Offset", Vector) = (0,1,0,0)
        _WireColor ("Wire Color", Color) = (1,0,0,1)
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Cull Front
        ZWrite Off
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
            };

            float2 _Offset;
            float4 _WireColor;
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv + _Offset;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float3 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb;

                // luminância (mesmo cálculo do shader original)
                float lum = dot(tex, float3(0.2126, 0.7152, 0.0722));

                float alpha = 1.0 - lum;

                return float4(_WireColor.rgb, alpha);
            }
            ENDHLSL
        }
    }
}