using UnityEngine;

public class CheckLineOfSightUtility : MonoBehaviour
{
    public LayerMask obstacleMask, targetMask;
    private bool CheckLineOfSight(Transform startTransform, Transform target, float length)
    {
        Vector3 directionToPlayer = target.position - startTransform.position;

        RaycastHit2D hitPlayer = Physics2D.Raycast(startTransform.position, directionToPlayer, length, targetMask);
        RaycastHit2D hitWall = Physics2D.Raycast(startTransform.position, directionToPlayer, length, obstacleMask);

        if (hitWall.collider.CompareTag("Wall") && hitPlayer.collider.CompareTag("Player") && hitPlayer.distance < hitWall.distance)
        {
            return true;
        }
        return false;
    }
   
}
