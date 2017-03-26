using UnityEngine;
using System.Collections;

public enum blockType: int{
	earth = 1,
	fire = 2,
	water = 3,
	air = 4,
	any = 5,
	non = 0
}
;

public class GridObject:MonoBehaviour ,IGetPartBlocks{
	public Blocks[,] gridSpaces;
	protected MainGrid main;
	public int length, hight, startX, startY;

	public virtual blockType[,] getBlockTyepes(){
		throw new System.NotImplementedException();
	}
	
	public virtual string getName(){
		throw new System.NotImplementedException();
	}

	public virtual string getDiscription(){
		throw new System.NotImplementedException();
	}
	
	/** all information must be filled before this method can be used 
	 the length an hight are filled by the child script the startX and StartY
	 are filled by the SelectionGrid */
	//not used
	protected void fillGridSpaces(MainGrid m, int length, int hight, int startX, int startY){
		main = m;
		gridSpaces = new Blocks[length, hight];
		GameObject tempCube;
		for (int y = 0; y < hight; y++){
			for (int x = 0; x < length; x++){
				tempCube = Instantiate(main.basicCube)as GameObject;
				tempCube.transform.SetParent(transform);
				gridSpaces [x, y] = tempCube.GetComponent<GridBlock>();
				gridSpaces [x, y].setMainGrid(main);
				gridSpaces [x, y].setLocalGridLocation(startX + x, startY + y);
			}
		}
	}
}
