using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
  public void PlayGame()
  {
    // loads first scene 
    SceneManager.LoadSceneAsync("AquariumScene");
  }

  public void QuitGame()
  {
    // closes the game down completely 
    Application.Quit();
  }
}
