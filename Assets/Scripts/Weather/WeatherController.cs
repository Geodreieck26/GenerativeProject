using UnityEngine;
using System.Collections;

[RequireComponent(typeof (LightningMaker))]

public class WeatherController : MonoBehaviour {

	public AudioAnalyzer analyzer;
	public GameObject Lightningpref;

	private Vector3 lightningPosition;

	public Vector3 LightningPosition {
		get{return this.lightningPosition;}
		set{this.lightningPosition = value;}
	}

	private float scaleLightning = 0f;
	private bool sendLightning;
	private float timeLeft = 0.1f;

	public bool SendLightning {
		get{return this.sendLightning;}
		set{this.sendLightning = value;}
	}

	private GameObject actualMesh;
	private Mesh mesh;
	private Vector3[] vertices;

	private float[] frequencies = new float[0];

	private float weatherHeight = 3f;

	// Use this for initialization
	void Start () {

		analyzer = FindObjectOfType<AudioAnalyzer>();
		//analyzer.addAudioCallback(this);

		actualMesh = GameObject.FindGameObjectWithTag ("Mesh");
		mesh = actualMesh.GetComponent<MeshFilter>().mesh;
		vertices = mesh.vertices; 

	}
	
	// Update is called once per frame
	void Update () {
		frequencies = analyzer.GetFrequencyData ();
	
	}

	void generateLightnings () {
		Destroy(Instantiate (Lightningpref, this.transform.position, Quaternion.identity) as GameObject, 0.3f);
	}

	public void onOnbeatDetected (float beatStrength) {
		

		
	}
	
	public void onSpectrum (float[] spectrum) {
		
	}
}
