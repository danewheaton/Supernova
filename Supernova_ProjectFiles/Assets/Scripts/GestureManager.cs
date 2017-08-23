using UnityEngine;
using Windows.Kinect;
using System.Collections;
using System.Collections.Generic;
using System;

// IMPORTANT NOTE! go to the script "GestureDetector.cs" when you want to add a new gesture to the database

public class GestureManager : MonoBehaviour
{
    // events are signals that are sent out by this class to let other classes know when something important happens - in this case, when a gesture is detected by the Kinect
    #region gesture events

    public delegate void FlickDetected();
    public static event FlickDetected OnFlickDetected;

    public delegate void LeanLeftDetected();
    public static event LeanLeftDetected OnLeanLeftDetected;

    public delegate void LeanRightDetected();
    public static event LeanRightDetected OnLeanRightDetected;

    public delegate void PressDetected();
    public static event PressDetected OnPressDetected;

    public delegate void PushDetected();
    public static event PushDetected OnPushDetected;

    public delegate void SquatDetected();
    public static event SquatDetected OnSquatDetected;

    #endregion

    [SerializeField]
    float flickDetectionConfidence = .75f, leanDetectionConfidence = .75f, pressDetectionConfidence = .75f,
        pushDetectionConfidence = .99f, squatDetectionConfidence = .75f;

    KinectSensor sensor;
    BodyFrameReader bodyFrameReader;
    Body[] bodies;

    int bodyCount;

    List<GestureDetector> gestureDetectorList = null;

    void Start()
    {
        Cursor.visible = false;

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

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    private EventHandler<GestureEventArgs> CreateOnGestureHandler(int bodyIndex)
    {
        return (object sender, GestureEventArgs e) => OnGestureDetected(sender, e, bodyIndex);
    }


    // this is the function we're most concerned with - as the name implies, it's called when a gesture is detected
    // IMPORTANT NOTE! go to the script "GestureDetector.cs" when you want to add a new gesture to the database
    private void OnGestureDetected(object sender, GestureEventArgs e, int bodyIndex)
    {
        if (e.GestureID == "Lean_Left" && e.DetectionConfidence > leanDetectionConfidence && OnLeanLeftDetected != null)
            OnLeanLeftDetected();

        if (e.GestureID == "Lean_Right" && e.DetectionConfidence > leanDetectionConfidence && OnLeanRightDetected != null)
            OnLeanRightDetected();

        if (e.GestureID == "flick" && e.DetectionConfidence > flickDetectionConfidence && OnFlickDetected != null)
            OnFlickDetected();

        if (e.GestureID == "press" && e.DetectionConfidence > pressDetectionConfidence && OnPressDetected != null)
            OnPressDetected();

        if (e.GestureID == "push" && e.DetectionConfidence > pushDetectionConfidence && OnPushDetected != null)
            OnPushDetected();

        if (e.GestureID == "squat" && e.DetectionConfidence > squatDetectionConfidence && OnSquatDetected != null)
            OnSquatDetected();
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
}
