using UnityEngine;
using System.Collections;

public class Tafal : MonoBehaviour {

    private Renderer rend;
    private ColorGenerator cGen;
    private bool colorChangeEnabled = true;
    [SerializeField]
    public float colorChangeRate = 0.05f;

    // Use this for initialization
    void Start () {
        rend = GetComponent<Renderer>();
        cGen = FindObjectOfType<ColorGenerator>();
    }
	
	public void Change () {
        if (colorChangeEnabled)
        {
            rend.materials[2].SetColor("_EmissionColor", cGen.GenColor(1f, 1f));
            colorChangeEnabled = false;
            StartCoroutine(WaitforEnable());
        }
    }

    IEnumerator WaitforEnable()
    {
        yield return new WaitForSeconds(colorChangeRate);
        colorChangeEnabled = true;
    }
}
