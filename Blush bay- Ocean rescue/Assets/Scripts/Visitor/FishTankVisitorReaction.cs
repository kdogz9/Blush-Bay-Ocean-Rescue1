using UnityEngine;

public class FishTankVisitorReaction : MonoBehaviour
{
    [Header("Tank")]
    [SerializeField] private Tank tank;

    private void Start()
    {
        // If no tank was manually assigned, try to find it on the parent
        if (tank == null)
        {
            tank = GetComponentInParent<Tank>();
        }

        if (tank == null)
        {
            Debug.LogError("No Tank found for visitor reaction zone.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Try to find the visitor's heart bubble script
        VisitorThoughtBubble visitorThoughtBubble = other.GetComponentInParent<VisitorThoughtBubble>();

        // Try to find the visitor's movement/behaviour script
        VisitorBehaviour visitorBehaviour = other.GetComponentInParent<VisitorBehaviour>();

        // If this object is not a visitor, stop here
        if (visitorThoughtBubble == null || visitorBehaviour == null) return;

        // If the tank is missing, stop
        if (tank == null) return;

        // Only react if the tank actually has a fish
        if (!tank.HasFish)
        {
            Debug.Log("Visitor reached tank, but tank is empty.");
            return;
        }

        // Show the heart bubble above the visitor's head
        visitorThoughtBubble.ShowHeartBubble();

        // Tell the visitor they saw a fish.
        // This makes them go to the donation box.
        visitorBehaviour.ReactToFishTank(tank);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Hide the heart bubble when the visitor leaves the tank area
        VisitorThoughtBubble visitorThoughtBubble = other.GetComponentInParent<VisitorThoughtBubble>();

        if (visitorThoughtBubble == null) return;

        visitorThoughtBubble.HideHeartBubble();
    }
}