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
	[Range(70.0f, 200.0f)]
	public float weatherHeight = 70f;

	[SerializeField]
	[Range(10.0f, 30.0f)]
	public float cloudHeightRange = 15f;

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
	[Range(0.01f, 0.1f)]
	public float scaleCloudX = 0.01f;

	[SerializeField]
	[Range(0.01f, 0.1f)]
	public float scaleCloudY = 0.01f;

	[SerializeField]
	[Range(0.01f, 0.1f)]
	public float scaleCloudZ = 0.01f;

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

	private float timerHelper = 0f;

	// Use this for initialization
	void Start () {
		colorGenerator = FindObjectOfType<ColorGenerator>();
		generator = FindObjectOfType<MeshGenerator> ();
		objectsToMove = generator.ObjectsToMove;

	}
	
	// Update is called once per frame
	void Update () {

		generateClouds ();
		generateLightning ();

		timerHelper += Time.deltaTime;
	}

	void generateClouds () {
		Vector3[] pos = generator.GetSpawnPositions ();
		cloudPos = new Vector3 (pos[0].x, Random.Range (weatherHeight + cloudHeightRange, weatherHeight - cloudHeightRange), Random.Range(pos[0].z, pos[1].z));

		if(timerHelper <= 0f) {
			col = colorGenerator.GenColor (1f,1f);
			timerHelper = 5f;
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
		Vector3[] pos = generator.GetSpawnPositions ();
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
