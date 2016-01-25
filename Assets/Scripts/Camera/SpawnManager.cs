using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour {

    public GameObject[]prefabs;

    public int beatIndex = 0;

    MeshGenerator meshGeneratorScript;

    private bool spawnAllowed = false;

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
            if ( Random.Range(0.0f, 100.0f) <= 10)
            {
                spawnIndex = 1;
            } else
            {
                spawnIndex = 0;
            }
            if (positions != null)
            {
                if (meshGeneratorScript.IsSectorFreeway())
                {
                    spawnIndex = 0;
                }
                    
                // Spawn the Lanterns   
                    if (spawnIndex == 0)
                {
                    if (meshGeneratorScript.IsSectorFreeway())
                    {
                        spawnIndex = 2;

                        float random = Random.Range(0.0f , 25f) + 25f;

                        GameObject spawnedAsset1 = Instantiate(prefabs[spawnIndex]);
                        spawnedAsset1.transform.position = new Vector3(positions[0].x, positions[0].y, positions[0].z + random);
                        spawnedAsset1.transform.Rotate(0, 0, 0);
                        spawnedAsset1.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);

                        random = Random.Range(0.0f, 25f) + 25f;

                        GameObject spawnedAsset2 = Instantiate(prefabs[spawnIndex]);
                        spawnedAsset2.transform.position = new Vector3(positions[1].x, positions[1].y, positions[1].z - random);
                        spawnedAsset2.transform.Rotate(0, 0, 0);
                        spawnedAsset2.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);

                        meshGeneratorScript.ObjectsToMove.Add(spawnedAsset1);
                        meshGeneratorScript.ObjectsToMove.Add(spawnedAsset2);
                    } else
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
                    }
                   

                    spawnAllowed = false;
                    StartCoroutine(WaitforEnable());
                }
                // Spawn the bridge
                else if(spawnIndex == 1)
                {
                    spawnAllowed = false;
                    GameObject spawnedAsset = Instantiate(prefabs[spawnIndex]);
                    spawnedAsset.transform.position = ((positions[0] - positions[1]) * 0.5f) + positions[1];
                    spawnedAsset.transform.position =new Vector3(spawnedAsset.transform.position.x , spawnedAsset.transform.position.y - 1f, spawnedAsset.transform.position.z);
                    spawnedAsset.transform.Rotate(0, 0, 0);
                    spawnedAsset.GetComponent<Renderer>().material.SetColor("_EmissionColor", color); ;
                    StartCoroutine(WaitforEnable());

                    meshGeneratorScript.ObjectsToMove.Add(spawnedAsset);

                    StartCoroutine(WaitBridgeEnable(5f));
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
    IEnumerator WaitOnStart()
    {
        yield return new WaitForSeconds(3f);
        spawnAllowed = true;
    }
    // Use this for initialization
    void Start () {
        meshGeneratorScript = FindObjectOfType<MeshGenerator>();
        colorGenerator = FindObjectOfType<ColorGenerator>();
        StartCoroutine(WaitOnStart());
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}
