using UnityEngine;
using UnityEngine.SceneManagement;
using Windows.Kinect;
using System.Collections;
using System.Collections.Generic;
using System;

public class MenuSwipe : MonoBehaviour
{
    [SerializeField]
    Transform[] swipeables;

    [SerializeField]
    float flickForce = 2;

    KinectSensor sensor;
    BodyFrameReader bodyFrameReader;
    Body[] bodies;

    int bodyCount;
    bool canSwipe;

    List<GestureDetector> gestureDetectorList = null;

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

        Cursor.visible = false;
        Invoke("SetCanSwipe", 1);
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
            if (e.DetectionConfidence > .85f && canSwipe)
            {
                StartCoroutine(PushOffscreen());
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

    void SetCanSwipe()
    {
        canSwipe = true;
    }

    IEnumerator PushOffscreen()
    {
        float timer = 1;
        float elapsedTime = 0;
        while (elapsedTime < timer)
        {
            foreach (Transform t in swipeables)
            {
                if (t is RectTransform) t.transform.position += Vector3.right * flickForce * 100;
                else t.transform.position += Vector3.right * flickForce;

                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        SceneManager.LoadScene(1);
    }
}
