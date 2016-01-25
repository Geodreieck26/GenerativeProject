using UnityEngine;
using System.Collections;
using System;

public class BeatEventManager : MonoBehaviour {

    public enum BeatIndex
    {
        Kick, Snare, Hihat
    }

    public int beatIndex = 3;

    // Defines, if the changing of the Object is allowed.
    private bool colorChangeAllowed = true;

    // sound analyzer script
    private AudioAnalyzer audioAnalyzer;

    // color generator script
    public ColorGenerator colorGenerator;

    public CarScript carScript;
    public CameraScript cameraScript;
    public SpawnManager spawnManager;

    // Use this for initialization
    void Start () {
        FindObjectOfType<BeatDetection>().CallBackFunction = BeatCallbackEventHandler;
        carScript = FindObjectOfType<CarScript>();
        cameraScript = FindObjectOfType<CameraScript>();
        spawnManager = FindObjectOfType<SpawnManager>();
    }

    private void BeatCallbackEventHandler(BeatDetection.EventInfo eventInfo)
    {
        switch (eventInfo.messageInfo)
        {
            case BeatDetection.EventType.Energy:
                break;
            case BeatDetection.EventType.HitHat:
                carScript.BeatChangeColor(BeatIndex.Hihat);
                cameraScript.calculateWaypoint(BeatIndex.Hihat);
                spawnManager.placeAssets(BeatIndex.Hihat);
                break;
            case BeatDetection.EventType.Kick:
                carScript.BeatChangeColor(BeatIndex.Kick);
                cameraScript.calculateWaypoint(BeatIndex.Kick);
                spawnManager.placeAssets(BeatIndex.Kick);
                break;
            case BeatDetection.EventType.Snare:
                cameraScript.calculateWaypoint(BeatIndex.Snare);
                carScript.BeatChangeColor(BeatIndex.Snare);
                spawnManager.placeAssets(BeatIndex.Snare);
                break;
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
