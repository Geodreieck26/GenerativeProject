using UnityEngine;
using System.Collections;

public class LightningMaker : MonoBehaviour {

	public Lightning Lightningpref; 
	private bool stop;

	IEnumerator Start() {
	
		while(!stop){
			Instantiate(Lightningpref, this.transform.position, Quaternion.identity);
			Instantiate(Lightningpref, this.transform.position, Quaternion.identity);
			yield return null;
		}
	
	}


	void OnDisable() {
		stop = true;	
	}

}