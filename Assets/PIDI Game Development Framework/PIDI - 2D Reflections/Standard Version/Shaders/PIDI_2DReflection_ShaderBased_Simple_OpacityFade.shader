﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

/*
    Copyright© 2018, Jorge Pinal Negrete. All Rights Reserved
*/
Shader "PIDI Shaders Collection/2D/Reflection Shaders/Shader Based/Simple (Opacity Fade)"
{

	Properties {

		[Space(12)]
		[Header(General Properties)]
		[Space(12)]
        [Toggle]_InvertProjection("Invert Projection", Float) = 0
        [PerRendererData]_MainTex( "Main Texture", 2D ) = "white"{}
		[Space(12)]
		[Header(Reflection Properties)]
		[Space(12)]
        _Color( "Reflection Color (RGB) Opacity(A)",Color) = (1,1,1,1)
        _ReflectionFade("Reflection Fade", Range(0,1)) = 1
		_SurfaceLevel("Surface Level",Range(-5,5)) = 0
	}

    SubShader
    {   
		
		// Draw ourselves after all opaque geometry
        Tags { "Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True" }

       
        GrabPass {
        }

        Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

        // Render the object with the texture generated above, and invert the colors
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

			sampler2D _GrabTexture;
            sampler2D _MainTex;
			float _SurfaceLevel;
            float _ReflectionFade;
			half4 _Color;
            half _InvertProjection;

            struct v2f
            {
                float2 screenUV : TEXCOORD0;
                float2 uv : TEXCOORD1;
                fixed4 color : COLOR;
                float4 pos : SV_POSITION;
                float fade : COLOR1;
            };

            v2f vert(appdata_full v) {
                v2f o;
                // use UnityObjectToClipPos from UnityCG.cginc to calculate 
                // the clip-space of the vertex
                o.pos = mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, v.vertex));
                float4 srfc = mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, float4(0,_SurfaceLevel,0,1) ) );
                srfc.xy /= srfc.w;
                o.uv = v.texcoord;
                o.fade = v.texcoord.y;
                o.color = v.color;
                // use ComputeGrabScreenPos function from UnityCG.cginc
                // to get the correct texture coordinate
				float4 pos = ComputeGrabScreenPos(o.pos);
                pos.xy/=pos.w;
                o.screenUV = pos.xy;
                #if !UNITY_UV_STARTS_AT_TOP
                o.screenUV.y = lerp(1-pos.y-srfc.y, 1-pos.y+srfc.y, 1-_InvertProjection );
                #else
                o.screenUV.y = lerp(1-pos.y-srfc.y, 1-pos.y+srfc.y, _InvertProjection );
                #endif
                return o;
            }

           

            half4 frag(v2f i) : SV_Target
            {   
                half4 col = tex2D(_MainTex, i.uv);
                half4 bgcolor = tex2D(_GrabTexture, i.screenUV)*_Color*i.color*_Color.a*col.a;
                return lerp( bgcolor*i.color.a,col*col.a*i.color*i.color.a, 1-( pow( i.uv.y, 16*(1-_ReflectionFade) ) ) );
            }
            ENDCG
        }

    }
}