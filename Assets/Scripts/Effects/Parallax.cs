using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos;
    public GameObject camera;
    public float parallaxEffect;

    // Limites para o parallax
    public float parallaxStartLimit; // Onde o efeito começa visualmente
    public float parallaxEndLimit;   // Onde o efeito termina

    // Buffer de pré-carregamento
    public float preloadBuffer = 10f; // Pré-carrega o parallax antes da câmera entrar no limite

    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        // Calcula os limites de pré-carregamento
        float preloadStart = parallaxStartLimit - preloadBuffer;
        float preloadEnd = parallaxEndLimit + preloadBuffer;

        // Verifica se a posição da câmera está dentro dos limites de pré-carregamento
        if (camera.transform.position.x >= preloadStart && camera.transform.position.x <= preloadEnd)
        {
            float temp = (camera.transform.position.x * (1 - parallaxEffect));
            float dist = (camera.transform.position.x * parallaxEffect);

            transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

            // Faz o loop do background, se necessário
            if (temp > startpos + length) startpos += length;
            else if (temp < startpos - length) startpos -= length;
        }
    }
}
