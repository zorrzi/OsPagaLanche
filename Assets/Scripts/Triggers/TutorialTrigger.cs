using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class TutorialTrigger : MonoBehaviour
{
    [Header("Imagem desse tutorial")]
    public Image tutorialImage;

    [Header("Comportamento")]
    public bool showOnlyOnce = true;
    public float hideDelayAfterExit = 1f;

    private static TutorialPopup popup;
    private bool hasBeenShown = false;
    private bool playerInside = false;
    private float hideTimer = 0f;

    void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"TutorialTrigger '{name}': marcando IsTrigger automaticamente.");
            col.isTrigger = true;
        }

        if (popup == null)
            popup = FindObjectOfType<TutorialPopup>(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (showOnlyOnce && hasBeenShown) return;

        playerInside = true;
        hideTimer = 0f;

        if (popup != null && tutorialImage != null)
        {
            popup.Show(tutorialImage);
            hasBeenShown = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
        hideTimer = hideDelayAfterExit;
    }

    void Update()
    {
        if (!playerInside && hideTimer > 0f)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0f)
            {
                if (popup != null) popup.Hide();
            }
        }
    }

    void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawCube(transform.position + (Vector3)col.offset, col.bounds.size);
    }
}