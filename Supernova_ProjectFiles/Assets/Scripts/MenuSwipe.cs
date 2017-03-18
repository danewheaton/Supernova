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
    }

    // likewise, when this script is disabled, it's a good idea to unsubscribe (so that function calls don't pile up)
    void OnDisable()
    {
        GestureManager.OnFlickDetected -= SwipeText;
    }

    void Start()
    {
        Invoke("SetCanSwipe", 1);
    }

    void SetCanSwipe()
    {
        canSwipe = true;
    }

    void SwipeText()
    {
        if (canSwipe) StartCoroutine(PushTextOffscreen());
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

                if (t is RectTransform) t.transform.position += Vector3.right * flickForce * 100;
                else t.transform.position += Vector3.right * flickForce;

                #endregion

                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        SceneManager.LoadScene(1);
    }
}
