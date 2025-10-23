Shader "Custom/PortalWindow"
{
    SubShader
    {
        Pass
        {
            ZWrite off
            ColorMask 0

            Stencil {
                Ref 1
                Pass replace
            }
        }
    }
}
