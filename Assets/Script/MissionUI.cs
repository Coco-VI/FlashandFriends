using UnityEngine;
using TMPro;

public class MissionUI : MonoBehaviour
{
    public MissionManager missionManager;
    public TextMeshProUGUI missionTitleText;
    public TextMeshProUGUI missionDescriptionText;

    void Update()
    {
        if (missionManager == null)
            return;

        if (missionTitleText != null)
            missionTitleText.text = missionManager.GetMissionTitle();

        if (missionDescriptionText != null)
            missionDescriptionText.text = missionManager.GetMissionDescription();
    }
}