using UnityEngine;
using System.Collections;

public class Clouds : MonoBehaviour {

	private GameObject cloud;
	private float timerHelper = 0f;
	private float randomRotation;

	// Use this for initialization
	void Start () {	
		cloud = gameObject;
		randomRotation = Random.Range(0f, 1f);

		if (randomRotation < 0.25f) {
			randomRotation = 0f;
		}else if (randomRotation < 0.5f && randomRotation > 0.25f){
			randomRotation = 90f;
		}else if (randomRotation < 0.75f && randomRotation > 0.5f){
			randomRotation = 270f;
		}else{
			randomRotation = 360f;
		}

		cloud.transform.RotateAround (this.transform.position, new Vector3(0,1,0),randomRotation);
		generateRandomClouds ();
	}
	
	// Update is called once per frame
	void Update () {	
		//destroyCloud ();
	}

	void generateRandomClouds () {	
		cloud.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);	
	}

	void destroyCloud() {
		timerHelper += Time.deltaTime;

		if(timerHelper >= Random.Range(3f,5f)) {
			Destroy(this.gameObject);
		}

	}
}
