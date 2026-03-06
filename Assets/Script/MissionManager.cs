using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class MissionManager : MonoBehaviour
{
    [Header("Camera")]
    public Camera playerCamera;

    [Header("Mission List")]
    public List<PhotoMission> missions = new List<PhotoMission>();
    public int currentMissionIndex = 0;

    [Header("Detection")]
    public float detectionDistance = 50f;
    public LayerMask obstacleMask = ~0;

    [Header("UI")]
    public Transform missionListParent;
    public GameObject missionTextPrefab;

    [Header("Popup UI")]
    public MissionPopupUI missionPopupUI;

    private List<TextMeshProUGUI> missionTexts = new List<TextMeshProUGUI>();

    public PhotoMission currentMission
    {
        get
        {
            if (missions == null || missions.Count == 0)
                return null;

            if (currentMissionIndex < 0 || currentMissionIndex >= missions.Count)
                return null;

            return missions[currentMissionIndex];
        }
    }

    void Start()
    {
        CreateMissionUI();
        UpdateMissionUI();
    }

    void CreateMissionUI()
    {
        if (missionListParent == null || missionTextPrefab == null)
            return;

        foreach (Transform child in missionListParent)
        {
            Destroy(child.gameObject);
        }

        missionTexts.Clear();

        foreach (PhotoMission mission in missions)
        {
            GameObject obj = Instantiate(missionTextPrefab, missionListParent);
            TextMeshProUGUI txt = obj.GetComponent<TextMeshProUGUI>();

            if (txt != null)
                missionTexts.Add(txt);
        }
    }

    void UpdateMissionUI()
    {
        for (int i = 0; i < missionTexts.Count && i < missions.Count; i++)
        {
            if (missions[i] == null)
                continue;

            string prefix = "- ";

            if (missions[i].completed)
                prefix = "V ";
            else if (i == currentMissionIndex)
                prefix = "• ";

            missionTexts[i].text = prefix + missions[i].missionTitle;
        }
    }

    public string GetMissionTitle()
    {
        if (currentMission == null)
            return "Aucune mission";

        return currentMission.missionTitle;
    }

    public string GetMissionDescription()
    {
        if (currentMission == null)
            return "";

        return currentMission.missionDescription;
    }

    public void CheckPhoto()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Camera non assignée !");
            return;
        }

        if (currentMission == null)
        {
            Debug.Log("Pas de mission active");
            return;
        }

        bool missionComplete = true;

        foreach (GameObject target in currentMission.requiredObjects)
        {
            if (target == null)
                continue;

            if (!IsTargetVisible(target))
            {
                missionComplete = false;
                Debug.Log("OBJET MANQUANT OU NON VISIBLE : " + target.name);
            }
        }

        if (missionComplete)
        {
            currentMission.completed = true;
            Debug.Log("MISSION COMPLETE ! : " + currentMission.missionTitle);

            if (missionPopupUI != null)
            {
                missionPopupUI.ShowPopup("Mission Complete!", currentMission.missionTitle);
            }

            if (currentMissionIndex + 1 < missions.Count)
            {
                currentMissionIndex++;
                Debug.Log("Nouvelle mission : " + missions[currentMissionIndex].missionTitle);
            }
            else
            {
                Debug.Log("FIN DU FESTIVAL !");

                if (missionPopupUI != null)
                {
                    missionPopupUI.ShowPopup(
                        "Festival Complete!",
                        "You captured the best moments of the festival."
                    );
                }
            }

            UpdateMissionUI();
        }
        else
        {
            Debug.Log("PHOTO INCORRECTE");
        }
    }

    bool IsTargetVisible(GameObject target)
    {
        Vector3 targetPos = GetBestTargetPoint(target);

        Vector3 viewportPos = playerCamera.WorldToViewportPoint(targetPos);

        // Dans l'écran
        bool inView =
            viewportPos.z > 0f &&
            viewportPos.x >= 0f && viewportPos.x <= 1f &&
            viewportPos.y >= 0f && viewportPos.y <= 1f;

        if (!inView)
            return false;

        // Pas trop loin
        float distance = Vector3.Distance(playerCamera.transform.position, targetPos);
        if (distance > detectionDistance)
            return false;

        // Vérifie qu'il n'y a pas un mur entre la caméra et la cible
        Vector3 origin = playerCamera.transform.position;
        Vector3 dir = (targetPos - origin).normalized;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, detectionDistance, obstacleMask))
        {
            Transform hitRoot = hit.collider.transform.root;
            Transform targetRoot = target.transform.root;

            if (hitRoot != targetRoot)
            {
                Debug.Log("Cible bloquée par : " + hit.collider.name);
                return false;
            }
        }

        return true;
    }

    Vector3 GetBestTargetPoint(GameObject target)
    {
        Collider col = target.GetComponentInChildren<Collider>();

        if (col != null)
            return col.bounds.center;

        Renderer rend = target.GetComponentInChildren<Renderer>();

        if (rend != null)
            return rend.bounds.center;

        return target.transform.position;
    }
}