using UnityEngine;
using System.Collections;

public class WhiteBlock : Blocks{
	public override blockType getBlockType(){
		return blockType.air;
	}
}
