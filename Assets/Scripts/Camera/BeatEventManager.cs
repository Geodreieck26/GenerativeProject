using UnityEngine;
using System.Collections;
using System;

public class BeatEventManager : MonoBehaviour {

    public enum BeatIndex
    {
        Kick, Snare, Hihat
    }

    public CarScript carScript;
    public CameraScript cameraScript;
    public SpawnManager spawnManager;
    public FlockController flockController;

    // Use this for initialization
    void Start () {
        FindObjectOfType<BeatDetection>().CallBackFunction = BeatCallbackEventHandler;
        carScript = FindObjectOfType<CarScript>();
        cameraScript = FindObjectOfType<CameraScript>();
        spawnManager = FindObjectOfType<SpawnManager>();
        flockController = FindObjectOfType<FlockController>();
    }

    private void BeatCallbackEventHandler(BeatDetection.EventInfo eventInfo)
    {
        switch (eventInfo.messageInfo)
        {
            case BeatDetection.EventType.Energy:
                break;
            case BeatDetection.EventType.HitHat:
                if (carScript)
                    carScript.BeatChangeColor(BeatIndex.Hihat);
                if(cameraScript)
                    cameraScript.calculateWaypoint(BeatIndex.Hihat);
                if(spawnManager)
                    spawnManager.placeAssets(BeatIndex.Hihat);
                if(flockController)
                    flockController.BeatChangeColor(BeatIndex.Hihat);
                break;
            case BeatDetection.EventType.Kick:
                if (carScript)
                    carScript.BeatChangeColor(BeatIndex.Kick);
                if (cameraScript)
                    cameraScript.calculateWaypoint(BeatIndex.Kick);
                if (spawnManager)
                    spawnManager.placeAssets(BeatIndex.Kick);
                if (flockController)
                    flockController.BeatChangeColor(BeatIndex.Kick);
                break;
            case BeatDetection.EventType.Snare:
                if (cameraScript)
                    cameraScript.calculateWaypoint(BeatIndex.Snare);
                if (carScript)
                    carScript.BeatChangeColor(BeatIndex.Snare);
                if (spawnManager)
                    spawnManager.placeAssets(BeatIndex.Snare);
                if (flockController)
                    flockController.BeatChangeColor(BeatIndex.Snare);
                break;
        }
    }
}
