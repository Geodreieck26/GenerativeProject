using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

    // Mesh as GameObject.
    private GameObject generatedMesh;

    // Mesh of the Scene.
    private Mesh mesh;

    // Velocity.
    private Vector3 velocity = Vector3.zero;

    // Array of the vertices of the mesh.
    private Vector3[] vertices;

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

    /// <summary>
    /// Initialize everything.
    /// </summary>
    void Start () {
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

        xAxisScale = generatedMesh.transform.localScale.x;
        yAxisScale = generatedMesh.transform.localScale.y;
        zAxisScale = generatedMesh.transform.localScale.z;

        CalculateEverything();
    }

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
        Debug.Log(" row amount middle vertice position: " + vertices[middleVerticeValue].x * xAxisScale + ", " + vertices[middleVerticeValue].y * xAxisScale + ", " + vertices[middleVerticeValue].z * xAxisScale);
        //int middleVerticeValue = (int)(xRowAmount / 2 + vertices.Length / 2);
        lookAtPosition = new Vector3(vertices[middleVerticeValue].x * xAxisScale, vertices[middleVerticeValue].y, vertices[middleVerticeValue].z * zAxisScale);
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
    /// Calculate function for all calculate methods.
    /// </summary>
    void CalculateEverything()
    {
        calculateCameraLookAt();
        calculateTargetPosition();
    }

    void Update()
    {
        //fancyCameraMove();
        cameraMove();
    }
}
