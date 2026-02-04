Shader "GRD/Sprites/Doodle_Boiling"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0

        // --- DOODLE PROPERTIES ---
        [Header(Doodle Settings)]
        _DoodleFPS ("Doodle FPS (Stop Motion)", Float) = 12
        _NoiseScale ("Noise Scale (Frequency)", Float) = 25
        _NoiseSnap ("Noise Snap (Hardness)", Range(0, 1)) = 0 // 0 = Ondulado, 1 = Quadrado/Glitch
        _DoodleSpeed ("Animation Speed", Float) = 10
        _DoodleAmpX ("Distortion Amount X", Range(0, 0.1)) = 0.01
        _DoodleAmpY ("Distortion Amount Y", Range(0, 0.1)) = 0.01
        // -------------------------
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnityCG.cginc"

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
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _Color;
            fixed4 _RendererColor;
            fixed4 _Flip;

            // Variáveis do Doodle
            fixed _DoodleFPS;
            fixed _NoiseScale;
            fixed _NoiseSnap;
            fixed _DoodleSpeed;
            fixed _DoodleAmpX;
            fixed _DoodleAmpY;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color * _RendererColor;

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            float _EnableExternalAlpha;

            // Função para deixar a onda "quadrada" (mais hard/glitch)
            float hardWave(float value, float hardness)
            {
                float smooth = sin(value);
                float hard = sign(smooth); // Retorna -1 ou 1 (onda quadrada)
                return lerp(smooth, hard, hardness);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // 1. Controle de FPS (Time Snapping)
                float timeStep = floor(_Time.y * _DoodleFPS) / _DoodleFPS;

                // 2. Cálculo do Ruído (Noise)
                // Usamos a posição UV para variar o ruído ao longo do sprite
                // Somamos o tempo para animar
                
                // Ruído Horizontal
                float noiseX = hardWave(IN.texcoord.y * _NoiseScale + timeStep * _DoodleSpeed, _NoiseSnap);
                
                // Ruído Vertical (com fase deslocada para não ficar diagonal perfeito)
                float noiseY = hardWave(IN.texcoord.x * _NoiseScale + timeStep * (_DoodleSpeed * 0.8) + 33.0, _NoiseSnap);

                // 3. Aplicação do Offset
                float2 uvOffset = float2(noiseX * _DoodleAmpX, noiseY * _DoodleAmpY);
                float2 finalUV = IN.texcoord + uvOffset;

                // 4. Sample da Textura
                fixed4 c = tex2D(_MainTex, finalUV);
                
                // 5. Tratamento de bordas (Opcional: evita que o pixel repita se sair da área)
                // Se quiser que a imagem corte "seco" ao sair da UV 0-1, descomente abaixo:
                // if(finalUV.x < 0 || finalUV.x > 1 || finalUV.y < 0 || finalUV.y > 1) c.a = 0;

                fixed4 color = c * IN.color;
                
                // Lógica padrão de Alpha do Unity Sprites
                #if ETC1_EXTERNAL_ALPHA
                fixed4 alpha = tex2D (_AlphaTex, finalUV);
                color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
                #endif

                color.rgb *= color.a;
                return color;
            }
        ENDCG
        }
    }
}