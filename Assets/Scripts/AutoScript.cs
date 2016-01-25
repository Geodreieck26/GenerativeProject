using UnityEngine;
using System.Collections;

public class AutoScript : MonoBehaviour {

    public float speed = 1f;
    ColorGenerator colorGenerator;
    // Use this for initialization
    void Start () {
        colorGenerator = FindObjectOfType<ColorGenerator>();
        Color color = colorGenerator.GenColor(1f, 1f);
        GetComponent<Renderer>().material.SetColor("_EmissionColor", color);
    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(transform.forward * Time.deltaTime);
	}
}
