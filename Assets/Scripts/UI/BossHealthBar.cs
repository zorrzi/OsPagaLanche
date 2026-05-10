using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealthBar : MonoBehaviour
{
    [Header("Referências")]
    public EnemyHealth bossHealth;
    public Image fillImage;
    public CanvasGroup canvasGroup;

    [Header("Configuração")]
    public float fillSpeed = 4f;
    public float fadeSpeed = 3f;

    [Header("Cores por estágio de vida")]
    public Color colorHigh = new Color(0.9f, 0.2f, 0.2f);    // vermelho normal (>50%)
    public Color colorMid = new Color(1f, 0.6f, 0.2f);       // laranja (50% - 25%)
    public Color colorLow = new Color(0.7f, 0f, 0f);         // vermelho escuro/sangue (<25%)

    private float targetFill = 1f;
    private bool barShown = false;

    void Start()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Começa invisível
        canvasGroup.alpha = 0f;

        if (bossHealth == null)
        {
            Debug.LogWarning("BossHealthBar: bossHealth não atribuído!");
            return;
        }

        // Escuta eventos do boss
        bossHealth.OnDamaged += UpdateBar;
        bossHealth.OnDeath += HideBar;

        // Define cor inicial
        if (fillImage != null)
            fillImage.color = colorHigh;

        targetFill = 1f;
    }

    void OnDestroy()
    {
        if (bossHealth != null)
        {
            bossHealth.OnDamaged -= UpdateBar;
            bossHealth.OnDeath -= HideBar;
        }
    }

    void Update()
    {
        // Anima o preenchimento suavemente
        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFill, Time.deltaTime * fillSpeed);
        }

        // Fade in suave depois que a barra apareceu
        if (barShown && canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1f, Time.deltaTime * fadeSpeed);
        }
    }

    void UpdateBar(int currentHP, int maxHP)
    {
        targetFill = (float)currentHP / maxHP;

        // Atualiza cor baseado na vida
        if (fillImage != null)
        {
            if (targetFill <= 0.25f)
                fillImage.color = colorLow;
            else if (targetFill <= 0.5f)
                fillImage.color = colorMid;
            else
                fillImage.color = colorHigh;
        }

        // Mostra a barra na primeira vez que toma dano
        if (!barShown)
        {
            barShown = true;
            canvasGroup.alpha = 0.01f; // dispara o fade in
        }
    }

    void HideBar()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        // Espera um pouco pra dar tempo do som de death e animação
        yield return new WaitForSeconds(0.5f);

        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);

    }
}