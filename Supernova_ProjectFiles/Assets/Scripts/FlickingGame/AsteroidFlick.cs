﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Windows.Kinect;
using System.Collections;
using System.Collections.Generic;
using System;

public class AsteroidFlick : MonoBehaviour
{
    [HideInInspector]
    public List<Rigidbody> asteroidsInTrigger;

    [SerializeField]
    GameObject[] asteroids;

    [SerializeField]
    Text scoreText, goalText;

    [SerializeField]
    float flickForce = 500, screenShakeTimer = 1, screenShakeIntensity = .5f;

    [SerializeField]
    int maxScore = 100;
    
    List<Rigidbody> flickedAsteroids;

    Color originalScoreTextColor;
    Vector2 originalScoreTextSize;
    bool canFlick = true;

    // whenever this script is enabled, we can subscribe to events in GestureManager
    void OnEnable()
    {
        // when OnFlickDetected happens, link it to our own FlickAsteroids function
        GestureManager.OnFlickDetected += FlickAsteroids;
    }

    // likewise, when this script is disabled, it's a good idea to unsubscribe (so that function calls don't pile up)
    void OnDisable()
    {
        GestureManager.OnFlickDetected -= FlickAsteroids;
    }

    void Start()
    {
        asteroidsInTrigger = new List<Rigidbody>();
        flickedAsteroids = new List<Rigidbody>();

        originalScoreTextColor = scoreText.color;
        originalScoreTextSize = scoreText.rectTransform.localScale;

        goalText.text = "/ " + maxScore;
    }

    void Update()
    {
        // print score to screen
        if (flickedAsteroids.Count > 0) scoreText.text = flickedAsteroids.Count.ToString();

        // if enough asteroids have been swiped, go to win screen
        if (flickedAsteroids.Count >= maxScore) SceneManager.LoadScene(3);

        if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene(0);
    }

    void OnTriggerEnter(Collider other)
    {
        // if an asteroid bumps into you, shake the screen
        StartCoroutine(ScreenShake());
    }

    void FlickAsteroids()
    {
        // for each asteroid that's close enough
        foreach (Rigidbody r in asteroidsInTrigger)
        {
            // push the asteroid offscreen to the right
            r.AddForce(Vector3.right * flickForce);
            if (canFlick) flickedAsteroids.Add(r);
        }
        if (canFlick)
        {
            // make the score text flash
            StartCoroutine(ScoreTextEffects());
        }
        canFlick = false;
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

        yield return new WaitForSeconds(2);
        canFlick = true;
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