using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform firepoint; // Add a reference to the firepoint

    void Start()
    {
        DisableLaser();
    }

    public void EnableLaser()
    {
        lineRenderer.enabled = true;
        GetComponentInChildren<BoxCollider2D>().enabled = true;
    }

    public void DisableLaser()
    {
        lineRenderer.enabled = false;
        GetComponentInChildren<BoxCollider2D>().enabled = false;
    }

    /*public void UpdateLaserPosition(Vector2 direction)
    {
        // Set the laser's start position to the firepoint's position
        Vector3 startPosition = firepoint.position;

        // Set the laser's start position
        lineRenderer.SetPosition(0, startPosition);

        // Calculate the end position of the laser based on direction
        Vector3 endPosition = startPosition + (Vector3)direction * 100f;

        // Set the end position of the laser
        lineRenderer.SetPosition(1, endPosition);
    } */
}
