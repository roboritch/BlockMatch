using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading;

public class MainGrid : MonoBehaviour{
	
	#region otherVars
	public Text selected;
	public Text currentSelectionMode;
	public DefaultTextHolder universalInformationDisplay;
	
	private int currentSelectionModeLength;
	public GameObject selectionSizeEmpty;
	

	private Blocks[,] grid;
	public Blocks getBlockAtLocation(int x, int y){
		return grid [x, y];
	}
	
	private GameObject[,] selectionEmpties;
	private SelectionPlaneScript[,] selectionPlanes;
	private SelectionGrid shipSelection;
	private SelectionGrid partSelection;
	public SelectionGrid getPartSelection(){
		return partSelection;
	}
	
	public bool selectionRemoveMode;
	#endregion
		
	/*
	* y|
	*  |
	*  |
	*  |
	*  |
	* 	|
	*  \-----------
	* 0,0		   x
	* x,y
	*/
	#region prefabVars
	public GameObject basicCube;
	private GameObject emptyCube;
	private GameObject redCube;
	private GameObject blueCube;
	private GameObject yellowCube;
	private GameObject greenCube;
	private GameObject brownCube;
	private GameObject whiteCube;
	private GameObject dash;
	private GameObject empty;
	private GameObject selectionPlane;
	private GameObject selectionParent;
	
	public GameObject airCubeDisplay;
	public GameObject fireCubeDisplay;
	public GameObject earthCubeDisplay;
	public GameObject waterCubeDisplay;
	public GameObject anyCubeDisplay;
	
	private Material dashMat;
	//TODO get dashHandler to implment these mats
	/*these vars are used to switch bettwean the differnet dash matereals*/
	private Renderer defaultDashRenderer;
	#endregion
	
	#region testing vars
	private GameObject tempCube;
	private Blocks tempBlock;
	
	#endregion
	
	#region tweaking values
	public int gridWidth = 12;
	public int gridHight = 16;
	public int numberOfBlockTypes;
	private float screenDistance = 0.0f;
	#endregion

	#region testing 
	private int[] xyCubeSelection = new int[2]; // no selection is -1,-1
	private int[] xyCubehovering = new int[2];
	
	public void setSelectedCube(int x, int y){
		xyCubeSelection [0] = x;
		xyCubeSelection [1] = y;
	}
	
	public void setMouseHoverCube(int x, int y){
		xyCubehovering [0] = x;
		xyCubehovering [1] = y;
	}
	
	/** remove a block and cube at the asinged coordinates */
	private void removeBlock(int x, int y){
		Destroy(grid [x, y].gameObject);
	}
	
	/**instantly move 2 blocks, does not work without setup*/
	public void CubeMoveAction(){
		if (xyCubeSelection [0] != -1 && xyCubehovering [0] != -1){
			tempBlock = grid [xyCubehovering [0], xyCubehovering [1]];
			grid [xyCubehovering [0], xyCubehovering [1]] = grid [xyCubeSelection [0], xyCubeSelection [1]];
			grid [xyCubeSelection [0], xyCubeSelection [1]] = tempBlock;
			grid [xyCubehovering [0], xyCubehovering [1]].transform.localPosition.Set(xyCubehovering [0], xyCubehovering [1], 0);
			grid [xyCubeSelection [0], xyCubeSelection [1]].transform.localPosition.Set(xyCubeSelection [0], xyCubeSelection [1], 0);
			grid [xyCubehovering [0], xyCubehovering [1]].setLocalGridLocation(xyCubehovering [0], xyCubehovering [1]);
			grid [xyCubeSelection [0], xyCubeSelection [1]].setLocalGridLocation(xyCubeSelection [0], xyCubeSelection [1]);
		}
	}
	
	/** This method is used to test parts of other methods for errors */
	public void testMethod(Blocks block){
		currentDragLine.addLast(block);
	}
	
	#endregion

	#region initalization
	// Use this for initialization
	void Start(){
		selectionRemoveMode = false;
		grid = new Blocks[gridWidth, gridHight]; // set basic grid dimentions
		selectionPlanes = new SelectionPlaneScript[gridWidth, gridHight];
		selectionEmpties = new GameObject[gridWidth + 1, gridHight + 1];
		
		shipSelection = new SelectionGrid(this);
		partSelection = new SelectionGrid(this);
		
		
		selectionMode = SelectionPlaneScript.textureState.off;
		currentSelectionModeLength = currentSelectionMode.text.Length;
		currentSelectionMode.text += "DragLine";
		
		loadResorces();
		//fill the grid with empty blocks to prevent null errors
		for (int y = 0; y < gridHight; y++){
			for (int x = 0; x < gridWidth; x++){
				tempCube = Instantiate(emptyCube)as GameObject;
				tempCube.transform.SetParent(transform);
				grid [x, y] = tempCube.AddComponent<Blocks>();
				grid [x, y].setMainGrid(this);
			}
		}

		selectionSizeEmpty.transform.SetParent(transform, true);
		cubeDistace = -1f * selectionSizeEmpty.transform.localPosition.x;
		//set cube selections to defalt
		xyCubeSelection [0] = -1;
		xyCubeSelection [1] = -1;
		xyCubehovering [0] = -1;
		xyCubehovering [1] = -1;
		buildSelectionSystem();
		createSelectionGUI(Color.red); // the first dash line does not need a colour
		clawControler = claw.GetComponents<ClawAnimator>() [0];
		createSelectionSystem();
	}
	
	/**This meathod will create an effishet meathod for creating the grapical component of the selection system */
	private void createSelectionGUI(Color lastSelectionColor){
		GameObject dashS = Instantiate(dash) as GameObject;
		GameObject dashP = Instantiate(empty) as GameObject;
		dashS.name = "DASHSORCE";
		dashP.name = "DashParent";
		dashS.transform.SetParent(dashP.transform);
		Material instancedDashMat = new Material(Shader.Find("Standard"));
		instancedDashMat.CopyPropertiesFromMaterial(dashMat);
		if (currentDragLine == null){
			currentDragLine = new DragLine(dashS, dashP);
			currentDragLine.dashHandler.dashParRenderer.material = instancedDashMat; //each dash sorce must have it's own instance of material
		} else{
			//change the color of the dashes in the current drag line
			//change the color of all of the dashes to show that the selection is done 
			currentDragLine.dashHandler.dashParRenderer.sharedMaterial.color = lastSelectionColor;
			//creat a new drag line
			//all the other dashes are move down so the user can see the current selection
			currentDragLine.dashHandler.parent.transform.Translate(0f, 0f, 0.01f); // move the movment selections behined the current user created selection
			currentDragLine.prev = new DragLine(dashS, dashP); //new drag line as the first one
			currentDragLine.prev.next = currentDragLine; //give the new drag line the pointer for the current one
			currentDragLine = currentDragLine.prev; //make the current drag line the new one
			//the drag line expends to the left of the current one
			//the next movment opperation alwayes move to the left 
			currentDragLine.dashHandler.dashParRenderer.material = instancedDashMat; //each dash sorce must have it's own instance of material
		
		}
	}
	
	/**
* load all cube objects at start for use later
* TODO finish prefabs
*/
	private void loadResorces(){
		emptyCube = Resources.Load("CubePrefabs/Empty Cube")as GameObject;
		redCube = Resources.Load("CubePrefabs/redCube")as GameObject;
		blueCube = Resources.Load("CubePrefabs/blueCube")as GameObject;
		yellowCube = Resources.Load("CubePrefabs/yellowCube")as GameObject;
		greenCube = Resources.Load("CubePrefabs/greenCube")as GameObject;
		brownCube = Resources.Load("CubePrefabs/brownCube")as GameObject;
		whiteCube = Resources.Load("CubePrefabs/whiteCube")as GameObject;
		
		selectionPlane = Resources.Load("selectionPlane/SelectionPlane")as GameObject;
		
		dash = Resources.Load("single dash")as GameObject;
		empty = Resources.Load("Empty")as GameObject;
		dashMat = Resources.Load("Defalt dash")as Material;
	}
	
	private float cubeDistace;
	public void buildSelectionSystem(){
		GameObject selectionEmpty = Resources.Load("SelectionEmpty") as GameObject;

		float CD2 = cubeDistace * 2;
		for (int y = 0; y < gridHight+1; y++){
			for (int x = 0; x < gridWidth+1; x++){
				selectionEmpties [x, y] = Instantiate(selectionEmpty) as GameObject;
				selectionEmpties [x, y].transform.SetParent(transform);
				selectionEmpties [x, y].transform.localPosition = new Vector3((float)x * CD2 - cubeDistace, (float)y * CD2 - cubeDistace);
			}
		}
	}
	
	/**starts the game with a grid*/
	public void FillGrid(int fillHight){
		System.Random rnd = new System.Random();
		int blockType;
		for (int y = 0; y < fillHight; y++){
			for (int x = 0; x < gridWidth; x++){
				removeBlock(x, y);
				blockType = rnd.Next(numberOfBlockTypes);
				if (blockType == 0){
					tempCube = Instantiate(redCube)as GameObject;
					grid [x, y] = tempCube.GetComponent<RedBlock>();
					tempCube.transform.SetParent(transform);
				} else if (blockType == 1){
					tempCube = Instantiate(blueCube)as GameObject;
					grid [x, y] = tempCube.GetComponent<BlueBlock>();
					tempCube.transform.SetParent(transform);
				} else if (blockType == 2){
					tempCube = Instantiate(brownCube)as GameObject;
					grid [x, y] = tempCube.GetComponent<BrownBlock>();
					tempCube.transform.SetParent(transform);
				} else if (blockType == 3){
					tempCube = Instantiate(whiteCube)as GameObject;
					grid [x, y] = tempCube.GetComponent<WhiteBlock>();
					tempCube.transform.SetParent(transform);
				} else if (blockType == 4){
					tempCube = Instantiate(yellowCube)as GameObject;
					grid [x, y] = tempCube.GetComponent<YellowBlock>();
					tempCube.transform.SetParent(transform);
				} else if (blockType == 5){
					tempCube = Instantiate(greenCube)as GameObject;
					grid [x, y] = tempCube.GetComponent<GreenBlock>();
					tempCube.transform.SetParent(transform);
				}
				grid [x, y].transform.localPosition = new Vector3(x, y, screenDistance);
				grid [x, y].setMainGrid(this);
				grid [x, y].setLocalGridLocation(x, y);
			}
		}
	}
		
	/**a layer of invisable plains lets the player select parts of the grid regardless of cube movment*/
	private void createSelectionSystem(){		
		selectionParent = Instantiate(empty);
		selectionParent.name = "selection system parent";
		selectionParent.transform.SetParent(transform);
		selectionParent.transform.localPosition = new Vector3(0, 0, 0);
		GameObject tempPlane;
		float plainDistance = screenDistance - 0.6f;
		for (int y = 0; y < gridHight; y++){
			for (int x = 0; x < gridWidth; x++){
				tempPlane = Instantiate(selectionPlane)as GameObject;
				selectionPlanes [x, y] = tempPlane.GetComponent<SelectionPlaneScript>();
				selectionPlanes [x, y].planeColider = tempPlane.transform.GetComponent<MeshCollider>();
				tempPlane.transform.SetParent(selectionParent.transform);
				tempPlane.transform.localPosition = new Vector3(x, y, plainDistance);
				tempPlane.GetComponent<SelectionPlaneScript>().setVars(x, y, this);
			}
		}
	}
	
	#endregion
	
	#region selection operations
	/** returns true if the checkBlock is directly adjasent to blockMiddle(up,down,left,right, no diaganals)*/	
	private DragLine currentDragLine;
	public bool cubeAdjacent(Blocks blockMiddle, Blocks checkBlock){
		int[] xyMid = new int[2];
		xyMid [0] = blockMiddle.getLocX();
		xyMid [1] = blockMiddle.getLocY();
		int[] xySurround = new int[2];
		xySurround [0] = checkBlock.getLocX();
		xySurround [1] = checkBlock.getLocY();
		return  xySurround [0] == xyMid [0] + 1 && xySurround [1] == xyMid [1] ||
			xySurround [0] == xyMid [0] - 1 && xySurround [1] == xyMid [1] ||
			xySurround [1] == xyMid [1] + 1 && xySurround [0] == xyMid [0] ||
			xySurround [1] == xyMid [1] - 1 && xySurround [0] == xyMid [0];
	}
	
	public cubeAdjLocation cubeLocation(Blocks blockMiddle, Blocks checkBlock){
		int[] xyMid = new int[2];
		xyMid [0] = blockMiddle.getLocX();
		xyMid [1] = blockMiddle.getLocY();
		int[] xySurround = new int[2];
		xySurround [0] = checkBlock.getLocX();
		xySurround [1] = checkBlock.getLocY();
		if (xySurround [0] == xyMid [0] + 1 && xySurround [1] == xyMid [1]){
			return cubeAdjLocation.right;
		} else if (xySurround [0] == xyMid [0] - 1 && xySurround [1] == xyMid [1]){
			return cubeAdjLocation.left;
		} else if (xySurround [1] == xyMid [1] + 1 && xySurround [0] == xyMid [0]){
			return cubeAdjLocation.top;
		} else if (xySurround [1] == xyMid [1] - 1 && xySurround [0] == xyMid [0]){
			return cubeAdjLocation.bottem;
		} else{
			Debug.LogWarning("no cube detected error");
			return cubeAdjLocation.non;
		}
	}
	
	public enum cubeAdjLocation : int{
		non = 0,
		bottem = 1,
		left = 2,
		top = 3,
		right = 4}
	;
	
	// add a cube to the current movment path
	public void tryAddCubeToDragLine(Blocks block){
		if (selectionMode == SelectionPlaneScript.textureState.off){
			bool inCurrentSelection;
			if (block.getCurrentDragLine() == null){
				inCurrentSelection = false;
			} else{
				inCurrentSelection = block.getCurrentDragLine().Equals(currentDragLine);
			}
			
			if (currentDragLine.lastBlockPosition == -1 && !block.inSelection()){ // start the list
				currentDragLine.addLast(block); //add the first block
			} else if (currentDragLine.lastBlockPosition == 0 && block.getDLPos() == 0 && inCurrentSelection){
				currentDragLine.removeLast(); // end the current selection on re-click
			} else if (inCurrentSelection && !(currentDragLine.lastBlockPosition == block.getDLPos())){ 
				currentDragLine.shortenLineUpTo(block.getDLPos()); // remove all blocks up to the the current click
			} else if (block.inSelection()){
				return; // the block is alredy in a selection that is not this one 
			} else if (cubeAdjacent(currentDragLine.getLastBlock(), block)){
				currentDragLine.addLast(block); //add a new block to the selection
			} else{
				Debug.LogWarning("unknown cube selection"); // somthing went wrong
			}
			drawOutlines(currentDragLine); // draw the new outlines for the current selection
		}
	}

	
	public SelectionPlaneScript.textureState selectionMode;
	public SelectionGrid makeSelection(){
		switch (selectionMode){
			case(SelectionPlaneScript.textureState.partSelected):
				return partSelection;
			case(SelectionPlaneScript.textureState.shipSelected):
				return shipSelection;
			default:
				Debug.LogError("not valid selection mode for this object");
				return null;
		}
	}

	#endregion

	#region part selection ui
	public Text partDisplayName;
	public Text partDisplayDiscription;
	
	
	#endregion
	
	#region Outlines
	
	//simple cases to make building dashes easer
	#region Cases
	public void drawCase1Bottem(Blocks block){
		drawHorizontalDashes(false, false, block);
	}

	public void drawCase2Left(Blocks block){
		drawVerticalDashes(true, false, block); 
	}

	public void drawCase3Top(Blocks block){
		drawHorizontalDashes(true, true, block);
	}

	public void drawCase4Right(Blocks block){
		drawVerticalDashes(false, true, block);
	}
	#endregion
	
	/**returns the last case
	  *this runs on a base 4 number system 1 to 4
	  *caseToDo should be given the last case */
	public int drawCaseUpTo(Blocks currentBlock, int caseToDo, int numCases){
		if (numCases < 0){			
			numCases += 4;
		}
		while (numCases > 0){
			if (caseToDo == 1){
				drawCase1Bottem(currentBlock);
				caseToDo++;
			} else if (caseToDo == 2){
				drawCase2Left(currentBlock);
				caseToDo++;
			} else if (caseToDo == 3){
				drawCase3Top(currentBlock);
				caseToDo++;
			} else if (caseToDo == 4){
				drawCase4Right(currentBlock);
				caseToDo = 1;
			}
			numCases--;
		}
		caseToDo--;
		if (caseToDo <= 0){
			return caseToDo + 4;
		} else{
			return caseToDo;
		}
	}
	
	/** used to minamize the number of vareables passed */
	public DragLine runningLine;
	/**
	 * will draw all dashes for a perticular movment pattren 
	 */
	public void drawOutlines(DragLine drawing){
		runningLine = drawing;
		
		drawing.dashHandler.curretDash = 0;
		while (!drawing.dashHandler.isEmpty()){
			Destroy(drawing.dashHandler.removeLastDash());
		}
		drawing.positonalIndicator = 0;
		int lastCase = 0;
		
		if (drawing.lastBlockPosition == -1){
			return;
		}
		
		if (drawing.lastBlockPosition == 0){
			drawHorizontalDashes(false, false, drawing.currentBlock()); // case 1: bottem, right to left
			drawVerticalDashes(true, false, drawing.currentBlock());   //  case 2: left, bottem to top
			drawHorizontalDashes(true, true, drawing.currentBlock()); //   case 3: top, left to right
			drawVerticalDashes(false, true, drawing.currentBlock()); //	case 4: right, top to bottem
			return;
		}
		
		if (cubeLocation(drawing.currentBlock(), drawing.nextBlock()) == cubeAdjLocation.right){ //left
			drawCase1Bottem(drawing.currentBlock());
			drawCase2Left(drawing.currentBlock());
			drawCase3Top(drawing.currentBlock());
			lastCase = 3;
		} else if (cubeLocation(drawing.currentBlock(), drawing.nextBlock()) == cubeAdjLocation.bottem){ //left
			drawCase2Left(drawing.currentBlock());
			drawCase3Top(drawing.currentBlock());
			drawCase4Right(drawing.currentBlock());
			lastCase = 4;
		} else if (cubeLocation(drawing.currentBlock(), drawing.nextBlock()) == cubeAdjLocation.left){ //left
			drawCase3Top(drawing.currentBlock());
			drawCase4Right(drawing.currentBlock());
			drawCase1Bottem(drawing.currentBlock());
			lastCase = 1;
		} else if (cubeLocation(drawing.currentBlock(), drawing.nextBlock()) == cubeAdjLocation.top){ //left
			drawCase4Right(drawing.currentBlock());
			drawCase1Bottem(drawing.currentBlock());
			drawCase2Left(drawing.currentBlock());
			lastCase = 2;
		} 
		
		int cubeAdjNumber;
		drawing.positonalIndicator++;
		
		if (drawing.nextBlock() == null){
			cubeAdjNumber = (int)cubeLocation(drawing.currentBlock(), drawing.prevBlock());
			lastCase = drawCaseUpTo(drawing.currentBlock(), lastCase, cubeAdjNumber - lastCase);
			return;
		}
		//draw all dashes up to the last case
		while (drawing.positonalIndicator < drawing.lastBlockPosition){
			//If the next block is null then the line is only to blocks long and must be compleated early
			cubeAdjNumber = (int)cubeLocation(drawing.currentBlock(), drawing.nextBlock());
			lastCase = drawCaseUpTo(drawing.currentBlock(), lastCase, cubeAdjNumber - lastCase);
			drawing.positonalIndicator++;
		}

		//draw the last case
		cubeAdjNumber = (int)cubeLocation(drawing.currentBlock(), drawing.prevBlock());
		lastCase = drawCaseUpTo(drawing.currentBlock(), lastCase, cubeAdjNumber - lastCase);
		drawing.positonalIndicator--;
		while (drawing.positonalIndicator >= drawing.firstBlockPostion+1){
			cubeAdjNumber = (int)cubeLocation(drawing.currentBlock(), drawing.prevBlock());
			lastCase = drawCaseUpTo(drawing.currentBlock(), lastCase, cubeAdjNumber - lastCase);
			drawing.positonalIndicator--;
		}	
	}

	private int numberOfDashes = 3;
	private float dashDistance = 0.4f;
	private float dashDistanceFromScreen = -0.6f;
	/**
	 * draw all the dashes for the bottem part of the cube
	 */
	public void drawHorizontalDashes(bool leftToRight, bool top, Blocks block){
		int dashNumber = 0;
		GameObject curretDash; //set the parent of the curret dash to be the grid
		//first: set transform to proper coner of the cube 
		//second: set rotation
		while (dashNumber != numberOfDashes){
			curretDash = Instantiate(runningLine.dashHandler.dashSorce)as GameObject;
			curretDash.transform.SetParent(runningLine.dashHandler.parent.transform, false);
			if (leftToRight){
				if (top){
					curretDash.transform.localPosition = selectionEmpties [block.getLocX(), block.getLocY() + 1].transform.position;
					curretDash.transform.localEulerAngles = new Vector3(90f, 270f, 0f);
				} else{
					curretDash.transform.localPosition = selectionEmpties [block.getLocX(), block.getLocY()].transform.position;
					curretDash.transform.localEulerAngles = new Vector3(270f, 270f, 0f);
				}
				curretDash.transform.localPosition = new Vector3(curretDash.transform.localPosition.x + dashDistance * dashNumber, curretDash.transform.localPosition.y, curretDash.transform.localPosition.z + dashDistanceFromScreen);
			} else{
				if (top){
					curretDash.transform.localPosition = selectionEmpties [block.getLocX() + 1, block.getLocY() + 1].transform.position;
					curretDash.transform.localEulerAngles = new Vector3(90f, 270f, 0f);
				} else{
					curretDash.transform.localPosition = selectionEmpties [block.getLocX() + 1, block.getLocY()].transform.position;
					curretDash.transform.localEulerAngles = new Vector3(270f, 270f, 0f);	
				}
				curretDash.transform.localPosition = new Vector3(curretDash.transform.localPosition.x - dashDistance * dashNumber, curretDash.transform.localPosition.y, curretDash.transform.localPosition.z + dashDistanceFromScreen);
			}
			currentDragLine.dashHandler.addNextDash(curretDash); //add to a list of dashes for deletion and animation
			dashNumber++;
		}
	}
	
	public void drawVerticalDashes(bool bottemToTop, bool right, Blocks block){
		int dashNumber = 0;
		GameObject curretDash;//set the parent of the curret dash to be the grid
		//first: set transform to proper coner of the cube 
		//second: set rotation
		while (dashNumber != numberOfDashes){
			curretDash = Instantiate(runningLine.dashHandler.dashSorce)as GameObject;
			curretDash.transform.SetParent(runningLine.dashHandler.parent.transform, false);
			if (bottemToTop){
				if (right){
					curretDash.transform.localPosition = selectionEmpties [block.getLocX() + 1, block.getLocY()].transform.position;
					curretDash.transform.localEulerAngles = new Vector3(180f, 270f, 0f);
				} else{
					curretDash.transform.localPosition = selectionEmpties [block.getLocX(), block.getLocY()].transform.position;
					curretDash.transform.localEulerAngles = new Vector3(180f, 270f, 0f);
				}
				curretDash.transform.localPosition = new Vector3(curretDash.transform.localPosition.x, curretDash.transform.localPosition.y + dashDistance * dashNumber, curretDash.transform.localPosition.z + dashDistanceFromScreen);
			} else{
				if (right){
					curretDash.transform.localPosition = selectionEmpties [block.getLocX() + 1, block.getLocY() + 1].transform.position;
					curretDash.transform.localEulerAngles = new Vector3(0f, 270f, 0f);
				} else{
					curretDash.transform.localPosition = selectionEmpties [block.getLocX(), block.getLocY() + 1].transform.position;
					curretDash.transform.localEulerAngles = new Vector3(0f, 270f, 0f);
				}
				curretDash.transform.localPosition = new Vector3(curretDash.transform.localPosition.x, curretDash.transform.localPosition.y - dashDistance * dashNumber, curretDash.transform.localPosition.z + dashDistanceFromScreen);
			}
			currentDragLine.dashHandler.addNextDash(curretDash); //add to a list of dashes for deletion and animation
			dashNumber++;
		}
	}
	#endregion
	
	#region cube Movment animation system
	private bool movingThings = false;
	public GameObject claw;
	private ClawAnimator clawControler;
	public void startCubeMovment(){
		DragLine dl = currentDragLine;
		while (dl.next != null){
			dl = dl.next;
		}
		dl.startMovment(this);	
	}
	
	public void setBlockPositionInGrid(Blocks block, int x, int y){
		block.setLocalGridLocation(x, y);
		grid [x, y] = block;
	}
	
	private DragLine moveingDragLine;
	private bool readyingForDropoff = false;
	public void readyGridForDropoff(){
		Debug.Log("readying for dropoff");
		
		moveingDragLine = currentDragLine;
		if (moveingDragLine.next != null){
			while (moveingDragLine.next != null){
				moveingDragLine = moveingDragLine.next; //the last drag line is always the one moveing 
				//be sure to remove the last drag line before this is called again
			}
			moveingDragLine.positonalIndicator = moveingDragLine.firstBlockPostion + 1;
			
			//these two instructions are order spesific
			setWantedLocationForCubeMovementOnGrid();
			clawControler.moveBesideGrid(moveingDragLine.getFirstBlock(), moveingDragLine.getLastBlock().transform.position);
			clawControler.setDropoffGrid(moveingDragLine.getLastBlock().x, moveingDragLine.getLastBlock().y);
			
			readyingForDropoff = true;
		}
	}
	
	/** this var is used to set the proper grid location without messing the current movment up */
	private int prevBlockCurrent = -1;
	public void setWantedLocationForCubeMovementOnGrid(){
		cubeAdjLocation adj = cubeLocation(moveingDragLine.currentBlock(), moveingDragLine.prevBlock());
		if (prevBlockCurrent == 0){
			moveingDragLine.prevBlock().x--;
		} else if (prevBlockCurrent == 1){
			moveingDragLine.prevBlock().x++;
		} else if (prevBlockCurrent == 2){
			moveingDragLine.prevBlock().y++;
		} else if (prevBlockCurrent == 3){
			moveingDragLine.prevBlock().y--;
		}	
		switch (adj){
			case cubeAdjLocation.left:
				wantedLocation = moveingDragLine.currentBlock().transform.position;
				wantedLocation.x--;	
				prevBlockCurrent = 0;
				break;
			case cubeAdjLocation.right:
				wantedLocation = moveingDragLine.currentBlock().transform.position;
				wantedLocation.x++;
				prevBlockCurrent = 1;
				break;					
			case cubeAdjLocation.top:
				wantedLocation = moveingDragLine.currentBlock().transform.position;
				wantedLocation.y++;
				prevBlockCurrent = 2;
				break;
			case cubeAdjLocation.bottem:
				wantedLocation = moveingDragLine.currentBlock().transform.position;
				wantedLocation.y--;
				prevBlockCurrent = 3;
				break;
			case cubeAdjLocation.non:
				Debug.LogError("cube movment error, not adjacent location");
				break;
		}
	}

	/** gives the last cube moved by the grid to the proper position and removes the current drag line properly*/
	public void destroyMovingDragLine(){
		if (prevBlockCurrent == 0){
			moveingDragLine.prevBlock().x--;
		} else if (prevBlockCurrent == 1){
			moveingDragLine.prevBlock().x++;
		} else if (prevBlockCurrent == 2){
			moveingDragLine.prevBlock().y++;
		} else if (prevBlockCurrent == 3){
			moveingDragLine.prevBlock().y--;
		}			
		moveingDragLine.shortenLineUpTo(0);
		Destroy(moveingDragLine.dashHandler.dashSorce);
		Destroy(moveingDragLine.dashHandler.parent);
		moveingDragLine.prev.next = null;
		moveingDragLine.next = null;
		moveingDragLine = null;
	}
	
	/** gets rid of the current drag line used for movment and try's to start the next movment*/
	public bool endMovment(){
		destroyMovingDragLine();
		movingThings = false;
		return currentDragLine.next != null;
	}
	
	public ClawAnimator getClawControler(){
		return clawControler;
	}

	#endregion
	
	#region updates
	
	void Update(){
		#region dragLine selection display 
		if (!currentDragLine.isEmpty()){
			int counter = currentDragLine.firstBlockPostion;
			selected.text = selected.text.Substring(0, 10);
			while (counter <= currentDragLine.lastBlockPosition){
				selected.text += "(" + currentDragLine.dragLineArray [counter].getLocX() + "," + currentDragLine.dragLineArray [counter].getLocY() + ")\n";
				counter++;
			}
		}
		#endregion
		
		#region slection mode cycle
		if (Input.GetKeyDown(KeyCode.S)){
			currentSelectionMode.text = currentSelectionMode.text.Substring(0, currentSelectionModeLength);
			switch (selectionMode){
				case(SelectionPlaneScript.textureState.off):
					selectionMode = SelectionPlaneScript.textureState.partSelected;
					currentSelectionMode.text += "part";
					for (int x = 0; x < gridWidth; x++){ //enable clicking on the selection planes
						for (int y = 0; y < gridHight; y++){
							selectionPlanes [x, y].planeColider.enabled = true;
						}
					}
					break;
				case(SelectionPlaneScript.textureState.partSelected):
					selectionMode = SelectionPlaneScript.textureState.shipSelected;
					currentSelectionMode.text += "ship";
					break;
				case(SelectionPlaneScript.textureState.shipSelected):
					selectionMode = SelectionPlaneScript.textureState.off;
					currentSelectionMode.text += "dragline";
					for (int x = 0; x < gridWidth; x++){
						for (int y = 0; y < gridHight; y++){ //disable clicking on the selection planes
							selectionPlanes [x, y].planeColider.enabled = false;
						}
					}
					break;
				default:
					selectionMode = SelectionPlaneScript.textureState.off;
					currentSelectionMode.text += "dragline";
					break;
			}		
		}
		#endregion
		
		#region finalize dragLine
		if (Input.GetKeyDown(KeyCode.F)){
			if (selectionMode == SelectionPlaneScript.textureState.off){
				Debug.Log("finalizing Cube");
				if (currentDragLine.lastBlockPosition <= 0){
					currentDragLine.removeLast();
				} else{
					//TODO drag line finalization 
					if (!movingThings || clawControler.betweenStatesBugFix){
						movingThings = true;
						startCubeMovment();
					}
					createSelectionGUI(Color.blue);
				}
			} else if (selectionMode == SelectionPlaneScript.textureState.partSelected){
				partSelection.createPart();
			} else if (selectionMode == SelectionPlaneScript.textureState.shipSelected){
				shipSelection.launchShip();
			}
		}
		#endregion
	}

	
	public float FU_cubeMoveSpeed;
	private Vector3 wantedLocation = new Vector3(-1111.01f, -1111.21f, -1111.64f);
	void FixedUpdate(){
		#region ready grid for cube dropOf by claw
		if (readyingForDropoff){
			moveingDragLine.currentBlock().transform.position = Vector3.MoveTowards(moveingDragLine.currentBlock().transform.position, wantedLocation, FU_cubeMoveSpeed);
			if (moveingDragLine.currentBlock().transform.position == wantedLocation){ //if non is set wanted location is an very unlikly float
				//TODO see if this sets the location properly 
				setBlockPositionInGrid(moveingDragLine.currentBlock(), moveingDragLine.currentBlock().getLocX(), moveingDragLine.currentBlock().getLocY());
				moveingDragLine.positonalIndicator++;
				if (moveingDragLine.positonalIndicator == moveingDragLine.lastBlockPosition + 1){
					readyingForDropoff = false;
					Debug.Log("cube movment is done");
					//sets all block positions NOT including the one grabed by the claw 
					currentDragLine.positonalIndicator = currentDragLine.firstBlockPostion + 1;
					while (moveingDragLine.positonalIndicator != moveingDragLine.lastBlockPosition + 1){
						grid [moveingDragLine.currentBlock().x, moveingDragLine.currentBlock().y] = moveingDragLine.currentBlock();
						moveingDragLine.positonalIndicator++;
					}
					clawControler.setCurrentStateToDropOffCube();
				} else{
					setWantedLocationForCubeMovementOnGrid();
				}
			}
		}
		#endregion
	}
	#endregion
}