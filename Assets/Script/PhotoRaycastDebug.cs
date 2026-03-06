using UnityEngine;

public class PhotoRaycastDebug : MonoBehaviour
{
    public Camera playerCamera;
    public float rayDistance = 100f;

    void Update()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.green);

            Debug.Log("Raycast touche : " + hit.collider.gameObject.name);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);
        }
    }
}