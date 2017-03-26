using UnityEngine;
using System.Collections;

public class AnyCubeColorSwap : MonoBehaviour{
	
	private int colour;
	public Material[] blockTypes;
	private MeshRenderer cubeMat;
	
	
	// Use this for initialization
	void Start(){
		colour = 0;
		cubeMat = transform.GetComponent<MeshRenderer>();
		InvokeRepeating("swapColours", 0f, 0.9f);
	}
	
	// Update is called once per frame
	void Update(){
		
	}
	
	public void  swapColours(){
		cubeMat.material = blockTypes [colour++];
		if (colour == 4){
			colour = 0;
		}
	}
	
}
