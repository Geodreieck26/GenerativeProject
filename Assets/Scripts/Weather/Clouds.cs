using UnityEngine;
using System.Collections;

public class Clouds : MonoBehaviour {

	private WeatherController controller;
	private GameObject cloud;
	private Color colour;

	private float scaleX;
	private float scaleZ;

	private float timerHelper = 0f;

	private Vector3 cloudPosition;	
	public Vector3 CloudPosition {
		get{return this.cloudPosition;}
		set{this.cloudPosition = value;}
	}

	// Use this for initialization
	void Start () {	
		controller = FindObjectOfType<WeatherController> ();
		cloud = gameObject;
		cloud.transform.RotateAround (this.transform.position, new Vector3(0,1,0),270f);
		this.colour = controller.cloudColor;

		generateRandomClouds ();
	}
	
	// Update is called once per frame
	void Update () {	
		destroyCloud ();
	}

	void generateRandomClouds () {	
		cloudPosition = new Vector3(Random.Range(-20,20), Random.Range(this.transform.position.y+3,this.transform.position.y-3),Random.Range(-20,20));	
		cloud.transform.position = cloudPosition;

		scaleX = Random.Range (0,1);
		scaleZ = Random.Range (0,1);

		cloud.transform.localScale += new Vector3 (scaleX,0f,scaleZ);
	}

	void destroyCloud() {
		timerHelper += Time.deltaTime;

		if(timerHelper >= Random.Range(3f,5f)) {
			Destroy(this.gameObject);
		}

	}
}
