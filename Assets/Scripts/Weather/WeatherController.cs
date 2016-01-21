using UnityEngine;
using System.Collections;

public class WeatherController : MonoBehaviour {
	
	public ColorGenerator colorGenerator;
	public AudioAnalyzer analyzer;
	public GameObject Lightningpref;

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

	public Color lightningColor;

	private Vector3 lightningPosition;

	public Vector3 LightningPosition {
		get{return this.lightningPosition;}
		set{this.lightningPosition = value;}
	}

	private GameObject actualMesh;
	private Mesh mesh;
	private Vector3[] vertices;

	private float[] frequencies = new float[0];

	public enum BeatIndex {
		Kick, Snare, Hihat
	}

	// Use this for initialization
	void Start () {

		analyzer = FindObjectOfType<AudioAnalyzer>();
		colorGenerator = FindObjectOfType<ColorGenerator>();
		FindObjectOfType<BeatDetection>().CallBackFunction = BeatCallbackEventHandler;

		actualMesh = GameObject.FindGameObjectWithTag ("Mesh");
		mesh = actualMesh.GetComponent<MeshFilter>().mesh;
		vertices = mesh.vertices; 

		LightningPosition = new Vector3 (0,weatherHeight,0);

	}
	
	// Update is called once per frame
	void Update () {
		frequencies = analyzer.GetFrequencyData ();

	}

	void checkLightning() {

	
	}

	void generateLightning () {
		Destroy(Instantiate (Lightningpref, lightningPosition, Quaternion.identity) as GameObject, 0.3f);
	}

	public void onOnbeatDetected (float beatStrength) {
		

		
	}
	
	public void onSpectrum (float[] spectrum) {
		
	}

	// handles the beat events
	public void BeatCallbackEventHandler(BeatDetection.EventInfo eventInfo)
	{
		switch (eventInfo.messageInfo)
		{
		case BeatDetection.EventType.Energy:
			break;
		case BeatDetection.EventType.HitHat:
			BeatSetLightning(BeatIndex.Hihat);
			break;
		case BeatDetection.EventType.Kick:
			BeatChangeColor(BeatIndex.Kick);
			break;
		case BeatDetection.EventType.Snare:
			break;
		}
	}

	void BeatSetLightning(BeatIndex index) {
		generateLightning ();
	}

	void BeatChangeColor(BeatIndex index) {
		lightningColor = colorGenerator.GenColor(1f, 1f);
	} 
}
