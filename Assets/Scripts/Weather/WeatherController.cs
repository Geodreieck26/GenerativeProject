using UnityEngine;
using System.Collections.Generic;

public class WeatherController : MonoBehaviour {
	
	public ColorGenerator colorGenerator;
	public GameObject Lightningpref;
	public GameObject Cloudpref;

	private MeshGenerator meshGenerator;
	private List<GameObject> objectsToMove;
	private Color col;

	[SerializeField]
	[Range(300.0f, 450.0f)]
	public float weatherHeight = 400f;

	[SerializeField]
	[Range(30.0f, 100.0f)]
	public float cloudHeightRange = 40f;

	[SerializeField]
	[Range(500.0f, 700.0f)]
	public int lightningSegments = 500;

	[SerializeField]
	[Range(8f, 15f)]
	public float lightningWidth = 10f;

	[SerializeField]
	[Range(10f, 17f)]
	public float lightningOffset = 15f;

	[SerializeField]
	[Range(0.5f, 1f)]
	public float lightningProbability = 0.8f;

	[SerializeField]
	[Range(0.2f, 1f)]
	public float cloudSpawnRate = 0.5f;

	[SerializeField]
	[Range(3f, 5f)]
	public float cloudDistMult = 4f;

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

	private float scaleCloudX = 1f;
	private float scaleCloudY = 1f;
	private float scaleCloudZ = 1f;

	private float timerHelperCloudColor = 0f;
	private float timerHelperCloudSpawn = 0f;
	private float timerHelperLightningSpawn = 0f;
	private bool setCloud;
	private bool setLightning;

	private float lightPosWidth = 100f;
	private float lightNearPlane = 350f;
	private float lightFarPlane = 2500f;

	private float lightningHeight = 5000f;


	// Use this for initialization
	void Start () {
		colorGenerator = FindObjectOfType<ColorGenerator>();
		meshGenerator = FindObjectOfType<MeshGenerator> ();
		objectsToMove = meshGenerator.ObjectsToMove;

		pos = meshGenerator.CloudSpawns;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

			timerHelperCloudColor -= Time.deltaTime;
			timerHelperCloudSpawn += Time.deltaTime;
			timerHelperLightningSpawn += Time.deltaTime;

			if (meshGenerator.IsSectorFreeway ()) {
				cloudSpawnRate = 0.1f;
			} else {
				cloudSpawnRate = 0.8f;
			}

			if (setCloud) {
				generateClouds();
				setCloud = false;
			} else if(timerHelperCloudSpawn >= cloudSpawnRate) {
				setCloud = true;
				timerHelperCloudSpawn = 0f;
			}

	}
	

	void generateClouds () {
		cloudPos = new Vector3 (pos[0].x * cloudDistMult, Random.Range (weatherHeight + cloudHeightRange, weatherHeight - cloudHeightRange), Random.Range(pos[0].z, pos[1].z));

		if(timerHelperCloudColor <= 0f) {
			col = colorGenerator.GenColor (1f,1f);
			timerHelperCloudColor = 2f;
		}

		GameObject cloud = Instantiate (Cloudpref, CloudPos, Quaternion.identity) as GameObject;
		cloud.transform.localScale = scaleCloud ();
		cloud.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", col);
		objectsToMove.Add (cloud);
	}

	Vector3 scaleCloud() {
		if (!meshGenerator.IsSectorFreeway()) {
			scaleCloudX = 2.5f;
			scaleCloudY = 2.5f;
			scaleCloudZ = 2.5f;
		} else {
			scaleCloudX = 1.3f;
			scaleCloudY = 1.3f;
			scaleCloudZ = 1.3f;
		}
		return new Vector3 (Random.Range (scaleCloudX+0.5f, scaleCloudX-0.5f), Random.Range (scaleCloudY+0.5f, scaleCloudY-0.5f),Random.Range (scaleCloudZ+0.5f, scaleCloudZ-0.5f));
	}

	void generateLightning () {

		float rndX = Random.Range (pos[0].x - lightFarPlane, pos[0].x - lightNearPlane);
		float rndZ = Random.Range (pos[0].z - lightPosWidth, pos[1].z + lightPosWidth);
		LightningPos = new Vector3 (rndX, lightningHeight, rndZ); 

		Destroy (Instantiate (Lightningpref, LightningPos, Quaternion.identity) as GameObject, 0.3f);

	}


	public void BeatSetLightning() {
		if (meshGenerator.IsSectorFreeway ()) {
			generateLightning ();
		} else {
			if(setLightning) {
				generateLightning();
				setLightning = false;
			} else if(timerHelperLightningSpawn > 0.5f) {
				setLightning = true;
				timerHelperLightningSpawn = 0f;
			}

		}
	} 
}
