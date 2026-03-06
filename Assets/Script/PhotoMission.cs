using UnityEngine;
using System.Collections.Generic;

public class PhotoMission : MonoBehaviour
{
    [Header("Mission Info")]
    public string missionTitle;

    [TextArea(2, 5)]
    public string missionDescription;

    [Header("Objects that must be in the photo")]
    public List<GameObject> requiredObjects = new List<GameObject>();

    [HideInInspector]
    public bool completed = false;
}