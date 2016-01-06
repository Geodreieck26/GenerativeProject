using UnityEngine;
using System.Collections;

public class Boid : MonoBehaviour
{
    // the controller the boid belongs to
    private GameObject flockController;

    // initializing variable
    private bool init = false;

    // the corresponding animator
    private Animator anim;

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
        if (init)
        {
            InvokeRepeating("Flying", 0, flockController.GetComponent<FlockController>().updateRate);
            init = false;
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

        // rotate boid for animation purposes
        // transform.forward = velocity.normalized;
        iTween.LookUpdate(gameObject, transform.position+velocity.normalized, controller.updateRate);

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
        Vector3 cohesion = controller.flockCenter;
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
        Vector3 following = controller.target.transform.position;
        following = following - transform.position;

        return (cohesion * controller.cohesionWeight + separation * controller.separationWeight + alignment * controller.alignmentWeight + following * controller.followingWeight + bounding * controller.boundingWeight + randomize * controller.randomnessWeight);
    }

    public void SetController(GameObject flockController)
    {
        this.flockController = flockController;
        init = true;
    }

    void OnDisable()
    {
        CancelInvoke();
    }
}
