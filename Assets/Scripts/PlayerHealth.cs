using UnityEngine;
using UnityEngine.UI;

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

    private Animator anim1;
    private Animator anim2;
    private Animator anim3;

    void Start()
    {
        // Se as referências estiverem vazias, busca dinamicamente no Canvas
        if (heart1 == null) heart1 = FindHeart("Heart1");
        if (heart2 == null) heart2 = FindHeart("Heart2");
        if (heart3 == null) heart3 = FindHeart("Heart3");

        // Se algum coração ainda for null, avisa mas não quebra
        if (heart1 == null || heart2 == null || heart3 == null)
        {
            Debug.LogWarning(" Algum coração da UI não foi encontrado! Verifique os nomes (Heart1, Heart2, Heart3) no Canvas.");
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
        if (Input.GetKeyDown(KeyCode.H))
            TakeDamage();
    }

    public void TakeDamage(int amount = 1)
    {
        if (currentLives <= 0) return;
        currentLives -= amount;
        if (currentLives < 0) currentLives = 0;
        Debug.Log("Vida perdida! Vidas: " + currentLives);
        UpdateHeartsUI();

        if (PostFXEvents.Instance != null)
        {
            PostFXEvents.Instance.DamageFlash();
            PostFXEvents.Instance.SetLowHP(currentLives > 0 && currentLives <= 1);
        }

        if (currentLives <= 0)
            Debug.Log("Game Over!");
    }

    public void AddLife()
    {
        if (currentLives >= maxLives) return;
        currentLives++;
        Debug.Log("Vida recuperada! Vidas: " + currentLives);
        UpdateHeartsUI();

        if (PostFXEvents.Instance != null)
            PostFXEvents.Instance.SetLowHP(currentLives <= 1);
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