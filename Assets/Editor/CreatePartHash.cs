using UnityEditor;
using UnityEngine;

using System.IO;
using System.Collections;

public class CreatePartHash : Editor{
	
	[MenuItem("Assets/Get Part Hash")]
	static void copyPartHashToClipbord(){

			
		GameObject tempPart = Selection.activeObject as GameObject;
		GridObject gridObject = tempPart.GetComponent<GridObject>();
		
	
		blockType[,] types = gridObject.getBlockTyepes();
		int hashCode = 1;
		for (int x = 0; x < types.GetLength(0); x++){
			for (int y = 0; y < types.GetLength(1); y++){
				hashCode *= (int)types [x, y] * (x + (y * 20)) + 1;	
			}
		}
		Debug.Log("part hash is this: " + hashCode);
		EditorGUIUtility.systemCopyBuffer = hashCode.ToString();
		
		
	}
	
	
}
