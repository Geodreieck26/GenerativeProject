using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour {

    public GameObject[]prefabs;

    public int beatIndex = 0;

    MeshGenerator meshGeneratorScript;

    private bool spawnAllowed = true;

    public float spawnTime = 0.05f;

    public void placeAssets(BeatEventManager.BeatIndex index)
    {
        if (this.beatIndex == (int)index && spawnAllowed)
        {
            Vector3[] positions = meshGeneratorScript.GetSpawnPositions();
            if (positions != null)
            {
                GameObject spawnedAsset1 = Instantiate(prefabs[0]);
                spawnedAsset1.transform.position = (positions[0]);

                GameObject spawnedAsset2 = Instantiate(prefabs[0]);
                spawnedAsset2.transform.position = (positions[1]);

                meshGeneratorScript.ObjectsToMove.Add(spawnedAsset1);
                meshGeneratorScript.ObjectsToMove.Add(spawnedAsset2);

                spawnAllowed = false;
                StartCoroutine(WaitforEnable());
            }
        }
    }

    IEnumerator WaitforEnable()
    {
        yield return new WaitForSeconds(spawnTime);
        spawnAllowed = true;
    }

    // Use this for initialization
    void Start () {
        meshGeneratorScript = FindObjectOfType<MeshGenerator>();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
