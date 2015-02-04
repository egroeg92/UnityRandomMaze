using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float moveSpeed = 60;
	public float rotateSpeedx = 60;


	public CharacterController control;
	Vector3 move;
	Transform cam;
	void Start(){
		control = GetComponent<CharacterController> ();
		cam = transform.Find ("Camera") as Transform;
	}
		// Update is called once per frame
	void Update() {

		//rotation
		transform.Rotate(0, Input.GetAxis("Mouse X") * rotateSpeedx * Time.deltaTime, 0);
		cam.transform.Rotate (-Input.GetAxis ("Mouse Y") * rotateSpeedx * Time.deltaTime, 0, 0);
		//movement
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis ("Vertical");

		move = new Vector3 (horizontal, 0, vertical);
		move.Normalize ();
		move = transform.TransformDirection (move);
		control.SimpleMove (move  * moveSpeed * Time.deltaTime);


	}



}
