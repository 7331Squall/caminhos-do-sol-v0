Shader "Custom/GridUnlitShader"
{
    Properties
    {
        [Enum(Cull)]
        _Cull ("Cull Mode", Float) = 2
        _Offset ("Texture Offset", Vector) = (0,0,0,0)
        _GridColor ("Grid Color", Color) = (1,0,0,1)
        _MainTex ("Grid Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Pass
        {
            Cull [_Cull]
            Name "Forward"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
				float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _GridColor;
            float4 _Offset;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv + _Offset.xy;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float grid = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).r;
                float alpha = 1.0 - grid;

                return half4(_GridColor.rgb, alpha);
                            }
            ENDHLSL
        }
    }
}