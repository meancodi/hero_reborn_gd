Shader "Custom/VisionCone"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,0,0.3)   // Yellow
        _AlertColor("Alert Color", Color) = (1,0,0,0.45) // Red
        _AlertAmount("Alert Amount", Range(0,1)) = 0     // 0 = Yellow, 1 = Red
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 _BaseColor;
            fixed4 _AlertColor;
            float _AlertAmount;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return lerp(_BaseColor, _AlertColor, _AlertAmount);
            }
            ENDCG
        }
    }
}
