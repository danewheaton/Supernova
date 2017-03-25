using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlanetSquat : MonoBehaviour
{
    [HideInInspector]
    public List<Rigidbody> planetsInTrigger;

    [SerializeField]
    Text scoreText, goalText;

    [SerializeField]
    float squatDistance = 30, moveSpeed = 2, screenShakeTimer = 1, screenShakeIntensity = .5f;

    [SerializeField]
    int maxScore = 100;

    List<Rigidbody> planetsDodged;
    Vector3 targetPos, originalPos, squattingPos;
    Color originalScoreTextColor;
    Vector2 originalScoreTextSize;

    bool canSquat = true;

    void OnEnable()
    {
        GestureManager.OnSquatDetected += Duck;
    }
    void OnDisable()
    {
        GestureManager.OnSquatDetected -= Duck;
    }

    void Start()
    {
        originalPos = transform.position;
        squattingPos = new Vector3(originalPos.x, originalPos.y - squatDistance, originalPos.z);
        targetPos = originalPos;

        planetsInTrigger = new List<Rigidbody>();
        planetsDodged = new List<Rigidbody>();

        originalScoreTextColor = scoreText.color;
        originalScoreTextSize = scoreText.rectTransform.localScale;
        goalText.text = "/ " + maxScore;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);

        // print score to screen
        if (planetsDodged.Count > 0) scoreText.text = planetsDodged.Count.ToString();

        // if enough asteroids have been swiped, go to win screen
        if (planetsDodged.Count >= maxScore) SceneManager.LoadScene(3);

        if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene(0);
    }

    void Duck()
    {
        if (canSquat)
        {
            canSquat = false;
            targetPos = squattingPos;

            // make the score text flash
            StartCoroutine(ScoreTextEffects());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // if an asteroid bumps into you, shake the screen
        StartCoroutine(ScreenShake());
    }

    IEnumerator ScoreTextEffects()
    {
        float timer = .1f;
        float elapsedTime = 0;
        while (elapsedTime < timer)
        {
            // make the score text yellow
            scoreText.color = Color.Lerp(originalScoreTextColor, Color.yellow, elapsedTime / timer);

            // make the score text bigger
            scoreText.transform.localScale = Vector2.Lerp(originalScoreTextSize, originalScoreTextSize * 1.5f, elapsedTime / timer);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        elapsedTime = 0;
        while (elapsedTime < timer)
        {
            // change the score text back to its original color
            scoreText.color = Color.Lerp(Color.yellow, originalScoreTextColor, elapsedTime / timer);

            // change the score text back to its original size
            scoreText.transform.localScale = Vector2.Lerp(originalScoreTextSize * 1.5f, originalScoreTextSize, elapsedTime / timer);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        scoreText.color = originalScoreTextColor;
        scoreText.rectTransform.localScale = originalScoreTextSize;

        yield return new WaitForSeconds(.5f);
        canSquat = true;
        targetPos = originalPos;
    }

    IEnumerator ScreenShake()
    {
        Vector3 originalCamPos = Camera.main.transform.position;

        float elapsedTime = 0;
        while (elapsedTime < screenShakeTimer)
        {
            // shake the camera
            Camera.main.transform.localPosition = new Vector3
                (originalCamPos.x + UnityEngine.Random.insideUnitSphere.x * screenShakeIntensity,
                originalCamPos.z + UnityEngine.Random.insideUnitSphere.z * screenShakeIntensity,
                transform.position.z);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
