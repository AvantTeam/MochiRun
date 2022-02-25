Shader "Unlit/ShieldShader"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
        _TopColor ("Top Color", Color) = (0,1,0,1)
        _OtlWidth ("Width", Range(0,20)) = 1
        _Alpha ("Alpha", Range(0,1)) = 1
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _BaseColor1("Base Color 1", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZTest Always

        //main
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _BaseColor, _BaseColor1;
            float _Alpha;
         
            struct v2f {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
            };
            
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float c = sin(_Time.z) * 0.5 + 0.5;
                o.color = _BaseColor * c + _BaseColor1 * (1.0 - c);

                o.color.a *= (1.0 - abs(v.normal.z)) * _Alpha;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target { return i.color; }
            ENDCG
        }

        //outline
        Pass
        {
			Tags { "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
			Blend SrcAlpha One
            Cull Front
            ZWrite Off
            ZTest Always

            CGPROGRAM
            #pragma vertex vert
 			#pragma fragment frag

			#include "UnityCG.cginc"
			//#include "STCore.cginc"

            float _OtlWidth, _Alpha;
            fixed4 _Color, _TopColor;

            struct appdata
            {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
                fixed4 color : COLOR;
			};

            v2f vert (appdata v)
            {
                v2f o;
			    o.pos = v.vertex;
			    o.pos.xyz += normalize(v.normal.xyz) * _OtlWidth * 0.008 * (1.1 + 0.1 * sin(_Time.w));
			    o.pos = UnityObjectToClipPos(o.pos);
                float c = smoothstep(-1.0, 1.0, v.normal.y); //todo
                o.color = _Color * (1.0 - c) + _TopColor * c;
                o.color.a *= _Alpha;
			    return o;
            }

            fixed4 frag(v2f i) : SV_Target
			{
				//clip(-negz(_OtlWidth));
		    	return i.color;
			}

            ENDCG
        }
    }
}
