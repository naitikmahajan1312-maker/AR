using UnityEngine;
using Vuforia;

public class ShootScript : MonoBehaviour
{
    public Camera arCamera;
    public GameObject hitEffect; // optional particle prefab

    public void Shoot()
    {
        Debug.Log("SHOOT");
        RaycastHit hit;
        if (Physics.Raycast(arCamera.transform.position, arCamera.transform.forward, out hit))
        {
            Debug.Log(hit.collider.tag);
            if (hit.collider.CompareTag("Obj"))
            {
                Destroy(hit.collider.gameObject);
                // if (hitEffect) Instantiate(hitEffect, hit.point, Quaternion.identity);
            }
        }
    }
}
