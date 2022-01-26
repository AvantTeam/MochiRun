Shader "Custom/GrassTest"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
        _TopColor ("Top Color", Color) = (0,1,0,1)
        _LBound ("Lower Bound", Range(-1, 1)) = 0
        _UBound ("Upper Bound", Range(-1, 1)) = 0.5
    }
    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color, _TopColor;
            float _LBound, _UBound;
         
            struct v2f {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
            };
            
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float c = smoothstep(_LBound, _UBound, v.normal.y);
                o.color = _Color * (1.0 - c) + _TopColor * c;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target { return i.color; }
            ENDCG
        }
    }
}
