Shader "Tiny/PaletteShader"
{
	Properties
	{
        [PerRendererData] _Color("Tint", Color) = (1, 1, 1, 1)
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _AlphaTex("Lookup Texture", 2D) = "white" {}
        [PerRendererData] _PaletteTex("Palette Texture", 2D) = "white" {}
        [PerRendererData] _PaletteRow("Palette Row", float) = 0
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

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#include "UnityCG.cginc"

			struct appdata_t
			{
				fixed4 vertex : POSITION;
				fixed4 color : COLOR;
				fixed2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				fixed4 vertex : SV_POSITION;
				fixed2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
			};

            uniform sampler2D _PaletteTex;
            uniform fixed _PaletteRow;
            fixed2 _PaletteTex_TexelSize;
            uniform sampler2D _AlphaTex;
            uniform sampler2D _MainTex;
            uniform fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 AlphaLookup = tex2D(_AlphaTex, IN.texcoord);
                fixed AlphaValue = (AlphaLookup.a * 255.0f) + 0.5f;

				fixed PaletteU = _PaletteTex_TexelSize.x * AlphaValue;
				fixed PaletteV = 1.0f - (_PaletteTex_TexelSize.y * (_PaletteRow + 0.5f));

                fixed4 FinalColor = tex2D(_PaletteTex, fixed2(PaletteU, PaletteV));
                FinalColor *= IN.color;

                return(FinalColor);
			}
			ENDCG
		}
	}
}
