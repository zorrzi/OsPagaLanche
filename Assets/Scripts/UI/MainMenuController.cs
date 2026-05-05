using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnPlayButton()
    {
        Debug.Log("Indo para tela de seleção de personagem");
        if (SceneFader.Instance != null)
            SceneFader.Instance.LoadSceneWithFade("CharacterSelect");
        else
            SceneManager.LoadScene("CharacterSelect");
    }

    public void OnLeaderboardButton()
    {
        Debug.Log("Indo para Leaderboard");
        if (SceneFader.Instance != null)
            SceneFader.Instance.LoadSceneWithFade("Leaderboard");
        else
            SceneManager.LoadScene("Leaderboard");
    }

    public void OnQuitButton()
    {
        Debug.Log("Saindo do jogo");
        Application.Quit();
    }
}