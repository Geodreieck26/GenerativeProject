using UnityEngine;
using System.Collections;

public class Clouds : MonoBehaviour {

	private GameObject cloud;
	private float timerHelper = 0f;

	// Use this for initialization
	void Start () {	
		cloud = gameObject;
		cloud.transform.RotateAround (this.transform.position, new Vector3(0,1,0),270f);
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
