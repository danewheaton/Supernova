using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuSquat : MonoBehaviour
{
    [SerializeField]
    Transform[] swipeables;

    [SerializeField]
    float squatForce = 2;

    bool canSquat;

    // whenever this script is enabled, we can subscribe to events in GestureManager
    void OnEnable()
    {
        // when OnFlickDetected happens, link it to our own SwipeText function
        GestureManager.OnSquatDetected += PushText;
    }

    // likewise, when this script is disabled, it's a good idea to unsubscribe (so that function calls don't pile up)
    void OnDisable()
    {
        GestureManager.OnSquatDetected -= PushText;
    }

    void Start()
    {
        Invoke("SetCanSquat", 1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2)) SceneManager.LoadScene(2);

        if (Input.GetKeyDown(KeyCode.S)) PushText();
    }

    void SetCanSquat()
    {
        canSquat = true;
    }

    void PushText()
    {
        if (canSquat && ChangeGame.currentGame == GameState.SQUATTING) StartCoroutine(PushTextOffscreen());
    }

    IEnumerator PushTextOffscreen()
    {
        float timer = 1;
        float elapsedTime = 0;
        while (elapsedTime < timer)
        {
            foreach (Transform t in swipeables)
            {
                #region push text offscreen

                if (t is RectTransform) t.transform.position += Vector3.up * squatForce * 100;
                else t.transform.position += Vector3.up * squatForce;

                #endregion

                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        SceneManager.LoadScene(2);
    }
}
