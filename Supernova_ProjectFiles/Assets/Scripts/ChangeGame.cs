using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum GameState { FLICKING, SQUATTING }

public class ChangeGame : MonoBehaviour
{
    public static GameState currentGame;

    [SerializeField]
    float rotationSpeed = 3;

    [SerializeField]
    Material cutesySkybox;

    [SerializeField]
    GameObject[] nebulae;

    [SerializeField] Text[] instructionsTexts;

    Material prettySkybox;
    Vector3 asteroidsGameEulers, planetsGameEulers, targetEulers, direction;

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
        asteroidsGameEulers = transform.eulerAngles;
        planetsGameEulers = new Vector3(transform.rotation.x, transform.rotation.y, 180);
        targetEulers = asteroidsGameEulers;
        direction = Vector3.zero;

        prettySkybox = RenderSettings.skybox;
        currentGame = GameState.FLICKING;

        Invoke("SetCanLean", 1);
    }

    void Update()
    {
        transform.Rotate(direction * rotationSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.RightArrow)) SwitchGameRight();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) SwitchGameLeft();

        if (Mathf.Abs(transform.eulerAngles.z - targetEulers.z) < 5 && !canLean)
        {
            transform.eulerAngles = targetEulers;
            foreach (Text t in instructionsTexts) t.enabled = true;
            direction = Vector3.zero;

            switch (currentGame)
            {
                case GameState.FLICKING:
                    RenderSettings.skybox = prettySkybox;
                    foreach (GameObject g in nebulae) g.SetActive(true);
                    instructionsTexts[0].text = "SWIPE YOUR RIGHT HAND TO PLAY!";
                    break;
                case GameState.SQUATTING:
                    RenderSettings.skybox = cutesySkybox;
                    foreach (GameObject g in nebulae) g.SetActive(false);
                    instructionsTexts[0].text = "SQUAT DOWN TO PLAY!";
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
            direction = Vector3.back;
        }
    }

    void SwitchGame()
    {
        canLean = false;
        foreach (Text t in instructionsTexts) t.enabled = false;

        switch (currentGame)
        {
            case GameState.FLICKING:
                targetEulers = planetsGameEulers;
                currentGame = GameState.SQUATTING;
                break;
            case GameState.SQUATTING:
                targetEulers = asteroidsGameEulers;
                currentGame = GameState.FLICKING;
                break;
        }
    }
}
