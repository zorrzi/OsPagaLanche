# Os Paga Lanche

## Visão geral do projeto

**Os Paga Lanche** é um jogo de plataforma 2D feito em Unity com foco em fases curtas, combate arcade e humor com temática de comida. A proposta do jogo é escolher um personagem, atravessar as fases derrotando inimigos e chefes, sobreviver aos obstáculos do cenário e terminar a campanha no menor tempo possível para registrar o resultado no leaderboard.

O fluxo principal do jogo é:

1. abrir o menu principal;
2. escolher um personagem e informar o nome do jogador;
3. iniciar em `Fase1`;
4. avançar pelas fases, acumulando o tempo total;
5. derrotar o boss da fase para liberar a bandeira de saída;
6. concluir a última fase para enviar a run ao ranking.

### Mecânicas e proposta

- **Plataforma 2D** com corrida, pulo, escada e perigos no cenário.
- **Combate corpo a corpo e à distância**, com projétil de hambúrguer.
- **Progressão por fases**, com tempo acumulado entre cenas.
- **Coleta de chaves, baús e corações**, ajudando na progressão e na recuperação de vida.
- **Lanches finitos saindo dos baús**, adicionando recursos extras durante a partida.
- **Chefes temáticos** (como pizza, coxinha e food truck), reforçando a identidade cômica do projeto.
- **Leaderboard por tempo**, incentivando terminar a campanha rapidamente.

### Inspiração do projeto

O projeto mistura referências de:

- jogos clássicos de **ação/plataforma 2D**;
- progressão por **fases com chefes**;
- estética **pixel art**;
- humor interno e personagens com temática de **lanches e comida**.

## Como jogar

### Objetivo do jogador

Chegue ao fim das fases com vida, elimine os chefes quando necessário, toque a bandeira de saída e tente fechar a campanha com o menor tempo possível.

### Controles

- **Mover:** eixo `Horizontal` (`A/D` ou `←/→`)
- **Subir/descer escada:** eixo `Vertical` (`W/S` ou `↑/↓`)
- **Pular:** `Espaço`
- **Ataque corpo a corpo:** `J`
- **Ataque à distância:** `K`
- **Interagir / abrir baú:** `E`
- **Menus:** mouse

### Dicas rápidas

- Derrote o boss da fase antes de tentar sair, porque a bandeira pode exigir isso.
- Guarde chaves para abrir baús.
- Corações recuperam vida, então vale explorar o mapa.
- Baús também podem render lanches úteis ao longo da fase.
- O cronômetro continua entre fases, então errar menos também melhora sua posição no ranking.

## Estrutura da campanha

As cenas principais do jogo são:

- `MainMenu`
- `CharacterSelect`
- `Fase1`
- `Fase2`
- `Fase3`
- `Leaderboard`
- `GameOverScene`

## Créditos dos assets

### Assets utilizados no projeto

| Categoria | Caminho / uso no projeto | Fonte |
| --- | --- | --- |
| Props, vegetação, plataformas, baús, escadas e elementos visuais de cenário | `Assets/Cainos/Pixel Art Platformer - Village Props/` e derivados usados em partes de `Assets/Art/Objects/Swamp/` | [🔗]([https://assetstore.unity.com/packages/2d/environments/2d-platfrom-tile-set-cave-61672](https://www.google.com/url?sa=t&rct=j&q=&esrc=s&source=web&cd=&cad=rja&uact=8&ved=2ahUKEwi1rNzLpsSUAxWRjpUCHXo0E8YQFnoECBwQAQ&url=https%3A%2F%2Fassetstore.unity.com%2Fpackages%2F2d%2Fenvironments%2Fpixel-art-platformer-village-props-166114%3Fsrsltid%3DAfmBOoqjeql2u5pOBVRLkw24irkH0mSqovIcZPmev9JdzKEt3sZw7HRK&usg=AOvVaw109y5nao9JlpF1s0KQ6nfJ&cshid=1779157867173866&opi=89978449)) |
| Tiles, fundos de caverna, objetos e sprites de monstro usados na ambientação de caverna | `Assets/Art/Objects/Cave/Cave Platformer Tileset/`, `Assets/Art/Backgrounds/Cave/`, `Assets/Art/Tilesets/Cave/` | [🔗](https://assetstore.unity.com/packages/2d/environments/2d-platfrom-tile-set-cave-61672)  |
| Vozes dos personagens na seleção | `Assets/Audio/Voices/Felipe/`, `Assets/Audio/Voices/Leo/`, `Assets/Audio/Voices/Marinheiro/`, `Assets/Audio/Voices/Zorzi/` | Auxílio de IA |
| Inimigos e bosses | `Assets/Art/Enemys/` | Auxílio de IA |
| Cursores e ícones de interface | `Assets/Icons/IconPack/` | Auxílio de IA |
| Músicas de menu / seleção | `Assets/Audio/Music/main-menu-sound.mp3`, `Assets/Audio/Music/character-select-sound.mp3` | [link da fonte](https://uppbeat.io/zones/gaming?gad_source=1&gad_campaignid=20865019414&gclid=CjwKCAjw8arQBhB9EiwAfIKdQlAogRgSmynO-9GBSAs-2NKhZEC05OX9U1xiIRenrAOmFa0dZGozIRoCfBYQAvD_BwE) |
| Efeitos sonoros de gameplay e UI | `Assets/Audio/SFX/` e `Assets/Audio/UI/` | [link da fonte] |
| Personagens jogáveis, retratos e animações | `Assets/Characters/Felipe/`, `Assets/Characters/Leonardo/`, `Assets/Characters/Marinheiro/`, `Assets/Characters/Zorzi/` | Auxílio de IA |
| Backgrounds | `Assets/Backgrounds/` | Auxílio de IA |
| Itens de comida e corações | `Assets/Art/Food/`, `Assets/Art/Hearts/` | Auxílio de IA |

### Scripts do projeto

Os scripts do jogo ficam em `Assets/Scripts`, organizados por responsabilidade:

- `Enemies/`: inimigos, bosses e projéteis.
- `Items/`: itens coletáveis específicos.
- `Managers/`: fluxo global da campanha, áudio, timer e seleção de personagem.
- `Services/`: integração com serviços externos, como leaderboard.
- `Triggers/`: gatilhos de fase, tutorial e progressão.
- `UI/`: HUD, menus e telas de interface.
- raiz de `Assets/Scripts/`: scripts centrais de gameplay, câmera, pausa, vida e inventário.
