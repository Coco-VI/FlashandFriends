using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class MissionPopupUI : MonoBehaviour
{
    public GameObject popupRoot;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public float displayDuration = 2.5f;

    private Coroutine currentRoutine;

    void Start()
    {
        Debug.Log("MissionPopupUI Start");

        if (popupRoot != null)
            popupRoot.SetActive(false);
        else
            Debug.LogError("popupRoot non assigné !");
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            ShowPopup("TEST POPUP", "Si tu vois ça, le popup marche.");
        }
    }

    public void ShowPopup(string title, string description)
    {
        Debug.Log("ShowPopup appelé : " + title + " / " + description);

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowPopupRoutine(title, description));
    }

    IEnumerator ShowPopupRoutine(string title, string description)
    {
        Debug.Log("Routine popup lancée");

        if (popupRoot != null)
            popupRoot.SetActive(true);

        if (titleText != null)
            titleText.text = title;
        else
            Debug.LogError("titleText non assigné !");

        if (descriptionText != null)
            descriptionText.text = description;
        else
            Debug.LogError("descriptionText non assigné !");

        yield return new WaitForSecondsRealtime(displayDuration);

        if (popupRoot != null)
            popupRoot.SetActive(false);

        currentRoutine = null;
    }
}