using UnityEngine;

[System.Serializable]
public class CharacterData
{
    [Header("Identidade")]
    public string characterName;
    public Sprite portrait;
    public RuntimeAnimatorController animatorController;
    public bool isAvailable = true;

    [Header("Vozes (tocadas aleatoriamente ao selecionar)")]
    public AudioClip[] selectionVoices;
}