using UnityEngine;
using System.Collections;

public class SelectionPlaneScript : MonoBehaviour{
	
	public Material shipSelectionImg, partSelectionImg, bothSelectionImg;
	private MeshRenderer planeRenderer;
	public textureState currentTextureState = textureState.off;
	public MeshCollider planeColider;
	
	public enum textureState{
		off,
		shipSelected,
		partSelected,
		bothSelected
	}
	;
	
	private int x, y;
	private MainGrid main;

	public void setVars(int setX, int setY, MainGrid m){
		x = setX;
		y = setY;
		main = m;
		planeRenderer = gameObject.GetComponent<MeshRenderer>();
	}
	
	/**sets the proper visibility and material for the selection state new selection state
		all additions of texture state are handled, though only shipSelected and partSelected should be passed to this method*/
	public void addTextureType(textureState addtext){
		if (currentTextureState == textureState.off){
			currentTextureState = addtext;
			switch (currentTextureState){
				case(textureState.shipSelected):
					planeRenderer.material = shipSelectionImg;
					break;
				case(textureState.partSelected):
					planeRenderer.material = partSelectionImg;
					break;
			}
		} else if (currentTextureState == textureState.shipSelected){
			if (addtext == textureState.partSelected){
				currentTextureState = textureState.bothSelected;
				planeRenderer.material = bothSelectionImg;
			}
		} else if (currentTextureState == textureState.partSelected){
			if (addtext == textureState.shipSelected){
				currentTextureState = textureState.bothSelected;
				planeRenderer.material = bothSelectionImg;
			}
		}
		planeRenderer.enabled = true;
	}
	
	/**sets the proper visibility and material for the selection state new selection state
		all removals of texture state are handled, though only shipSelected and partSelected should be passed to this method*/
	public void removeTextureType(textureState removeText){
		if (currentTextureState == textureState.bothSelected){
			if (removeText == textureState.shipSelected){
				currentTextureState = textureState.partSelected;
				planeRenderer.material = partSelectionImg;
			} else if (removeText == textureState.partSelected){
				currentTextureState = textureState.shipSelected;
				planeRenderer.material = shipSelectionImg;
			}
		} else if (currentTextureState == textureState.shipSelected){
			if (removeText == textureState.shipSelected){
				currentTextureState = textureState.off;
				planeRenderer.enabled = false;
			}
		} else if (currentTextureState == textureState.partSelected){
			if (removeText == textureState.partSelected){
				currentTextureState = textureState.off;
				planeRenderer.enabled = false;
			}
		}
	}
	
	// Update is called once per frame
	void Update(){
		
	}
	
	void OnMouseDown(){
		main.selectionRemoveMode = main.makeSelection().selectLocation(x, y, this);
	}
	
	void OnMouseEnter(){
		if (Input.GetMouseButton(0)){
			if (main.selectionRemoveMode){
				main.makeSelection().removeGridLocationToSelection(x, y, this);
			} else{
				main.makeSelection().addGridLocationToSelection(x, y, this);
			}			
		}
	}
	
	void OnMouseUp(){
		main.selectionRemoveMode = false;
	}
}
