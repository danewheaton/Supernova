using UnityEngine;
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

    KinectSensor sensor;
    BodyFrameReader bodyFrameReader;
    Body[] bodies;

    int bodyCount;

    List<GestureDetector> gestureDetectorList = null;
    List<Rigidbody> flickedAsteroids;

    Color originalColor;
    Vector2 originalSize;
    bool canFlick = true;

    void Start()
    {
        sensor = KinectSensor.GetDefault();

        if (sensor != null)
        {
            bodyFrameReader = sensor.BodyFrameSource.OpenReader();
            bodyCount = sensor.BodyFrameSource.BodyCount;
            bodies = new Body[bodyCount];

            gestureDetectorList = new List<GestureDetector>();
            for (int bodyIndex = 0; bodyIndex < bodyCount; bodyIndex++)
                gestureDetectorList.Add(new GestureDetector(sensor));

            sensor.Open();
        }

        asteroidsInTrigger = new List<Rigidbody>();
        flickedAsteroids = new List<Rigidbody>();

        originalColor = scoreText.color;
        originalSize = scoreText.rectTransform.localScale;

        goalText.text = "/ " + maxScore;
    }

    void Update()
    {
        bool newBodyData = false;
        using (BodyFrame bodyFrame = bodyFrameReader.AcquireLatestFrame())
        {
            if (bodyFrame != null)
            {
                bodyFrame.GetAndRefreshBodyData(bodies);
                newBodyData = true;
            }
        }

        if (newBodyData)
        {
            for (int bodyIndex = 0; bodyIndex < bodyCount; bodyIndex++)
            {
                Body body = bodies[bodyIndex];
                if (body != null)
                {
                    ulong trackingId = body.TrackingId;

                    if (trackingId != gestureDetectorList[bodyIndex].TrackingId)
                    {
                        gestureDetectorList[bodyIndex].TrackingId = trackingId;
                        gestureDetectorList[bodyIndex].IsPaused = (trackingId == 0);
                        gestureDetectorList[bodyIndex].OnGestureDetected += CreateOnGestureHandler(bodyIndex);
                    }
                }
            }
        }

        if (flickedAsteroids.Count > 0) scoreText.text = flickedAsteroids.Count.ToString();
        if (flickedAsteroids.Count >= maxScore) SceneManager.LoadScene(2);
    }

    void OnTriggerEnter(Collider other)
    {
        StartCoroutine(ScreenShake());
    }

    private EventHandler<GestureEventArgs> CreateOnGestureHandler(int bodyIndex)
    {
        return (object sender, GestureEventArgs e) => OnGestureDetected(sender, e, bodyIndex);
    }

    private void OnGestureDetected(object sender, GestureEventArgs e, int bodyIndex)
    {
        bool isDetected = e.IsBodyTrackingIdValid && e.IsGestureDetected;

        if (e.GestureID == "flick")
        {
            if (e.DetectionConfidence > .85f)
            {
                foreach (Rigidbody r in asteroidsInTrigger)
                {
                    r.AddForce(Vector3.right * flickForce);
                    if (canFlick) flickedAsteroids.Add(r);
                }
                canFlick = false;
                StartCoroutine(ScoreTextEffects());
            }
        }
    }

    void OnApplicationQuit()
    {
        if (bodyFrameReader != null)
        {
            bodyFrameReader.Dispose();
            bodyFrameReader = null;
        }

        if (sensor != null)
        {
            if (sensor.IsOpen) sensor.Close();
            sensor = null;
        }
    }

    IEnumerator ScoreTextEffects()
    {
        float timer = .1f;
        float elapsedTime = 0;
        while (elapsedTime < timer)
        {
            scoreText.color = Color.Lerp(originalColor, Color.yellow, elapsedTime / timer);
            scoreText.transform.localScale = Vector2.Lerp(originalSize, originalSize * 1.5f, elapsedTime / timer);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        elapsedTime = 0;
        while (elapsedTime < timer)
        {
            scoreText.color = Color.Lerp(Color.yellow, originalColor, elapsedTime / timer);
            scoreText.transform.localScale = Vector2.Lerp(originalSize * 1.5f, originalSize, elapsedTime / timer);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        scoreText.color = originalColor;
        scoreText.rectTransform.localScale = originalSize;

        yield return new WaitForSeconds(2);
        canFlick = true;
    }

    IEnumerator ScreenShake()
    {
        Vector3 originalCamPos = Camera.main.transform.position;

        float elapsedTime = 0;
        while (elapsedTime < screenShakeTimer)
        {
            Camera.main.transform.localPosition = new Vector3
                (originalCamPos.x + UnityEngine.Random.insideUnitSphere.x * screenShakeIntensity,
                originalCamPos.z + UnityEngine.Random.insideUnitSphere.z * screenShakeIntensity,
                transform.position.z);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
