using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour {

    public GameObject[]prefabs;

    public int beatIndex = 0;

    MeshGenerator meshGeneratorScript;

    private bool spawnAllowed = true;

    public float spawnTime = 0.05f;
    private ColorGenerator colorGenerator;

    // Defines, which prefab should be spawned.
    public int spawnIndex = 0;

    public void placeAssets(BeatEventManager.BeatIndex index)
    {
        if (this.beatIndex == (int)index && spawnAllowed)
        {
            Color color = colorGenerator.GenColor(1f, 1f);
            Vector3[] positions = meshGeneratorScript.GetSpawnPositions();
            if ( Random.Range(0.0f, 100.0f) <= 5)
            {
                spawnIndex = 1;
            } else
            {
                spawnIndex = 0;
            }
            if (positions != null)
            {
                // Spawn the Lanterns   
                if (spawnIndex == 0)
                {
                    GameObject spawnedAsset1 = Instantiate(prefabs[spawnIndex]);
                    spawnedAsset1.transform.position = (positions[0]);
                    spawnedAsset1.transform.Rotate(0, 0, -90);
                    spawnedAsset1.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);

                    GameObject spawnedAsset2 = Instantiate(prefabs[spawnIndex]);
                    spawnedAsset2.transform.position = (positions[1]);
                    spawnedAsset2.transform.Rotate(0, 0, 90);
                    spawnedAsset2.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);

                    meshGeneratorScript.ObjectsToMove.Add(spawnedAsset1);
                    meshGeneratorScript.ObjectsToMove.Add(spawnedAsset2);

                    spawnAllowed = false;
                    StartCoroutine(WaitforEnable());
                }
                // Spawn the bridge
                else if(spawnIndex == 1)
                {
                    spawnAllowed = false;
                    GameObject spawnedAsset = Instantiate(prefabs[spawnIndex]);
                    spawnedAsset.transform.position = ((positions[0] - positions[1]) * 0.5f) + positions[1];
                    spawnedAsset.transform.Rotate(0, 0, 0);
                    spawnedAsset.GetComponent<Renderer>().material.SetColor("_EmissionColor", color); ;
                    StartCoroutine(WaitforEnable());

                    meshGeneratorScript.ObjectsToMove.Add(spawnedAsset);

                    StartCoroutine(WaitBridgeEnable(3f));
                }  
            }
        }
    }

    IEnumerator WaitforEnable()
    {
        yield return new WaitForSeconds(spawnTime);
        spawnAllowed = true;
    }
    IEnumerator WaitBridgeEnable(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        spawnAllowed = true;
    }
    // Use this for initialization
    void Start () {
        meshGeneratorScript = FindObjectOfType<MeshGenerator>();
        colorGenerator = FindObjectOfType<ColorGenerator>();
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}
