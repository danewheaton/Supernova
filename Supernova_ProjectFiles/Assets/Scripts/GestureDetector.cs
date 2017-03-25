//------------------------------------------------------------------------------
// <copyright file="GestureDetector.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using Windows.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using UnityEngine;

// this is microsoft's confusing, complicated proprietary script for detecting gestures via kinect and linking them with our existing gesture databases (the .gdb files in the "StreamingAssets" folder)
// IMPORTANT NOTE! go to the script "GestureManager.cs" when you want to add a gesture event for your gameplay scripts to subscribe behaviors to

public class GestureEventArgs : EventArgs
{
    public bool IsBodyTrackingIdValid { get; private set; }

    public bool IsGestureDetected { get; private set; }

    public float DetectionConfidence { get; private set; }

    //my modification
    public string GestureID { get; private set; }

    //my mod
    public GestureEventArgs(bool isBodyTrackingIdValid, bool isGestureDetected, float detectionConfidence, string gestureID)
    {
        this.IsBodyTrackingIdValid = isBodyTrackingIdValid;
        this.IsGestureDetected = isGestureDetected;
        this.DetectionConfidence = detectionConfidence;
        this.GestureID = gestureID; 
    }
}

/// <summary>
/// Gesture Detector class which listens for VisualGestureBuilderFrame events from the service
/// and calls the OnGestureDetected event handler when a gesture is detected.
/// </summary>
public class GestureDetector : IDisposable
{
    /// <summary> Path to the gesture database that was trained with VGB </summary>
    private readonly string leanDB = "GestureDB\\Lean.gbd";
    private readonly string flickDB = "GestureDB\\flick.gbd";
    private readonly string pressDB = "GestureDB\\press.gbd";
    private readonly string pushDB = "GestureDB\\push.gbd";
    private readonly string squatDB = "GestureDB\\squatwhole.gbd";


    /// <summary> Name of the discrete gesture in the database that we want to track </summary>
    private readonly string leanLeftGestureName = "Lean_Left";
    private readonly string leanRightGestureName = "Lean_Right";
    private readonly string flickGestureName = "flick";
    private readonly string pressGestureName = "press";
    private readonly string pushGestureName = "push";
    private readonly string squatGestureName = "squat";

    /// <summary> Gesture frame source which should be tied to a body tracking ID </summary>
    private VisualGestureBuilderFrameSource vgbFrameSource = null;

    /// <summary> Gesture frame reader which will handle gesture events coming from the sensor </summary>
    private VisualGestureBuilderFrameReader vgbFrameReader = null;

    public event EventHandler<GestureEventArgs> OnGestureDetected;

    /// <summary>
    /// Initializes a new instance of the GestureDetector class along with the gesture frame source and reader
    /// </summary>
    /// <param name="kinectSensor">Active sensor to initialize the VisualGestureBuilderFrameSource object with</param>
    public GestureDetector(KinectSensor kinectSensor)
    {
        if (kinectSensor == null)
        {
            throw new ArgumentNullException("kinectSensor");
        }

        // create the vgb source. The associated body tracking ID will be set when a valid body frame arrives from the sensor.
        this.vgbFrameSource = VisualGestureBuilderFrameSource.Create(kinectSensor, 0);
        this.vgbFrameSource.TrackingIdLost += this.Source_TrackingIdLost;

        // open the reader for the vgb frames
        this.vgbFrameReader = this.vgbFrameSource.OpenReader();
        if (this.vgbFrameReader != null)
        {
            this.vgbFrameReader.IsPaused = true;
            this.vgbFrameReader.FrameArrived += this.Reader_GestureFrameArrived;
        }


        // load the 'Seated' gesture from the gesture database
        var databasePath = Path.Combine(Application.streamingAssetsPath, this.leanDB);
        using (VisualGestureBuilderDatabase database = VisualGestureBuilderDatabase.Create(databasePath))
        {
            foreach (Gesture gesture in database.AvailableGestures)
            {
                if (gesture.Name.Equals(this.leanLeftGestureName))
                {
                    this.vgbFrameSource.AddGesture(gesture);
                }
                if (gesture.Name.Equals(this.leanRightGestureName))
                {
                    this.vgbFrameSource.AddGesture(gesture);
                }
            }
        }

        var secondDatabasePath = Path.Combine(Application.streamingAssetsPath, this.flickDB);
        using (VisualGestureBuilderDatabase secondDatabase = VisualGestureBuilderDatabase.Create(secondDatabasePath))
        {
            foreach (Gesture gesture in secondDatabase.AvailableGestures)
            {
                if (gesture.Name.Equals(this.flickGestureName))
                {
                    this.vgbFrameSource.AddGesture(gesture);
                }
            }
        }

        var thirdDatabasePath = Path.Combine(Application.streamingAssetsPath, this.pressDB);
        using (VisualGestureBuilderDatabase thirdDatabase = VisualGestureBuilderDatabase.Create(thirdDatabasePath))
        {
            foreach (Gesture gesture in thirdDatabase.AvailableGestures)
            {
                if (gesture.Name.Equals(this.pressGestureName))
                {
                    this.vgbFrameSource.AddGesture(gesture);
                }
            }
        }

        var fourthDatabasePath = Path.Combine(Application.streamingAssetsPath, this.pushDB);
        using (VisualGestureBuilderDatabase fourthDatabase = VisualGestureBuilderDatabase.Create(fourthDatabasePath))
        {
            foreach (Gesture gesture in fourthDatabase.AvailableGestures)
            {
                if (gesture.Name.Equals(this.pushGestureName))
                {
                    this.vgbFrameSource.AddGesture(gesture);
                }
            }
        }

        var fifthDatabasePath = Path.Combine(Application.streamingAssetsPath, this.squatDB);
        using (VisualGestureBuilderDatabase fifthDatabase = VisualGestureBuilderDatabase.Create(fifthDatabasePath))
        {
            foreach (Gesture gesture in fifthDatabase.AvailableGestures)
            {
                if (gesture.Name.Equals(this.squatGestureName))
                {
                    this.vgbFrameSource.AddGesture(gesture);
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the body tracking ID associated with the current detector
    /// The tracking ID can change whenever a body comes in/out of scope
    /// </summary>
    public ulong TrackingId
    {
        get
        {
            return this.vgbFrameSource.TrackingId;
        }

        set
        {
            if (this.vgbFrameSource.TrackingId != value)
            {
                this.vgbFrameSource.TrackingId = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the detector is currently paused
    /// If the body tracking ID associated with the detector is not valid, then the detector should be paused
    /// </summary>
    public bool IsPaused
    {
        get
        {
            return this.vgbFrameReader.IsPaused;
        }

        set
        {
            if (this.vgbFrameReader.IsPaused != value)
            {
                this.vgbFrameReader.IsPaused = value;
            }
        }
    }

    /// <summary>
    /// Disposes all unmanaged resources for the class
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the VisualGestureBuilderFrameSource and VisualGestureBuilderFrameReader objects
    /// </summary>
    /// <param name="disposing">True if Dispose was called directly, false if the GC handles the disposing</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (this.vgbFrameReader != null)
            {
                this.vgbFrameReader.FrameArrived -= this.Reader_GestureFrameArrived;
                this.vgbFrameReader.Dispose();
                this.vgbFrameReader = null;
            }

            if (this.vgbFrameSource != null)
            {
                this.vgbFrameSource.TrackingIdLost -= this.Source_TrackingIdLost;
                this.vgbFrameSource.Dispose();
                this.vgbFrameSource = null;
            }
        }
    }

    /// <summary>
    /// Handles gesture detection results arriving from the sensor for the associated body tracking Id
    /// </summary>
    /// <param name="sender">object sending the event</param>
    /// <param name="e">event arguments</param>
    private void Reader_GestureFrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
    {
        VisualGestureBuilderFrameReference frameReference = e.FrameReference;
        using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
        {
            if (frame != null)
            {
                // get the discrete gesture results which arrived with the latest frame
                var discreteResults = frame.DiscreteGestureResults;

                if (discreteResults != null)
                {
                    // we only have one gesture in this source object, but you can get multiple gestures
                    foreach (Gesture gesture in this.vgbFrameSource.Gestures)
                    {

                        if (gesture.Name.Equals(this.leanLeftGestureName) && gesture.GestureType == GestureType.Discrete)
                        {
                            DiscreteGestureResult result = null;
                            discreteResults.TryGetValue(gesture, out result);

                            if (result != null)
                            {
                                if (this.OnGestureDetected != null)
                                {
                                    this.OnGestureDetected(this, new GestureEventArgs(true, result.Detected, result.Confidence, this.leanLeftGestureName));
                                }
                            }
                        }

                        if (gesture.Name.Equals(this.leanRightGestureName) && gesture.GestureType == GestureType.Discrete)
                        {
                            DiscreteGestureResult result = null;
                            discreteResults.TryGetValue(gesture, out result);

                            if (result != null)
                            {
                                if (this.OnGestureDetected != null)
                                {
                                    this.OnGestureDetected(this, new GestureEventArgs(true, result.Detected, result.Confidence, this.leanRightGestureName));
                                }
                            }
                        }

                        if (gesture.Name.Equals(this.flickGestureName) && gesture.GestureType == GestureType.Discrete)
                        {
                            DiscreteGestureResult result = null;
                            discreteResults.TryGetValue(gesture, out result);

                            if (result != null)
                            {
                                if (this.OnGestureDetected != null)
                                {
                                    this.OnGestureDetected(this, new GestureEventArgs(true, result.Detected, result.Confidence, this.flickGestureName));
                                }
                            }
                        }

                        if (gesture.Name.Equals(this.pressGestureName) && gesture.GestureType == GestureType.Discrete)
                        {
                            DiscreteGestureResult result = null;
                            discreteResults.TryGetValue(gesture, out result);

                            if (result != null)
                            {
                                if (this.OnGestureDetected != null)
                                {
                                    this.OnGestureDetected(this, new GestureEventArgs(true, result.Detected, result.Confidence, this.pressGestureName));
                                }
                            }
                        }

                        if (gesture.Name.Equals(this.pushGestureName) && gesture.GestureType == GestureType.Discrete)
                        {
                            DiscreteGestureResult result = null;
                            discreteResults.TryGetValue(gesture, out result);

                            if (result != null)
                            {
                                if (this.OnGestureDetected != null)
                                {
                                    this.OnGestureDetected(this, new GestureEventArgs(true, result.Detected, result.Confidence, this.pushGestureName));
                                }
                            }
                        }

                        if (gesture.Name.Equals(this.squatGestureName) && gesture.GestureType == GestureType.Discrete)
                        {
                            DiscreteGestureResult result = null;
                            discreteResults.TryGetValue(gesture, out result);

                            if (result != null)
                            {
                                if (this.OnGestureDetected != null)
                                {
                                    this.OnGestureDetected(this, new GestureEventArgs(true, result.Detected, result.Confidence, this.squatGestureName));
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Handles the TrackingIdLost event for the VisualGestureBuilderSource object
    /// </summary>
    /// <param name="sender">object sending the event</param>
    /// <param name="e">event arguments</param>
    private void Source_TrackingIdLost(object sender, TrackingIdLostEventArgs e)
    {
        if (this.OnGestureDetected != null)
        {
            this.OnGestureDetected(this, new GestureEventArgs(false, false, 0.0f, "none"));
        }
    }
}
