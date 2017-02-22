using UnityEngine;
using Windows.Kinect;
using System.Collections;
using System.Collections.Generic;
using System;

public class AsteroidLean : MonoBehaviour
{
    [SerializeField] GameObject[] asteroids1, asteroids2;

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

        if (e.GestureID == "Lean_Left")
        {
            if (e.DetectionConfidence > 0.65f)
            {
                print("leaning left");
            }
            else
            {
                print("not leaning left");
            }
        }

        if (e.GestureID == "Lean_Right")
        {
            if (e.DetectionConfidence > 0.65f)
            {
                print("leaning right");
            }
            else
            {
                print("not leaning right");
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
}
