using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    // Método para reiniciar a partida (voltar para a Fase 1)
    public void RestartGame()
    {
        Debug.Log("Botão Reiniciar clicado!");
        
        // Certifique-se de que "Fase1" é o nome exato da cena no Build Settings
        SceneManager.LoadScene("Fase1");
    }

    // Método para voltar ao menu principal
    public void GoToMainMenu()
    {
        Debug.Log("Botão Voltar ao Menu clicado!");
        
        // Certifique-se de que "MainMenu" é o nome exato da cena no Build Settings
        SceneManager.LoadScene("MainMenu");
    }
}