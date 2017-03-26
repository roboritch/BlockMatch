using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClawAnimator : MonoBehaviour{
	#region vars
	private Animator clawAnimator;
	/**this is true when the animator is runing and becomes false when it is done the changing of this stat is handled by scripts in the animator itself */
	public bool animationState;
	
	private Blocks grabedGridObject;
	private int blockDropoffX, blockDropoffY;
	public void setDropoffGrid(int x, int y){
		blockDropoffX = x;
		blockDropoffY = y;
	}
	
	public Transform rotatedClawEndLocation;
	private Vector3 wantedBlockLocation;
	private Vector3 blockDropOfLocation;
	private Vector3 idleLocation;
	public void setBlockDropOfLocation(Vector3 DO){
		blockDropOfLocation = DO;
	} 
	
	
	/** there is an empty transform at the end of the claw for this the ray is used to avoid hitting objects */
	public GameObject verticalRaycastObject;
	/** this raycastHit should be asumed ready for information dump an never explicitly reset */ 
	private RaycastHit rayHitInformation;
	/**this raycastHit is reset once the animation in FU is done with it, this is to minimize the number of raycasts done */
	private RaycastHit cubeRaycast;
	
	private clawAction currentState;
	public bool betweenStatesBugFix;
	
	private clawAction nextState;
	public void setCurrentStateToDropOffCube(){
		currentState = clawAction.dropingOffCube;
	}
	
	
	public float clawMoveSpeed;
	public MainGrid mainGrid; //the transform of the main grid is used for som operations
	
	private int currentStateTextLength;
	#endregion
	
	#region enums
	public enum currentLocation{
		moving,
		overGrid,
		inFrontOfGrid,
		launchLocation,
	}
	
	public enum clawAction{
		idle,
		movingToIntermediate,
		movingOverCube,
		movingBesideCube,
		grabingOverCube,
		grabingBesideCube,
		moveingCube,
		dropingOffCube,
		movingToIdle,
	} 
	#endregion
	
	#region initalization
	void Start(){
		currentStateTextLength = stateText.text.Length;
		//just to be safe, this may not be required
		cubeRaycast = new RaycastHit();
		clawAnimator = transform.GetComponent<Animator>();
		//this will allow the animator to send information to this script
		
		idleLocation = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		currentState = clawAction.idle;
		intermediateLocation = new Vector3(transform.position.x, 8f, -1f);
		
		if (verticalRaycastObject == null){
			Debug.LogError("no raycast start location set!");
		}
	}
	#endregion
	
	#region fixed update
	private Vector3 intermediateLocation; 
	public Text stateText;
	 
	void FixedUpdate(){
		//all of the states represent the current action the claw is taking 
		//the state is changed at the end of every state action exept idle
		//the claw is returned to it's resting position only if there is no gridObject to move afterwords
		switch (currentState){
			case clawAction.idle:
				stateText.text = stateText.text.Substring(0, currentStateTextLength);
				stateText.text += "idle";
				break;
			case clawAction.movingToIntermediate:
				stateText.text = stateText.text.Substring(0, currentStateTextLength);
				stateText.text += "moving to intermediate";
				
				
				intermediateLocation.x = transform.position.x;
				transform.position = Vector3.MoveTowards(transform.position, intermediateLocation, clawMoveSpeed);
				if (transform.position == intermediateLocation){
					currentState = nextState; //TODO set the next state properly
				}
				break;
//			case clawAction.movingOverCube: // not used in current ver
//				stateText.text = stateText.text.Substring(0, currentStateTextLength);
//				stateText.text += "moving over cube";
//				
//				if (transform.position.z != 0.5f){
//					transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, 0.5f), clawMovespeed);				
//				} else{	
//					movment = Vector3.MoveTowards(transform.position, wantedBlockLocation, clawMovespeed);
//					Vector3 vroGlobalPosition = verticalRaycastObject.transform.position;
//					
//					//object avodance, there may be jittering at the top of objects.
//					//TODO check to see how movment looks and fix if it is distracting
//					if ((movment - transform.position).x > 0f){ 
//						Physics.Raycast(vroGlobalPosition, Vector3.right, out rayHitInformation, 10f);
//						movment.y += (1f / rayHitInformation.distance); // move up faster the closer the next object is that is in the way
//					} else{
//						Physics.Raycast(vroGlobalPosition, Vector3.left, out rayHitInformation, 10f);	
//						movment.y += (1f / rayHitInformation.distance);
//					}
//					
//					//this stops the claw from falling down into blocks
//					if (Physics.Raycast(vroGlobalPosition, Vector3.down, 1f)){
//						if (movment.y < 0f){
//							movment.y = 0f;
//						} 
//					}
//					transform.position = movment;
//					if (transform.position == wantedBlockLocation){
//						currentState = clawAction.grabingOverCube;
//					}
//				}
//				break;
			case clawAction.movingBesideCube:
				stateText.text = stateText.text.Substring(0, currentStateTextLength);
				stateText.text += "moving besideCube";

				Vector3 adjustedLocation = wantedBlockLocation;
				adjustedLocation -= rotatedClawEndLocation.localPosition;
				adjustedLocation.z = -1.5f;
				transform.position = Vector3.MoveTowards(transform.position, adjustedLocation, clawMoveSpeed);
				
				
				if (transform.position == adjustedLocation){
					currentState = clawAction.grabingBesideCube;
					setRotatedClaw();
				}

				break;
//			case clawAction.grabingOverCube: // this will not be used in the inital relese 
//				stateText.text = stateText.text.Substring(0, currentStateTextLength);
//				stateText.text += "grabing over cube";
//				
//				if (animationState){
//					return;
//				} else{
//					if (cubeRaycast.Equals(new RaycastHit())){
//						//to get direction the vro is subtracted from it's parrents (the the end of the claw) to get an arrow pointing out of the claw
//						Physics.Raycast(verticalRaycastObject.transform.position, verticalRaycastObject.GetComponentInParent<Transform>().position - verticalRaycastObject.transform.position, out cubeRaycast, 1.5f);
//					}
//					//the movment is the vro movment is adjusted to to the root of the claw
//					transform.position = Vector3.MoveTowards(transform.position, rayHitInformation.point, clawMovespeed) - (transform.position - verticalRaycastObject.transform.position);
//				}
//				if (verticalRaycastObject.transform.position == rayHitInformation.point){
//					grabBlock(grabedGridObject);
//					currentState = clawAction.moveingCube;
//					cubeRaycast = new RaycastHit();
//				}
//				break;
			case clawAction.grabingBesideCube:
				stateText.text = stateText.text.Substring(0, currentStateTextLength);
				stateText.text += "grabing beside cube";
				
				if (verticalRaycastObject.transform.position != rotatedClawEndLocation.transform.position){
					return;
				} else{
					if (cubeRaycast.point == new RaycastHit().point){
						//to get direction the vro is subtracted from it's parrents (the the end of the claw) to get an arrow pointing out of the claw
						Debug.Log("raycasting");
						//Debug.DrawRay(verticalRaycastObject.transform.position, -verticalRaycastObject.transform.up, Color.red, 200f);
						Physics.Raycast(verticalRaycastObject.transform.position, -verticalRaycastObject.transform.up, out cubeRaycast, 2.0f);
					}
					//the movment is the vro movment is adjusted to to the root of the claw
					transform.position = Vector3.MoveTowards(transform.position, cubeRaycast.point + (transform.position - verticalRaycastObject.transform.position), clawMoveSpeed);
					if (verticalRaycastObject.transform.position == cubeRaycast.point){
						grabBlock(grabedGridObject);
						currentState = clawAction.moveingCube;
						cubeRaycast = new RaycastHit();
					}
				}
				break;
			case clawAction.moveingCube:
				stateText.text = stateText.text.Substring(0, currentStateTextLength);
				stateText.text += "moving cube";
				
				Vector3 safeMovmentDistance = mainGrid.transform.position; //the distance of the grid that it is safe to move the cube without hitting anything
				safeMovmentDistance.z -= 4.0f;
				if (transform.position.z != safeMovmentDistance.z){ //move to the safe positon
					transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, safeMovmentDistance.z), clawMoveSpeed);
				} else{ // move across the serface of the grid
					Vector3 overDropLocation = blockDropOfLocation + (transform.position - grabedGridObject.transform.position);
					overDropLocation.z = safeMovmentDistance.z;
					transform.position = Vector3.MoveTowards(transform.position, overDropLocation, clawMoveSpeed);
					//the block drop of location is relitive to the block so the 
					//transform must be adjusted to the root of the claw
					if (transform.position == overDropLocation){
						
						mainGrid.readyGridForDropoff();
						currentState = clawAction.idle; //claw will stay idle till the maingrid is done
						//set the block drop of location to be the slot
						//the slot is created by sliding cubes in to the empty spaces created by removing the grabed object
					}
				}
				break;
			case clawAction.dropingOffCube:
				stateText.text = stateText.text.Substring(0, currentStateTextLength);
				stateText.text += "droping off cube";
				
				Vector3 dropLication = blockDropOfLocation + (transform.position - grabedGridObject.transform.position);
				transform.position = Vector3.MoveTowards(transform.position, dropLication, clawMoveSpeed);
				
				if (transform.position == dropLication){
					//set the new position on the grid
					
					grabedGridObject.transform.SetParent(mainGrid.transform, true);
					//this must not be set before the MainGrid is done moving the rest of the drag line around
					grabedGridObject.setLocalGridLocation(blockDropoffX, blockDropoffY);
					//this will set the current grabed blocks location, other block locations are set in fixedUp. in MainGrid 
					mainGrid.setBlockPositionInGrid(grabedGridObject, grabedGridObject.getLocX(), grabedGridObject.getLocY());
					grabedGridObject.setMainGrid(mainGrid);
					grabedGridObject.removeSelection();
					grabedGridObject = null;
					setRetractedClaw();
					if (mainGrid.endMovment()){
						currentState = clawAction.idle;
						mainGrid.startCubeMovment();
					} else{
						betweenStatesBugFix = true;
						currentState = clawAction.movingToIntermediate;
						nextState = clawAction.movingToIdle;
					}	
				}
				break;
			case clawAction.movingToIdle:
				stateText.text = stateText.text.Substring(0, currentStateTextLength);
				stateText.text += "moving to idle";
				
				transform.position = Vector3.MoveTowards(transform.position, idleLocation, clawMoveSpeed);
				if (transform.position == idleLocation){
					currentState = clawAction.idle;
				}
				betweenStatesBugFix = false;
				break;
		}
	}
	#endregion
	
	#region set states
	public void setExtendedClaw(){
		clawAnimator.SetBool("rotate", false);
		clawAnimator.SetBool("goto idle", false);
	}
	
	public void setRetractedClaw(){
		clawAnimator.SetBool("rotate", false);
		clawAnimator.SetBool("goto idle", true);
	}
	
	public void setRotatedClaw(){
		clawAnimator.SetBool("rotate", true);
		clawAnimator.SetBool("goto idle", false);
	}
	#endregion
	
	#region grabing operations
	private void grabBlock(Blocks block){
		grabedGridObject = block;
		block.transform.SetParent(transform, true);
	}
	#endregion
	
	#region movment operations
	public void moveBesideGrid(Blocks block, Vector3 BDOLocation){
		if (currentState != clawAction.idle && betweenStatesBugFix == false){
			return;
		}
		betweenStatesBugFix = false;
		grabedGridObject = block;
		
		currentState = clawAction.movingBesideCube;
		
		wantedBlockLocation = block.transform.position;
		blockDropOfLocation = BDOLocation;

	}
	
	/** this is not ready to be used yet will not be used in first release*/
	public void moveOverGrid(Blocks block, Blocks moveToThisLocation){
		if (currentState != clawAction.idle){
			Debug.LogError("Alredy moving block!");
			return;
		}
		currentState = clawAction.movingToIntermediate;
		nextState = clawAction.movingOverCube;
		wantedBlockLocation = block.transform.position;
		wantedBlockLocation.y += 2.5f;
	}
	#endregion
}
