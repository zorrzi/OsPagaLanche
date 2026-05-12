using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3;
    public int currentLives;

    [Header("UI - pode deixar vazio que ele busca dinamicamente")]
    public Image heart1;
    public Image heart2;
    public Image heart3;

    [Header("Sprites dos corações")]
    public Sprite heartFull;
    public Sprite heartEmpty;

    [Header("Morte e Respawn")]
    [Tooltip("Y mínimo da fase. Abaixo disso, player morre (caiu do mapa)")]
    public float fallDeathY = -20f;
    [Tooltip("Tempo até reiniciar a cena depois de morrer")]
    public float respawnDelay = 2f;

    private Animator anim1;
    private Animator anim2;
    private Animator anim3;
    private bool isDead = false;

    void Start()
    {
        if (heart1 == null) heart1 = FindHeart("Heart1");
        if (heart2 == null) heart2 = FindHeart("Heart2");
        if (heart3 == null) heart3 = FindHeart("Heart3");

        if (heart1 == null || heart2 == null || heart3 == null)
        {
            Debug.LogWarning("Algum coração da UI não foi encontrado!");
            return;
        }

        anim1 = heart1.GetComponent<Animator>();
        anim2 = heart2.GetComponent<Animator>();
        anim3 = heart3.GetComponent<Animator>();

        currentLives = maxLives;
        UpdateHeartsUI();
    }

    Image FindHeart(string heartName)
    {
        GameObject heartObj = GameObject.Find(heartName);
        if (heartObj != null)
            return heartObj.GetComponent<Image>();
        return null;
    }

    void Update()
    {
        if (isDead) return;

        // Cheat de teste
        if (Input.GetKeyDown(KeyCode.H))
            TakeDamage();

        // Detecta queda do mapa
        if (transform.position.y < fallDeathY)
        {
            Debug.Log("Caiu do mapa!");
            Die();
        }
    }

    public void TakeDamage(int amount = 1)
    {
        if (isDead) return;
        if (currentLives <= 0) return;

        currentLives -= amount;
        if (currentLives < 0) currentLives = 0;

        Debug.Log("Vida perdida! Vidas: " + currentLives);

        if (currentLives > 0)
            SFXManager.Instance?.Play("damage_taken");

        UpdateHeartsUI();

        if (PostFXEvents.Instance != null)
        {
            PostFXEvents.Instance.DamageFlash();
            PostFXEvents.Instance.SetLowHP(currentLives > 0 && currentLives <= 1);
        }

        if (currentLives <= 0)
        {
            Die();
        }
    }

    public void AddLife()
    {
        if (isDead) return;
        if (currentLives >= maxLives) return;

        currentLives++;
        Debug.Log("Vida recuperada! Vidas: " + currentLives);

        SFXManager.Instance?.Play("heart_pickup");

        UpdateHeartsUI();

        if (PostFXEvents.Instance != null)
            PostFXEvents.Instance.SetLowHP(currentLives <= 1);
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Game Over!");

        SFXManager.Instance?.Play("game_over");

        // Para o player de se mexer
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Salva o tempo atual + vidas e chaves NO ZERO (vai resetar ao respawnar)
        if (GameData.Instance != null && LevelTimer.Instance != null)
        {
            // Salva tempo apenas se não for Fase1 (Fase1 reinicia tudo)
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (currentScene != "Fase1")
            {
                GameData.Instance.SaveStateBeforeLevelChange(3, 0, LevelTimer.Instance.CurrentTime);
            }
            else
            {
                // Na Fase1 reseta tudo
                GameData.Instance.accumulatedTime = 0f;
                GameData.Instance.currentLives = 3;
                GameData.Instance.currentKeys = 0;
            }
        }

        StartCoroutine(RestartCurrentSceneAfterDelay());
    }

    IEnumerator RestartCurrentSceneAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);

        // Recarrega a cena ATUAL (não volta pra Fase1)
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (SceneFader.Instance != null)
            SceneFader.Instance.LoadSceneWithFade(currentScene);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentScene);
    }

    void UpdateHeartsUI()
    {
        if (heart1 == null || heart2 == null || heart3 == null) return;

        float syncTime = GetActiveHeartNormalizedTime();
        UpdateHeart(heart1, anim1, currentLives >= 1, syncTime);
        UpdateHeart(heart2, anim2, currentLives >= 2, syncTime);
        UpdateHeart(heart3, anim3, currentLives >= 3, syncTime);
    }

    float GetActiveHeartNormalizedTime()
    {
        if (anim1 != null && anim1.enabled) return anim1.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
        if (anim2 != null && anim2.enabled) return anim2.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
        if (anim3 != null && anim3.enabled) return anim3.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
        return 0f;
    }

    void UpdateHeart(Image heart, Animator anim, bool isFull, float syncTime)
    {
        if (heart == null) return;

        if (isFull)
        {
            if (anim != null)
            {
                bool wasDisabled = !anim.enabled;
                anim.enabled = true;
                heart.sprite = heartFull;

                if (wasDisabled)
                {
                    var state = anim.GetCurrentAnimatorStateInfo(0);
                    anim.Play(state.fullPathHash, 0, syncTime);
                }
            }
            else
            {
                heart.sprite = heartFull;
            }
        }
        else
        {
            if (anim != null) anim.enabled = false;
            heart.sprite = heartEmpty;
        }
    }
}