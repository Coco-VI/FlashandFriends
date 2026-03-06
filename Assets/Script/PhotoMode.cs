using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;
using System.IO;

public class PhotoMode : MonoBehaviour
{
    [Header("References")]
    public CinemachineVirtualCamera virtualCam;
    public GameObject photoUI;
    public Image flashImage;
    public MissionManager missionManager;
    public PhotoScoreManager scoreManager;

    [Header("Zoom Settings")]
    public float zoomSpeed = 0.05f;
    public float minFOV = 20f;
    public float maxFOV = 60f;

    [Header("Photo Settings")]
    public float flashDuration = 0.15f;

    private float defaultFOV;
    private float currentFOV;
    private bool isPhotoMode = false;

    void Start()
    {
        if (virtualCam == null)
        {
            Debug.LogError("Virtual Camera non assignée !");
            return;
        }

        defaultFOV = virtualCam.m_Lens.FieldOfView;
        currentFOV = defaultFOV;

        if (photoUI != null)
            photoUI.SetActive(false);

        if (flashImage != null)
            flashImage.color = new Color(1f, 1f, 1f, 0f);
    }

    void Update()
    {
        HandlePhotoMode();

        if (!isPhotoMode)
            return;

        HandleZoom();

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartCoroutine(TakePhoto());
        }
    }

    void HandlePhotoMode()
    {
        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
        {
            if (!isPhotoMode)
            {
                isPhotoMode = true;

                if (photoUI != null)
                    photoUI.SetActive(true);
            }
        }
        else
        {
            if (isPhotoMode)
            {
                isPhotoMode = false;

                if (photoUI != null)
                    photoUI.SetActive(false);

                currentFOV = defaultFOV;
                virtualCam.m_Lens.FieldOfView = defaultFOV;
            }
        }
    }

    void HandleZoom()
    {
        if (Mouse.current == null)
            return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0f)
        {
            currentFOV -= scroll * zoomSpeed;
            currentFOV = Mathf.Clamp(currentFOV, minFOV, maxFOV);

            virtualCam.m_Lens.FieldOfView = currentFOV;
        }
    }

    IEnumerator TakePhoto()
    {
        if (photoUI != null)
            photoUI.SetActive(false);

        yield return new WaitForEndOfFrame();

        string folderPath;

#if UNITY_EDITOR
        folderPath = Path.Combine(Application.dataPath, "Photos");
#else
        folderPath = Path.Combine(Application.persistentDataPath, "Photos");
#endif

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string fileName = "Photo_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".png";
        string fullPath = Path.Combine(folderPath, fileName);

        ScreenCapture.CaptureScreenshot(fullPath);
        Debug.Log("PHOTO SAVED AT: " + fullPath);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        if (missionManager != null)
        {
            missionManager.CheckPhoto();
        }

        if (scoreManager != null)
        {
            scoreManager.CalculateScore();
        }

        yield return StartCoroutine(Flash());

        if (isPhotoMode && photoUI != null)
        {
            photoUI.SetActive(true);
        }
    }

    IEnumerator Flash()
    {
        if (flashImage == null)
            yield break;

        float timer = 0f;

        while (timer < flashDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / flashDuration);
            flashImage.color = new Color(1f, 1f, 1f, alpha);

            timer += Time.deltaTime;
            yield return null;
        }

        flashImage.color = new Color(1f, 1f, 1f, 0f);
    }
}