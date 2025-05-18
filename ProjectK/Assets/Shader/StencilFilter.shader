Shader "Custom/StencilFilter"
{
    Properties
    {
        _Color("Color", Color) = (0,0,0,0.75)
    }

    SubShader
    {
        Tags { "Queue"="Overlay" }
        Pass
        {
            Stencil
            {
                Ref 1
                Comp NotEqual
                Pass keep
            }

            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 _Color;
            fixed4 frag(v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}
