using UnityEngine;
using System.Collections;


/** atached to the a newly instateated part to move it to the proper location */
public class PartDisplay : MonoBehaviour{
	[SerializeField] private float distaceFromLeftOfScreen, addedVerticalDistace, distaceFromCamra, distaceFromRightOfScreen;
	public GameObject bottemLeftCubeLocation;
	public int partNumber = 0;


	// Use this for initialization
	void Start(){
		//default values for distace
		distaceFromRightOfScreen = 90f;
		distaceFromCamra = 24f;
		transform.localScale.Set(1f, 1f, 1f);
		InvokeRepeating("setPartPosition", 0f, 2f);
	}

	public void setPartPosition(){
		// 0,0 is the top left
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(distaceFromRightOfScreen, 0f, distaceFromCamra));
		transform.position = worldPoint;
	}

	
	public void removeThisPart(){
		Destroy(gameObject);
	}

}
