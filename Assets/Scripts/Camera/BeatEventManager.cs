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
	public WeatherController weatherController;

    // Use this for initialization
    void Start () {
        FindObjectOfType<BeatDetection>().CallBackFunction = BeatCallbackEventHandler;
        carScript = FindObjectOfType<CarScript>();
        cameraScript = FindObjectOfType<CameraScript>();
        spawnManager = FindObjectOfType<SpawnManager>();
		weatherController = FindObjectOfType<WeatherController> ();
    }

    private void BeatCallbackEventHandler(BeatDetection.EventInfo eventInfo)
    {
        GameObject[] flocks = GameObject.FindGameObjectsWithTag("Flock");

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
                foreach (GameObject flock in flocks)
                {
                    flock.GetComponent<FlockController>().BeatChangeColor(BeatIndex.Hihat);
                }
				if(weatherController)
					weatherController.BeatSetLightning();
                break;
            case BeatDetection.EventType.Kick:
                if (carScript)
                    carScript.BeatChangeColor(BeatIndex.Kick);
                if (cameraScript)
                    cameraScript.calculateWaypoint(BeatIndex.Kick);
                if (spawnManager)
                    spawnManager.placeAssets(BeatIndex.Kick);
                foreach (GameObject flock in flocks)
                {
                    flock.GetComponent<FlockController>().BeatChangeColor(BeatIndex.Kick);
                }
                GameObject[] tafaln = GameObject.FindGameObjectsWithTag("Tafalende");
                foreach(GameObject tafal in tafaln)
                {
                    tafal.GetComponent<Tafal>().Change();
                }

                break;
            case BeatDetection.EventType.Snare:
                if (cameraScript)
                    cameraScript.calculateWaypoint(BeatIndex.Snare);
                if (carScript)
                    carScript.BeatChangeColor(BeatIndex.Snare);
                if (spawnManager)
                    spawnManager.placeAssets(BeatIndex.Snare);
                foreach (GameObject flock in flocks)
                {
                    flock.GetComponent<FlockController>().BeatChangeColor(BeatIndex.Snare);
                }
                break;
        }
    }
}
