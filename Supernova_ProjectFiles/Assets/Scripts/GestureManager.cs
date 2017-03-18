using UnityEngine;
using Windows.Kinect;
using System.Collections;
using System.Collections.Generic;
using System;

public class GestureManager : MonoBehaviour
{
    // events are signals that are sent out by this class to let other classes know when something important happens - in this case, when a gesture is detected by the Kinect
    #region gesture events
    public delegate void FlickDetected();
    public static event FlickDetected OnFlickDetected;
    public delegate void LeanLeftDetected();
    public static event LeanRightDetected OnLeanLeftDetected;
    public delegate void LeanRightDetected();
    public static event LeanRightDetected OnLeanRightDetected;
    #endregion

    [SerializeField]
    float flickDetectionConfidence = .75f, leanDetectionConfidence = .75f;

    KinectSensor sensor;
    BodyFrameReader bodyFrameReader;
    Body[] bodies;

    int bodyCount;

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
        if (e.GestureID == "flick" && e.DetectionConfidence > flickDetectionConfidence && OnFlickDetected != null)
            OnFlickDetected();

        if (e.GestureID == "Lean_Left" && e.DetectionConfidence > leanDetectionConfidence && OnLeanLeftDetected != null)
            OnLeanLeftDetected();

        if (e.GestureID == "Lean_Right" && e.DetectionConfidence > leanDetectionConfidence && OnLeanRightDetected != null)
            OnLeanRightDetected();
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
