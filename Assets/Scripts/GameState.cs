using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {

	public int wallCount, pickUpCount;
	public GUIText display;

	void Start(){
			display.text = "";
	}
	//public GUIText end; 
	public void destroyPickUp()
	{
		pickUpCount--;
	}
	public void destroyWall()
	{
		wallCount--;
	}

	void Update () {
		if (pickUpCount == 0 && wallCount == 0) {
				display.text = "YOU MADE IT OUT ALIVE : WIN";
				//end.text = "YOU WIN";

		} else if (wallCount > pickUpCount) {
				display.text = "YOU ARE STUCK FOREVER : LOSE";
		}
	}

}
