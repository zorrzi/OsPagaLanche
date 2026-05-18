using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnPlayButton()
    {
        Debug.Log("Indo para tela de sele��o de personagem");
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

    public void OnLeaderboardBackButton()
    {
        Debug.Log("Voltando do Leaderboard para o Menu Principal");
        if (SceneFader.Instance != null)
            SceneFader.Instance.LoadSceneWithFade("MainMenu");
        else
            SceneManager.LoadScene("MainMenu");
    }

    public void OnQuitButton()
    {
        Debug.Log("Saindo do jogo");
        Application.Quit();
    }
}