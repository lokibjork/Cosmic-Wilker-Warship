using UnityEngine;

public class BackgroundObject : MonoBehaviour
{
    [Header("Movimento")]
    [Tooltip("Velocidade mínima e máxima para dar variedade.")]
    public Vector2 speedRange = new Vector2(2f, 5f);

    [Tooltip("Se deve girar aleatoriamente enquanto se move.")]
    public bool randomRotation = true;
    public float rotationSpeed = 50f;

    [Header("Limpeza")]
    [Tooltip("Tempo em segundos até o objeto se destruir.")]
    public float lifetime = 10f;

    private float _actualSpeed;
    private float _actualRotationDirection;

    void Start()
    {
        // Escolhe uma velocidade aleatória dentro do range definido
        _actualSpeed = Random.Range(speedRange.x, speedRange.y);

        // Define uma direção de rotação aleatória (horário ou anti-horário)
        _actualRotationDirection = Random.value > 0.5f ? 1f : -1f;

        // Agenda a destruição do objeto para evitar vazamento de memória
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move o objeto para a esquerda (eixo X negativo)
        // Usamos Vector3.left que é o mesmo que new Vector3(-1, 0, 0)
        transform.Translate(Vector3.left * _actualSpeed * Time.deltaTime, Space.World);

        if (randomRotation)
        {
            // Gira o objeto no eixo Z
            transform.Rotate(Vector3.forward, rotationSpeed * _actualRotationDirection * Time.deltaTime);
        }
    }
}