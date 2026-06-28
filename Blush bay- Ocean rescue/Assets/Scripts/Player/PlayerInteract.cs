using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    // This stores the tank the player is standing near
    private Tank nearbyTank;

    private void Update()
    {
        // If player is near a tank and presses E
        if (nearbyTank != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Open the fish info panel for that tank
            FishInfoUI.Instance.OpenPanel(nearbyTank);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Look for a Tank script on this object OR its parent
        Tank tank = other.GetComponentInParent<Tank>();

        // If this trigger belongs to a tank
        if (tank != null)
        {
            nearbyTank = tank;
            Debug.Log("Press E to inspect tank");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Look for a Tank script on this object OR its parent
        Tank tank = other.GetComponentInParent<Tank>();

        // If we walked away from the same tank
        if (tank != null && nearbyTank == tank)
        {
            nearbyTank = null;
        }
    }
}