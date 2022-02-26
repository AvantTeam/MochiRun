Shader "Unlit/MapGridShader"
{
    Properties
	{
		[Header(Main Grid)][Space(5)]
		_GridColor("Grid Color", Color) = (1,1,1,1)
		_MeshColor("Mesh Color", Color) = (0,0,0,0)
		_GridSize("Grid Size", Float) = 1  
		_LineThickness("Line Thickness", Float) = 0.03

		[Header(Secondary Grid)][Space(5)]
		_SGridScale("Scale", Float) = 5 
		_SGridColor("Secondary Grid Color", Color) = (1,1,1,1)
		_SLineThickness("Secondary Line Thickness", Float) = 0.05

		[Header(Detail)][Space(5)]
		[IntRange] _Xonly("Show X only", Range(0, 1)) = 0
		[IntRange] _Yonly("Show Y only", Range(0, 1)) = 0
		_OffsetX("X Offset", Range(0, 1)) = 0 
		_OffsetY("Y Offset", Range(0, 1)) = 0  
	}

	SubShader
	{
		Tags { "Queue"="Transparent"}
		LOD 100
		//ZWrite Off

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			//Blend One OneMinusSrcAlpha
			Offset -20, -20

			CGPROGRAM
#define IF(a, b, c) lerp(b, c, step((fixed) (a), 0))
#define IFS(a, b, c) lerp(b, c, a)
#define DS(a) step(0.5, a) * 0.5 + step(0.1, a) * 0.5
#pragma vertex vert
#pragma fragment frag

# include "UnityCG.cginc"

			fixed4 _GridColor, _MeshColor, _SGridColor;
			float _GridSize, _LineThickness, _Xonly, _Yonly, _OffsetX, _OffsetY, _SGridScale, _SLineThickness;

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = mul(unity_ObjectToWorld, v.vertex).xy;
				return o;
			}

			float DrawGrid(float2 uv, float sz, float aa)
			{
				float aaThresh = aa;
				float aaMin = aa * 0.1;

				float2 gUV = float2(uv.x + _OffsetX, uv.y + _OffsetY) / sz + aaThresh;

				// Check if want X only.
				gUV = IF(
					_Xonly > 0
					, uv.x + _OffsetX / sz + aaThresh
					, gUV
				);
				
				// Check if want Y only.
				gUV = IF(
					_Yonly > 0
					, uv.y + _OffsetY / sz + aaThresh
					, gUV
				);

				float2 fl = floor(gUV);
				gUV = frac(gUV);
				gUV -= aaThresh;
				gUV = smoothstep(aaThresh, aaMin, abs(gUV));
				float d = max(gUV.x, gUV.y);

				return d;
			}

			fixed4 frag(v2f i) : SV_Target
			{   
				fixed r = DS(DrawGrid(i.uv, _GridSize, _LineThickness / _GridSize));
				fixed r2 = DS(DrawGrid(i.uv, _GridSize * _SGridScale, _SLineThickness / (_GridSize * _SGridScale)));
				return IFS(
					r2
					, IFS(r, _MeshColor, _GridColor)
					, _SGridColor
				);
			}
		
			ENDCG
		}
	}
}
