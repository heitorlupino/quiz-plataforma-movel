using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Monta a cena de teste (chao com um buraco, plataforma movel, camera e personagem).
// Roda sozinho na primeira vez que o projeto abre, ou pelo menu Quiz > Montar Cena.
[InitializeOnLoad]
public static class MontarCena
{
    const string CaminhoCena = "Assets/Scenes/PlataformaMovel.unity";

    static MontarCena()
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (!System.IO.File.Exists(CaminhoCena)) Montar();
        };
    }

    [MenuItem("Quiz/Montar Cena")]
    public static void Montar()
    {
        var cena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        Sprite grama = Carregar("grama");
        Sprite terra = Carregar("terra");
        Sprite plataforma = Carregar("plataforma");
        Sprite jogador = Carregar("jogador_parado");

        // camera
        var camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 8f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.45f, 0.75f, 0.95f);
        camGO.transform.position = new Vector3(1.5f, 2f, -10f);

        // chao do lado esquerdo (x de -15 ate -4)
        Bloco("Chao Esquerdo (terra)", terra, new Vector2(-9.5f, -3f), new Vector2(11f, 3f), 0, true);
        Bloco("Chao Esquerdo (grama)", grama, new Vector2(-9.5f, -1f), new Vector2(11f, 1f), 0, true);

        // buraco entre x = -4 e x = 9

        // chao do lado direito (x de 9 ate 18)
        Bloco("Chao Direito (terra)", terra, new Vector2(13.5f, -3f), new Vector2(9f, 3f), 0, true);
        Bloco("Chao Direito (grama)", grama, new Vector2(13.5f, -1f), new Vector2(9f, 1f), 0, true);

        // plataforma movel no comeco do buraco
        var plataformaGO = new GameObject("Plataforma Movel");
        plataformaGO.transform.position = new Vector3(-2f, 0f, 0f);
        var srPlat = plataformaGO.AddComponent<SpriteRenderer>();
        srPlat.sprite = plataforma;
        srPlat.drawMode = SpriteDrawMode.Tiled;
        srPlat.size = new Vector2(3f, 1f);
        var rbPlat = plataformaGO.AddComponent<Rigidbody2D>();
        rbPlat.bodyType = RigidbodyType2D.Kinematic; // parada e sem gravidade no inicio
        rbPlat.gravityScale = 0f;
        rbPlat.interpolation = RigidbodyInterpolation2D.Interpolate;
        plataformaGO.AddComponent<BoxCollider2D>().size = new Vector2(3f, 1f);
        plataformaGO.AddComponent<PlataformaMovel>();

        // paredes invisiveis nas pontas para o personagem nao cair
        Parede("Parede Esquerda", -15.5f);
        Parede("Parede Direita", 18.5f);

        // personagem
        var player = new GameObject("Jogador");
        player.tag = "Player"; // a plataforma usa essa tag para saber que foi o jogador
        player.transform.position = new Vector3(-10f, 0.5f, 0f);
        var sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = jogador;
        sr.sortingOrder = 5;
        var rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        var col = player.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.8f, 1f);
        col.sharedMaterial = SemAtrito();
        var mov = player.AddComponent<MovimentoJogador>();
        mov.velocidade = 6f;
        mov.forcaPulo = 14f;

        System.IO.Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(cena, CaminhoCena);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(CaminhoCena, true) };
        Debug.Log("Cena montada em " + CaminhoCena + ". Aperte Play para testar.");
    }

    static Sprite Carregar(string nome)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/" + nome + ".png");
    }

    static void Bloco(string nome, Sprite sprite, Vector2 pos, Vector2 tamanho, int ordem, bool colisao)
    {
        var go = new GameObject(nome);
        go.transform.position = pos;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = ordem;
        sr.drawMode = SpriteDrawMode.Tiled; // repete o sprite de 16x16 no tamanho do bloco
        sr.size = tamanho;
        if (colisao)
        {
            var box = go.AddComponent<BoxCollider2D>();
            box.size = tamanho;
        }
    }

    static void Parede(string nome, float x)
    {
        var go = new GameObject(nome);
        go.transform.position = new Vector3(x, 4f, 0f);
        go.AddComponent<BoxCollider2D>().size = new Vector2(1f, 20f);
    }

    // material sem atrito: evita o personagem ficar "grudado" na lateral das plataformas
    static PhysicsMaterial2D SemAtrito()
    {
        const string caminho = "Assets/SemAtrito.physicsMaterial2D";
        var mat = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(caminho);
        if (mat == null)
        {
            mat = new PhysicsMaterial2D("SemAtrito") { friction = 0f, bounciness = 0f };
            AssetDatabase.CreateAsset(mat, caminho);
        }
        return mat;
    }
}
