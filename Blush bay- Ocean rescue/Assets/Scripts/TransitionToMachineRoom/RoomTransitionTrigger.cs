using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTransitionTrigger : MonoBehaviour
{
    [Header("Scene To Load")]
    [SerializeField] private string sceneToLoad = "MachineRoomScene";

    [Header("Player")]
    [SerializeField] private string playerTag = "Player";

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.CompareTag(playerTag))
        {
            hasTriggered = true;

            Debug.Log("Loading scene: " + sceneToLoad);

            SceneManager.LoadScene(sceneToLoad);
        }
    }
}