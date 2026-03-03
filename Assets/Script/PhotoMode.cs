using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.IO;

public class PhotoMode : MonoBehaviour
{
    public GameObject cameraOverlay;
    public Camera playerCamera;

    public float normalFOV = 60f;
    public float photoFOV = 45f;
    public float zoomSpeed = 8f;

    private InputSystem_Actions inputActions;
    private bool isInPhotoMode = false;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        isInPhotoMode = inputActions.Player.PhotoMode.ReadValue<float>() > 0;

        cameraOverlay.SetActive(isInPhotoMode);

        float targetFOV = isInPhotoMode ? photoFOV : normalFOV;

        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            targetFOV,
            Time.deltaTime * zoomSpeed
        );

        if (isInPhotoMode && inputActions.Player.TakePhoto.triggered)
        {
            StartCoroutine(TakePhoto());
        }
    }

    IEnumerator TakePhoto()
    {
        cameraOverlay.SetActive(false);

        yield return new WaitForEndOfFrame();

        string folderPath = Application.dataPath + "/Photos";

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string fileName = "Photo_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        string fullPath = Path.Combine(folderPath, fileName);

        ScreenCapture.CaptureScreenshot(fullPath);

        Debug.Log("Photo sauvegardée : " + fullPath);

        yield return new WaitForSeconds(0.1f);

        cameraOverlay.SetActive(true);
    }
}