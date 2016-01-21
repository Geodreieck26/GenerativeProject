//by Bob Berkebile : Pixelplacement : http://www.pixelplacement.com

using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class iTweenPath : MonoBehaviour
{
    public bool newCalculate;
    public string pathName ="";
	public Color pathColor = Color.cyan;
	public List<Vector3> nodes = new List<Vector3>(){Vector3.zero, Vector3.zero};
	public int nodeCount;
	public static Dictionary<string, iTweenPath> paths = new Dictionary<string, iTweenPath>();
	public bool initialized = false;
	public string initialName = "";
    private GameObject[] waypoints;

    /*
    void Start()
    {
        Vector3[] nodeArray = nodes.ToArray();
        int i = 0;
        waypoints = transform.GetComponentsInChildren<GameObject>();
        if (waypoints == null)
        {
            foreach (Vector3 vert in nodeArray)
            {
                nodeArray[i] = transform.TransformPoint(vert);
                GameObject handle = new GameObject("node " + i);
                handle.transform.position = vert;
                handle.transform.parent = transform;
                handle.tag = "PathWaypoint";
                i++;
            }
        }
    }

    void Update()
    {
        if (newCalculate)
        {
            foreach(GameObject child in waypoints)
            {
                DestroyObject(child);
            }
            Start();
        }
        if (waypoints != null)
        {
            List<Vector3> dummyNodes = new List<Vector3>();
            for (int i = 0; i < waypoints.Length; i++)
            {
                dummyNodes.Add(waypoints[i].transform.localPosition);
            }
            nodes = dummyNodes;
        }
    }
    */
    void OnEnable(){
		paths.Add(pathName.ToLower(), this);
	}
	
	void OnDrawGizmosSelected(){
		if(enabled) { // dkoontz
			if(nodes.Count > 0){
				iTween.DrawPath(nodes.ToArray(), pathColor);
			}
		} // dkoontz
	}
	
	public static Vector3[] GetPath(string requestedName){
		requestedName = requestedName.ToLower();
		if(paths.ContainsKey(requestedName)){
			return paths[requestedName].nodes.ToArray();
		}else{
			Debug.Log("No path with that name exists! Are you sure you wrote it correctly?");
			return null;
		}
	}
}

