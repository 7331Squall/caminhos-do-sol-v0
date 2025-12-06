Shader "Custom/DepthMaskShader"
{
    SubShader
    {
        Tags
        {
            "Queue"="Geometry-10"
        }
        Cull Off
        ZWrite On
        ColorMask 0
        Pass {}
    }
}