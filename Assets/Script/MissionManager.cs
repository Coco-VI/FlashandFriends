using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public Camera playerCamera;
    public float photoDistance = 100f;

    public string requiredTag = "PhotoTarget";

    public void CheckPhoto()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, photoDistance))
        {
            Transform obj = hit.collider.transform;

            while (obj != null)
            {
                if (obj.CompareTag(requiredTag))
                {
                    Debug.Log("MISSION REUSSIE : " + obj.name);
                    return;
                }

                obj = obj.parent;
            }

            Debug.Log("Photo incorrecte : objet touché = " + hit.collider.name);
        }
        else
        {
            Debug.Log("Photo incorrecte : rien touché");
        }
    }
}