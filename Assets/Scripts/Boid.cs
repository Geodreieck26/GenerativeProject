using UnityEngine;
using System.Collections;

public class Boid : MonoBehaviour
{
    // the controller the boid belongs to
    private GameObject flockController;

    // the corresponding animator
    private Animator anim;

    // counter for updateRate
    private float currFlyTime = 0.0f;
    private float currColorTime = 0.0f;

    // spectrum/beat index, between 1/0 and 5/2
    public int index = 0;
    
    // spectrum color change variables
    private Color genColor;
    private Color currColor;
    private float average = 0;
    private int averageCounter = 0;

    void Start()
    {
        iTween.Init(gameObject);
        FlockController controller = flockController.GetComponent<FlockController>();

        // init fly time
        currFlyTime = controller.flyUpdateRate;

        // set index
        if (controller.beatColor)
        {
            index = (int)Random.Range(0, 2.99f);
        } else
        {
            index = (int)Random.Range(1, 5.99f);
        }
        Debug.Log(index);
        currColor = GetComponentsInChildren<Renderer>()[0].material.GetColor("_EmissionColor");

        // set animation
        anim = GetComponent<Animator>();
        float random = Random.Range(-controller.animOffset, controller.animOffset);
        anim.speed = anim.speed + random;
    }

    void Update()
    {
        FlockController controller = flockController.GetComponent<FlockController>();

        currFlyTime += Time.deltaTime;
        if (currFlyTime >= controller.flyUpdateRate)
        {
            currFlyTime = 0.0f;
            Flying();
        }

        if (!controller.beatColor)
        { 
            currColorTime += Time.deltaTime;
            float[] averageValues = controller.averageValues;
            for (int i = 0; i < averageValues.Length; i++)
            {
                average += averageValues[i];
                averageCounter++;
            }
            if (currColorTime >= controller.colorUpdateRate)
            {
                average = average / (averageCounter * 8);
                if (controller.averageValues[index] > average)
                {
                    SpectrumChangeColor();
                }
                ResetColorVars();
            }
        }
    }

    private void Flying()
    {
        FlockController controller = flockController.GetComponent<FlockController>();
        float minVelocity = controller.minVel;
        float maxVelocity = controller.maxVel;

        // calculate new velocity
        Vector3 velocity = GetComponent<Rigidbody>().velocity;
        velocity = velocity + CalculateVel() * Time.deltaTime;

        // clamp forward rotation
        Vector3 velN = velocity.normalized;
        if (controller.clampXY)
        {
            velN = new Vector3(velN.x / 10.0f, velN.y / 10.0f, Mathf.Clamp(velN.z, 0.4f, 1.0f));
        }
        else
        {
            velN = new Vector3(Mathf.Clamp(velN.x, 0.4f, 1.0f), velN.y / 10.0f, velN.z / 10.0f);
        }
        // rotate boid for animation purposes
        iTween.LookUpdate(gameObject, transform.position + velN, controller.flyUpdateRate);

        // clamp boids speed to minimum and maximum
        float speed = velocity.magnitude;
        if (speed > maxVelocity)
            velocity = velocity.normalized * maxVelocity;
        else if (speed < minVelocity)
            velocity = velocity.normalized * minVelocity;

        // set velocity
        GetComponent<Rigidbody>().velocity = velocity;
    }

    private Vector3 CalculateVel()
    {
        Vector3 randomize = new Vector3((Random.value * 2) - 1, (Random.value * 2) - 1, (Random.value * 2) - 1);
        randomize.Normalize();

        FlockController controller = flockController.GetComponent<FlockController>();

        // fly towards the centre of mass of all boids
        Vector3 cohesion = controller.flockCenter + controller.transform.position;
        cohesion = cohesion - transform.position;

        // push boids out of the flock collider box to seperate them
        Vector3 separation = Vector3.zero;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, controller.pushOutRadius);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            separation += transform.position - hitColliders[i].transform.position;
        }

        // bounding = encourage boids to stay within a certain area
        // (area is 2x starting collider of flock conroller)
        Vector3 bounding = Vector3.zero;
        Vector3 box = controller.transform.position;
        Vector3 halfSize = flockController.GetComponent<Collider>().bounds.size;
        if (transform.position.x < box.x - halfSize.x)
            bounding.x = controller.boundingOffset;
        else if (transform.position.x > box.x + halfSize.x)
            bounding.x = -controller.boundingOffset;
        if (transform.position.y < box.y - halfSize.y)
            bounding.y = controller.boundingOffset;
        else if (transform.position.y > box.y + halfSize.y)
            bounding.y = -controller.boundingOffset;
        if (transform.position.z < box.z - halfSize.z)
            bounding.z = controller.boundingOffset;
        else if (transform.position.z > box.z + halfSize.z)
            bounding.z = -controller.boundingOffset;

        // try to match velocity with all boids
        Vector3 alignment = controller.flockVel;
        alignment = alignment - gameObject.GetComponent<Rigidbody>().velocity;

        // follow target
        Vector3 following = Vector3.zero;
        if (controller.target != null)
        {
            following = controller.target.transform.position;
            following = following - transform.position;
        }

        return (cohesion * controller.cohesionWeight + separation * controller.separationWeight + alignment * controller.alignmentWeight + following * controller.followingWeight + bounding * controller.boundingWeight + randomize * controller.randomnessWeight);
    }

    public void SetController(GameObject flockController)
    {
        this.flockController = flockController;
    }

    //changes the color according to averageValues
    void SpectrumChangeColor()
    {
        FlockController controller = flockController.GetComponent<FlockController>();
        GetComponentsInChildren<Renderer>()[0].material.SetColor("_EmissionColor", controller.colorGenerator.GenColor(1f, 1f));
    }

    // resets the variables needed for color change
    void ResetColorVars()
    {
        average = 0.0f;
        averageCounter = 0;
        currColorTime = 0.0f;
    }
}
