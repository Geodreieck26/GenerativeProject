using UnityEngine;
using System.Collections;
using System;

public class CarScript : MonoBehaviour {

    public enum BeatIndex
    {
        Kick, Snare, Hihat
    }

    public int beatIndex = 3;

    // color generator script
    public ColorGenerator colorGenerator;

    // Defines, if the changing of the Object is allowed.
    private bool colorChangeAllowed = true;

    // Renderer of this game object.
    Renderer rendererComponent;

    [SerializeField]
    float colorChangeRate;

    // Changes the color according to the beat
    public void BeatChangeColor(BeatEventManager.BeatIndex index)
    {
        Color color = colorGenerator.GenColor(1f, 1f);
        if (this.beatIndex == (int)index && colorChangeAllowed == true)
        { 
            colorChangeAllowed = false;
            rendererComponent.material.SetColor("_EmissionColor", color);
            
            StartCoroutine(WaitforEnable());
        }
    }

    IEnumerator WaitforEnable()
    {
        if (colorChangeRate <= 0)
        {
            colorChangeRate = 0.1f;
        }
        yield return new WaitForSeconds(colorChangeRate);
        colorChangeAllowed = true;
    }

    // Use this for initialization
    void Start () {
        colorGenerator = FindObjectOfType<ColorGenerator>();
        rendererComponent = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update () {
	
	}
}
