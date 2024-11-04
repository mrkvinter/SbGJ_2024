Shader "Custom/WorldSpaceTexture2D"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScreenSize ("Screen Size", Vector) = (1920, 1080, 0, 0) // Размер экрана
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 screenUV : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _ScreenSize; // Размер экрана

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float2 screenPos = o.pos.xy / o.pos.w;
                screenPos = screenPos * 0.5 + 0.5;
                screenPos.y = 1.0 - screenPos.y;
                o.screenUV = screenPos * _ScreenSize.xy;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Применяем текстуру по экранным координатам
                float2 uv = i.screenUV / _ScreenSize.xy;
                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Sprites/Default"
}