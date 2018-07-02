// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/UIShader"
{
	Properties
	{
		/*
		_FirstText("First Texture",2D) = "white"{}
		_SecondText("Second Texture",2D) = "white"{}
		_ThirdText("Third Texture",2D) = "white"{}
		_FloorText("Floor Texture",2D) = "white"{}
		_LogoText("Logo Texture",2D) = "white"{}
		

		_RightFloorAngle("Right Floor Angle", Range(0,180)) = 0.5
		_LeftFloorAngle("Left Floor Angle", Range(0,180)) = 0.5

		_FirstAngle("First Angle", Range(0,180)) = 0.5
		_SecondAngle("Second Angle", Range(0,180)) = 0.5

		_CircleHorizontalOffset("Horizontal Circle Offset", Range(0,1)) = 0.5
		_CircleVerticalOffset("Vertical Circle Offset", Range(0,1)) = 0.5
		_CircleInnerRadius("_CircleInnerRadius", Range(0,1)) = 0.5
		_CircleOuterRadius("_CircleOuterRadius", Range(0,2)) = 0.5


		*/
		_InnerColor("InnerWheel", Color) = (1,1,1,1)

		
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
	}

		Stencil
	{
		Ref[_Stencil]
		Comp[_StencilComp]
		Pass[_StencilOp]
		ReadMask[_StencilReadMask]
		WriteMask[_StencilWriteMask]
	}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass
		{
			Name "Default"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP

			struct vector2
			{
				float x;
				float y;
			};

			float CrossVector2(inout vector2 vect1,inout vector2 vect2)
			{
				return vect1.x*vect2.y - vect1.y*vect2.x;
			}

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			static const float M_PI = 3.14159265359 / 180.0;


			sampler2D _MainTex;
			sampler2D _FirstText;
			sampler2D _SecondText;
			sampler2D _ThirdText;
			sampler2D _FloorText;
			sampler2D _LogoText;

			float _RightFloorAngle;
			float _LeftFloorAngle;
			float _FirstAngle;
			float _SecondAngle;


			float _CircleHorizontalOffset;
			float _CircleVerticalOffset;
			float _CircleInnerRadius;
			float _CircleOuterRadius;


			fixed4 _InnerColor;
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			float4 _MainTex_ST;

			v2f vert(appdata_t v)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.worldPosition = v.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

				OUT.color = v.color * _Color;
				return OUT;
			}
			//half4 colorLogo = (tex2D(_LogoText, IN.texcoord));

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color;

				//color = (_InnerColor - half4(0,1,0,0)) * ( IN.texcoord.y ) + half4(0, 1, 0, 0);
				color = _InnerColor;

				float x = IN.texcoord.x-0.5;
				float y = IN.texcoord.y+0.1;
				if(y*y + x*x < 0.4)
				color = half4(0, 1, 0, 0);






				#ifdef UNITY_UI_CLIP_RECT
						color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				#endif

				#ifdef UNITY_UI_ALPHACLIP
						clip(color.a - 0.001);
				#endif



				color.a = 1;
				return color;
			}
			ENDCG
		}
	}
}
