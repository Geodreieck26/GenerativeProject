using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(BoxCollider))]

public class FlockController : MonoBehaviour
{

    // update rate for the single boids
    [SerializeField]
    [Range(0.0f, 5.0f)]
    public float flyUpdateRate = 1.0f;
    public float colorUpdateRate = 0.4f;

    // clamp forward rotation?
    [SerializeField]
    public bool clampZ = true;
    public bool clampX = false;

    // flock behaviour and stats variables
    [SerializeField]
    public float minVel = 5;
    [SerializeField]
    public float maxVel = 20;
    [SerializeField]
    public GameObject target;
    [SerializeField]
    public int flockSize = 100;
    [SerializeField]
    public int pushOutRadius = 5;
    [SerializeField]
    public int boundingOffset = 10;

    // flock behaviour weights
    [Range(0.0f, 1.0f)]
    public float cohesionWeight = 1.0f;
    [Range(0.0f, 1.0f)]
    public float separationWeight = 1.0f;
    [Range(0.0f, 1.0f)]
    public float alignmentWeight = 1.0f;
    [Range(0.0f, 1.0f)]
    public float followingWeight = 0.0f;
    [Range(0.0f, 1.0f)]
    public float boundingWeight = 1.0f;
    [Range(0.0f, 1.0f)]
    public float randomnessWeight = 0.5f;

    // the boid prefab
    [SerializeField]
    public GameObject boidPrefab;

    // general center and velocity
    public Vector3 flockCenter;
    public Vector3 flockVel;

    // the controlled boids
    private GameObject[] boids;
    [SerializeField]
    public float animOffset = 0.4f;

    // sound analyzer script
    private AudioAnalyzer audioAnalyzer;
    public float[] frequencyData;
    public float[] averageValues;

    // color generator script
    public ColorGenerator colorGenerator;

    void Start()
    {
        // get sound analyzer & color generator
        audioAnalyzer = FindObjectOfType<AudioAnalyzer>();
        colorGenerator = FindObjectOfType<ColorGenerator>();

        // init boids within collider of flock
        boids = new GameObject[flockSize];
        Collider collider = GetComponent<Collider>();
        for (var i = 0; i < flockSize; i++)
        {
            Vector3 position = new Vector3(
                UnityEngine.Random.value * collider.bounds.size.x,
                UnityEngine.Random.value * collider.bounds.size.y,
                UnityEngine.Random.value * collider.bounds.size.z
            ) - collider.bounds.extents;

            GameObject boid = Instantiate(boidPrefab, transform.position, transform.rotation) as GameObject;
            boid.transform.parent = transform;
            boid.transform.localPosition = position;
            boid.GetComponent<Boid>().SetController(gameObject);
            boids[i] = boid;
        }
    }

    void Update()
    {
        averageValues = audioAnalyzer.GetAverageValues();

        Vector3 center = Vector3.zero;
        Vector3 velocity = Vector3.zero;

        // calculate center of mass and average velocity 
        foreach (GameObject boid in boids)
        {
            center += boid.transform.localPosition;
            velocity += boid.GetComponent<Rigidbody>().velocity;
        }
        flockCenter = center / flockSize;
        flockVel = velocity / flockSize;

        // test change weights with frequency
        if (audioAnalyzer != null)
        {
            frequencyData = audioAnalyzer.GetFrequencyData();
            float data = 0;
            for (int i = 0; i < frequencyData.Length; i++)
            {
                data += frequencyData[i];
            }
            cohesionWeight = data;
        }
    }
}
