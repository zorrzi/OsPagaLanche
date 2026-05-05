using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public int keys = 0;
    public GameObject keyIconPrefab;
    public Transform keysContainer;

    private List<GameObject> keyIcons = new List<GameObject>();

    public void AddKey()
    {
        keys++;
        Debug.Log("Chave coletada! Total: " + keys);
        AddKeyIcon();
    }

    public bool UseKey()
    {
        if (keys > 0)
        {
            keys--;
            Debug.Log("Chave usada! Restantes: " + keys);
            RemoveKeyIcon();
            return true;
        }
        Debug.Log("Sem chaves!");
        return false;
    }

    void AddKeyIcon()
    {
        GameObject icon = Instantiate(keyIconPrefab, keysContainer);
        keyIcons.Add(icon);
    }

    void RemoveKeyIcon()
    {
        if (keyIcons.Count > 0)
        {
            GameObject icon = keyIcons[keyIcons.Count - 1];
            keyIcons.RemoveAt(keyIcons.Count - 1);
            Destroy(icon);
        }
    }
}