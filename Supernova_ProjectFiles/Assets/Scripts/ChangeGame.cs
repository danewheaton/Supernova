using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum GameState { FLICKING, SQUATTING }

public class ChangeGame : MonoBehaviour
{
    [SerializeField]
    float rotationSpeed = .005f;

    [SerializeField]
    Material cutesySkybox;

    [SerializeField]
    GameObject[] nebulae;

    [SerializeField] Text[] instructionsTexts;

    Material prettySkybox;
    Quaternion originalRotation, newRotation;
    GameState currentGame;

    private void OnEnable()
    {
        GestureManager.OnLeanLeftDetected += SwitchGameLeft;
        GestureManager.OnLeanRightDetected += SwitchGameLeft;
    }
    private void OnDisable()
    {
        GestureManager.OnLeanLeftDetected -= SwitchGameLeft;
        GestureManager.OnLeanRightDetected -= SwitchGameLeft;
    }

    private void Start()
    {
        originalRotation = transform.rotation;
        newRotation = new Quaternion(transform.rotation.x, transform.rotation.y, 180, transform.rotation.w);

        prettySkybox = RenderSettings.skybox;
        currentGame = GameState.FLICKING;

        Invoke("SwitchGameRight", 1);
    }

    void SwitchGameLeft()
    {
        SwitchGame();
        StartCoroutine(RotateAxis(false));
    }

    void SwitchGameRight()
    {
        SwitchGame();
        StartCoroutine(RotateAxis(true));
    }

    void SwitchGame()
    {
        foreach (Text t in instructionsTexts) t.enabled = false;

        switch (currentGame)
        {
            case GameState.FLICKING:
                RenderSettings.skybox = cutesySkybox;
                foreach (GameObject g in nebulae) g.SetActive(false);
                newRotation = new Quaternion(transform.rotation.x, transform.rotation.y, 180, transform.rotation.w);
                instructionsTexts[0].text = "CROUCH TO PLAY!";
                currentGame = GameState.SQUATTING;
                break;
            case GameState.SQUATTING:
                RenderSettings.skybox = prettySkybox;
                foreach (GameObject g in nebulae) g.SetActive(true);
                newRotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, transform.rotation.w);
                instructionsTexts[0].text = "SWIPE YOUR RIGHT HAND TO PLAY!";
                currentGame = GameState.FLICKING;
                break;
        }
    }

    IEnumerator RotateAxis(bool clockwise)
    {
        while (transform.rotation.z != newRotation.z)
        {
            transform.rotation = new Quaternion
                (transform.rotation.x, transform.rotation.y, clockwise ? transform.rotation.z - rotationSpeed : transform.rotation.z + rotationSpeed, transform.rotation.w);

            if (transform.eulerAngles.z - newRotation.eulerAngles.z < .5f)
                foreach (Text t in instructionsTexts) t.enabled = true;

            yield return new WaitForEndOfFrame();
        }

    }
}
