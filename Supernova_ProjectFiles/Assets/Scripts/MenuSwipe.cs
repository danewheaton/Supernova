using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuSwipe : MonoBehaviour
{
    [SerializeField]
    Transform[] swipeables;

    [SerializeField]
    float flickForce = 2;

    bool canSwipe;

    // whenever this script is enabled, we can subscribe to events in GestureManager
    void OnEnable()
    {
        // when OnFlickDetected happens, link it to our own SwipeText function
        GestureManager.OnFlickDetected += SwipeText;
        //GestureManager.OnSquatDetected += Squat;
        GestureManager.OnLeanLeftDetected += LoadMainMenu;
        GestureManager.OnLeanRightDetected += LoadMainMenu;
    }

    // likewise, when this script is disabled, it's a good idea to unsubscribe (so that function calls don't pile up)
    void OnDisable()
    {
        GestureManager.OnFlickDetected -= SwipeText;
        //GestureManager.OnSquatDetected -= Squat;
        GestureManager.OnLeanLeftDetected -= LoadMainMenu;
        GestureManager.OnLeanRightDetected -= LoadMainMenu;
    }

    void Start()
    {
        Invoke("SetCanSwipe", 1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SceneManager.LoadScene(1);

        if (Input.GetKeyDown(KeyCode.Space)) SwipeText();
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) LoadMainMenu();
    }

    void SetCanSwipe()
    {
        canSwipe = true;
    }

    void SwipeText()
    {
        if (canSwipe && ChangeGame.currentGame == GameState.FLICKING) StartCoroutine(PushTextOffscreen(true));
    }

    void Squat()
    {
        if (canSwipe && ChangeGame.currentGame == GameState.SQUATTING) StartCoroutine(PushTextOffscreen(false));
    }

    void LoadMainMenu()
    {
        if (SceneManager.GetActiveScene().buildIndex > 2) SceneManager.LoadScene(0);
    }

    IEnumerator PushTextOffscreen(bool flicking)
    {
        float timer = 1;
        float elapsedTime = 0;
        while (elapsedTime < timer)
        {
            foreach (Transform t in swipeables)
            {
                #region push text offscreen

                if (t is RectTransform) t.transform.position += (flicking ? Vector3.right : Vector3.up) * flickForce * 100;
                else t.transform.position += (flicking ? Vector3.right : Vector3.up) * flickForce;

                #endregion

                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        SceneManager.LoadScene(flicking ? "AsteroidFlick" : "PlanetSquat");
    }
}
