using UnityEngine;
using System.Collections.Generic;

public class WeatherController : MonoBehaviour {
	
	public ColorGenerator colorGenerator;
	public GameObject Lightningpref;
	public GameObject Cloudpref;

	private MeshGenerator generator;
	private List<GameObject> objectsToMove;
	private Color col;

	[SerializeField]
	[Range(400.0f, 550.0f)]
	public float weatherHeight = 450f;

	[SerializeField]
	[Range(30.0f, 50.0f)]
	public float cloudHeightRange = 40f;

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
	[Range(1f, 20f)]
	public float lightningDist = 10f;

	[SerializeField]
	[Range(0.1f, 1f)]
	public float lightningProbability = 0.5f;

	[SerializeField]
	[Range(0.7f, 2f)]
	public float scaleCloudX = 1f;

	[SerializeField]
	[Range(0.7f, 2f)]
	public float scaleCloudY = 1f;

	[SerializeField]
	[Range(0.7f, 2f)]
	public float scaleCloudZ = 1f;

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

	private Vector3[] pos;

	public enum BeatIndex {
		Kick, Snare, Hihat
	}

	private float timerHelper = 0f;

	// Use this for initialization
	void Start () {
		colorGenerator = FindObjectOfType<ColorGenerator>();
		generator = FindObjectOfType<MeshGenerator> ();
		objectsToMove = generator.ObjectsToMove;

		pos = generator.CloudSpawns;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		timerHelper -= Time.deltaTime;
		generateClouds ();
		generateLightning ();

	}
	

	void generateClouds () {
		cloudPos = new Vector3 (pos[0].x*3, Random.Range (weatherHeight + cloudHeightRange, weatherHeight - cloudHeightRange), Random.Range(pos[0].z, pos[1].z));

		if(timerHelper <= 0f) {
			col = colorGenerator.GenColor (1f,1f);
			timerHelper = 3f;
		}

		GameObject cloud = Instantiate (Cloudpref, CloudPos, Quaternion.identity) as GameObject;
		cloud.transform.localScale = scaleCloud ();
		cloud.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", col);
		objectsToMove.Add (cloud);
	}

	Vector3 scaleCloud() {
		return new Vector3 (scaleCloudX, scaleCloudY, scaleCloudZ);
	}

	void generateLightning () {
		float rndX = Random.Range (pos[0].x - lightningDist, pos[0].x + lightningDist);
		LightningPos = new Vector3 (rndX, weatherHeight, Random.Range(pos[0].z, pos[1].z));
		Destroy(Instantiate (Lightningpref, lightningPos, Quaternion.identity) as GameObject, 0.3f);
	}


	public void BeatSetLightning() {
		float rnd = Random.Range (0f,1f);

		if(rnd < (0.5f * lightningProbability)) {
			generateLightning ();
		}
	} 
}
