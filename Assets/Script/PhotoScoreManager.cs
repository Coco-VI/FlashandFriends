using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PhotoScoreManager : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;

    [Header("Detection")]
    public float detectionDistance = 60f;
    public LayerMask obstacleMask = ~0;

    [Header("Score")]
    public int baseScorePerCharacter = 100;
    public float centerBonusMultiplier = 1f;
    public float distanceBonusMultiplier = 1f;
    public float groupMultiplierPerExtraCharacter = 0.5f;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    public float displayDuration = 2f;

    private int totalScore = 0;
    private int bestScore = 0;

    void Start()
    {
        if (scoreText != null)
            scoreText.gameObject.SetActive(false);

        if (bestScoreText != null)
            bestScoreText.text = "Best Shot : 0";
    }

    public void CalculateScore()
    {
        if (playerCamera == null)
        {
            Debug.LogError("PhotoScoreManager : camera non assignée.");
            return;
        }

        GameObject[] allCharacters = GameObject.FindGameObjectsWithTag("Character");
        HashSet<GameObject> visibleCharacters = new HashSet<GameObject>();

        int photoScore = 0;

        foreach (GameObject character in allCharacters)
        {
            if (character == null)
                continue;

            if (IsCharacterVisible(character, out float centerBonus, out float distanceBonus))
            {
                visibleCharacters.Add(character);

                int characterScore = baseScorePerCharacter;
                characterScore += Mathf.RoundToInt(baseScorePerCharacter * centerBonus * centerBonusMultiplier);
                characterScore += Mathf.RoundToInt(baseScorePerCharacter * distanceBonus * distanceBonusMultiplier);

                photoScore += characterScore;

                Debug.Log("VISIBLE CHARACTER : " + character.name);
            }
        }

        int visibleCount = visibleCharacters.Count;

        if (visibleCount == 0)
        {
            ShowScore("No Subjects", 0);
            return;
        }

        float groupMultiplier = 1f + ((visibleCount - 1) * groupMultiplierPerExtraCharacter);
        photoScore = Mathf.RoundToInt(photoScore * groupMultiplier);

        totalScore += photoScore;

        if (photoScore > bestScore)
        {
            bestScore = photoScore;

            if (bestScoreText != null)
                bestScoreText.text = "Best Shot : " + bestScore;
        }

        ShowScore("Good Shot!", photoScore);

        Debug.Log("PHOTO SCORE : +" + photoScore + " | TOTAL : " + totalScore + " | PERSONNES : " + visibleCount);
    }

    bool IsCharacterVisible(GameObject character, out float centerBonus, out float distanceBonus)
    {
        centerBonus = 0f;
        distanceBonus = 0f;

        Vector3 targetPoint = GetBestTargetPoint(character);
        Vector3 viewport = playerCamera.WorldToViewportPoint(targetPoint);

        bool inView =
            viewport.z > 0f &&
            viewport.x >= 0f && viewport.x <= 1f &&
            viewport.y >= 0f && viewport.y <= 1f;

        if (!inView)
            return false;

        float distance = Vector3.Distance(playerCamera.transform.position, targetPoint);
        if (distance > detectionDistance)
            return false;

        Vector3 origin = playerCamera.transform.position;
        Vector3 direction = (targetPoint - origin).normalized;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, detectionDistance, obstacleMask))
        {
            if (hit.collider.transform.root != character.transform.root)
                return false;
        }

        float centerOffset = Vector2.Distance(
            new Vector2(viewport.x, viewport.y),
            new Vector2(0.5f, 0.5f)
        );

        centerBonus = Mathf.Clamp01(1f - centerOffset * 2f);
        distanceBonus = Mathf.Clamp01(1f - (distance / detectionDistance));

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

    void ShowScore(string label, int score)
    {
        if (scoreText == null)
            return;

        StopAllCoroutines();
        StartCoroutine(ShowScoreRoutine(label, score));
    }

    IEnumerator ShowScoreRoutine(string label, int score)
    {
        scoreText.gameObject.SetActive(true);
        scoreText.text = label + " +" + score;

        yield return new WaitForSecondsRealtime(displayDuration);

        scoreText.gameObject.SetActive(false);
    }
}