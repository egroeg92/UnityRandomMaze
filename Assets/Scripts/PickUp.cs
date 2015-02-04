using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {

	public GameObject wall;
	public Player player = null;
	public float shootSpeed = 10;

	public GameState game;
	Rigidbody rb;
	bool shot;
	void Start(){
		game = GameObject.Find("Game").GetComponent<GameState>();
		rb = GetComponent<Rigidbody> ();
		shot = false;
	}
	
	void Update(){

		//Debug.Log (Vector3.Distance (transform.position, player.transform.position));
		transform.rotation = Quaternion.identity;
		tryPickUp ();
		tryDrop ();
		tryShoot ();

		//checkCollision ();
		if (player.pickUp == this) {
			rb.velocity = Vector3.zero;
			rb.transform.position = player.transform.position + new Vector3(0,renderer.bounds.size.magnitude,0);
		}


	}
	void tryPickUp(){

		if (Input.GetKey (KeyCode.Period) && player.pickUp == null) {
			float distance = Vector3.Distance (transform.position, player.transform.position);
			if (distance < 2) {
				player.pickUp = this;
			}
		}
	}
	
	void tryDrop(){
		if(Input.GetKey(KeyCode.Comma) && player.pickUp == this)
		{
			player.pickUp = null;
		}
	}
	void tryShoot(){
		if(Input.GetMouseButtonDown(0) && player.pickUp == this){
			shot = true;
			player.pickUp = null;
			transform.position = transform.position + player.transform.forward ;
			rb.velocity = player.transform.forward * shootSpeed;
		}
	}
	void OnCollisionEnter(Collision collision){
		if (shot == true) {
			if (collision.gameObject == wall){
				Destroy (wall.gameObject);
				game.destroyWall();
			}
			game.destroyPickUp();
			Destroy (this.gameObject);


		}
	}

}
