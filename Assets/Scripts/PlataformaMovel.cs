using System.Collections;
using UnityEngine;

// Plataforma que fica parada ate o jogador encostar nela.
// Depois do contato ela anda para a direita por 5 segundos, para e cai com a gravidade.
// Quando sai da tela por baixo, reaparece no lado do buraco em que o jogador esta,
// e anda na direcao contraria, para o jogador poder voltar.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlataformaMovel : MonoBehaviour
{
    public float velocidade = 1.8f;              // velocidade constante do movimento
    public Vector2 direcao = Vector2.right;      // direcao do movimento
    public float duracaoMovimento = 5f;          // tempo andando, em segundos
    public float gravidadeQueda = 2f;            // gravidade usada na hora de cair
    public float alturaParaVoltar = -12f;        // abaixo dessa altura ela reaparece

    private Rigidbody2D rb;
    private bool foiAtivada = false;             // garante que so ativa uma vez por ciclo
    private Vector3 posicaoInicial;              // lado esquerdo (posicao da cena)
    private Vector3 posicaoFinal;                // lado direito (onde ela para depois dos 5 s)
    private Vector2 direcaoAtual;                // direcao do proximo movimento
    private Transform jogador;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        posicaoInicial = transform.position;
        posicaoFinal = posicaoInicial + (Vector3)(direcao.normalized * velocidade * duracaoMovimento);

        GameObject jogadorGO = GameObject.FindWithTag("Player");
        if (jogadorGO != null) jogador = jogadorGO.transform;

        Resetar();
    }

    // deixa a plataforma parada esperando o jogador, no lado do buraco em que ele esta
    void Resetar()
    {
        Vector3 posicao = posicaoInicial;
        direcaoAtual = direcao.normalized;

        // se o jogador esta do outro lado, a plataforma aparece la e volta no sentido contrario
        float meioDoCaminho = (posicaoInicial.x + posicaoFinal.x) / 2f;
        bool jogadorDoOutroLado = jogador != null && (jogador.position.x - meioDoCaminho) * direcao.x > 0f;
        if (jogadorDoOutroLado)
        {
            posicao = posicaoFinal;
            direcaoAtual = -direcao.normalized;
        }

        // Kinematic: a plataforma nao sofre gravidade e nao e empurrada pelo jogador
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.position = posicao;
        transform.position = posicao;
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
        rb.linearVelocity = direcaoAtual * velocidade;

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
        // depois que caiu e saiu da tela, reaparece para ser usada de novo
        if (foiAtivada && transform.position.y < alturaParaVoltar)
        {
            Resetar();
        }
    }
}
