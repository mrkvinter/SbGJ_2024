Shader "Custom/URP/SolidColorSpriteShaderWithAlpha"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1) // Основной цвет для закрашивания спрайта
        _MainTex ("Sprite Texture", 2D) = "white" {} // Текстура спрайта
    }
    SubShader
    {
        Tags
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "SpriteRenderType"="Sprite" 
            "IgnoreProjector"="True" 
        }
        Pass
        {
            Name "ColorPass"
            Blend SrcAlpha OneMinusSrcAlpha // Настройка смешивания для прозрачности
            Cull Off // Отключение отсечения граней
            ZWrite Off // Отключение записи в буфер глубины

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 position : POSITION; // Позиция вершины
                float4 color : COLOR;       // Цвет вершины (из Sprite Renderer)
                float2 uv : TEXCOORD0;      // UV координаты
            };

            struct Varyings
            {
                float4 position : SV_POSITION; // Позиция в пространстве клипа
                float4 color : COLOR;          // Передача цвета вершины
                float2 uv : TEXCOORD0;         // UV координаты
            };

            // Вершинный шейдер
            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                // Трансформация позиции вершины в пространство клипа
                OUT.position = TransformObjectToHClip(IN.position.xyz);
                // Передача UV координат
                OUT.uv = IN.uv;
                // Передача цвета вершины
                OUT.color = IN.color;
                return OUT;
            }

            float4 _Color; // Основной цвет

            // Объявление текстуры и сэмплера с использованием макросов URP
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // Фрагментный шейдер
            half4 frag (Varyings IN) : SV_Target
            {
                // Получение цвета из текстуры с использованием UV координат
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                
                // Установка RGB как заданного цвета, а альфа учитывается из текстуры и вершинного цвета
                return half4(_Color.rgb * IN.color.rgb, _Color.a * IN.color.a * texColor.a);
            }
            ENDHLSL
        }
    }
    FallBack "Sprites/Default" // Резервный шейдер на случай, если этот не сработает
}
