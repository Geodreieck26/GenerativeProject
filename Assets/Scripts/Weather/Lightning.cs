using UnityEngine;
using System.Collections;

public class Lightning : MonoBehaviour {

	private LineRenderer lineRenderer;
	private WeatherController controller;
	
	private int numOfSegments;
	private Color colour;
	private float lightningOffset;
	private float lineWidth;

	private float PosRange;

	private Vector3 direction;

	private Vector3 lightningPosition;

	public Vector3 LightningPosition {
		get{return this.lightningPosition;}
		set{this.lightningPosition = value;}
	}

	public Vector3 Direction {
		get { return this.direction; }
		set { this.direction = value; }
	}

	void Start () { 
		controller = FindObjectOfType<WeatherController> ();

		lineRenderer = GetComponent<LineRenderer>();

		this.direction = new Vector3 (0,-1,0);
		this.numOfSegments = controller.lightningSegments;
		this.lineWidth = controller.lightningWidth;
		this.colour = controller.lightningColor;
		this.lightningOffset = controller.lightningOffset;

		lineRenderer.SetVertexCount(numOfSegments);		
		lineRenderer.SetWidth(lineWidth / 2f, lineWidth / 2f);
		lineRenderer.SetColors (colour, colour);

		generateRandomLightnings ();
		setLightning ();
	}

	void generateRandomLightnings () {	
		lightningPosition = new Vector3(Random.Range (5,-5), this.transform.position.y, Random.Range(5,-5));		
	}

	void setLightning() {

			RaycastHit hitInfo;

			if(Physics.Raycast(LightningPosition, Direction, out hitInfo)) {
				generateRay(hitInfo);
			} 
	}

	void calculateLinePoints (float dist) {

			float step = dist / numOfSegments;
			float stepAdd = step;
			Vector3 offset = Vector3.zero;
		
			for (int i = 0; i < numOfSegments-2; i++) {
				offset.x = Random.Range (-lightningOffset, lightningOffset);
				offset.z = Random.Range (-lightningOffset, lightningOffset);
				offset.y = Random.Range (-lightningOffset, lightningOffset);
			
				lineRenderer.SetPosition (i + 1, lightningPosition + offset + Direction * stepAdd);
				stepAdd += step;
			}

	}

	void generateRay(RaycastHit hitInfo) {

		float distance = Vector3.Distance(LightningPosition, hitInfo.point);

		LightningPosition = new Vector3 (lightningPosition.x,0,lightningPosition.z);

		lineRenderer.SetPosition(0, LightningPosition);
		lineRenderer.SetPosition(numOfSegments - 1, LightningPosition + (Direction * distance));
		calculateLinePoints(distance);
	}

}