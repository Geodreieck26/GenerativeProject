using UnityEngine;
using System.Collections;

public class LightningMaker : MonoBehaviour {

	public Lightning Lightningpref;
	private WeatherController controller;

	void Start () { 

		controller = GetComponent<WeatherController>();
		StartCoroutine (StartLightning ());
	}

	void Update () { 



	} 

	IEnumerator StartLightning () {

		while(true) {				
				Instantiate(Lightningpref, controller.LightningPosition, Quaternion.identity);
				Instantiate(Lightningpref, controller.LightningPosition, Quaternion.identity);
				yield return null;
		}
	}


}