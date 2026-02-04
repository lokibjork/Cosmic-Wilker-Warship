using UnityEngine;

public class TextureScroller : MonoBehaviour
{
    [Header("Controles de Velocidade")]
    [Tooltip("Controla a velocidade horizontal. Negativo vai para a esquerda.")]
    // Ao declarar como 'public', ela aparece no Inspector e pode ser acessada por outros scripts
    [Range(-2f, 2f)] // Opcional: cria um slider para facilitar o ajuste fino
    public float scrollSpeedX = -0.1f;

    [Range(-2f, 2f)]
    public float scrollSpeedY = 0.0f;

    private Renderer _renderer;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // A lógica de movimento
        float offsetX = Mathf.Repeat(Time.time * scrollSpeedX, 1);
        float offsetY = Mathf.Repeat(Time.time * scrollSpeedY, 1);

        Vector2 offset = new Vector2(offsetX, offsetY);

        // Aplica na textura
        _renderer.material.mainTextureOffset = offset;
    }
}