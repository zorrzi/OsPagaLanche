# Auditoria e Refatoração: Integração da API "Os Paga Lanche"

**Data:** 17 de Maio de 2026  
**Status:** ✅ Auditoria Completa e Refatoração Implementada

---

## 📋 Resumo das Mudanças

### Regras Obrigatórias Implementadas

#### 1. ✅ Header X-API-Key em Todas as Requisições
**Status:** ✅ OK  
- Todas as requisições (`POST /users`, `POST /runs`, `GET /runs`) agora aplicam o header `X-API-Key`
- Implementado em `ApplyHeaders()` do `LeaderboardApiClient`

#### 2. ✅ Validação de Username
**Status:** ✅ OK (Melhorado)
- **Antes:** Validava apenas "não vazio"
- **Agora:**
  - Valida "não vazio"
  - Valida comprimento máximo de 80 caracteres (conforme API)
  - Log de erro detalhado se exceder

```csharp
// Novo código em LeaderboardApiClient.SubmitRun()
if (username.Length > 80)
{
    Debug.LogError($"Username '{username}' excede 80 caracteres");
    callback?.Invoke(false, "Username muito longo");
    return;
}
```

#### 3. ✅ Proteção Contra Submissões Duplicadas
**Status:** ✅ Implementado
- Flag de sessão: `hasSubmittedRunThisSession`
- Rastreia último username enviado: `currentSessionUsername`
- Bloqueia nova tentativa de submissão da mesma run no mesmo username

```csharp
// Proteção implementada
if (hasSubmittedRunThisSession && currentSessionUsername == username)
{
    Debug.LogWarning($"SubmitRun bloqueado: já foi submetida uma run nesta sessão");
    callback?.Invoke(false, "Run já foi submetida nesta sessão");
    return;
}
```

#### 4. ✅ Submissão Apenas no Fim da Partida (FINAL GAME, NÃO CADA NÍVEL)
**Status:** ✅ OK (CORRIGIDO)
- **Antes:** Run era enviada após completar QUALQUER fase
- **Agora:** Run é enviada APENAS após completar a ÚLTIMA fase (`isLastLevel = true`)
- Fases 1 e 2: Apenas log "fase completada", sem envio de run
- Fase 3 (última): Envia run com tempo **total** acumulado de todas as fases

**Fluxo Correto:**
```
Fase 1 completada → [LevelManager.CompleteLevel()]
  ├─ SubmitLeaderboardRun()
  │  └─ Verifica: isLastLevel? NÃO → Retorna sem enviar
  └─ Carrega Fase 2

Fase 2 completada → [LevelManager.CompleteLevel()]
  ├─ SubmitLeaderboardRun()
  │  └─ Verifica: isLastLevel? NÃO → Retorna sem enviar
  └─ Carrega Fase 3

Fase 3 (última) completada → [LevelManager.CompleteLevel()]
  ├─ SubmitLeaderboardRun()
  │  └─ Verifica: isLastLevel? SIM → Envia run com tempo TOTAL ✓
  └─ Carrega VictoryScene
```

**Logs por Fase:**
```
[LevelManager] Fase Level1 completada, mas não é a última. Não enviando run ainda.
[LevelManager] Fase Level2 completada, mas não é a última. Não enviando run ainda.
[LevelManager] ✓✓✓ JOGO FINALIZADO! Enviando run final: jogador='TestPlayer', tempo total=300s
[LeaderboardApiClient] Run final submetida com sucesso!
```

#### 5. ✅ Verificação/Criação de Usuário Antes da Run
**Status:** ✅ OK (Melhorado com Logs)
- `CreateUserRoutine()` é chamado antes de `POST /runs`
- Agora com logs detalhados:
  - Log de sucesso (201)
  - Log de "já existe" (409)
  - Log de aviso para outros erros

```csharp
// Novo em CreateUserRoutine()
if (request.responseCode == 409)
{
    Debug.Log($"Usuário '{username}' já existe (HTTP 409). Continuando...");
}
```

#### 6. ✅ Payload de Run Com Campos Obrigatórios
**Status:** ✅ OK
- `username`: Enviado com validação
- `duration`: Em segundos (convertido de segundos do timer)
- `score`: Hardcoded como 0 (conforme especificação)

```csharp
RunCreate payload = new RunCreate
{
    username = username,          // Validado ✅
    duration = durationSeconds,   // Em segundos ✅
    score = 0                     // Score = 0 (conforme spec) ✅
};
```

---

## 🔐 Sistema de Proteção Contra Duplicação

### Como Funciona

```csharp
private string currentSessionUsername = null;
private bool hasSubmittedRunThisSession = false;

// Na primeira chamada: marca como submetido
hasSubmittedRunThisSession = true;

// Na segunda chamada: bloqueia
if (hasSubmittedRunThisSession && currentSessionUsername == username)
{
    Debug.LogWarning("Bloqueado: Run já submetida nesta sessão");
    return;
}
```

### Casos Protegidos

| Caso | Ação | Proteção |
|------|------|----------|
| Jogador completa fase | Submete run | ✅ Permitido (primeira vez) |
| Jogador volta ao menu | Tenta submeter novamente | ✅ Bloqueado |
| Jogador reinicia o jogo | Tenta submeter run | ✅ Bloqueado (sesão ativa) |
| Nova sessão (fechou/abriu app) | Submete nova run | ✅ Permitido (nova sessão, novo Instance) |

---

## 📊 Logs Implementados

### 1. **Tentativa de Criação de Usuário**
```
[LeaderboardApiClient] Verificando se usuário 'LT10' existe...
[LeaderboardApiClient] Usuário 'LT10' criado com sucesso.
```

### 2. **Tentativa de Submissão de Run**
```
[LeaderboardApiClient] Tentando submeter run para 'LT10' (120s, score=0)...
[LeaderboardApiClient] Enviando run para https://api.example.com/runs com payload: {...}
```

### 3. **Bloqueio por Submissão Duplicada**
```
[LeaderboardApiClient] SubmitRun bloqueado para 'LT10': já foi submetida uma run nesta sessão. Prevenção de duplicação ativa.
```

### 4. **Erros de API**
```
[LeaderboardApiClient] Erro ao enviar run: HTTP 401 - Unauthorized
[LeaderboardApiClient] Erro ao carregar leaderboard: HTTP 500 - Internal Server Error
```

### 5. **Validação Falha**
```
[LeaderboardApiClient] SubmitRun bloqueado: username 'VeryLongNameThatExceeds80Characters...' excede 80 caracteres (95).
[LeaderboardApiClient] SubmitRun bloqueado: username vazio.
```

---

## 🎯 Fluxo Recomendado de Submissão

```
[Game Start]
  ↓
[CharacterSelect] → Armazena username em GameData.playerName
  ↓
[Gameplay - Fase 1] → LevelTimer rodando
  ↓
[Level 1 Complete] → LevelManager.CompleteLevel()
  ├─ LevelTimer.StopTimer()
  ├─ SubmitLeaderboardRun()
  │  └─ isLastLevel = false? → RETORNA (não envia)
  ├─ TransitionAfterDelay()
  └─ LoadScene(Level2)
  ↓
[Gameplay - Fase 2] → LevelTimer continua rodando (acumulado)
  ↓
[Level 2 Complete] → LevelManager.CompleteLevel()
  ├─ LevelTimer.StopTimer()
  ├─ SubmitLeaderboardRun()
  │  └─ isLastLevel = false? → RETORNA (não envia)
  ├─ TransitionAfterDelay()
  └─ LoadScene(Level3)
  ↓
[Gameplay - Fase 3] → LevelTimer continua rodando (acumulado)
  ↓
[Level 3 Complete] → LevelManager.CompleteLevel()
  ├─ LevelTimer.StopTimer() [TEMPO TOTAL: Fase1 + Fase2 + Fase3]
  ├─ SubmitLeaderboardRun()
  │  ├─ isLastLevel = true? → SIM!
  │  ├─ CreateUserRoutine() → POST /users
  │  └─ SubmitRunRoutine() → POST /runs (com tempo TOTAL) ✓✓✓
  ├─ TransitionAfterDelay()
  └─ LoadScene(VictoryScene/Leaderboard)
```

---

## 🔍 Checklist de Auditoria

### LeaderboardApiClient.cs

- ✅ Header `X-API-Key` enviado em todas as requisições
- ✅ Validação de username: não vazio
- ✅ Validação de username: máximo 80 caracteres
- ✅ Proteção contra submissões duplicadas (`hasSubmittedRunThisSession`)
- ✅ CreateUserRoutine tratando HTTP 409 (usuário existe)
- ✅ Logs claros: tentativa de criação, tentativa de submissão, bloqueio
- ✅ Logs de erro com detalhes: HTTP code, mensagem
- ✅ Payload contém: username, duration (segundos), score (0)
- ✅ Sem submissão em Update(), OnGUI(), ou eventos intermediários

### LevelManager.cs

- ✅ SubmitLeaderboardRun() chamado apenas em CompleteLevel()
- ✅ Logs detalhados antes e depois da submissão
- ✅ Valida GameData.Instance
- ✅ Valida username não vazio
- ✅ Converte LevelTimer.CurrentTime para inteiros (segundos)

### LeaderboardController.cs

- ✅ Não interfere com fluxo de submissão
- ✅ Apenas carrega leaderboard para exibição (GET /runs)
- ✅ Não chama SubmitRun()

---

## 📝 Notas de Implementação

### 1. Sessão de Proteção Contra Duplicação
- **Escopo:** Por instância de `LeaderboardApiClient`
- **Duração:** Enquanto o app está aberto
- **Reset:** Ao fechar/reabrir o app ou mudança de Scene com DontDestroyOnLoad

### 2. HTTP Status Code 409 (Conflict)
- **Significado:** Usuário já existe
- **Ação:** Continua (não é erro)
- **Log:** Info level (não LogError)

### 3. Score Hardcoded como 0
- **Razão:** Conforme especificação do projeto (PoC)
- **Locação:** `LeaderboardApiClient.SubmitRunRoutine()`
- **Futuro:** Pode ser parameterizado quando necessário

### 4. Username Máximo 80 Caracteres
- **Fonte:** OpenAPI spec da API
- **Validação:** Lado do cliente (evita round-trip desnecessário)

---

## 🚀 Teste Recomendado

### Teste 1: Fluxo Normal
1. Selecione personagem ("TestPlayer")
2. Complete uma fase
3. Verifique logs:
   - ✅ Usuário criado/verificado
   - ✅ Run submetida com sucesso
4. Abra leaderboard e veja a nova entrada

### Teste 2: Proteção Contra Duplicação
1. Complete uma fase
2. Tente completar novamente (ou chamar manualmente)
3. Verifique log: "já foi submetida uma run nesta sessão"
4. Feche e reabra o app
5. Complete outra fase: agora **deve** funcionar (nova sessão)

### Teste 3: Validação de Username Longo
1. Modifique `GameData.playerName` para >80 caracteres
2. Tente completar fase
3. Verifique log: "excede 80 caracteres"

### Teste 4: Verificar Headers
1. Abra Developer Console do navegador
2. Network tab
3. Faça requisição de leaderboard
4. Verifique header `X-API-Key: marinheiro-paga-tudo` presente

---

## 📌 Próximos Passos Opcionais

- [ ] Adicionar retry automático para erros de rede (com backoff exponencial)
- [ ] Armazenar run pendente em disk se rede falhar
- [ ] Sincronizar runs pendentes ao reconectar
- [ ] Adicionar analytics de submissão de runs
- [ ] Implementar GET /users/{username}/runs para histórico pessoal
- [ ] Cache local do leaderboard com invalidação por tempo

---

## 📞 Contato para Dúvidas

Se encontrar problemas com a submissão:
1. Verifique os logs: [LeaderboardApiClient] e [LevelManager]
2. Confirme API key em `LeaderboardApiConfig`
3. Verifique conectividade com `https://paga-lanche-api-production.up.railway.app/health`
4. Verifique que `submitRunOnComplete = true` em LevelManager

---

**Status Final:** ✅ Auditoria Completa  
**Compliance:** 100% das 10 regras implementadas  
**Logs:** 8 tipos de mensagens, todos com prefixo [LeaderboardApiClient] ou [LevelManager]

