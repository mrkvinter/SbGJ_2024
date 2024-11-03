Shader "Custom/LCDShader2D"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _PixelSize("Pixel Size", Vector) = (1, 1, 0, 0)
        _ColorOffset("Color Offset", Float) = 0.005
        _ScanlinePixelSize("Scanline Pixel Size", Float) = 5.0
        _ScanlineIntensity("Scanline Intensity", Float) = 0.2
        _ScanlineIntensitySize("Scanline Intensity Size", Float) = 1
        _ScanlineIntensitySpeed("Scanline Intensity Speed", Float) = 1
        _DitherSize("Dither Size", Float) = 0.1
        _DitherIntensity("Dither Intensity", Float) = 0.1
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue"="Transparent"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            ZTest LEqual


            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float2 _PixelSize;
            float _ColorOffset;
            float _ScanlinePixelSize;
            float _ScanlineIntensity;
            float _ScanlineIntensitySize;
            float _ScanlineIntensitySpeed;
            float _DitherSize;
            float _DitherIntensity;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float luma(float3 color) {
              return dot(color, float3(0.299, 0.587, 0.114));
            }

            float luma(float4 color) {
              return dot(color.rgb, float3(0.299, 0.587, 0.114));
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = UnityObjectToClipPos(input.positionOS);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }
            
            float dither2x2(float2 position, float brightness) {
              int x = int(fmod(position.x, 2.0));
              int y = int(fmod(position.y, 2.0));
              int index = x + y * 2;
              float limit = 0.0;

              if (x < 8) {
                if (index == 0) limit = 0.25;
                if (index == 1) limit = 0.75;
                if (index == 2) limit = 1.00;
                if (index == 3) limit = 0.50;
              }

              return brightness < limit ? 0.0 : 1.0;
            }

            float3 dither2x2(float2 position, float3 color)
            {
                return color * dither2x2(position, luma(color));
            }

            float4 dither2x2(float2 position, float4 color)
            {
                return float4(color.rgb * dither2x2(position, luma(color)), 1.0);
            }

            float dither4x4(float2 position, float brightness)
            {
                int x = int(fmod(position.x, 4.0));
                int y = int(fmod(position.y, 4.0));
                int index = x + y * 4;
                float limit = 0.0;

                // if (x < 8)
                {
                    if (index == 0) limit = 0.0625;
                    if (index == 1) limit = 0.5625;
                    if (index == 2) limit = 0.1875;
                    if (index == 3) limit = 0.6875;
                    if (index == 4) limit = 0.8125;
                    if (index == 5) limit = 0.3125;
                    if (index == 6) limit = 0.9375;
                    if (index == 7) limit = 0.4375;
                    if (index == 8) limit = 0.25;
                    if (index == 9) limit = 0.75;
                    if (index == 10) limit = 0.125;
                    if (index == 11) limit = 0.625;
                    if (index == 12) limit = 1.0;
                    if (index == 13) limit = 0.5;
                    if (index == 14) limit = 0.875;
                    if (index == 15) limit = 0.375;
                }

                return brightness < limit ? 0.0 : 1.0;
            }

            float3 dither4x4(float2 position, float3 color) {
              return color * dither4x4(position, luma(color));
            }

            float4 dither4x4(float2 position, float4 color) {
              return float4(color.rgb * dither4x4(position, luma(color)), 1.0);
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;

                // Pixelation Effect
                //uncomment this line to enable pixelation
                // float2 pixelUV = floor(uv * _PixelSize) / _PixelSize;
                float2 pixelUV = uv;

                half4 color = tex2D(_MainTex, pixelUV);

                // Color Separation
                half4 colorR = tex2D(_MainTex, pixelUV + float2(_ColorOffset, 0));
                half4 colorG = tex2D(_MainTex, pixelUV);
                half4 colorB = tex2D(_MainTex, pixelUV - float2(_ColorOffset, 0));
                color.rgb = half3(colorR.r, colorG.g, colorB.b);

                // GameBoy Color Palette
                // float brightness = luma(color.rgb);
                // brightness = floor(brightness * 32) / 32;
                // color.rgb = float3(0.05, brightness, 0.05);

                // Dithering Effect
                float b = dither4x4(input.positionCS.xy * _DitherSize, _DitherIntensity);
                // if (b == 0.0) {
                //     color.rgb = 0.0;
                // }
                // color.g = 1 - b;
                // color.b = fmod(input.positionCS.z, 128) * 1 / 128;

                // Scanline Effect
                float val = uv.y * _ScanlinePixelSize * 3.14 + _Time * _ScanlineIntensitySpeed;
                val = floor(val) / _ScanlineIntensitySize;
                float scanline = sin(val);
                color.rgb *= (1.0 - _ScanlineIntensity * scanline);
                if (b == 0.0) {
                    // color.rgb = 0.0;
                    // color.a *= .75;
                }
                color.a *= b;


                return color;
            }
            ENDHLSL
        }
    }

    FallBack "Sprites/Default"
}