using UnityEngine;
using System.Collections;

//[RequireComponent(typeof (AudioAnalyzer))]
//[RequireComponent(typeof (Lightning))]

/*public class WeatherGeneration : MonoBehaviour, AudioAnalyzer.AudioCallbacks {

	private AudioAnalyzer audioanalyzer;
	private LineRenderer linerenderer;

	private Lightning lightning;

	private int lightningVerticeCount = 6;
	private float lightningLifeTime = 0.25f;
	private float lightningLength;
	private float lightningLineWidth = 2.0f;
	private float lightningTimer = 5.0f;
	private float lightningOffset;
	private Material lightningMaterial;
	private Vector3 lightningDirection;

	private float weatherHeight = 200.0f;


	private float[] frequencies;
	private bool beat;

	protected int targetLayer;

	public int TargetLayer
	{
		get { return this.targetLayer; }
		set { this.targetLayer = value; }
	}

	public Vector3 Direction
	{
		get { return this.lightningDirection; }
		set { this.lightningDirection = value; }
	} 

	protected void Awake()
	{
		initLineRenderer();
	}

	// Use this for initialization
	void Start () {

		audioanalyzer = FindObjectOfType<AudioAnalyzer>();
		audioanalyzer.addAudioCallback(this);

	
	}
	
	// Update is called once per frame
	void Update () {

		this.generateWeather ();
		this.generateClouds ();
	
	}

	// ----- Audio stuff ---------------------------------------------------

	public void onOnbeatDetected (float beatStrength) {

		this.beated (beatStrength);
	
	}

	public void onSpectrum (float[] spectrum) {

	}

	/// ----- General Weather ----------------------------------------------

	private void generateWeather () {
	
		frequencies = audioanalyzer.GetFrequencyData ();
	
		for (int i = 0; i < frequencies.Length; i++) {
		
			if(frequencies[i] > 1500) {



			}
		}

	}


	// ----- Lightnings -----------------------------------------------------
	private void addLighting () {

		RaycastHit hitInfo;

		if(Physics.Raycast(transform.position, Direction, out hitInfo, lightningLength)){

			this.generateLighting(hitInfo);
		}

	/*	if (beat) {
		
			this.generateLighting(hitInfo);

			beat = false;
		}
	*/

/*	}

	private void generateLighting (RaycastHit hitInfo) {
	
		float distance = Vector3.Distance(transform.position, hitInfo.transform.position);
		linerenderer.SetPosition(0, transform.position);
		linerenderer.SetPosition(lightningVerticeCount - 1, transform.position + (Direction * distance));
		calculateLinePoints(distance);
	} 


	private void initLineRenderer () {
	
		linerenderer = gameObject.AddComponent<LineRenderer>();
		linerenderer.material = lightningMaterial;
		linerenderer.SetWidth(lightningLineWidth / 2f, lightningLineWidth / 2f);
		linerenderer.SetColors(Color.white, Color.white);
		linerenderer.SetVertexCount(lightningVerticeCount);
	
	}

	private void calculateLinePoints(float distance) {

		float step = distance / lightningVerticeCount;
		float stepAdd = step;
		Vector3 offset = Vector3.zero;
		
		for (int i = 0; i < lightningVerticeCount - 2; i++) {
			offset.x = Random.Range(-lightningOffset, lightningOffset);
			offset.z = Random.Range(-lightningOffset, lightningOffset);
			offset.y = Random.Range(-lightningOffset, lightningOffset);
			
			linerenderer.SetPosition(i + 1, transform.position + offset +  Direction * stepAdd);
			stepAdd += step;
		}

	}

	private void beated (float strength) {	

		beat = true;
		this.addLighting ();
	
	}

	// ----- Clouds -----------------------------------------------------

	private void addCloud () {
	



	}

	private void generateClouds () {
	
	
	} 
}*/
