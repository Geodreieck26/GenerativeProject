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
	[Range(30.0f, 100.0f)]
	public float cloudHeightRange = 40f;

	[SerializeField]
	[Range(10.0f, 50.0f)]
	public int lightningSegments = 25;

	[SerializeField]
	[Range(6f, 10f)]
	public float lightningWidth = 8f;

	[SerializeField]
	[Range(5f, 10f)]
	public float lightningOffset = 15f;

	[SerializeField]
	[Range(0.1f, 1f)]
	public float lightningProbability = 0.5f;

	[SerializeField]
	[Range(1f, 2f)]
	public float scaleCloudX = 1f;

	[SerializeField]
	[Range(1f, 2f)]
	public float scaleCloudY = 1f;

	[SerializeField]
	[Range(1f, 2f)]
	public float scaleCloudZ = 1f;

	[SerializeField]
	[Range(0.3f, 2f)]
	public float cloudSpawnRate = 0.3f;

	[SerializeField]
	[Range(3f, 5f)]
	public float cloudDistMult = 4f;

	[SerializeField]
	[Range(0f, 360f)]
	public float randomCloudAngle = 90f;

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

	private float timerHelperCloudColor = 0f;
	private float timerHelperCloudSpawn = 0f;
	private bool setCloud;

	private float lightPosWidth = 200f;
	private float lightNearPlane = 350f;
	private float lightFarPlane = 1000f;

	private Vector3 cloudRotate = new Vector3(0,1,0);


	// Use this for initialization
	void Start () {
		colorGenerator = FindObjectOfType<ColorGenerator>();
		generator = FindObjectOfType<MeshGenerator> ();
		objectsToMove = generator.ObjectsToMove;

		pos = generator.CloudSpawns;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		timerHelperCloudColor -= Time.deltaTime;
		timerHelperCloudSpawn += Time.deltaTime;

		if (setCloud) {
			generateClouds ();
			setCloud = false;
		} else if(timerHelperCloudSpawn >= cloudSpawnRate) {
			setCloud = true;
			timerHelperCloudSpawn = 0f;
		}

		generateLightning ();

	}
	

	void generateClouds () {
		cloudPos = new Vector3 (pos[0].x * cloudDistMult, Random.Range (weatherHeight + cloudHeightRange, weatherHeight - cloudHeightRange), Random.Range(pos[0].z, pos[1].z));

		if(timerHelperCloudColor <= 0f) {
			col = colorGenerator.GenColor (1f,1f);
			timerHelperCloudColor = 3f;
		}

		GameObject cloud = Instantiate (Cloudpref, CloudPos, Quaternion.identity) as GameObject;
		//cloud.transform.RotateAround (this.transform.position, cloudRotate , randomCloudAngle);
		cloud.transform.localScale = scaleCloud ();
		cloud.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", col);
		objectsToMove.Add (cloud);
	}

	Vector3 scaleCloud() {
		return new Vector3 (scaleCloudX, scaleCloudY, scaleCloudZ);
	}

	void generateLightning () {

		float rndX = Random.Range (pos[0].x - lightFarPlane, pos[0].x - lightNearPlane);
		float rndZ = Random.Range (pos[0].z + lightPosWidth, pos[1].z - lightPosWidth);
		LightningPos = new Vector3 (rndX, cloudPos.y, rndZ);
		Destroy(Instantiate (Lightningpref, lightningPos, Quaternion.identity) as GameObject, 0.3f);

	}


	public void BeatSetLightning() {
		float rnd = Random.Range (0f,1f);

		if(rnd < (0.5f)) {
			generateLightning ();
		}
	} 
}
