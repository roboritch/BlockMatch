using UnityEngine;
using System.Collections;


/**
 * This is an array based data struct with a linked list style
 * blocks can not be removed from the middle of the drag line
 */
public class DragLine{
	public Blocks[] dragLineArray;
	public int lastBlockPosition;
	public int firstBlockPostion;
	public int positonalIndicator;
	public DashHandler dashHandler;
	public DragLine prev;
	public DragLine next;
	public bool hasClawControl;
	public int finalBlockX, finalBlockY;
	
	/**
	 * The defalt dash must be instanteated from a monodovelop class
	 */
	public DragLine(GameObject dashS, GameObject dashP){
		dragLineArray = new Blocks[200];
		dashHandler = new DashHandler(dashS, dashP);
		lastBlockPosition = -1;
		firstBlockPostion = 0;
		positonalIndicator = 0;
		hasClawControl = false;
	}
	
	#region get opperations
	/**returns the last block */
	public Blocks getLastBlock(){
		return dragLineArray [lastBlockPosition];
	}
	
	/** retruns the first block */
	public Blocks getFirstBlock(){
		return dragLineArray [firstBlockPostion];
	}
	
	/** return am block acording to the pram: positonalIndicator */
	public Blocks currentBlock(){
		return dragLineArray [positonalIndicator];
	}
	
	/** retruns the block ahead of the positional indicator without moving it */
	public Blocks nextBlock(){
		return dragLineArray [positonalIndicator + 1];
	}
	
	/** retruns the block behined of the positional indicator without moving */
	public Blocks prevBlock(){
		return dragLineArray [positonalIndicator - 1];		
	}
	#endregion
	
	#region set/remove opperations
	/**adds a block to the end of the list and increments the pointer*/
	public void addLast(Blocks block){
		lastBlockPosition++;
		dragLineArray [lastBlockPosition] = block;
		block.addSelection(this, lastBlockPosition);
	}
		
	/**
	 * this will remove all duplicet blocks exept the last block
	 */
	public void shortenLineUpTo(int newLastBlock){
		if (lastBlockPosition <= 0){
			return;
		}
		while (lastBlockPosition != newLastBlock){
			dragLineArray [lastBlockPosition--].removeSelection();
		}
	}
	
	public void removeLast(){
		dragLineArray [lastBlockPosition].removeSelection();
		lastBlockPosition--;
	}
	
	public void removeFirst(){
		dragLineArray [firstBlockPostion].removeSelection();
		firstBlockPostion++;
	}
	#endregion
	
	public void startMovment(MainGrid grid){
		finalBlockX = getLastBlock().getLocX();
		finalBlockY = getLastBlock().getLocY();
		hasClawControl = true;
		grid.getClawControler().moveBesideGrid(getFirstBlock(), getLastBlock().transform.position);
	}
	

	
	//TODO this can be made into a binary search with a little work
	//check preformance later if neaded
	public bool blockInCurrentSelection(Blocks block){
		for (int x = 0; x < lastBlockPosition; x++){
			if (dragLineArray [x].Equals(block)){
				return true;
			}
		}
		return false;
	}
	
	/** returns true if empty */
	public bool isEmpty(){
		return lastBlockPosition == -1;
	}
}

