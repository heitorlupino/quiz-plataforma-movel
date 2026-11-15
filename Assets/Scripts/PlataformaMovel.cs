using System.Collections;
using UnityEngine;

// Plataforma que fica parada ate o jogador encostar nela.
// Depois do contato ela anda para a direita por 5 segundos, para e cai com a gravidade.
// Quando sai da tela por baixo, volta para a posicao inicial e pode ser usada de novo.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlataformaMovel : MonoBehaviour
{
    public float velocidade = 1.8f;              // velocidade constante do movimento
    public Vector2 direcao = Vector2.right;      // direcao do movimento
    public float duracaoMovimento = 5f;          // tempo andando, em segundos
    public float gravidadeQueda = 2f;            // gravidade usada na hora de cair
    public float alturaParaVoltar = -12f;        // abaixo dessa altura ela volta para o inicio

    private Rigidbody2D rb;
    private bool foiAtivada = false;             // garante que so ativa uma vez por ciclo
    private Vector3 posicaoInicial;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        posicaoInicial = transform.position;
        Resetar();
    }

    // deixa a plataforma parada na posicao inicial, esperando o jogador
    void Resetar()
    {
        // Kinematic: a plataforma nao sofre gravidade e nao e empurrada pelo jogador
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.position = posicaoInicial;
        transform.position = posicaoInicial;
        foiAtivada = false; // libera para ser ativada de novo
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
        // depois que caiu e saiu da tela, volta para o lugar de origem
        if (foiAtivada && transform.position.y < alturaParaVoltar)
        {
            Resetar();
        }
    }
}
