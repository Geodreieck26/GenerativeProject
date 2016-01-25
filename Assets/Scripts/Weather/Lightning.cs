using UnityEngine;
using System.Collections;

public class Lightning : MonoBehaviour {

	private LineRenderer lineRenderer;
	private WeatherController controller;
	
	private int numOfSegments;
	private Color colourFloor = Color.white;
	private Color colourSky = Color.blue;
	private float lightningOffset;
	private float lineWidth;

	private Vector3 direction;
	private Vector3 lightningPosition;


	private float widthFadeTimer = 0f;
	private float timerHelper = 0f;
		
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
		this.lightningOffset = controller.lightningOffset;

		lineRenderer.SetVertexCount(numOfSegments);		
		lineRenderer.SetWidth(lineWidth / 2f, lineWidth / 2f);
		lineRenderer.SetColors (colourSky, colourFloor);

		generateRandomLightnings ();
		setLightning ();
	}

	void Update() {
		SizeFadeOut ();
	}

	void generateRandomLightnings () {	
		lightningPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);		
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

	private void SizeFadeOut() {
		if (timerHelper >= 0.3f / 1.2f){
			float lerpWidth = Mathf.Lerp(lineWidth / 1.2f, 0f, widthFadeTimer / (0.3f / 1.2f));
			lineRenderer.SetWidth(lerpWidth, lerpWidth);
			widthFadeTimer += Time.deltaTime;
		}		
		timerHelper += Time.deltaTime;
	}
	
}