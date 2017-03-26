using UnityEngine;
using System.Collections.Generic;


public class Blocks : MonoBehaviour,IBlockType{
	public MainGrid mainGrid; //must be set at startup of Block
	public bool isPartOfObject;
	
	public virtual blockType getBlockType(){
		return blockType.non;
	}
	
	//all grid objects must be made aware of there curret location on the grid 
	#region grid location
	public int x;
	public int y;

	public void setLocalGridLocation(int xSet, int ySet){
		x = xSet;
		y = ySet;
	}
	
	public void setMainGrid(MainGrid main){
		mainGrid = main;
	}
	
	public int getLocX(){
		return x;
	}
	
	public int getLocY(){
		return y;
	}
	
	#endregion
	
	#region initalization
	void Awake(){
		isPartOfObject = false;
		x = 0;
		y = 0;
	}
	#endregion
	
	#region selection system
	
	public DragLine currentDragline;
	private int dragLinePostion;
	public void addSelection(DragLine dl, int locationIn){
		currentDragline = dl;
		dragLinePostion = locationIn;
	}
	public void removeSelection(){
		currentDragline = null;
	}	
	public bool inSelection(){
		if (currentDragline != null){
			return true;
		}
		return false;
	}
	/**this posion is only valid if inSelection retuns true*/
	public int getDLPos(){
		return dragLinePostion;
	}
	public DragLine getCurrentDragLine(){
		return currentDragline;
	}
	
	#endregion
	
	#region update and Input
	//TODO add cube move events to mouse click actions
	// Update is called once per frame
	void Update(){
		
	}

	void OnMouseDown(){
		if (getBlockType() != blockType.non){
			mainGrid.tryAddCubeToDragLine(this);
		}
	}

	void OnMouseEnter(){
		if (Input.GetMouseButton(0) && getBlockType() != blockType.non){
			mainGrid.tryAddCubeToDragLine(this);
		}
	}

	void OnMouseExit(){

	}

	void OnMouseUp(){

	}
	#endregion
}

interface IBlockType{
	blockType getBlockType();
}