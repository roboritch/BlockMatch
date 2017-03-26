using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PartHash : MonoBehaviour{
	private Hashtable partList;

	#region part list

	public GameObject elementalFireEngine;

	#endregion

	private GameObject currentPart;
	
	public MainGrid main;
	public Text partName;
	public Text partDiscription;

	/** and parts in this */
	void Start(){
		partList = new Hashtable();
		partList.Add(ElementalFireEnginePart.partHash, elementalFireEngine);
		
		InvokeRepeating("displayCurrentlySelectedPart", 10f, 1f);
	}

	public GameObject getPart(blockType[,] copList){
		int hashCode = getPartConstructionHash(copList);
		if (partList.Contains(hashCode)){
			Debug.Log("geting the part");
			return partList [hashCode] as GameObject;
		} else{
			Debug.Log("no part found");
			return null;
		}
	}

	/// <summary>
	/// Gets the parts hash from how it is constructed.
	/// EDITOR NOTE: this method is not used in the editor script
	/// if it is changed the editor script must be changed to match it.
	/// </summary>
	/// <returns>The parts hash.</returns>
	/// <param name="construction">the location an type of each block that makes up the part.</param>
	private int getPartConstructionHash(blockType[,] construction){
		int hashCode = 1;
		for (int x = 0; x < construction.GetLength(0); x++){
			for (int y = 0; y < construction.GetLength(1); y++){
				hashCode *= (int)construction [x, y] * (x + (y * 20)) + 1;	
			}
		}
		return hashCode;
	}

	#region part display

	/// <summary>
	/// Displaies the currently selected part by creating a part and atatching the PartDisplay script to it.
	/// </summary>
	public void displayCurrentlySelectedPart(){
		Debug.Log("checking part");
		GameObject part = getPart(main.getPartSelection().getCurrentPart());		
		if (part == currentPart){
			return;
		}
		if (part != null){
			currentPart = part;
			part = Instantiate(part) as GameObject;
			PartDisplay currentPartDisplay = part.AddComponent<PartDisplay>();  
			GridObject gridScipt = part.GetComponent<GridObject>();
			currentPartDisplay.bottemLeftCubeLocation = part.transform.FindChild("First Block Location").gameObject;
			//currentPartDisplay.showBlockTypeLocations(gridScipt.getBlockTyepes(), main);
			partName.text = gridScipt.getName();
			partDiscription.text = gridScipt.getDiscription();
		}
	}

	#endregion

	// Update is called once per frame
	void Update(){
		
	}
}