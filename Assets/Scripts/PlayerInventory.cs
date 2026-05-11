using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public int keys = 0;
    public GameObject keyIconPrefab;
    public Transform keysContainer;

    private List<GameObject> keyIcons = new List<GameObject>();

    void Start()
    {
        if (keysContainer == null)
        {
            GameObject foundContainer = GameObject.Find("KeysContainer");
            if (foundContainer != null)
            {
                keysContainer = foundContainer.transform;
                Debug.Log("KeysContainer encontrado dinamicamente");
            }
            else
            {
                Debug.LogWarning("KeysContainer não encontrado na cena!");
            }
        }
    }

    public void AddKey()
    {
        keys++;
        Debug.Log("Chave coletada! Total: " + keys);

        SFXManager.Instance?.Play("key_pickup");

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
        if (keyIconPrefab == null)
        {
            Debug.LogWarning("keyIconPrefab é NULL!");
            return;
        }
        if (keysContainer == null)
        {
            Debug.LogWarning("keysContainer é NULL na hora de adicionar ícone!");
            return;
        }

        GameObject icon = Instantiate(keyIconPrefab, keysContainer);
        icon.transform.localScale = Vector3.one;
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