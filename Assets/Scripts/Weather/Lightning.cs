using UnityEngine;
using System.Collections;

public class Lightning : MonoBehaviour {

	private LineRenderer lineRenderer;
	private float maxZ = 8f;
	private int numOfSegments = 12;
	private Color colour = Color.white;
	private float PosRange = 0.5f;

	private bool ray;
	private Vector3 direction;
	//must be as long as weather height
	private float maxLength = 3f;
	private float lightningOffset = 0.3f;

	private float lineWidth = 0.1f;

	public Vector3 Direction {
		get { return this.direction; }
		set { this.direction = value; }
	}

	public float MaxLength
	{
		get { return this.maxLength; }
		set { this.maxLength = value; }
	}

	void Start () { 
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.SetVertexCount(numOfSegments);		
		lineRenderer.SetWidth(lineWidth / 2f, lineWidth / 2f);

		this.direction = new Vector3 (0,-1,0);
	}

	void Update () {
		setLightning ();
	}

	void setLightning() {

		if(!ray){

			RaycastHit hitInfo;

			if(Physics.Raycast(transform.position, Direction, out hitInfo, maxLength)) {

				generateRay(hitInfo);
				Debug.Log("Ray");

			} else {  

				lineRenderer.SetPosition(0, transform.position);
				lineRenderer.SetPosition(numOfSegments - 1, transform.position + Direction * MaxLength);
				calculateLinePoints(MaxLength);
 
			} 

		} 

		ray = true;
		Destroy(this.gameObject, 0.3f);
	}

	void calculateLinePoints (float dist) {

			float step = dist / numOfSegments;
			float stepAdd = step;
			Vector3 offset = Vector3.zero;
		
			for (int i = 0; i < numOfSegments-2; i++) {
				offset.x = Random.Range (-lightningOffset, lightningOffset);
				offset.z = Random.Range (-lightningOffset, lightningOffset);
				offset.y = Random.Range (-lightningOffset, lightningOffset);
			
				lineRenderer.SetPosition (i + 1, transform.position + offset + Direction * stepAdd);
				stepAdd += step;
			}

	}

	void generateRay(RaycastHit hitInfo) {

		float distance = Vector3.Distance(transform.position, hitInfo.transform.position);
		lineRenderer.SetPosition(0, transform.position);
		lineRenderer.SetPosition(numOfSegments - 1, transform.position + (Direction * distance));
		calculateLinePoints(distance);
	}

}