using UnityEngine;

public class TouchControlsBridge : MonoBehaviour
{
    private PlayerMovement player;

    void Start()
    {
        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go != null)
            player = go.GetComponent<PlayerMovement>();
    }

    private bool EnsurePlayer()
    {
        if (player == null) FindPlayer();
        return player != null;
    }

    public void SetHorizontal(float value)
    {
        if (EnsurePlayer()) player.SetHorizontal(value);
    }

    public void SetVertical(float value)
    {
        if (EnsurePlayer()) player.SetVertical(value);
    }

    public void Jump()
    {
        if (EnsurePlayer()) player.PressJump();
    }

    public void Melee()
    {
        if (EnsurePlayer()) player.PressMelee();
    }

    public void Ranged()
    {
        if (EnsurePlayer()) player.PressRanged();
    }
    public void Interact()
    {
        ChestInteraction[] chests = Object.FindObjectsByType<ChestInteraction>(FindObjectsSortMode.None);
        foreach (ChestInteraction c in chests)
            c.PressInteract();
    }

}