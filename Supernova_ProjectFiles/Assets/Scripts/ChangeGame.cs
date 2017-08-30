using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum GameState { SUPERNOVA, JETTISON, HORIZON }

public class ChangeGame : MonoBehaviour
{
    public static GameState currentGame;

    [SerializeField]
    float rotationSpeed = 150, rotationMarginOfError = 10;

    [SerializeField]
    Material cutesySkybox, horizonSkybox;

    [SerializeField]
    GameObject[] nebulae;

    [SerializeField] Text[] instructionsTexts;

    Material prettySkybox;
    Vector3 supernovaEulers, jettisonEulers, horizonEulers, targetEulers, direction;

    bool canLean;

    private void OnEnable()
    {
        GestureManager.OnLeanLeftDetected += SwitchGameLeft;
        GestureManager.OnLeanRightDetected += SwitchGameRight;
    }
    private void OnDisable()
    {
        GestureManager.OnLeanLeftDetected -= SwitchGameLeft;
        GestureManager.OnLeanRightDetected -= SwitchGameRight;
    }

    private void Start()
    {
        supernovaEulers = transform.eulerAngles;
        jettisonEulers = new Vector3(transform.rotation.x, transform.rotation.y, 120);
        horizonEulers = new Vector3(transform.rotation.x, transform.rotation.y, 240);
        targetEulers = supernovaEulers;
        direction = Vector3.zero;

        prettySkybox = RenderSettings.skybox;
        currentGame = GameState.SUPERNOVA;

        Invoke("SetCanLean", .1f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) SwitchGameRight();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) SwitchGameLeft();

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    private void FixedUpdate()
    {
        transform.Rotate(direction * rotationSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.eulerAngles.z - targetEulers.z) < rotationMarginOfError && !canLean)
        {
            transform.eulerAngles = targetEulers;
            foreach (Text t in instructionsTexts) t.enabled = true;
            direction = Vector3.zero;

            switch (currentGame)
            {
                case GameState.SUPERNOVA:
                    RenderSettings.skybox = prettySkybox;
                    foreach (GameObject g in nebulae) g.SetActive(true);
                    instructionsTexts[0].text = "SWIPE YOUR RIGHT HAND TO PLAY!";
                    break;
                case GameState.JETTISON:
                    RenderSettings.skybox = cutesySkybox;
                    foreach (GameObject g in nebulae) g.SetActive(false);
                    instructionsTexts[0].text = "SQUAT DOWN TO PLAY!";
                    break;
                case GameState.HORIZON:
                    RenderSettings.skybox = horizonSkybox;
                    foreach (GameObject g in nebulae) g.SetActive(false);
                    instructionsTexts[0].text = "SWIPE YOUR RIGHT HAND TO PLAY!";
                    break;
            }

            canLean = true;
        }
    }

    void SetCanLean()
    {
        canLean = true;
    }

    void SwitchGameLeft()
    {
        if (canLean)
        {
            SwitchGame();
            direction = Vector3.forward;
        }
    }

    void SwitchGameRight()
    {
        if (canLean)
        {
            SwitchGame();
            direction = Vector3.forward;
        }
    }

    void SwitchGame()
    {
        canLean = false;
        GetComponent<AudioSource>().Play();
        foreach (Text t in instructionsTexts) t.enabled = false;

        switch (currentGame)
        {
            case GameState.SUPERNOVA:
                targetEulers = jettisonEulers;
                currentGame = GameState.JETTISON;
                break;
            case GameState.JETTISON:
                targetEulers = horizonEulers;
                currentGame = GameState.HORIZON;
                break;
            case GameState.HORIZON:
                targetEulers = supernovaEulers;
                currentGame = GameState.SUPERNOVA;
                break;
        }
    }
}
