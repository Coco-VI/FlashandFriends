using UnityEngine;
using System.Collections.Generic;

public class PhotoMission : MonoBehaviour
{
    public List<GameObject> requiredTargets = new List<GameObject>();

    public bool CheckMission(Camera cam)
    {
        foreach (GameObject target in requiredTargets)
        {
            if (!IsVisible(cam, target))
                return false;
        }

        return true;
    }

    bool IsVisible(Camera cam, GameObject obj)
    {
        Vector3 viewportPos = cam.WorldToViewportPoint(obj.transform.position);

        bool onScreen =
            viewportPos.x > 0 &&
            viewportPos.x < 1 &&
            viewportPos.y > 0 &&
            viewportPos.y < 1 &&
            viewportPos.z > 0;

        return onScreen;
    }
}