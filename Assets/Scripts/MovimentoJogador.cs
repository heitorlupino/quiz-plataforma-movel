using UnityEngine;

// Movimentacao basica do personagem 2D: andar para os lados e pular.
// Usa o Rigidbody2D (fisica da Unity) e os eixos do Input Manager.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class MovimentoJogador : MonoBehaviour
{
    public float velocidade = 6f;
    public float forcaPulo = 14f;
    public float limiteQueda = -10f; // se cair abaixo disso, volta para o inicio

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private float inputHorizontal;
    private bool querPular;
    private Vector3 posicaoInicial;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        rb.freezeRotation = true; // nao deixa o personagem tombar
        posicaoInicial = transform.position;
    }

    void Update()
    {
        // A/D ou setas (eixo "Horizontal" do Input Manager)
        inputHorizontal = Input.GetAxisRaw("Horizontal");

        // Espaco (botao "Jump" do Input Manager) ou W / seta para cima
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            querPular = true;
        }

        // caiu no buraco: volta para o comeco
        if (transform.position.y < limiteQueda)
        {
            transform.position = posicaoInicial;
            rb.linearVelocity = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        Collider2D chao = ChaoAbaixo();

        // se estiver em cima de algo que se move (a plataforma), anda junto com ele
        float velocidadeDoChao = 0f;
        if (chao != null && chao.attachedRigidbody != null)
        {
            velocidadeDoChao = chao.attachedRigidbody.linearVelocity.x;
        }

        // movimento horizontal: muda so o X e deixa a gravidade cuidar do Y
        rb.linearVelocity = new Vector2(inputHorizontal * velocidade + velocidadeDoChao, rb.linearVelocity.y);

        // so pula se estiver encostando no chao
        if (querPular && chao != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
        }
        querPular = false;

        // vira o sprite para o lado que esta andando
        if (inputHorizontal != 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = Mathf.Abs(escala.x) * Mathf.Sign(inputHorizontal);
            transform.localScale = escala;
        }
    }

    // devolve o collider que esta logo abaixo dos pes (ou null se estiver no ar)
    Collider2D ChaoAbaixo()
    {
        Bounds b = col.bounds;
        Vector2 centro = new Vector2(b.center.x, b.min.y - 0.05f);
        Vector2 tamanho = new Vector2(b.size.x * 0.9f, 0.1f);

        Collider2D[] colisores = Physics2D.OverlapBoxAll(centro, tamanho, 0f);
        foreach (Collider2D c in colisores)
        {
            if (c != col) return c;
        }
        return null;
    }
}
