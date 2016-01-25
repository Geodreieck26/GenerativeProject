using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Pool))]

public class MeshGenerator : MonoBehaviour
{

    public AudioAnalyzer audioAnalyzer;
    private Pool pool;
    //private Camera mainCamera;

    private Queue<ObjectTypes> currentObjects;



    private enum ObjectTypes
    {
        SimpleRow, Crossing
    }

    private enum Sector
    {
        Building, Crossing, Freeway
    }



    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh;
    private List<Vector3> vertices;
    //private List<int> indices;
    private List<Vector2> uvs;
    //private List<Vector3> normals;



    private float[] currentFrequencyData;
    private int[] prevRowIndices;


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

    [Range(0.05f, 5.0f)]
    public float velocity;

    [Range(0.05f, 1.0f)]
    public float updateRate;
    private float currentTime;

    private int rowsAdded = 0;

    private bool firstCall;


    public int[] indicesLine;
    public int[] subMesh;

    private int waitCycles = 0;

    private int half;


    public int allowedRowCount;

    public float buildingWidth;

    public int streetWidth;

    private List<int>[] indices;
    //private List<int> s1;

    public GameObject buidlingTemplate;
    private GameObject[] buildings;


    public List<GameObject> objectsToMove;

    public Material[] mats;

    private float pointOfDoom;

    public Vector3 buildingScale;

    private bool once;

    private bool[] instruction;

    private int amountOfBuildings;


    public float[] propability;

    private ColorGenerator colorGen;

    public Material[] buildingMaterials;

    public int crossingWidth;
    public int crossingRows;
    public bool crossing;
    private bool oldSidewalk = true;

    [SerializeField]
    private int[]sectorProbabilty;

    [SerializeField]
    private float[] realSectorProbability;
    //public int CrossingSector;
    private int sum;

    [SerializeField]
    private Sector currentSector;


    [SerializeField]
    private float sectorCooldown;


   
    public float currentCooldown;



    public List<GameObject> ObjectsToMove
    {

        get { return objectsToMove; }

    }

    public Vector3[] assetSpawns;

    private bool onceUpdate;


    private Vector3[] cloudSpawns;


    private float[] buildingControlPropability;

    private Sector prev;


    private bool freeway;

    [SerializeField]
    private float freewayBuidlingEaseTime;
    private float freewayTimer;

    public GameObject tafal;


    public Vector3[] CloudSpawns
    {
        get
        {
            return cloudSpawns;
        }
    }



    // Use this for initialization
    void Awake()
    {
        //sectorProbabilty = new int[3];
        currentSector = Sector.Building;
        realSectorProbability = new float[sectorProbabilty.Length];
       
        amountOfBuildings = 6;

        pool = GetComponent<Pool>();
        firstCall = true;
        currentObjects = new Queue<ObjectTypes>();

        currentFrequencyData = new float[0];

        currentTime = 0.0f;
        //mainCamera = Camera.main;

        mesh = new Mesh();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        audioAnalyzer = FindObjectOfType<AudioAnalyzer>();


        vertices = new List<Vector3>();
        indices = new List<int>[2];
        indices[0] = new List<int>();
        indices[1] = new List<int>();

        uvs = new List<Vector2>();
        //normals = new List<Vector3>();

        vertex = new Vector3();
        uv = new Vector2();
        //normal = new Vector3();

        xStep = 1.0f / (xVertices - 1);
        zStep = 1.0f / (zVertices - 1);

        //sampleStep = audioAnalyzer.Samples/zVertices;
        frequencyData = new float[0];

        moduloCounter = 1;



        indicesLine = new int[zVertices];
        subMesh = new int[zVertices];


        expandMesh = true;

        //buildings = new GameObject[50];

        //for (int i = 0; i < buildings.Length; i++)
        //{
        //    buildings[i] = Instantiate(buidlingTemplate, Vector3.zero, Quaternion.identity) as GameObject;
        //}

        mesh.subMeshCount = 2;
        meshRenderer.materials = mats;

        meshFilter.mesh = mesh;

        objectsToMove = new List<GameObject>();

        colorGen = Camera.main.GetComponent<ColorGenerator>();

        assetSpawns = new Vector3[2];
        cloudSpawns = new Vector3[2];
        buildingControlPropability = new float[3];

        prev = Sector.Building;
       

    }




    private void RemoveStructure()
    {
        ObjectTypes type = currentObjects.Dequeue();

        if (type == ObjectTypes.SimpleRow)
        {
            RemoveRow(1);
        }
        if (type == ObjectTypes.Crossing)
        {
            RemoveRow(2);
        }
        

    }


    private void MoveMesh()
    {
        for (int i = 0; i < vertices.Count; i++)
        {

            vertex.Set(vertices[i].x - velocity * Time.deltaTime, vertices[i].y, vertices[i].z);
            vertices[i] = vertex;
        }
        mesh.SetVertices(vertices);
        mesh.RecalculateBounds();
        mesh.Optimize();


    }


    public Vector3[] GetSpawnPositions()
    {
        if(currentSector == Sector.Building)
        {
            return assetSpawns;
        }
        return null;
    }



    private void OffsetHeight(int[] indicesLine, bool sideWalk)
    {
        int streetVerts = 2;
        int half = indicesLine.Length / 2;
        for (int i = 0; i < indicesLine.Length; i++)
        {
            if (i < half - streetVerts || i > half + streetVerts)
            {
                if (sideWalk)
                {
                    vertex.Set(vertices[indicesLine[i]].x, vertices[indicesLine[i]].y + 20, vertices[indicesLine[i]].z);
                }
                else
                {
                    vertex = vertices[indicesLine[i]];
                }
                
                vertices[indicesLine[i]] = vertex;
            }
            else if (i == half - streetVerts)
            {
                vertex.Set(vertices[indicesLine[i]].x, vertices[indicesLine[i]].y, vertices[indicesLine[i - 1]].z);
                vertices[indicesLine[i]] = vertex;
            }
            else if (i == half + streetVerts)
            {
                vertex.Set(vertices[indicesLine[i]].x, vertices[indicesLine[i]].y, vertices[indicesLine[i + 1]].z);
                vertices[indicesLine[i]] = vertex;
            }
        }
    }


    void OffsetZVertices(int[] indicesLine)
    {
        int verts = 5;
        half = zVertices / 2;
        float prevZleft = 0;
        float nextZright = 0;

        float tmp = 0;


        for (int i = 0; i < half; i++)
        {

            if (i == 0)
            {
                prevZleft = vertices[indicesLine[0]].z;
                nextZright = vertices[zVertices - 1].z;
            }

            if (i % verts == 1)
            {

                tmp = vertices[indicesLine[i]].z;
                vertex.Set(vertices[indicesLine[i]].x, vertices[indicesLine[i]].y, prevZleft);
                vertices[indicesLine[i]] = vertex;
                prevZleft = tmp;


                tmp = vertices[indicesLine[zVertices - 1 - i]].z;
                vertex.Set(vertices[indicesLine[zVertices - 1 - i]].x, vertices[indicesLine[zVertices - 1 - i]].y, nextZright);
                vertices[indicesLine[zVertices - 1 - i]] = vertex;
                nextZright = tmp;

            }
            else if (i % verts == 2)
            {
                tmp = vertices[indicesLine[i]].z;
                vertex.Set(vertices[indicesLine[i]].x, vertices[indicesLine[i]].y, tmp);
                vertices[indicesLine[i]] = vertex;
                prevZleft = tmp;


                tmp = vertices[indicesLine[zVertices - 1 - i]].z;
                vertex.Set(vertices[indicesLine[zVertices - 1 - i]].x, vertices[indicesLine[zVertices - 1 - i]].y, tmp);
                vertices[indicesLine[zVertices - 1 - i]] = vertex;
                nextZright = tmp;
            }
            else if (i % verts == 3 || i % verts == 4)
            {
                tmp = vertices[indicesLine[i]].z;
                vertex.Set(vertices[indicesLine[i]].x, vertices[indicesLine[i]].y, prevZleft);
                vertices[indicesLine[i]] = vertex;
                prevZleft = tmp;


                tmp = vertices[indicesLine[zVertices - 1 - i]].z;
                vertex.Set(vertices[indicesLine[zVertices - 1 - i]].x, vertices[indicesLine[zVertices - 1 - i]].y, nextZright);
                vertices[indicesLine[zVertices - 1 - i]] = vertex;
                nextZright = tmp;
            }



            else
            {
                prevZleft = vertices[indicesLine[i]].z;
                nextZright = vertices[indicesLine[zVertices - 1 - i]].z;
            }


            if (i == half - 1)
            {
                vertices[indicesLine[i]].Set(vertices[indicesLine[i]].x, vertices[indicesLine[i]].y, 0);
            }


        }


        vertex.Set(vertices[indicesLine[half]].x, 0, vertices[indicesLine[half]].z);
        vertices[indicesLine[half]] = vertex;


    }



    private void CrossingStart()
    {
        //AddBasicRow();
        AddBasicRow(indicesLine, 0, true, false, true);
        AddBasicRow(indicesLine, 0, true, true, false);



    }

    private void CrossingEnd()
    {

        AddBasicRow(indicesLine, 0, true, true, false);
        AddBasicRow(indicesLine, 0, true, true, true);

    }




    private void PlaceBuidlingRow(bool[] instructions)
    {
        frequencyData = audioAnalyzer.GetFrequencyData();
        int steps = indicesLine.Length / 6;
        int half = indicesLine.Length / 2;
        Material[] tempMats = new Material[2];
        float z;
        GameObject go;
        Vector3 pos = new Vector3();
        Vector3 scale = new Vector3();
        Debug.Log("place buildings");
        MeshRenderer mr;
        for (int i = 0; i < 3; i++)
        {
            if (instructions[i])
            {
                go = pool.getPooledObject();
                go.SetActive(true);

                mr = go.GetComponent<MeshRenderer>();
                tempMats[0] = mr.materials[0];
                tempMats[1] = buildingMaterials[1];
                mr.materials = tempMats;

                if (!once)
                {
                    buildingScale = go.transform.localScale;
                }

                z = frequencyData[i];

                scale.Set(go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z * ((i * 0.3f) + z * 3 + 0.2f));


                go.transform.localScale = Vector3.zero;

                iTween.ScaleTo(go.gameObject, iTween.Hash("scale", scale, "time", 0.05f, "easetype", iTween.EaseType.linear));

                pos = vertices[indicesLine[half - (steps * (i + 1))]];
                pos.z -= zStep * 2;
                pos = transform.TransformPoint(pos);
                go.transform.position = pos;
                //go.transform.localScale = scale;
                objectsToMove.Add(go);
            }

            if (instructions[amountOfBuildings - 1 - i])
            {

                z = frequencyData[i + 3];


                go = pool.getPooledObject();
                go.SetActive(true);

                mr = go.GetComponent<MeshRenderer>();
                mr.materials[1] = buildingMaterials[1];

                scale.Set(go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z * ((i * 0.3f) + z * 3 + 0.2f));

                go.transform.localScale = Vector3.zero;

                iTween.ScaleTo(go.gameObject, iTween.Hash("scale", scale, "time", 0.05f, "easetype", iTween.EaseType.linear));

                pos = vertices[indicesLine[half + (steps * (i + 1))]];
                pos.z += zStep * 2;
                pos = transform.TransformPoint(pos);

                go.transform.position = pos;
                //go.transform.localScale = scale;
                objectsToMove.Add(go);
            }
        }

    }





    private bool Propability(float threshold)
    {
        if (Random.value < threshold)
        {
            return true;
        }
        return false;
    }



    private bool[] GenerateAssembleInstruction()
    {
        instruction = new bool[amountOfBuildings];

        for (int i = 0; i < amountOfBuildings / 2; i++)
        {
            instruction[i] = Propability(propability[i]);
            instruction[amountOfBuildings - 1 - i] = Propability(propability[i]);
        }
        return instruction;
    }




    void AddBasicRow(int[] indicesLine, int offset, bool addToQueue, bool shiftRow, bool sideWalk)
    {
        List<int> indexList = new List<int>();
        bool sidewalkChange = false;
        if (oldSidewalk!= sideWalk)
        {
            sidewalkChange = true;
        }

        bool firstLine = false;
        if (firstCall)
        {
            firstCall = false;
            firstLine = true;
        }

        int half = zVertices / 2;
        int streetverts = 3;

        for (int i = 0; i < zVertices; i++)
        {
            if (!shiftRow)
            {
                vertex.Set(1 + (buildingWidth * offset), 0, zStep * i);
            }
            else
            {
                vertex.Set(vertices[indicesLine[i]].x,0,vertices[indicesLine[i]].z);
            }

            vertices.Add(vertex);
            int currentIndex = vertices.Count - 1;
            if (!firstLine)
            {
                if (i != zVertices - 1)
                {
                    indexList.Add(currentIndex);
                    indexList.Add(indicesLine[i]);
                    indexList.Add(indicesLine[i + 1]);
                }
                if (i != 0)
                {

                    indexList.Add(currentIndex);
                    indexList.Add(currentIndex - 1);
                    indexList.Add(indicesLine[i]);
                }
            }
        }

        int tmp = vertices.Count - 1;
        for (int i = zVertices - 1; i >= 0; i--)
        {
            indicesLine[zVertices - 1 - i] = tmp - i;
        }

      
        OffsetHeight(indicesLine, sideWalk);
   



        for (int i = 0; i < indexList.Count / 3; i++)
        {
            if (!sidewalkChange)
            {
                if (i != (half + streetverts) * 2 && i != (half + streetverts - 1) * 2 - 1 && i != (half - streetverts) * 2 - 1 && i != (half - streetverts + 1) * 2)
                {
                    indices[0].Add(indexList[i * 3]);
                    indices[0].Add(indexList[(i * 3) + 1]);
                    indices[0].Add(indexList[(i * 3) + 2]);
                }
                else
                {
                    if (sideWalk)
                    {
                        indices[1].Add(indexList[i * 3]);
                        indices[1].Add(indexList[(i * 3) + 1]);
                        indices[1].Add(indexList[(i * 3) + 2]);
                    }
                    else
                    {
                        indices[0].Add(indexList[i * 3]);
                        indices[0].Add(indexList[(i * 3) + 1]);
                        indices[0].Add(indexList[(i * 3) + 2]);
                    }
                }
            }
            else
            {               
                indices[1].Add(indexList[i * 3]);
                indices[1].Add(indexList[(i * 3) + 1]);
                indices[1].Add(indexList[(i * 3) + 2]);
            }
           
        }

        if (addToQueue)
        {            
            mesh.SetVertices(vertices);
            for (int i = 0; i < indices.Length; i++)
            {
                mesh.SetIndices(indices[i].ToArray(), meshTopolgy, i);
            }
           
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            meshFilter.mesh = mesh;
            meshRenderer.materials = mats;
            currentObjects.Enqueue(ObjectTypes.SimpleRow);
            rowsAdded++;
        }


        oldSidewalk = sideWalk;
    }



    private void DetermineSector()
    {        
        if (currentCooldown <= 0)
        {
            sum = 0;
            for (int i = 0; i < sectorProbabilty.Length; i++)
            {
                sum += sectorProbabilty[i];
            }

            for (int i = 0; i < sectorProbabilty.Length; i++)
            {
                realSectorProbability[i] = sectorProbabilty[i]/(float)sum;
            }
            int chosenSector = -1;

            float start = 0.0f;

                        
            float rand = Random.Range(0.0f, 1.0f);           
           
            for(int i = 0; i < realSectorProbability.Length; i++)
            {               
                if (rand > start)
                {                    
                    chosenSector++;
                    start  = start+ realSectorProbability[i];                    
                }
            }

            prev = currentSector;

            if (chosenSector == 0)
            {
               if(prev == Sector.Freeway)
                {
                    freeway = false;

                }
                currentSector = Sector.Building;
                currentCooldown = Random.Range(3, 5);
            }
            else if (chosenSector == 1)
            {
                if(prev == Sector.Building)
                {
                    currentSector = Sector.Crossing;
                    currentCooldown = 1.0f;
                }
                else
                {
                    currentSector = Sector.Building;
                    currentCooldown = Random.Range(3, 5);
                }
               
            }else if (chosenSector == 2)
            {
                if(prev == Sector.Building)
                {
                    currentSector = Sector.Freeway;
                    currentCooldown = Random.Range(8, 12);
                }
                else
                {
                    currentCooldown = Random.Range(1,2);
                }                
            }
        }
    }





    void RemoveRow(int rows)
    {
        pointOfDoom = vertices[0].x;
        vertices.RemoveRange(0, (zVertices * rows));
        CorrectIndices(zVertices * rows);
        CorrectIndexLine(zVertices * rows);

        mesh.Clear();
        mesh.subMeshCount = 2;
        mesh.SetVertices(vertices);
        for (int i = 0; i < indices.Length; i++)
        {
            mesh.SetIndices(indices[i].ToArray(), MeshTopology.Triangles, i);
        }

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }





    void CorrectIndices(int amount)
    {
        for (int submesh = 0; submesh < indices.Length; submesh++)
        {
            for (int i = 0; i < indices[submesh].Count; i += 3)
            {
                indices[submesh][i] -= amount;
                indices[submesh][i + 1] -= amount;
                indices[submesh][i + 2] -= amount;
                if (indices[submesh][i] < 0 || indices[submesh][i + 1] < 0 || indices[submesh][i + 2] < 0)
                {
                    indices[submesh].RemoveRange(i, 3);
                    i -= 3;
                }
            }
        }
    }


    void CorrectIndexLine(int amount)
    {
        for (int i = 0; i < indicesLine.Length; i++)
        {
            indicesLine[i] -= amount;
        }
    }

    // Update is called once per frame
    void Update()
    {        
        if (expandMesh)
        {
            currentCooldown -= Time.deltaTime;            
            currentTime += Time.deltaTime;

            if (currentTime >= updateRate)
            {
                currentTime = 0;
                if (waitCycles <= 0)
                {
                    DetermineSector();
                    if (currentSector == Sector.Building)
                    {
                        if (crossing)
                        {
                            crossing = false;
                            CrossingEnd();
                        }

                        if (freeway)
                        {
                            freeway = false;


                        }
                        else
                        {
                            AddBasicRow(indicesLine, 0, true, false, true);

                            if (Propability(0.7f))
                            {
                                PlaceBuidlingRow(GenerateAssembleInstruction());
                            }
                        }                       
                    }
                    else if(currentSector == Sector.Crossing)
                    {

                        if (!crossing)
                        {
                            crossing = true;                         
                            CrossingStart();
                        }
                        else
                        {
                            AddBasicRow(indicesLine, 0, true, false, false);
                        }
                    }
                    if(currentSector == Sector.Freeway)
                    {
                        if (!freeway)
                        {
                            freeway = true;
                            freewayTimer = freewayBuidlingEaseTime;
                        }


                        if (freewayTimer > 0)
                        {
                            freewayTimer -= Time.deltaTime;
                            for(int i = 0; i < propability.Length; i++)
                            {
                                if(propability[i] > 0)
                                {
                                    propability[i] -= Time.deltaTime;
                                }
                            }
                        }

                        AddBasicRow(indicesLine, 0, true, false, true);






                    }

                    if (rowsAdded > allowedRowCount)
                    {
                        RemoveStructure();
                    }


                    if (!onceUpdate)
                    {
                        int streetVerts = 2;
                        int half = indicesLine.Length / 2;

                        assetSpawns[0] = new Vector3(vertices[ indicesLine[half-streetVerts-2]].x, vertices[indicesLine[half - streetVerts-2]].y, vertices[indicesLine[half - streetVerts-2]].z);
                        assetSpawns[0] = transform.TransformPoint(assetSpawns[0]);

                        assetSpawns[1] = new Vector3(vertices[indicesLine[half + streetVerts+2]].x, vertices[indicesLine[half + streetVerts+2]].y, vertices[indicesLine[half + streetVerts+2]].z);
                        assetSpawns[1] = transform.TransformPoint(assetSpawns[1]);


                        cloudSpawns[0] = new Vector3(vertices[indicesLine[0]].x, vertices[indicesLine[0]].y, vertices[indicesLine[0]].z);
                        cloudSpawns[0] = transform.TransformPoint(cloudSpawns[0]);

                        cloudSpawns[1] = new Vector3(vertices[indicesLine[indicesLine.Length-1]].x, vertices[indicesLine[indicesLine.Length - 1]].y, vertices[indicesLine[indicesLine.Length - 1]].z);
                        cloudSpawns[1] = transform.TransformPoint(cloudSpawns[1]);





                    }
                }
                else
                {
                    waitCycles--;
                }
            }
            MoveObjects();
            MoveMesh();
        }
    }



    private void MoveObjects()
    {
        Vector3 vel = new Vector3();
        Vector3 min = new Vector3(pointOfDoom, 0, 0);
        min = transform.TransformPoint(min);
        for (int i = 0; i < objectsToMove.Count; i++)
        {
            if (objectsToMove[i].transform.position.x < min.x && rowsAdded > allowedRowCount)
            {
                if(objectsToMove[i].tag == "SkyScraper")
                {
                    objectsToMove[i].transform.localScale = buildingScale;
                    objectsToMove[i].GetComponent<MeshRenderer>().materials = buidlingTemplate.GetComponent<MeshRenderer>().sharedMaterials;
                    objectsToMove[i].SetActive(false);
                    objectsToMove.RemoveAt(i);
                    i--;
                }
                else
                {
                    Destroy(objectsToMove[i]);
                    objectsToMove.RemoveAt(i);
                    i--;
                }              
            }
            else
            {
                vel = objectsToMove[i].transform.position;
                vel.x += -velocity * Time.deltaTime * transform.localScale.x;
                objectsToMove[i].transform.position = vel;
            }
        }
    }





    void OffsetVertices(int start, int end)
    {
        frequencyData = audioAnalyzer.GetFrequencyData();
        int counter = 0;
        int mod = 0;

        for (int i = start; i <= end; i++)
        {
            if (breakCount >= 3)
            {
                {
                    breakCount = 0;
                }
                vertex.Set(vertices[i - zCurSize].x, 0, vertices[i].z);
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

    void OnDisable()
    {
        CancelInvoke();
    }
}
