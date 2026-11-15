using System.Collections;
using UnityEngine;

// Plataforma que fica parada ate o jogador encostar nela.
// Depois do contato ela anda para a direita por 5 segundos, para e cai com a gravidade.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlataformaMovel : MonoBehaviour
{
    public float velocidade = 1.8f;              // velocidade constante do movimento
    public Vector2 direcao = Vector2.right;      // direcao do movimento
    public float duracaoMovimento = 5f;          // tempo andando, em segundos
    public float gravidadeQueda = 2f;            // gravidade usada na hora de cair

    private Rigidbody2D rb;
    private bool foiAtivada = false;             // garante que so ativa uma vez

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Kinematic: a plataforma nao sofre gravidade e nao e empurrada pelo jogador
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    void OnCollisionEnter2D(Collision2D colisao)
    {
        // so reage ao jogador, e so na primeira vez
        if (foiAtivada) return;
        if (!colisao.gameObject.CompareTag("Player")) return;

        foiAtivada = true;
        StartCoroutine(MoverEDepoisCair());
    }

    IEnumerator MoverEDepoisCair()
    {
        // 1) anda em linha reta com velocidade constante
        rb.linearVelocity = direcao.normalized * velocidade;

        // 2) espera exatamente o tempo definido (5 segundos)
        yield return new WaitForSeconds(duracaoMovimento);

        // 3) para o movimento horizontal
        rb.linearVelocity = Vector2.zero;

        // 4) vira Dynamic e passa a cair com a gravidade
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = gravidadeQueda;
        rb.freezeRotation = true;
    }

    void Update()
    {
        // depois que caiu bem para baixo da tela, remove a plataforma
        if (transform.position.y < -20f)
        {
            Destroy(gameObject);
        }
    }
}
