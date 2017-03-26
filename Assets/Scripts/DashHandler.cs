using UnityEngine;
using System.Collections;

public class DashHandler{
	private GameObject[] dashes = new GameObject[400];
	public int lastDash = -1;
	public int curretDash = 0;
	public GameObject dashSorce;
	public Renderer dashParRenderer;
	public GameObject parent;
	
	public DashHandler(GameObject dashS, GameObject dashP){
		dashSorce = dashS;
		parent = dashP;
		dashParRenderer = dashSorce.GetComponent<Renderer>();
	}
	
	/**
	* add a dash to list and incrment list
	*/
	public void addNextDash(GameObject dash){
		lastDash++;
		dash.name = "Dash " + lastDash;
		dashes [lastDash] = dash;
	}
	
	public void setDashColour(Color col){
		dashSorce.GetComponent<Renderer>().sharedMaterial.color = col;
	}
	
	/**to fully remove dash Destroy() must be called from a monobehavior script */
	public GameObject removeLastDash(){
		lastDash--;//incrment occures after return 
		return dashes [lastDash + 1]; 
	}
	
		
	/**gets the dash at the position of pram: currentDash */
	public GameObject getCurrentDash(){
		return dashes [curretDash];
	}
		
	/**returns true if empty */
	public bool isEmpty(){
		return lastDash == -1;
	}

}
