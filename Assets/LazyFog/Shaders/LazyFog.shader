Shader "LemonSpawn/LazyFog_URP" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Scale ("Scale", Range(0,5)) = 1
        _Intensity ("Intensity", Range(0,1)) = 0.5
        _Alpha ("Alpha", Range(0,2.5)) = 0.75
        _AlphaSub ("AlphaSub", Range(0,1)) = 0.0
        _Pow ("Pow", Range(0,4)) = 1.0
    }

    SubShader {
        Tags { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent+101" 
            "RenderPipeline" = "UniversalPipeline" 
        }
        LOD 400

        Pass {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
            };

            struct Varyings {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 worldPos     : TEXCOORD1;
                float4 color        : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _Scale;
                float _Intensity;
                float _Alpha;
                float _AlphaSub;
                float _Pow;
            CBUFFER_END

            Varyings vert(Attributes input) {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.worldPos = vertexInput.positionWS;
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            half4 frag(Varyings input) : SV_Target {
                float3 viewDir = normalize(_WorldSpaceCameraPos - input.worldPos);
                
                // Calcul de la texture
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv * _Scale);
                
                float xx = texColor.r * _Intensity;
                xx = pow(max(0, xx), _Pow);
                
                half4 c;
                c.rgb = xx * _Color.rgb;
                
                // Masque circulaire basé sur l'UV (similaire à l'original)
                float distFromCenter = length(input.uv - float2(0.5, 0.5));
                float alphaMask = input.color.a - 2.5 * distFromCenter;
                
                c.a = texColor.r * alphaMask * _Alpha;
                c.a = saturate(c.a - _AlphaSub);

                return c;
            }
            ENDHLSL
        }
    }
}