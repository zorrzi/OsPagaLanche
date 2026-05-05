using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Referências")]
    public Button button;
    public Image portrait;

    [Header("Configuração de scale")]
    public float hoverScale = 1.05f;
    public float selectedScale = 1.25f;
    public float scaleSpeed = 8f;

    [Header("Configuração visual de selected")]
    public Color normalColor = Color.white;
    public Color selectedColor = new Color(1f, 0.92f, 0.6f, 1f); // dourado sutil
    public float pulseSpeed = 3f;
    public float pulseMinAlpha = 0.85f;
    public float pulseMaxAlpha = 1f;

    private bool isSelected = false;
    private bool isHovered = false;
    private bool isAvailable = true;
    private Vector3 originalScale;
    private Vector3 targetScale;

    void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        // Anima scale suavemente
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);

        // Pulsação quando selecionado
        if (isSelected && portrait != null)
        {
            float pulseAlpha = Mathf.Lerp(pulseMinAlpha, pulseMaxAlpha, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
            Color c = selectedColor;
            c.a = pulseAlpha;
            portrait.color = c;
        }
    }

    public void SetData(CharacterData data, Sprite portraitSprite)
    {
        isAvailable = data.isAvailable;

        if (portrait != null && portraitSprite != null)
            portrait.sprite = portraitSprite;

        button.interactable = isAvailable;

        if (!isAvailable && portrait != null)
        {
            portrait.color = new Color(0.5f, 0.5f, 0.5f, 0.6f);
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        // Reseta a cor quando deselecionado
        if (!selected && isAvailable && portrait != null)
        {
            portrait.color = normalColor;
        }

        UpdateScale();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isAvailable) return;
        isHovered = true;
        UpdateScale();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateScale();
    }

    void UpdateScale()
    {
        if (isSelected)
            targetScale = originalScale * selectedScale;
        else if (isHovered)
            targetScale = originalScale * hoverScale;
        else
            targetScale = originalScale;
    }
}