Shader "Custom/FogBackground"
{
    Properties
    {
        // Cor principal do fundo (azul escuro espacial)
        _ColorA ("Cor Escura", Color) = (0.02, 0.02, 0.1, 1)
        // Cor da névoa (azul/roxo mais claro)
        _ColorB ("Cor da Névoa", Color) = (0.1, 0.05, 0.3, 1)
        // Velocidade do movimento da névoa
        _Speed ("Velocidade", Float) = 0.15
        // Escala do padrão de névoa (menor = nuvens maiores)
        _Scale ("Escala", Float) = 3.5
        // Intensidade do efeito
        _Intensity ("Intensidade", Range(0, 1)) = 0.7
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Background"
            // Compatível com URP
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Variáveis que vieram do Properties
            float4 _ColorA;
            float4 _ColorB;
            float  _Speed;
            float  _Scale;
            float  _Intensity;

            // Estrutura de entrada do vértice
            struct Attributes
            {
                float4 positionOS : POSITION;  // posição do vértice
                float2 uv         : TEXCOORD0; // coordenada UV (0 a 1)
            };

            // Estrutura passada do vértice para o fragmento
            struct Varyings
            {
                float4 positionHCS : SV_POSITION; // posição final na tela
                float2 uv          : TEXCOORD0;   // UV interpolado
            };

            // =============================================
            // FUNÇÃO DE RUÍDO - O coração do fog!
            // Recebe uma posição 2D e retorna um valor 0~1
            // Usamos math puro (sem textura) para ser didático
            // =============================================

            // Ruído simples baseado em seno (Value Noise simplificado)
            float hash(float2 p)
            {
                // Embaralha as coordenadas com uma fórmula matemática
                p = frac(p * float2(234.34, 435.345));
                p += dot(p, p + 34.23);
                return frac(p.x * p.y);
            }

            // Interpola suavemente entre pontos de ruído
            // Isso é chamado de "Smooth Noise" ou "Value Noise"
            float smoothNoise(float2 uv)
            {
                float2 i = floor(uv); // parte inteira (célula da grade)
                float2 f = frac(uv);  // parte fracionária (posição dentro da célula)

                // Pega valores nos 4 cantos da célula
                float a = hash(i);
                float b = hash(i + float2(1, 0));
                float c = hash(i + float2(0, 1));
                float d = hash(i + float2(1, 1));

                // Curva de suavização: f*f*(3-2*f) → mais suave que interpolação linear
                float2 u = f * f * (3.0 - 2.0 * f);

                // Interpola bilinear entre os 4 cantos
                return lerp(a, b, u.x) +
                       (c - a) * u.y * (1.0 - u.x) +
                       (d - b) * u.x * u.y;
            }

            // "Fractal Brownian Motion" — empilha camadas de ruído
            // Cada camada (oitava) tem frequência dobrada e amplitude menor
            // Resultado: detalhes em múltiplas escalas → parece névoa real!
            float fbm(float2 uv)
            {
                float valor = 0.0;
                float amplitude = 0.5;  // quanto cada oitava contribui
                float frequencia = 1.0;

                // 5 oitavas de detalhe
                for (int i = 0; i < 5; i++)
                {
                    valor += amplitude * smoothNoise(uv * frequencia);
                    frequencia *= 2.0;   // dobra a frequência
                    amplitude  *= 0.5;   // diminui a amplitude
                }
                return valor;
            }

            // =============================================
            // VERTEX SHADER — transforma vértices 3D → tela
            // =============================================
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                // TransformObjectToHClip: converte posição do objeto para clip space
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            // =============================================
            // FRAGMENT SHADER — decide a cor de cada pixel
            // =============================================
            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv * _Scale;

                // _Time.y = tempo em segundos desde o início
                // Movemos o UV ao longo do tempo para animar a névoa
                float2 movimento = float2(_Time.y * _Speed, _Time.y * _Speed * 0.7);

                // Calculamos 2 camadas de fbm com direções ligeiramente diferentes
                // Isso cria um turbilhão mais orgânico
                float n1 = fbm(uv + movimento);
                float n2 = fbm(uv + movimento * 1.3 + float2(5.2, 1.3));

                // Combina as duas camadas
                float nebula = fbm(uv + n1 * 0.5 + n2 * 0.3 + movimento);

                // Mistura as cores de acordo com o ruído
                float4 cor = lerp(_ColorA, _ColorB, nebula * _Intensity);

                return cor;
            }

            ENDHLSL
        }
    }
}