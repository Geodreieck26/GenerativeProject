﻿using UnityEngine;
using System.Collections;

public class WeatherController : MonoBehaviour {
	
	public ColorGenerator colorGenerator;
	public AudioAnalyzer analyzer;
	public GameObject Lightningpref;
	public GameObject Cloudpref;

	public Camera cam;

	[SerializeField]
	[Range(5.0f, 25.0f)]
	public float weatherHeight = 10f;

	[SerializeField]
	[Range(5.0f, 50.0f)]
	public int lightningSegments = 10;

	[SerializeField]
	[Range(0.1f, 0.6f)]
	public float lightningWidth = 0.3f;

	[SerializeField]
	[Range(0.1f, 0.6f)]
	public float lightningOffset = 0.3f;

	[SerializeField]
	[Range(0.1f, 1f)]
	public float scaleCloudX = 0.3f;

	[SerializeField]
	[Range(0.1f, 0.3f)]
	public float scaleCloudY = 0.3f;

	[SerializeField]
	[Range(0.1f, 1f)]
	public float scaleCloudZ = 0.3f;

	public Color lightningColor;
	public Color cloudColor;

	private Vector3 lightningPos;

	public Vector3 LightningPos {
		get{return this.lightningPos;}
		set{this.lightningPos = value;}
	}

	private Vector3 cloudPos;
	
	public Vector3 CloudPos {
		get{return this.cloudPos;}
		set{this.cloudPos = value;}
	}

	public enum BeatIndex {
		Kick, Snare, Hihat
	}

	private GameObject[] clouds;

	// Use this for initialization
	void Start () {

		analyzer = FindObjectOfType<AudioAnalyzer>();
		colorGenerator = FindObjectOfType<ColorGenerator>();
		cam = FindObjectOfType<Camera> ();
		FindObjectOfType<BeatDetection>().CallBackFunction = BeatCallbackEventHandler;

		LightningPos = new Vector3 (cam.transform.position.x, weatherHeight, cam.transform.position.z);
		CloudPos = new Vector3 (cam.transform.position.x, weatherHeight, cam.transform.position.z);

	}
	
	// Update is called once per frame
	void Update () {
	}

	void scaleCloud() {
		Cloudpref.transform.localScale = new Vector3 (scaleCloudX,scaleCloudY,scaleCloudZ);
	}

	void generateClouds () {
		Instantiate (Cloudpref, CloudPos, Quaternion.identity);
		scaleCloud ();
	}

	void generateLightning () {
		Destroy(Instantiate (Lightningpref, lightningPos, Quaternion.identity) as GameObject, 0.3f);
	}

	// handles the beat events
	public void BeatCallbackEventHandler(BeatDetection.EventInfo eventInfo) {
		switch (eventInfo.messageInfo)
		{
		case BeatDetection.EventType.Energy:
			break;
		case BeatDetection.EventType.HitHat:
			BeatSetLightning(BeatIndex.Hihat);
			BeatSetClouds(BeatIndex.Hihat);
			break;
		case BeatDetection.EventType.Kick:
			BeatChangeColor(BeatIndex.Kick, true);
			break;
		case BeatDetection.EventType.Snare:
			BeatChangeColor(BeatIndex.Snare, false);
			break;
		}
	}

	void BeatSetClouds(BeatIndex index) {
		generateClouds ();
	}

	void BeatSetLightning(BeatIndex index) {
		generateLightning ();
	}

	void BeatChangeColor(BeatIndex index, bool lightning) {
		if(lightning) {
			lightningColor = colorGenerator.GenColor(1f, 1f);
		} else {
			cloudColor = colorGenerator.GenColor (1f,1f);

			clouds = GameObject.FindGameObjectsWithTag("Cloud");

			for (int i = 0; i < clouds.Length; i++){
				clouds[i].GetComponentsInChildren<Renderer>()[0].material.SetColor("_EmissionColor", cloudColor);
			}
		}
	} 
}
