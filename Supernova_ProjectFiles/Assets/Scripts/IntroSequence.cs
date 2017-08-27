using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroSequence : MonoBehaviour
{
    [SerializeField] Text title, instructions;
    [SerializeField] Image panel;
    [SerializeField] float longTimer = 2, shortTimer = 1;
    [SerializeField] Color instructionsColor;

    Color startColor = new Color(1, 1, 1, 0);
    bool canSwipe;

    private void OnEnable()
    {
        GestureManager.OnFlickDetected += CallFadeOut;
    }
    private void OnDisable()
    {
        GestureManager.OnFlickDetected += CallFadeOut;
    }

    private void Start()
    {
        StartCoroutine(Intro());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CallFadeOut();
        }
    }

    IEnumerator Intro()
    {
        float elapsedTime = 0;
        while (elapsedTime < shortTimer)
        {
            panel.color = Color.Lerp(Color.black, Color.white, elapsedTime / shortTimer);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        panel.color = Color.white;

        elapsedTime = 0;
        while (elapsedTime < longTimer)
        {
            panel.color = Color.Lerp(Color.white, startColor, elapsedTime / longTimer);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        panel.color = startColor;

        yield return new WaitForSeconds(.5f);

        elapsedTime = 0;
        while (elapsedTime < longTimer)
        {
            title.color = Color.Lerp(startColor, Color.white, elapsedTime / longTimer);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        title.color = Color.white;

        yield return new WaitForSeconds(.5f);

        elapsedTime = 0;
        while (elapsedTime < shortTimer)
        {
            instructions.color = Color.Lerp(startColor, instructionsColor, elapsedTime / shortTimer);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        instructions.color = instructionsColor;

        canSwipe = true;
    }

    void CallFadeOut()
    {
        if (canSwipe) StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0;
        while (elapsedTime < longTimer)
        {
            title.transform.position += Vector3.right * 100;
            instructions.transform.position += Vector3.right * 100;
            panel.color = Color.Lerp(Color.clear, Color.black, elapsedTime / longTimer);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        panel.color = Color.black;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
