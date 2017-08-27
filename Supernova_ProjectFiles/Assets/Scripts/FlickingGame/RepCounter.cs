using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RepCounter : MonoBehaviour
{
    [SerializeField]
    GameObject[] objectsToDeactivate;

    [SerializeField]
    Text repCounterText;

    [SerializeField]
    float flickForce = 2, coolDownTime = 1;
    
    Text repText;
    int totalReps, repsCompleted;
    bool gameIsStarted, canFlick = true;

    void OnEnable()
    {
        GestureManager.OnFlickDetected += StartGameOrCountReps;
        GestureManager.OnLeanLeftDetected += DecreaseReps;
        GestureManager.OnLeanRightDetected += IncreaseReps;
    }
    void OnDisable()
    {
        GestureManager.OnFlickDetected -= StartGameOrCountReps;
        GestureManager.OnLeanLeftDetected -= DecreaseReps;
        GestureManager.OnLeanRightDetected -= IncreaseReps;
    }

	void Start ()
    {
        repText = GetComponent<Text>();

        foreach (GameObject g in objectsToDeactivate) g.SetActive(false);
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) StartGameOrCountReps();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) DecreaseReps();
        if (Input.GetKeyDown(KeyCode.RightArrow)) IncreaseReps();
    }

    void IncreaseReps()
    {
        if (!gameIsStarted)
        {
            totalReps++;
            repText.text = totalReps.ToString();
        }
    }

    void DecreaseReps()
    {
        if (!gameIsStarted && totalReps > 0)
        {
            totalReps--;
            repText.text = totalReps.ToString();
        }
    }

    void StartGameOrCountReps()
    {
        StartCoroutine(CoolDown());

        if (!gameIsStarted) StartCoroutine(StartGame());
        else if (canFlick)
        {
            canFlick = false;

            repsCompleted++;
            repCounterText.text = repsCompleted + " reps out of " + totalReps;

            if (repsCompleted >= totalReps) StartCoroutine(ChangeScene());
        }
    }

    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(coolDownTime);
        canFlick = true;
    }

    IEnumerator StartGame()
    {
        float timer = 1;
        float elapsedTime = 0;
        while (elapsedTime < timer)
        {
            transform.position += Vector3.right * flickForce * 100;
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        foreach (GameObject g in objectsToDeactivate) g.SetActive(true);
        foreach (Text t in GetComponentsInChildren<Text>()) t.enabled = false;
        repCounterText.text = repsCompleted + " reps out of " + totalReps;

        yield return new WaitForSeconds(1);
        gameIsStarted = true;
    }

    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("SupernovaEndScene");
    }
}
