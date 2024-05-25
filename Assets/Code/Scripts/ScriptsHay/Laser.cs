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

}