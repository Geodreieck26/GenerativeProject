using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(AudioAnalyzer))]

public class MeshGenerator : MonoBehaviour {

    private AudioAnalyzer audioAnalyzer;
    //private Camera mainCamera;


    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> indices;
    private List<Vector2> uvs;
    //private List<Vector3> normals;

    private float xStep;
    private float zStep;

    private Vector3 vertex;
    private Vector2 uv;
    //private Vector3 normal;

    private int xCurSize;
    private int zCurSize;

    //private int sampleStep;
    private float[] frequencyData;


    public int xRemovedLines;


    public int xVertices;
    public int zVertices;
    public Material material;
    public bool expandMesh;
    public MeshTopology meshTopolgy;
    public float updateMesh;
    public float offsetStrength;
    public float maxDifference;

    private int moduloCounter;





    // Use this for initialization
    void Start () {

        audioAnalyzer = GetComponent<AudioAnalyzer>();
        //mainCamera = Camera.main;

        mesh = new Mesh();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        vertices = new List<Vector3>();
        indices = new List<int>();
        uvs = new List<Vector2>();
        //normals = new List<Vector3>();

        vertex = new Vector3();
        uv = new Vector2();
        //normal = new Vector3();

        xStep = 1.0f / (xVertices-1);
        zStep = 1.0f / (zVertices-1);

        //sampleStep = audioAnalyzer.Samples/zVertices;
        frequencyData = new float[0];

        moduloCounter = 1;


        GenerateBaseMesh();


        expandMesh = true;
	    
	}

    private void GenerateBaseMesh()
    {
        for(int i = 0; i < xVertices; i++)
        {
            for(int j = 0; j < zVertices; j++)
            {
                vertex.Set(xStep*i, 0, zStep*j);
                vertices.Add(vertex);

                if(j!= zVertices-1 && i!= xVertices-1)
                {
                    indices.Add(((i + 1) * (zVertices)) + j);
                    indices.Add((i * (zVertices)) + j);
                    indices.Add((i * (zVertices)) + j + 1);
                } 

                if(i != 0 && j != 0)
                {
                    indices.Add((i * (zVertices)) + j);
                    indices.Add((i * (zVertices)) + j - 1);
                    indices.Add(((i - 1) * (zVertices)) + j);
                }
                uv.Set(xStep * i, zStep * j);
                uvs.Add(uv);
            }
        }


        mesh.SetVertices(vertices);
        mesh.SetIndices(indices.ToArray(), meshTopolgy, 0);
        mesh.SetUVs(0, uvs);
        meshFilter.mesh = mesh;
        mesh.RecalculateBounds();
        //mesh.RecalculateNormals();
        meshRenderer.material = material;

        xCurSize = xVertices;
        zCurSize = zVertices;


    }


    void AddRow()
    {
        //xCurSize++;
       

        for (int i = 0; i <zCurSize; i++)
        {
            vertex.Set(xStep*(xCurSize), 0, zStep*i);
            vertices.Add(vertex);
           

            if(i!= zCurSize - 1)
            {
                indices.Add(((xCurSize  - 1) * (zVertices)) + i);
                indices.Add(((xCurSize  - 2) * (zVertices)) + i);
                indices.Add(((xCurSize  - 2) * (zVertices)) + i + 1);
            }

            if (i != 0 )
            {
                indices.Add(((xCurSize  - 1) * (zVertices)) + i);
                indices.Add(((xCurSize  - 1) * (zVertices)) + i - 1);
                indices.Add(((xCurSize  - 2) * (zVertices)) + i);
            }

            uv.Set(xStep*xCurSize, zStep * i);
            uvs.Add(uv);
        }

        OffsetVertices(vertices.Count-zCurSize,vertices.Count-1);

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices.ToArray(), meshTopolgy, 0);
        mesh.SetUVs(0, uvs);

        mesh.RecalculateBounds();
        //mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        for (int i = 0; i < vertices.Count; i++)
        {
            vertex.Set(vertices[i].x - xStep, vertices[i].y, vertices[i].z);
            vertices[i] = vertex;

            uv.Set(uvs[i].x-xStep, uvs[i].y);
            uvs[i] = uv;
        }

        RemoveRow();


    }


    void RemoveRow()
    {

       
        vertices.RemoveRange(0, zVertices);
        uvs.RemoveRange(0, zVertices);      
        CorrectIndices(zVertices);

        mesh.Clear();
        mesh.SetVertices(vertices);

        mesh.SetIndices(indices.ToArray(), meshTopolgy, 0);
        mesh.SetUVs(0, uvs);

        mesh.RecalculateBounds();
        //mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        //xRemovedLines++;
        
    }


    void CorrectIndices(int amount)
    {
        int counter = 0;
        for (int i = 0; i<indices.Count; i+=3)
        {
            counter++;
            indices[i] -= amount;
            
            indices[i + 1] -= amount;
            indices[i + 2] -= amount;            
            if (indices[i] < 0 || indices[i + 1] < 0 || indices[i + 2] < 0)
            {             
                indices.RemoveRange(i, 3);
                i -= 3;
            }
        }
        
    }
	
	// Update is called once per frame
	void Update () {
        if (expandMesh)
        {
            InvokeRepeating("AddRow", 0, updateMesh);
            //addRow();
            expandMesh = false;
            
        }
	}


    void OffsetVertices(int start, int end)
    {
        frequencyData = audioAnalyzer.GetFrequencyData();
      
        int counter = 0;
       
        for(int i = start; i <= end; i++)
        {
           
            vertex.Set(vertices[i].x, vertices[i].y + frequencyData[counter] * offsetStrength, vertices[i].z);

            //if (i % moduloCounter == 0)
            //{
            //    uv.Set(0, 0.2f);
            //    uvs[i] = uv;
            //}



            vertices[i] = vertex;
         
            counter++;

        }

        moduloCounter++;
        if (moduloCounter > zVertices)
        {
            moduloCounter = 1;
        }

    }

  

    void OnDisable()
    {
        CancelInvoke();
    }
}
