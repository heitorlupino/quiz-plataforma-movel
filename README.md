# Quiz - Plataforma que se move e cai

Atividade da disciplina de **Desenvolvimento de Jogos Digitais** (Ciência da Computação – UNISAGRADO).

Autor: **Heitor Lupino**

Projeto Unity 2D com uma plataforma que fica parada até o jogador encostar nela. A partir do contato, ela anda para a direita com velocidade constante por 5 segundos, depois para e cai com a gravidade. Quando sai da tela por baixo, a plataforma volta para a posição inicial e pode ser usada de novo.

## Controles

| Ação | Tecla |
|------|-------|
| Andar | A / D ou setas |
| Pular | Espaço, W ou seta para cima |

## Como testar

Ande até o buraco e suba na plataforma. Ela começa a andar para a direita e leva você até perto do outro lado. Pule para o chão da direita antes que ela pare e caia. Se você cair no buraco, volta para o começo. Depois que a plataforma cai, ela reaparece no lugar de origem, então dá para atravessar de novo.

## Como cada requisito foi feito

Script principal: [`Assets/Scripts/PlataformaMovel.cs`](Assets/Scripts/PlataformaMovel.cs)

| Requisito | Como foi feito |
|-----------|----------------|
| 1. Plataforma parada, sem gravidade | `BoxCollider2D` + `Rigidbody2D` como **Kinematic** e `gravityScale = 0` |
| 2. Imóvel até o jogador encostar | A velocidade começa em zero e o movimento só começa no `OnCollisionEnter2D`, verificando a tag `Player` |
| 3. Movimento em linha reta e velocidade constante | `rb.linearVelocity = direcao * velocidade` (direção: direita) |
| 4. Dura exatamente 5 segundos | Coroutine com `WaitForSeconds(duracaoMovimento)`, que vale 5 |
| 5. Para e cai | Velocidade zerada, depois o corpo vira **Dynamic** com `gravityScale` maior que zero |
| 6. Ativa só uma vez | A variável `foiAtivada` impede que o movimento recomece se o jogador encostar de novo durante o movimento ou a queda. Ela só volta a ser `false` quando a plataforma reaparece na posição inicial |

O personagem usa o mesmo script de movimentação do quiz anterior ([`MovimentoJogador.cs`](Assets/Scripts/MovimentoJogador.cs)). Ele ganhou duas coisas: acompanha a velocidade da plataforma quando está em cima dela e volta para o início se cair no buraco.

## Assets

Sprites em pixel art 16x16 (personagem, grama, terra e plataforma) do meu pack usado no jogo [A Aventura de Pip](https://github.com/heitorlupino/a-aventura-de-pip).

## Como abrir

1. Abra o projeto no Unity Hub com a **Unity 6.6** (`6000.6.9f1`).
2. Na primeira vez que o projeto abre, a cena `Assets/Scenes/PlataformaMovel.unity` é montada automaticamente pelo script `Assets/Editor/MontarCena.cs`. Se precisar montar de novo, use o menu **Quiz > Montar Cena**.
3. Aperte **Play**.
