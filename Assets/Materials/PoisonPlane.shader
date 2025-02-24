// Partial credits to Anuken's Mindustry slag shader
Shader "Custom/Poison Plane"
{
    Properties
    {
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _NoiseScl("Noise Scale", Range(0.1, 2)) = 1

        _Color("Color", Color) = (1,1,1,1)
        _LBound("Lower Bound", Range(0, 1)) = 0.3
        _Color1("Color 1", Color) = (1,1,0,1)
        _UBound("Upper Bound", Range(0, 1)) = 0.5
        _Color2("Color 2", Color) = (1,0,0,1)
           
        _Speed("Speed", Range(0.01, 2)) = 1
        _Scroll("Scroll Speed", Range(-2, 2)) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Transparent"}
        //ZWrite Off
        //Cull Off
            //ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _NoiseTex;
            //sampler2D _CameraDepthTexture;
            fixed4 _Color, _Color1, _Color2;
            float _LBound, _UBound, _Speed, _NoiseScl, _Scroll;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = float2(v.uv.x, v.uv.y * v.normal.y + (1.0 - v.uv.y) * (1.0 - v.normal.y));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float btime = _Time.x * _Speed;
                //float noise = tex2D(_NoiseTex, float2(i.uv.x + _Time.x * _Speed, i.uv.y - _Time.x * _Speed)).r;
                float2 pos = float2(i.uv.x - _Time.x * _Scroll, i.uv.y) / _NoiseScl;
                float noise = (tex2D(_NoiseTex, float2(btime, btime) * float2(-0.9, 0.8) + pos).r + tex2D(_NoiseTex, float2(btime * 1.1, btime * 1.2) * float2(0.8, -1.0) + pos).r) / 2.0;

                fixed4 col = _Color;
                if (noise > _UBound) col = _Color2;
                else if (noise > _LBound) col = _Color1;
                col.a *= smoothstep(0.0, 0.5, i.uv.y);
                return col;
            }
            ENDCG
        }
    }
}
