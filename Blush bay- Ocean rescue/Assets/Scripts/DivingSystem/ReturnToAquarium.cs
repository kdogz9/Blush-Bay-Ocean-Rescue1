using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnToAquarium : MonoBehaviour
{
    [Header("Scene Name")]
    [SerializeField] private string aquariumSceneName = "AquariumScene";

    [Header("Button")]
    [SerializeField] private Button returnButton;

    private void Awake()
    {
        if (returnButton == null)
        {
            returnButton = GetComponent<Button>();
        }
    }

    private void OnEnable()
    {
        // When the button is clicked, return to the aquarium.
        if (returnButton != null)
        {
            returnButton.onClick.RemoveListener(ReturnToAquariumScene);
            returnButton.onClick.AddListener(ReturnToAquariumScene);
        }
    }

    private void OnDisable()
    {
        // Clean up the button listener.
        if (returnButton != null)
        {
            returnButton.onClick.RemoveListener(ReturnToAquariumScene);
        }
    }

    public void ReturnToAquariumScene()
    {
        Debug.Log("Returning to aquarium: " + aquariumSceneName);

        // Load the aquarium scene.
        SceneManager.LoadScene(aquariumSceneName);
    }
}