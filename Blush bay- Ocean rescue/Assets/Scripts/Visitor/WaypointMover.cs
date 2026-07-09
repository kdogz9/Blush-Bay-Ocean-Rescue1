using System.Collections;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
// grabs all waypoints so it doesnt have to be done individually 
    public Transform WaypointParent;
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    public bool loopwaypoints = true; 
    
    private Transform [] waypoints;
    private int currentwaypointIndex;
    private bool iswaiting;
    
    public void Start()
    {
     waypoints = new Transform[WaypointParent.childCount];
     // array size is how many waypoints there is 
     for(int i = 0; i < WaypointParent.childCount; i++)
     {
      waypoints[i] = WaypointParent.GetChild(i);
     }
   }
    
    public void Update()
    {
    if (iswaiting)
    {
    return;
    }
    MoveToWaypoint();
  }
  
  public void MoveToWaypoint()
  {
  Transform target = waypoints[currentwaypointIndex];
  // sets the waypoint as a target to move towards
  transform.position = Vector2.MoveTowards(transform.position,target.position,moveSpeed * Time.deltaTime);
  // moves the character towards the waypoint 
  if (Vector2.Distance(transform.position, target.position) < 0.1f)
  {
  StartCoroutine(WaitAtWaypoint());
  }
 }
 
 IEnumerator WaitAtWaypoint()
  {
   iswaiting = true;
   yield return new WaitForSeconds(waitTime);
   
   currentwaypointIndex = loopwaypoints ? (currentwaypointIndex + 1) % waypoints.Length : Mathf.Min(currentwaypointIndex + 1, waypoints.Length - 1);
 // creates loop using increments, wraps around if needed, if its not looping then it still increments but doesnt exceed last waypoint
 iswaiting = false;
  }
}
