using System.Collections;
using UnityEngine;

public class VisitorBehaviour : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] private Transform tankViewWaypoint;
    [SerializeField] private Transform donationBoxWaypoint;
    [SerializeField] private Transform exitWaypoint;

    [Header("Donation Box")]
    [SerializeField] private DonationBox donationBox;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stoppingDistance = 0.08f;

    [Header("Timing")]
    [SerializeField] private float lookAtTankTime = 2f;
    [SerializeField] private float donationPauseTime = 1.5f;

    [Header("Donation")]
    [SerializeField] private int donationAmount = 5;

    // These are the different stages of the visitor's visit
    private enum VisitorState
    {
        GoingToTank,
        LookingAtTank,
        GoingToDonationBox,
        Donating,
        GoingToExit,
        Finished
    }

    private VisitorState currentState;

    // This becomes true when the visitor sees a fish
    private bool sawFish = false;

    // This stops the visitor donating more than once
    private bool donationGiven = false;

    // This stops movement while the visitor is waiting
    private bool isWaiting = false;

    private void OnEnable()
    {
        // OnEnable runs every time the visitor is turned on.
        // This is better than Start because the visitor begins inactive.

        currentState = VisitorState.GoingToTank;

        sawFish = false;
        donationGiven = false;
        isWaiting = false;

        Debug.Log("Visitor has appeared and is walking to the tank.");
    }

    private void OnDisable()
    {
        // Stop any waiting routines when the visitor is turned off
        StopAllCoroutines();
    }

    private void Update()
    {
        // Only move depending on the current visitor state
        switch (currentState)
        {
            case VisitorState.GoingToTank:
                MoveToTank();
                break;

            case VisitorState.GoingToDonationBox:
                MoveToDonationBox();
                break;

            case VisitorState.GoingToExit:
                MoveToExit();
                break;
        }
    }

    private void MoveToTank()
    {
        // Move towards the tank viewing position
        bool reachedTank = MoveTowards(tankViewWaypoint);

        // When the visitor reaches the tank, pause and look at it
        if (reachedTank && !isWaiting)
        {
            StartCoroutine(LookAtTankRoutine());
        }
    }

    private IEnumerator LookAtTankRoutine()
    {
        isWaiting = true;
        currentState = VisitorState.LookingAtTank;

        // Visitor waits at the tank
        yield return new WaitForSeconds(lookAtTankTime);

        // If the visitor saw a fish, go to the donation box
        if (sawFish)
        {
            currentState = VisitorState.GoingToDonationBox;
        }
        else
        {
            // This should rarely happen now because visitors only spawn when there is a fish,
            // but it is a useful safety check.
            currentState = VisitorState.GoingToExit;
        }

        isWaiting = false;
    }

    public void ReactToFishTank(Tank tank)
    {
        // This is called by FishTankVisitorReaction when the visitor enters the tank trigger

        if (tank == null) return;

        // Only react if the tank has a fish
        if (!tank.HasFish) return;

        // Do not react twice
        if (sawFish) return;

        sawFish = true;

        Debug.Log("Visitor saw a fish and now wants to donate.");
    }

    private void MoveToDonationBox()
    {
        // Move towards the donation box waypoint
        bool reachedDonationBox = MoveTowards(donationBoxWaypoint);

        // When the visitor reaches the donation box, pause and donate
        if (reachedDonationBox && !isWaiting)
        {
            StartCoroutine(DonationRoutine());
        }
    }

    private IEnumerator DonationRoutine()
    {
        isWaiting = true;
        currentState = VisitorState.Donating;

        Debug.Log("Visitor stopped at the donation box.");

        // Pause so the player can see the visitor donating
        yield return new WaitForSeconds(donationPauseTime);

        GiveDonation();

        // After donating, leave the aquarium
        currentState = VisitorState.GoingToExit;

        isWaiting = false;
    }

    private void GiveDonation()
    {
        // Prevent double donations
        if (donationGiven) return;

        donationGiven = true;

        // Send the donation to the donation box
        if (donationBox != null)
        {
            donationBox.ReceiveDonation(donationAmount);
            Debug.Log("Visitor gave a donation.");
        }
        else
        {
            Debug.LogWarning("No DonationBox assigned on VisitorBehaviour.");
        }
    }

    private void MoveToExit()
    {
        // Move towards the exit waypoint
        bool reachedExit = MoveTowards(exitWaypoint);

        if (reachedExit)
        {
            currentState = VisitorState.Finished;

            Debug.Log("Visitor left the aquarium.");

            // Instead of destroying the visitor, turn it off.
            // This allows the VisitorSpawner to reuse it later.
            gameObject.SetActive(false);
        }
    }

    private bool MoveTowards(Transform target)
    {
        // If the waypoint is missing, do not move
        if (target == null)
        {
            Debug.LogWarning("Visitor has a missing waypoint.");
            return false;
        }

        // Move the visitor towards the target
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        // Check how far the visitor is from the target
        float distance = Vector3.Distance(transform.position, target.position);

        // Return true when close enough
        return distance <= stoppingDistance;
    }
}