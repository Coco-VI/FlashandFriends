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
            flashImage.color = new Color(1, 1, 1, 0);
    }

    void Update()
    {
        HandlePhotoMode();

        if (!isPhotoMode) return;

        HandleZoom();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartCoroutine(TakePhoto());
        }
    }

    void HandlePhotoMode()
    {
        if (Mouse.current.rightButton.isPressed)
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

                // Reset zoom quand on quitte
                currentFOV = defaultFOV;
                virtualCam.m_Lens.FieldOfView = defaultFOV;
            }
        }
    }

    void HandleZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0)
        {
            currentFOV -= scroll * zoomSpeed;
            currentFOV = Mathf.Clamp(currentFOV, minFOV, maxFOV);

            virtualCam.m_Lens.FieldOfView = currentFOV;
        }
    }

    IEnumerator TakePhoto()
    {
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

        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        if (flashImage == null)
            yield break;

        float timer = 0f;

        while (timer < flashDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / flashDuration);
            flashImage.color = new Color(1, 1, 1, alpha);

            timer += Time.deltaTime;
            yield return null;
        }

        flashImage.color = new Color(1, 1, 1, 0f);
    }
}