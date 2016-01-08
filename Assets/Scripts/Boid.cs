using UnityEngine;
using System.Collections;

public class Boid : MonoBehaviour
{
    // the controller the boid belongs to
    private GameObject flockController;

    // the corresponding animator
    private Animator anim;

    // counter for updateRate
    private float currTime = 0.0f;

    void Start()
    {
        iTween.Init(gameObject);

        // set animation
        anim = GetComponent<Animator>();
        float random = Random.Range(-flockController.GetComponent<FlockController>().animOffset, flockController.GetComponent<FlockController>().animOffset);
        anim.speed = anim.speed + random;
    }

    void Update()
    {
        currTime += Time.deltaTime;
        if (currTime >= flockController.GetComponent<FlockController>().updateRate)
        {
            currTime = 0.0f;
            Flying();
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
        if (controller.clampZ)
        {
            velN = new Vector3(velN.x / 10.0f, velN.y / 10.0f, Mathf.Clamp(velN.z, 0.4f, 1.0f));
        }else if (controller.clampX)
        {
            velN = new Vector3(Mathf.Clamp(velN.x, 0.4f, 1.0f), velN.y / 10.0f, velN.z / 10.0f);
        }
        // rotate boid for animation purposes
        iTween.LookUpdate(gameObject, transform.position + velN, controller.updateRate);

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

        // maintain distance to other boids
        Vector3 separation = Vector3.zero;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, controller.separationRadius);
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
}
