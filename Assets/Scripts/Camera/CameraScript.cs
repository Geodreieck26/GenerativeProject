using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
    #region params

    public int beatIndex = 0;

    private bool calculateIsAllowed = false;
    public float wayPointChangeRate = 0.05f;
    private GameObject audioBeat;

    public float startDelayTime = 3f;

    // Mesh as GameObject.
    private GameObject generatedMesh;

    // Target (car) as GameObject.
    private GameObject car;

    // Mesh of the Scene.
    private Mesh mesh;

    // Startposition of the game object.
    private Vector3 startPosition;

    public GameObject LookTarget;

    // Velocity.
    private Vector3 velocity = Vector3.zero;

    // Array of the vertices of the mesh.
    private Vector3[] vertices;

    private bool firstTime = true;

    // Amount of x vertices in one row.
    private int xRowAmount = 0;

    // Amount of the y vertices in one row.
    private int yRowAmount = 0;

    [SerializeField]
    // Time for smoothing.
    private float smoothTime = 1f;

    // Target position of the camera.
    private Vector3 targetPosition;

    // Look At position for camera.
    private Vector3 lookAtPosition;

    [SerializeField]
    // Y offset of camera.
    private float yOffset = 200f;

    [SerializeField]
    // X offset of camera in vertices.
    private int xVerticeOffset = 100;

    // Dividing factor of the x vertice offset calculation.
    private int xVerticeOffsetDividingFactor = 3;

    // True if the camera should view from the right side.
    private bool rightenSideView = true;

    // Local scale on x axis of the generated mesh.
    private float xAxisScale;

    // local scale on y axis of the generated mesh.
    private float yAxisScale;

    // local scale on z axis of the generated mesh.
    private float zAxisScale;

    // Vertice which defines the middle of the mesh at this moment.
    private int middleVerticeValue = 0;

    [SerializeField]
    // X Offset for static camera use.
    private float xOffset = 1f;

    [SerializeField]
    // Z Offset for static camera use.
    private float zOffset = 1f;

    [SerializeField]
    // X Offset for static camera look at use.
    private float lookAtXOffset = 1f;

    [SerializeField]
    // Z Offset for static camera look at use.
    private float lookAtZOffset = 1f;

    iTweenEvent[] tweenAroundEvents;

    private int beatAmount;

    private int waypointIndex = 0;

    bool tween = true;

    bool afterDelay = false;

    private bool tweenComplete = true;
    [SerializeField]
    private GameObject[] wayPoints;

    [SerializeField]
    // Defines, when the iTween movement will be played.
    int beatAmountCompare = 4;

    #endregion params

    void Awake()
    {
        tweenAroundEvents = transform.GetComponents<iTweenEvent>();
        foreach (iTweenEvent tweenAroundEvent in tweenAroundEvents)
        {
            
            if (tweenAroundEvent.tweenName == "Waypoint Start")
            {
                tweenAroundEvent.playAutomatically = true;
            } else
            {
                tweenAroundEvent.playAutomatically = false;
            }
            Debug.Log(tweenAroundEvent.tweenName);
        }
    }

    /// <summary>
    /// Initialize everything.
    /// </summary>
    void Start () {
        startPosition = transform.position;
        car = GameObject.FindGameObjectWithTag("Car");

        if (car != null)
        {
            generatedMesh = GameObject.FindGameObjectWithTag("Mesh");
            if (generatedMesh != null)
            {
                mesh = generatedMesh.GetComponent<MeshFilter>().mesh;
                vertices = mesh.vertices;
                if (vertices != null && vertices.Length != 0)
                {
                    GetXRowVertices();
                    GetYRowVertices();
                }
                else
                {
                    Debug.Log("**********************No vertices in vertices array found!**********************");
                }
            }
            else
            {
                Debug.Log("**********************No generated Mesh found!**********************");
            }
        } else
        {
            Debug.Log("*********************No car found!********************************");
        }
        

        //xAxisScale = generatedMesh.transform.localScale.x;
        //yAxisScale = generatedMesh.transform.localScale.y;
        //zAxisScale = generatedMesh.transform.localScale.z;
        
        CalculateStaticEverything();
        StartCoroutine(StartDelay());
        //CalculateEverything();
    }
    #region methods
    /*
    /// <summary>
    /// Audio Callback eventhandler for beat detection.
    /// </summary>
    /// <param name="eventInfo">Info of the event which was triggered.</param>
    public void MyCallbackEventHandler(BeatDetection.EventInfo eventInfo)
    {
        Debug.Log("Callback!!");
        switch (eventInfo.messageInfo)
        {
            case BeatDetection.EventType.Kick:
                
                beatAmount++;
                //calculateStaticSideSweepTargetPosition();
                if (tweenComplete)
                {
                    calculateWaypoint();
                }                
                break;
        }

    }
    */

    /// <summary>
    /// Get the amount of Y-vertices in a row.
    /// </summary>
    void GetYRowVertices()
    {
        if (xRowAmount > 0)
        {
            yRowAmount = vertices.Length / xRowAmount;
        }
        else
        {
            Debug.Log("***************No vertices in the x rows!**************");
        }

    }

    /// <summary>
    /// Get the amount of X-vertices in a row.
    /// </summary>
    void GetXRowVertices()
    {
        bool rowFound = false;

        xRowAmount = -1;

        while (xRowAmount < vertices.Length && !rowFound)
        {
            xRowAmount++;
            if (vertices[xRowAmount].x > vertices[0].x)
            {
                rowFound = true;
            }
        }
    }

    /// <summary>
    /// Calculate the target position of the camera movement.
    /// </summary>
    void calculateTargetPosition()
    {
        if (rightenSideView)
        {
            targetPosition = new Vector3(vertices[middleVerticeValue - 1].x * xAxisScale - xVerticeOffset, vertices[xRowAmount - 1].y * yAxisScale + yOffset, vertices[xRowAmount - 1].z * zAxisScale);
        } else
        {
            targetPosition = new Vector3(vertices[middleVerticeValue].x * xAxisScale - xVerticeOffset, vertices[xRowAmount].y * yAxisScale + yOffset, vertices[xRowAmount].z * zAxisScale);
        }
        rightenSideView = !rightenSideView;

        Debug.Log(targetPosition);

        StartCoroutine(WaitForNextPosition());
    }

        /// <summary>
        /// Calculate the position where the camera should look at.
        /// </summary>
        void calculateCameraLookAt()
    {
        // Vertice in the middle of the grid.
        middleVerticeValue = (int)(xRowAmount / 2 + vertices.Length / 2);
        //Debug.Log(" row amount middle vertice position: " + vertices[middleVerticeValue].x * xAxisScale + ", " + vertices[middleVerticeValue].y * xAxisScale + ", " + vertices[middleVerticeValue].z * xAxisScale);
        //int middleVerticeValue = (int)(xRowAmount / 2 + vertices.Length / 2);
        lookAtPosition = new Vector3(vertices[middleVerticeValue].x * xAxisScale, vertices[middleVerticeValue].y, vertices[middleVerticeValue].z * zAxisScale);
    }


    /// <summary>
    /// Calculate the static target position of the camera movement.
    /// </summary>
    void calculateStaticSideSweepTargetPosition()
    {
        if (rightenSideView)
        {
            targetPosition = new Vector3(startPosition.x, startPosition.y + yOffset, startPosition.z + zOffset * zAxisScale);
        }
        else
        {
            targetPosition = new Vector3(startPosition.x, startPosition.y + yOffset, startPosition.z - zOffset * zAxisScale);
        }
        rightenSideView = !rightenSideView;

        //Debug.Log(targetPosition);

        StartCoroutine(WaitForNextStaticPosition());
    }

    /// <summary>
    /// Calculate the static position where the camera should look at.
    /// </summary>
    void calculateStaticCameraLookAt()
    {
        lookAtPosition = new Vector3(car.transform.position.x, car.transform.position.y, car.transform.position.z);
        
    }


    /*
    /// <summary>
    /// Should be used, if automated xVerticeOffset calculation of the camera movement is wanted.
    /// </summary>
    void calculateXOffset()
    {
        xVerticeOffset = xRowAmount / xVerticeOffsetDividingFactor;
    }
    */

    /// <summary>
    /// Fancy movement of the camera.
    /// </summary>
    void fancyCameraMove()
    {
        transform.LookAt(lookAtPosition, transform.up);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    /// <summary>
    /// Normal movement of the camera.
    /// </summary>
    void cameraMove()
    {
        transform.LookAt(lookAtPosition, new Vector3(0, 1, 0));
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    /// <summary>
    /// Wait for next position change of the camera.
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForNextPosition()
    {
        yield return new WaitForSeconds(2f);
        calculateTargetPosition();
    }

    /// <summary>
    /// Wait for next static position change of the camera.
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForNextStaticPosition()
    {
        yield return new WaitForSeconds(2f);
        calculateStaticSideSweepTargetPosition();
    }

    /// <summary>
    /// Calculate function for all non-static calculate methods.
    /// </summary>
    void CalculateEverything()
    {
        calculateCameraLookAt();
        calculateTargetPosition();
    }

    /// <summary>
    /// Calculate function for all static calculate methods.
    /// </summary>
    void CalculateStaticEverything()
    {
        calculateStaticCameraLookAt();
        calculateStaticSideSweepTargetPosition();
    }

    #endregion methods


    void SetIsComplete()
    {
        tweenComplete = true;
        Debug.Log("Completed!");
    }

    public void calculateWaypoint(BeatEventManager.BeatIndex index)
    {
        Debug.Log("Beat!");
        //Debug.Log("calculateIsAllowed allowed: " + calculateIsAllowed);
        beatAmount++;
        if (this.beatIndex == (int)index && tweenComplete && calculateIsAllowed && beatAmount >= beatAmountCompare)
        {
            calculateIsAllowed = false;
            int random = Random.Range(0, 2) - 1;
            if (random == 0)
            {
                random++;
            }
            if (random + waypointIndex < 0)
            {
                waypointIndex = tweenAroundEvents.Length - 1;
            }
            else if (random + waypointIndex >= tweenAroundEvents.Length - 1)
            {
                waypointIndex = 0;
            }

            waypointIndex += random;
            if (firstTime)
            {
                waypointIndex = 0;
                firstTime = false;
            }
            tweenComplete = false;
            //tweenAroundEvents[waypointIndex].
            
            Debug.Log("waypointindex Number: " + waypointIndex);
            tweenAroundEvents[waypointIndex].Play();
            beatAmount = 0;
            StartCoroutine(WaitforEnable());
            
        }

    }
    IEnumerator WaitforEnable()
    {
        yield return new WaitForSeconds(wayPointChangeRate);
        calculateIsAllowed = true;
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(startDelayTime);
        calculateIsAllowed = true;
        afterDelay = true;
    }

    void Update()
    {
        //transform.LookAt(lookAtPosition, new Vector3(0, 1, 0));
        if (afterDelay)
        {
            transform.LookAt(LookTarget.transform.position, new Vector3(0, 1, 0));
        }
        
        //fancyCameraMove();
        //cameraMove();       
    }
}
