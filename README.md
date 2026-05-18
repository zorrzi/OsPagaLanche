# Os Paga Lanche

## Visão geral do projeto

**Os Paga Lanche** é um jogo de plataforma 2D feito em Unity com foco em fases curtas, combate arcade e humor com temática de comida. Pelo que é possível observar no repositório, a proposta é escolher um personagem, atravessar as fases derrotando inimigos e chefes, sobreviver aos obstáculos do cenário e terminar a campanha no menor tempo possível para registrar o resultado no leaderboard.

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
- **Coleta de chaves e baús**, além de corações para recuperar vida.
- **Chefes temáticos** (como pizza, coxinha e food truck), reforçando a identidade cômica do projeto.
- **Leaderboard por tempo**, incentivando terminar a campanha rapidamente.

### Inspiração percebida

Com base nas cenas, scripts e nomes dos assets, o projeto mistura referências de:

- jogos clássicos de **ação/plataforma 2D**;
- progressão por **fases com chefes**;
- estética **pixel art**;
- humor interno/personagens próprios com temática de **lanches e comida**.

## Como jogar

### Objetivo do jogador

Chegue ao fim das fases com vida, elimine os chefes quando necessário, toque a bandeira de saída e tente fechar a campanha com o menor tempo possível.

### Controles identificados no projeto

> Alguns controles usam os eixos padrão do Unity. No teclado, isso normalmente significa `A/D` ou setas para os lados para andar, e `W/S` ou setas para cima/baixo ao subir escadas.

- **Mover:** eixo `Horizontal` (normalmente `A/D` ou `←/→`)
- **Subir/descer escada:** eixo `Vertical` (normalmente `W/S` ou `↑/↓`)
- **Pular:** `Espaço`
- **Ataque corpo a corpo:** `J`
- **Ataque à distância:** `K`
- **Interagir / abrir baú:** `E`
- **Menus:** mouse

### Dicas rápidas

- Derrote o boss da fase antes de tentar sair, porque a bandeira pode exigir isso.
- Guarde chaves para abrir baús.
- Corações recuperam vida, então vale explorar o mapa.
- O cronômetro continua entre fases, então errar menos também melhora sua posição no ranking.

## Estrutura observada da campanha

As cenas presentes no repositório indicam a seguinte estrutura de navegação principal:

- `MainMenu`
- `CharacterSelect`
- `Fase1`
- `Fase2`
- `Fase3`
- `Leaderboard`
- `GameOverScene`

Também existe a cena `BOSS NHEIRO` no repositório. Pelo código atual, a entrada padrão do jogo segue de `CharacterSelect` para `Fase1`.

## Tecnologias

- **Engine:** Unity `6000.3.12f1`
- **Linguagem principal:** C#
- **Pacotes observados:** TextMesh Pro, Input System, URP e pacotes 2D do ecossistema Unity

## Créditos dos assets

> **Importante:** esta seção foi montada a partir do que está versionado no repositório. Quando a origem não pôde ser confirmada pelos arquivos disponíveis, a lacuna foi mantida explicitamente, conforme solicitado na issue.

### Assets com origem identificável no repositório

| Categoria | Caminho / uso no projeto | Fonte | Autor / publicador | Licença |
| --- | --- | --- | --- | --- |
| Props, vegetação, plataformas, baús, escadas e elementos visuais de cenário | `Assets/Cainos/Pixel Art Platformer - Village Props/` e derivados usados em partes de `Assets/Art/Objects/Swamp/` | Unity Asset Store — pacote **Pixel Art Platformer - Village Props** (`productId: 166114`, `packageVersion: 2.3.1`) | **Cainos** | **Unity Asset Store EULA** / `licenseType: Store` |
| Editor auxiliar incluído junto ao pacote da Cainos | `Assets/Cainos/Third Party/Lucid Editor/` | GitHub — **Lucid Editor** | **Annulus Games** | **MIT** |
| Tiles, fundos de caverna, objetos e sprites de monstro usados na ambientação de caverna | `Assets/Art/Objects/Cave/Cave Platformer Tileset/`, `Assets/Art/Backgrounds/Cave/`, `Assets/Art/Tilesets/Cave/` | Unity Asset Store — pacote **2D Platform Tile Set - Cave** *(nos metadados locais aparece como `2D Platfrom Tile Set - Cave`)* (`productId: 61672`, `packageVersion: 1.2`) | **[lacuna: autor/publicador não aparece nos arquivos versionados]** | **Unity Asset Store EULA** / `licenseType: Store` |
| UI / texto | `Assets/TextMesh Pro/` | Pacote oficial do ecossistema Unity (TextMesh Pro) | **Unity Technologies** | Licença padrão dos pacotes Unity / distribuição junto ao editor |

### Assets com autoria provável do projeto, mas sem crédito explícito no repositório

Os grupos abaixo **não trazem origem externa identificável nos arquivos versionados**. Pelo contexto, nomes dos arquivos e ausência de `AssetOrigin`, eles **podem ser autorais do projeto** ou derivados de fontes externas ainda não documentadas.

| Categoria | Caminho / exemplos | Fonte | Autor | Licença |
| --- | --- | --- | --- | --- |
| Personagens jogáveis, retratos e animações | `Assets/Characters/Felipe/`, `Assets/Characters/Leonardo/`, `Assets/Characters/Marinheiro/`, `Assets/Characters/Zorzi/` | **[lacuna]** | **[aparentam ser assets do projeto / confirmar autoria]** | **[lacuna]** |
| Vozes dos personagens na seleção | `Assets/Audio/Voices/Felipe/`, `Assets/Audio/Voices/Leo/`, `Assets/Audio/Voices/Marinheiro/`, `Assets/Audio/Voices/Zorzi/` | **[lacuna]** | **[provavelmente gravações próprias / confirmar responsáveis]** | **[lacuna]** |
| Inimigos, bosses e parte dos fundos específicos de tela | `Assets/Art/Enemys/`, `Assets/Art/Backgrounds/MainMenu/`, `CharacterSelect/`, `GameEnd/`, `Leaderboard/`, `Fase2/` | **[lacuna]** | **[lacuna]** | **[lacuna]** |
| Itens de comida e corações | `Assets/Art/Food/`, `Assets/Art/Hearts/` | **[lacuna]** | **[lacuna]** | **[lacuna]** |
| Cursores e ícones de interface | `Assets/Icons/IconPack/` | **[lacuna]** | **[lacuna]** | **[lacuna]** |
| Músicas de menu / seleção | `Assets/Audio/Music/main-menu-sound.mp3`, `Assets/Audio/Music/character-select-sound.mp3` | **[lacuna]** | **[lacuna]** | **[lacuna]** |
| Efeitos sonoros de gameplay e UI | `Assets/Audio/SFX/` e `Assets/Audio/UI/` | **[lacuna]** | **[lacuna]** | **[lacuna]** |

### Scripts do projeto

Os scripts em `Assets/Scripts/` e `Assets/Characters/Marinheiro/Scripts/` aparentam ser desenvolvimento do próprio projeto para movimentação, combate, gerenciamento de cenas, timer e integração com leaderboard.

- **Fonte:** repositório `zorrzi/OsPagaLanche`
- **Autoria:** equipe/autores do projeto **[lacuna: nomes individuais não informados no repositório]**
- **Licença:** ver seção de licença deste README

## Licença

Até o momento, **não há um arquivo de licença na raiz do repositório** informando a licença geral do código e dos assets próprios.

Enquanto isso:

- o código e os assets autorais do projeto estão com **licença geral não especificada**;
- assets de terceiros continuam sujeitos às **licenças originais de seus respectivos autores/fontes**;
- os pacotes marcados como `licenseType: Store` devem respeitar os **termos da Unity Asset Store**;
- o conteúdo de `Lucid Editor` mantém a licença **MIT** indicada no próprio pacote.

## Pendências recomendadas

Para este README ficar 100% completo do ponto de vista de atribuição, seria ideal complementar:

1. autoria/licença das músicas e efeitos sonoros;
2. autoria dos sprites dos personagens, retratos, bosses e telas de menu;
3. origem do `IconPack` de cursores;
4. confirmação do publicador do pacote `2D Platfrom Tile Set - Cave` na página original da Asset Store;
5. inclusão de um `LICENSE` na raiz do repositório para o código e assets próprios.
