using UnityEngine;
using System.Collections;

public class Clouds : MonoBehaviour {

	private WeatherController controller;
	private Color colour;

	private float scaleX;
	private float scaleY;
	private float scaleZ;

	private Vector3 cloudPosition;	
	public Vector3 CloudPosition {
		get{return this.cloudPosition;}
		set{this.cloudPosition = value;}
	}

	// Use this for initialization
	void Start () {	
		controller = FindObjectOfType<WeatherController> ();
		this.colour = controller.cloudColor;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void generateRandomClouds () {	
		cloudPosition = new Vector3(Random.Range(-20,20), Random.Range(5,-5),Random.Range(-20,20));		
	}
}
