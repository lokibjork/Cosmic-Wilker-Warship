using UnityEngine;

public class BackgroundInfinito : MonoBehaviour
{
    public float velocidade = 0.5f;
    public Vector2 direcao = new Vector2(1, 0); // (1,0) move para a Direita, (-1,0) para a Esquerda

    void Update()
    {
        // Calcula o deslocamento total
        float movimento = Time.time * velocidade;

        // Multiplica o valor do movimento pela direção escolhida
        GetComponent<Renderer>().material.mainTextureOffset = movimento * direcao;
    }
}