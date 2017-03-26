using UnityEngine;
using System.Collections;

public class ElementalFireEnginePart : GridObject{
	//TODO scail of this part from blender
	public string partName = "elemental fire engine";
	public TextAsset partDiscription;
	
	/**this number is jenerated through a script that is also found in the part hash method*/ 
	public static int partHash = 25;
	
	public static blockType[,] construction = {{blockType.fire},{blockType.air},{blockType.fire}};
	/** WildCard is orderd starting from 0 in x,y || -1 means there is no wild card  */
	public static int[] WILDCARDLOCATION = {1,0};

	
	#region overide properties
	public override blockType[,] getBlockTyepes(){
		return construction;
	}
	
	public override string getName(){
		return partName;
	}

	public override string getDiscription(){
		return partDiscription.text;
	}
	#endregion
	
	// Use this for initialization
	void Start(){

	}
}
