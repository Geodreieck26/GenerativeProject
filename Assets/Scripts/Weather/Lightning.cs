using UnityEngine;
using System.Collections;

public class Lightning : MonoBehaviour {

	private LineRenderer lineRenderer;
	//public WeatherController controller;
	private LightningMaker maker;

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

	private Vector3 lightningPosition;

	public Vector3 LightningPosition {
		get{return this.lightningPosition;}
		set{this.lightningPosition = value;}
	}

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

		generateRandomLightnings ();
		setLightning ();
	}

	void generateRandomLightnings () {	
		lightningPosition = new Vector3(Random.Range (3,-3),3,Random.Range(3,-3));		
	}

	void setLightning() {

			RaycastHit hitInfo;

			if(Physics.Raycast(LightningPosition, Direction, out hitInfo)) {
				generateRay(hitInfo);
				Debug.DrawRay(LightningPosition, hitInfo.point-LightningPosition);
			} 

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