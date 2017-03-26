using UnityEngine;
using System.Collections;
using System.Threading;

public class SelectionGrid{
	/**true for ship false for part*/
	private bool[,] selection; 
	private MainGrid main;
	
	//TODO add a couter for the number of blocks in each colome and row  
	//part selectionOperations
	private int numberOfBlocks;
	private int lowestX, highestX, lowestY, highestY;
	private int[] numBlocksXColumns, numBlocksYRows;
	
	
	/** this is always the lowest block that is farthest left */
	private int blockX, blockY;
	
	public SelectionGrid(MainGrid m){
		main = m;
		
		numberOfBlocks = 0;
		
		numBlocksXColumns = new int[main.gridWidth];
		numBlocksYRows = new int[main.gridHight];
		
		for (int x = 0; x < main.gridWidth; x++){
			numBlocksXColumns [x] = 0;
		}
		for (int y = 0; y < main.gridHight; y++){
			numBlocksYRows [y] = 0;
		}
		
		
		selection = new bool[main.gridWidth, main.gridHight]; // size values are grid width and hight
		for (int x = 0; x < main.gridWidth; x++){
			for (int y = 0; y < main.gridHight; y++){
				selection [x, y] = false;
			}
		}
	}
	
	#region selection operations
	/**returns true if removed selection*/
	public bool selectLocation(int x, int y, SelectionPlaneScript selectionPlane){ 
		if (selection [x, y]){
			removeGridLocationToSelection(x, y, selectionPlane);
			return true;
		} else{
			addGridLocationToSelection(x, y, selectionPlane);
			return false;
		}
	}
	
	public void addGridLocationToSelection(int x, int y, SelectionPlaneScript selectionPlane){
		if (!selection [x, y]){
			if (numberOfBlocks == 0){
				selection [x, y] = true;
				selectionPlane.addTextureType(main.selectionMode);
				numberOfBlocks++;
				
				blockX = x;
				blockY = y;
				lowestX = x;
				lowestY = y;
				highestX = x;
				highestY = y;
				
				numBlocksXColumns [x]++;
				numBlocksYRows [y]++;
				
			} else{
				selectionAdjLocation location = cubeLocation(x, y);
				if (location == selectionAdjLocation.non){
				} else{
					selection [x, y] = true;
					selectionPlane.addTextureType(main.selectionMode);
					numberOfBlocks++;
				
					numBlocksXColumns [x]++;
					numBlocksYRows [y]++;
					
					//set selections bounds
					if (y > highestY){
						highestY = y;
					}
					if (y < lowestY){
						lowestY = y;
					}	
					if (x > highestX){
						highestX = x;
					}
					if (x < lowestX){
						lowestX = x;
					}
				}
			}
		}
	}
	
	public void removeGridLocationToSelection(int x, int y, SelectionPlaneScript selectionPlane){
		if (selection [x, y]){
			if (trueIfbetwean(x, y)){
				main.universalInformationDisplay.flashText(Color.red, 5f);
				main.universalInformationDisplay.addNewTextToDefalt("This cube can not be removed, it is between two others");
				main.universalInformationDisplay.setDefault(5f);
				return;
			}
			selection [x, y] = false;
			selectionPlane.removeTextureType(main.selectionMode);
			numberOfBlocks--;
			
			numBlocksXColumns [x]--;
			numBlocksYRows [y]--;
			
			if (numBlocksXColumns [x] == 0){
				if (x == lowestX){
					lowestX++;
				} else{
					highestX--;
				}
			}
			
			if (numBlocksYRows [y] == 0){
				if (y == lowestY){
					lowestY++;
				} else{
					highestY--;
				}
			}
		}
	}
	
	/** Insures that every added grid location is conected*/
	public selectionAdjLocation cubeLocation(int addedBlockX, int addedBlockY){
		if (selection.GetLength(0) - 1 > addedBlockX)
		if (selection [addedBlockX + 1, addedBlockY]){
			return selectionAdjLocation.left;
		}
		if (addedBlockX != 0)
		if (selection [addedBlockX - 1, addedBlockY]){
			return selectionAdjLocation.right;
		}
		if (selection.GetLength(1) - 1 > addedBlockY)
		if (selection [addedBlockX, addedBlockY + 1]){
			return selectionAdjLocation.top;
		}
		if (addedBlockY != 0)
		if (selection [addedBlockX, addedBlockY - 1]){
			return selectionAdjLocation.bottem;
		}
		return selectionAdjLocation.non;
	}
	
	//TODO find if a cube is betewen 2 others in a line 
	private bool trueIfbetwean(int selectionX, int selectionY){
		if (selection.GetLength(0) - 1 > selectionX || selectionX != 0 || selection.GetLength(1) - 1 > selectionY || selectionY != 0){
			return((selection [selectionX + 1, selectionY] && selection [selectionX - 1, selectionY]) ||
				(selection [selectionX, selectionY + 1] && selection [selectionX, selectionY - 1]));
		} else{
			return false;
		}
		
	}

	
	
	public enum selectionAdjLocation{
		non,
		bottem,
		left,
		top,
		right
	}
	;

	#endregion
	
	#region part creation
	public void createPart(){
		Debug.Log(System.String.Format("part minX,maxX,minY,maxY: {0},{1},{2},{3}", lowestX, highestX, lowestY, highestY));
		blockType[,] partMakeup = new blockType[highestX - lowestX, highestY - lowestY];
		for (int x = lowestX; x < highestX; x++){
			for (int y = lowestX; y < highestY; y++){
				partMakeup [x, y] = main.getBlockAtLocation(x + lowestX, y + lowestY).getBlockType();
			}
		}
		
	}
	
	public void launchShip(){
		
	}
	
	public blockType[,] getCurrentPart(){
		if (numberOfBlocks == 0){
			return new blockType[1, 1];
		}
		blockType[,] blocksSelected = new blockType[1 + highestX - lowestX, 1 + highestY - lowestY];
		for (int x = lowestX; x <= highestX; x++){
			for (int y = lowestY; y <= highestY; y++){
				if (selection [x, y]){
					blocksSelected [x - lowestX, y - lowestY] = main.getBlockAtLocation(x, y).getBlockType();
				} else{
					blocksSelected [x - lowestX, y - lowestY] = blockType.non;
				}
			}
		}
		return blocksSelected;
	}
	
	#endregion
}
