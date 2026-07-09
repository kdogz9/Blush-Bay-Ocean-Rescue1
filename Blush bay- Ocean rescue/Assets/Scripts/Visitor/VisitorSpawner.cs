using UnityEngine;

public class VisitorSpawner : MonoBehaviour
{
    [Header("Tank Check")]
    [SerializeField] private Tank tank;

    [Header("Visitor")]
    [SerializeField] private GameObject visitor;

    [Header("Spawn Point")]
    [SerializeField] private Transform visitorSpawnPoint;

    // This stops the visitor spawning again and again while the same fish is in the tank
    private bool hasSpawnedForCurrentFish = false;

    private void Update()
    {
        // If anything important is missing, stop here
        if (tank == null) return;
        if (visitor == null) return;
        if (visitorSpawnPoint == null) return;

        // If the tank has no fish, reset the spawn check
        // This means a new visitor can appear when a new fish is added later
        if (!tank.HasFish)
        {
            hasSpawnedForCurrentFish = false;
            return;
        }

        // If the tank has a fish and we have not spawned a visitor yet,
        // spawn the visitor at the entrance
        if (tank.HasFish && !hasSpawnedForCurrentFish)
        {
            SpawnVisitor();
        }
    }

    private void SpawnVisitor()
    {
        // Move the visitor to the entrance/spawn point
        visitor.transform.position = visitorSpawnPoint.position;

        // Turn the visitor on
        visitor.SetActive(true);

        // Remember that we have spawned a visitor for this fish
        hasSpawnedForCurrentFish = true;

        Debug.Log("Visitor spawned because there is a fish in the tank.");
    }
}