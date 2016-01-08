using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]


public class MeshGenerator : MonoBehaviour, AudioAnalyzer.AudioCallbacks{

    public AudioAnalyzer audioAnalyzer;
    //private Camera mainCamera;

    private Queue<ObjectTypes> currentObjects;

    private enum BiotopeTypes
    {
        City, Beach
    }

    private enum ObjectTypes
    {
        SimpleRow, BuildingRow 
    } 


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


    private int beatCounter = 0;

    private int breakCount = 0;
    public float velocity;
    public float updateRate;
    private float currentTime;
    // Use this for initialization
    void Awake () {

        currentObjects = new Queue<ObjectTypes>();

        currentTime = 0.0f;
        //mainCamera = Camera.main;

        mesh = new Mesh();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        audioAnalyzer = FindObjectOfType<AudioAnalyzer>();
        //audioAnalyzer.addAudioCallback(this);

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

        //Invoke("BPM",10);


        expandMesh = true;
	    
	}

    private void GenerateBaseMesh()
    {
        int mod = 0;
        int current = 0;
        for(int i = 0; i < xVertices; i++)
        {
            current = 0;
            for(int j = 0; j < zVertices; j++)
            {
                mod = j % 5;
                if (mod == 0 || mod == 1)
                {
                    vertex.Set(xStep * i, 0, zStep * current);
                    current++;
                }
                else if(mod == 2 || mod == 3)
                {
                    vertex.Set(xStep * i, 1, zStep * (current-1));
                    current++;
                }
                else
                {
                    vertex.Set(xStep * i, 0, zStep * (current-2));
                    current-=2;                
                }
               
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
        mesh.RecalculateNormals();
        meshRenderer.material = material;

        xCurSize = xVertices;
        zCurSize = zVertices;


    }


    private void RemoveStructure()
    {
        ObjectTypes type = currentObjects.Dequeue();

        if(type == ObjectTypes.SimpleRow)
        {
            RemoveRow();
        }




    }

    void AddHouseBlockRow()
    {
        float[] tempFrequencyData = audioAnalyzer.GetFrequencyData();
        int mod = 0;
        int current = 0;

        for (int i = 0; i < zCurSize; i++)
        {
            //vertex.Set(xStep*(xCurSize), 0, zStep*i);

            mod = i % 5;
            if (mod == 0 || mod == 1)
            {
                vertex.Set(xStep * (xCurSize), 0, zStep * current);
                current++;
            }
            else if (mod == 2 || mod == 3)
            {
                vertex.Set(xStep * (xCurSize), 1, zStep * (current - 1));
                current++;
            }
            else
            {
                vertex.Set(xStep * (xCurSize), 0, zStep * (current - 2));
                current -= 2;
            }

            vertices.Add(vertex);


            if (i != zCurSize - 1)
            {
                indices.Add(((xCurSize - 1) * (zVertices)) + i);
                indices.Add(((xCurSize - 2) * (zVertices)) + i);
                indices.Add(((xCurSize - 2) * (zVertices)) + i + 1);
            }

            if (i != 0)
            {
                indices.Add(((xCurSize - 1) * (zVertices)) + i);
                indices.Add(((xCurSize - 1) * (zVertices)) + i - 1);
                indices.Add(((xCurSize - 2) * (zVertices)) + i);
            }

            uv.Set(xStep * xCurSize, zStep * i);
            uvs.Add(uv);
        }

        OffsetVertices(vertices.Count - zCurSize, vertices.Count - 1);

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices.ToArray(), meshTopolgy, 0);
        mesh.SetUVs(0, uvs);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        for (int i = 0; i < vertices.Count; i++)
        {
            vertex.Set(vertices[i].x - xStep, vertices[i].y, vertices[i].z);
            vertices[i] = vertex;

            uv.Set(uvs[i].x - xStep, uvs[i].y);
            uvs[i] = uv;
        }

  



    }


    private void MoveMesh()
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            vertex.Set(vertices[i].x - velocity*Time.deltaTime, vertices[i].y, vertices[i].z);
            vertices[i] = vertex;

            //uv.Set(uvs[i].x - xStep, uvs[i].y);
            //uvs[i] = uv;


        }
        mesh.SetVertices(vertices);
        mesh.RecalculateBounds();
        mesh.Optimize();


    }


    void AddRow()
    {            
        int mod = 0;
        int current = 0;

        for (int i = 0; i <zCurSize; i++)
        {
            mod = i % 5;
            if (mod == 0 || mod == 1)
            {
                vertex.Set(xStep * (xCurSize), 0, zStep * current);
                current++;
            }
            else if (mod == 2 || mod == 3)
            {
                vertex.Set(xStep * (xCurSize), 1, zStep * (current - 1));
                current++;
            }
            else
            {
                vertex.Set(xStep * (xCurSize), 0, zStep * (current - 2));
                current -= 2;
            }

            vertices.Add(vertex);


            if (i!= zCurSize - 1)
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
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        currentObjects.Enqueue(ObjectTypes.SimpleRow);
        RemoveStructure();


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
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;        
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
            currentTime += Time.deltaTime;
            if(currentTime>= updateRate)
            {
                currentTime = 0;
                AddRow();
                
            }
            //InvokeRepeating("AddRow", 0, updateMesh);
            //InvokeRepeating("AddRow", 0, updateMesh);
            //addRow();
            MoveMesh();
        }
	}


    void OffsetVertices(int start, int end)
    {
        frequencyData = audioAnalyzer.GetFrequencyData();

        //Debug.Log(frequencyData[2]);
        int counter = 0;
        int mod = 0;
        
        for (int i = start; i <= end; i++)
        {
            if(breakCount >= 3)
            {
                if (breakCount >= 4 && i == end)
                {
                    breakCount = 0;
                }
                //Debug.Log("break!");

                vertex.Set(vertices[i-zCurSize].x, 0, vertices[i].z);
                vertices[i] = vertex;


            }
            else
            {
                mod = i % 5;
                if (mod == 3 || mod == 2)
                {
                    vertex.Set(vertices[i].x, vertices[i].y + frequencyData[counter] * offsetStrength, vertices[i].z);
                    vertices[i] = vertex;

                }
                else if (mod == 4)
                {

                    counter++;
                }
            }
            
        }
        breakCount++;
    }


    private void BPM()
    {
        Debug.Log("bpm: " + beatCounter *6);
    }

    public void onOnbeatDetected(float beatStrength)
    {
        beatCounter++;
    }

    public void onSpectrum(float[] spectrum)
    {
        ////The spectrum is logarithmically averaged
        ////to 12 bands

        //for (int i = 0; i < spectrum.Length; ++i)
        //{
        //    Vector3 start = new Vector3(i, 0, 0);
        //    Vector3 end = new Vector3(i, spectrum[i], 0);
        //    Debug.DrawLine(start, end);
        //}
    }



    void OnDisable()
    {
        CancelInvoke();
    }
}
