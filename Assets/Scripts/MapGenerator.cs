using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public int cellAmount;

	int rowAmount;


	const int RIGHT = 0;
	const int LEFT = 1;
	const int UP = 2;
	const int DOWN = 3;
	const int OPEN = 0;
	const int WALL = 1;
	
	
	public int RoomAmount;

	public GameObject room;
	public GameObject walls;
	public GameObject walls_a;
	public GameObject walls_b;
	public GameObject walls_c;
	public GameObject walls_d;
	public GameObject tunnels;
	public PickUp pickUp_a;
	public PickUp pickUp_b;
	public PickUp pickUp_c;
	public PickUp pickUp_d;
	public Player player;
	public GameObject light;

	int startIndexX = 5;
	int startIndexY = 4;
	Point entrance;
	Point exit;
	static Point room_a;
	static Point room_b;
	static Point room_c;
	static Point room_c_primary;
	Point[] rooms  = {room_a,room_b,room_c};

	ArrayList deadEnds = new ArrayList ();
	GameObject A;



	Point[,] grid;
	ArrayList maze = new ArrayList();

	// Use this for initialization
	void Start () {

		initializeGrid ();
		prims ();
		createTunnels();
		createRooms ();
		createPickUps ();


	}
	//	Initialize the grid with points
	// each point has 4 walls to start
	// this is the set up for prims
	void initializeGrid(){
		rowAmount = (int) Mathf.Sqrt (cellAmount);
		grid = new Point[rowAmount,rowAmount];

		int i;
		int row = 0;
		int col = 0;
		int x = 0;
		int y = 0;

		for (i =0; i < cellAmount; i++) {
			Point c = new Point (x, y);
			c.vector = room.transform.position + new Vector3(x*3, 0,y*3);
			c.light = Instantiate(light, light.transform.position+ c.vector , Quaternion.identity) as GameObject;
			deadEnds.Add(c);
			y++;
			if (y > 9) {	
				y = 0;
				x++;
			}
			grid [row,col] = c;
			col = (col + 1) % rowAmount;
			if(col == 0)
				row++;
		} 

	}
	void createPickUps(){
		pickUp_a = Instantiate(pickUp_a, entrance.vector + new Vector3(1,1,1.5f), Quaternion.identity) as PickUp;
		pickUp_b = Instantiate(pickUp_b, room_a.room.transform.position + new Vector3(1,0,1.5f), Quaternion.identity) as PickUp;
		pickUp_c = Instantiate(pickUp_c, room_b.room.transform.position + new Vector3(1,0,1.5f), Quaternion.identity) as PickUp;
		pickUp_d = Instantiate(pickUp_d, room_c.room.transform.position + new Vector3(1,0,1.5f), Quaternion.identity) as PickUp;
	//	player = GetComponent<Player> ();
		pickUp_a.player = player;
		pickUp_b.player = player;
		pickUp_c.player = player;
		pickUp_d.player = player;

		pickUp_a.wall = room_a.wall;
		pickUp_b.wall = room_b.wall;
		//pickUp_d.wall = exit.wall;


	}
	/// <summary>
	/// Creates the tunnels.
	/// </summary>
	void createTunnels()
	{
		foreach (Point p in grid) {
				if (p.wallAmount == 0) {					
						 p.tunnel = Instantiate (tunnels.transform.Find ("t").gameObject,
		                 tunnels.transform.position + p.vector,
		                 Quaternion.identity)as GameObject;

				} else if (p.wallAmount == 1) {
						if (p.walls [RIGHT] == WALL) {
							 p.tunnel = Instantiate (tunnels.transform.Find ("t1").gameObject,
					                        tunnels.transform.position + p.vector,
			                 Quaternion.identity)as GameObject;
						} else if (p.walls [LEFT] == WALL) {
							p.tunnel = Instantiate (tunnels.transform.Find ("t0").gameObject,
					                        tunnels.transform.position + p.vector,
			                Quaternion.identity)as GameObject;

						} else if (p.walls [UP] == WALL) {
							p.tunnel = Instantiate (tunnels.transform.Find ("t3").gameObject,
					                        tunnels.transform.position + p.vector,
			                Quaternion.identity)as GameObject;

						} else {//DOWN
							p.tunnel = Instantiate (tunnels.transform.Find ("t2").gameObject,
					                        tunnels.transform.position + p.vector,
			                Quaternion.identity)as GameObject;
						}

				} else if (p.wallAmount == 2) {
						if (p.walls [RIGHT] == OPEN && p.walls [LEFT] == OPEN) {
							p.tunnel = Instantiate (tunnels.transform.Find ("-").gameObject,
					                        tunnels.transform.position + p.vector,
			                Quaternion.identity)as GameObject;
						} else if (p.walls [UP] == OPEN && p.walls [DOWN] == OPEN) {
							p.tunnel = Instantiate (tunnels.transform.Find ("I").gameObject,
					                        tunnels.transform.position + p.vector,
			                Quaternion.identity)as GameObject;
						} else if (p.walls [UP] == OPEN && p.walls [LEFT] == OPEN) {
							p.tunnel = Instantiate (tunnels.transform.Find ("L12").gameObject,
					                        tunnels.transform.position + p.vector,
			                Quaternion.identity)as GameObject;
						} else if (p.walls [UP] == OPEN && p.walls [RIGHT] == OPEN) {
							p.tunnel = Instantiate (tunnels.transform.Find ("L20").gameObject,
					                        tunnels.transform.position + p.vector,
			                Quaternion.identity)as GameObject;
						} else if (p.walls [DOWN] == OPEN && p.walls [LEFT] == OPEN) {
							p.tunnel = Instantiate (tunnels.transform.Find ("L13").gameObject,
					                        tunnels.transform.position + p.vector,
			                Quaternion.identity)as GameObject;
						} else if (p.walls [DOWN] == OPEN && p.walls [RIGHT] == OPEN) {
							p.tunnel = Instantiate (tunnels.transform.Find ("L30").gameObject,
					                        tunnels.transform.position + p.vector,
			                Quaternion.identity)as GameObject;
						}

				}
			// Dead end rooms ( 3 walls ) will be made into a room with closed of walls
				else {
					p.room = Instantiate (room, room.transform.position + p.vector, Quaternion.identity) as GameObject;
					p.wall = Instantiate (walls, walls.transform.position + p.vector, Quaternion.identity) as GameObject;

				}
			}

	}
	/// <summary>
	/// Creates the rooms with right walls.
	/// </summary>
	void createRooms(){
		//create entrance
		entrance = grid [startIndexX, startIndexY];

		Destroy (entrance.tunnel);

		// A and B are created from dead ends 

		if (deadEnds.Contains (exit)) 
			deadEnds.Remove(exit);		
		if (deadEnds.Contains (room_c))
			deadEnds.Remove (room_c);

		//create A
		Point point = deadEnds[Random.Range (0,deadEnds.Count)] as Point;
		deadEnds.Remove(point);
		room_a = point;


		//create B
		point = deadEnds[Random.Range (0,deadEnds.Count)] as Point;
		deadEnds.Remove (point);
		room_b = point;



		//create C & exit
		// C and exit are already instantiated, but don't have rooms yet
		// need to create room, delete tunnel and then make the correct walls

		room_c.room = Instantiate(room, room.transform.position +room_c.vector,Quaternion.identity)as GameObject;
		//exit.room = Instantiate(room, room.transform.position + exit.vector,Quaternion.identity)as GameObject;

		Destroy (room_c.tunnel.gameObject);
		//Destroy (exit.tunnel.gameObject);
		Destroy (exit.wall);

		for (int i = 0; i < 4; i++) {
			if (room_a.walls [i] == OPEN) {
					destroyWall (room_a, i);
					createWalls (room_a, i, walls_a);
					createPrimaryRoom (room_a, i);
			}
			if (room_b.walls [i] == OPEN) {
					destroyWall (room_b, i);
					createWalls (room_b, i, walls_b);
					createPrimaryRoom (room_b, i);
			}

			if (room_c.walls [i] == WALL)
					createWalls (room_c, i, walls);
			if (exit.walls [i] == WALL)
					createWalls (exit, i, walls);
			if (room_c.walls [i] == OPEN && i != UP && i != RIGHT) {
					createPrimaryRoom (room_c, i);
					createWalls(room_c,i,walls_c);
					pickUp_c.wall = room_c.wall;

			}
	}

		

	}
	/// <summary>
	/// Destroiys the wall in dir d at point .
	/// </summary>
	/// <param name="point">Point.</param>
	/// <param name="dir">Dir.</param>
	void destroyWall(Point point, int dir ){
		Debug.Log (point.x+","+ point.y);

		switch (dir) {
		case(UP):
			if(point.wall.transform.Find("wall_n").gameObject != null)
				Destroy(point.wall.transform.Find("wall_n").gameObject);
			break;
		case(DOWN):
			if(point.wall.transform.Find("wall_s").gameObject != null)
				Destroy(point.wall.transform.Find("wall_s").gameObject);

			break;
		case(LEFT):
			if(point.wall.transform.Find("wall_w").gameObject != null)
				Destroy(point.wall.transform.Find("wall_w").gameObject);
			break;
		case(RIGHT):
			if(point.wall.transform.Find("wall_e").gameObject != null)
				Destroy(point.wall.transform.Find("wall_e").gameObject);
			break;
			
		default:
			break;
			
		}
		
	}
	/// <summary>
	/// Creates the walls.
	/// </summary>
	/// <param name="p">P is the point you want the walls for.</param>
	/// <param name="dir">Dir is the dirrection of the wall you want to create.</param>
	/// <param name="wallSet">Wall set is the wallset you want to create the wall from.</param>
	void createWalls(Point p, int dir , GameObject wallSet){

			switch (dir) {
			case(UP):
				p.wall = Instantiate (wallSet.transform.Find ("wall_n").gameObject,
				                      wallSet.transform.Find ("wall_n").transform.position + p.vector,
				                      Quaternion.identity)as GameObject;
				break;
			case(DOWN):
				p.wall = Instantiate (wallSet.transform.Find ("wall_s").gameObject,
				                      wallSet.transform.Find ("wall_s").transform.position +  p.vector,
				                      Quaternion.identity)as GameObject;
				break;
			case(LEFT):
				p.wall = Instantiate (wallSet.transform.Find ("wall_w").gameObject,
				                      wallSet.transform.Find ("wall_w").transform.position +  p.vector,
				                      Quaternion.identity)as GameObject;
				break;
			case(RIGHT):
				p.wall = Instantiate (wallSet.transform.Find ("wall_e").gameObject,
				                      wallSet.transform.Find ("wall_e").transform.position +  p.vector,
				                      Quaternion.identity)as GameObject;
				break;
				
			default:
				break;
				
			}

	}
	/// <summary>
	/// Creates the primary room for the 2ndary rooms.
	/// </summary>
	/// <param name="p">P is the secondary room</param>
	/// <param name="dir">Dir is the direction that you want to make the primary room</param>
	void createPrimaryRoom(Point p, int dir){
		Point primary = null;
		switch (dir) {
		case(UP):
			primary = grid[p.x,p.y+1];
			primary.room = Instantiate(room, room.transform.position +primary.vector,Quaternion.identity)as GameObject;

			break;
		case(DOWN):
			primary = grid[p.x,p.y-1];
			primary.room = Instantiate(room, room.transform.position +primary.vector,Quaternion.identity)as GameObject;

			break;
		case(LEFT):
			primary = grid[p.x-1,p.y];
			primary.room = Instantiate(room, room.transform.position +primary.vector,Quaternion.identity)as GameObject;

			break;
		case(RIGHT):
			primary = grid[p.x+1,p.y];
			primary.room = Instantiate(room, room.transform.position +primary.vector,Quaternion.identity)as GameObject;

			break;
		default:
			break;
		}
		
		Destroy (primary.tunnel);

		primary.wall = Instantiate (walls, primary.vector + walls.transform.position, Quaternion.identity) as GameObject;
		for (int i = 0; i<4; i++) {
			if (primary.walls [i] == OPEN) {
				destroyWall(primary,i);
			}
		}



	}
	/// <summary>
	/// Creates the exit (in top right corner) and room_c (neighbour of exit)
	/// </summary>
	void createExit(){

		exit = grid [9, 9];
		//exit.walls [UP] = OPEN;
		//exit.wallAmount = 3;
		maze.Add (exit);


		// can be 8,9
		if (Random.Range (0, 2) == 1) {
			room_c = addNeighbour (exit, LEFT);
			Debug.Log (room_c.x +","+ room_c.y);
			createWalls (exit, LEFT, walls_d);
			//createWalls (room_c, RIGHT, walls_c);

		} else {
			room_c = addNeighbour(exit,DOWN);
			Debug.Log (room_c.x +","+ room_c.y);
			createWalls (exit, DOWN, walls_d);
			//createWalls(room_c, LEFT, walls_c);
		}


		
		pickUp_d.wall = exit.wall;
		Point primary = addNewPointToMaze (room_c);


		maze.Remove (primary);

	}

	/// <summary>
	/* 	Prims algorithm to generate maze, slightly modified
	   	1) create start point, the entrance is a fixed point
	   	2) create exit point, also a fixed point
		3) add each point to maze list until all points are added (prims algo)
	*/
	/// </summary>
	void prims(){

		maze.Add (grid [startIndexX,startIndexY]);
		createExit ();

		//entrance,exit,secondary c
		int count = 3;

		while (count != cellAmount) {
			Point p = maze[Random.Range (0,maze.Count - 1)] as Point;

			if (p.Equals (exit) || p.Equals(room_c) )
				continue;

			if(addNewPointToMaze(p) != null)
			{
				count++;
			}
		}
	}
	/// <summary>
	/// Adds the new point to maze.
	/// </summary>
	/// <returns>The new point to maze, null if nothing added</returns>
	/// <param name="p">the new point will be a dirrect neighbour of p.</param>
	Point addNewPointToMaze(Point p){
		int neighbour = Random.Range (0,3);
		int count = 0;


		while (count < 4) {
			if(checkEdgeCase(p,neighbour))
			{
				Point pn = addNeighbour(p,neighbour);
				if(pn != null)
				{
					return pn;
				}
			}
			count++;
			neighbour++;
			if(neighbour == 4)
				neighbour = 0;
		}
		return null;


	}

	/// <summary>
	/// Checks the edge case.
	/// </summary>
	/// <returns><c>true</c>, if p's neighbour is within the grid<c>false</c> otherwise.</returns>
	/// <param name="p">P.</param>
	/// <param name="neighbour">Neighbour.</param>
	bool checkEdgeCase(Point p, int neighbour){
		if (p.x == rowAmount-1) {
			// no right
			if( neighbour == RIGHT ){
				return false;
			}
		} else if (p.x == 0) {
			// no left
			if(neighbour == LEFT){
				return false;
			}
		}
		if (p.y == rowAmount-1) {
			// no up
			if(neighbour == UP){
				return false;
			}
		} else if (p.y == 0) {
			// no down
			if (neighbour == DOWN){
				return false;
			}
			
		}
		return true;
	}
	/// <summary>
	/// Adds the point in the direction dir from p to maze.
	/// </summary>
	/// <returns>The neighbour if added. null otherwise</returns>
	/// <param name="p">P.</param>
	/// <param name="dir">Dir.</param>
	Point addNeighbour(Point p, int dir){
		Point pn;
		switch(dir){
		case (RIGHT):
			pn = grid[p.x + 1, p.y];
			if (!maze.Contains(pn)){
				maze.Add(pn);
				removeWall(p,pn,dir);
				return pn;
			}
			break;
		case (LEFT):
			pn = grid[p.x - 1, p.y];
			if (!maze.Contains(pn)){
				maze.Add(pn);
				removeWall(p,pn,dir);

				return pn;
			};
			break;
			
		case (UP):
			pn = grid[p.x, p.y + 1];
			if (!maze.Contains(pn)){
				maze.Add(pn);
				removeWall(p,pn,dir);
				return pn;
			}
			break;
			
		case (DOWN):
			pn = grid[p.x, p.y - 1];
			if (!maze.Contains(pn)){
				maze.Add(pn);
				removeWall(p,pn,dir);
				return pn;
			}
			break;
			
		default:
			break;
		}

		return null;
	}
	/// <summary>
	/// sets p's wall in the direction dir and p's neighbour's wall in the opposite dir to OPEN.
	/// </summary>
	/// <param name="p">P.</param>
	/// <param name="pNeighbour">P neighbour.</param>
	/// <param name="dir">Dir.</param>
	void removeWall(Point p,Point pNeighbour, int dir){
		switch (dir) {
			case(RIGHT):
				p.walls[RIGHT] = OPEN;
				pNeighbour.walls[LEFT] = OPEN;

				break;
			case(LEFT):
				p.walls[LEFT] = OPEN;
				pNeighbour.walls[RIGHT] = OPEN;

		
				break;
			case(UP):
				p.walls[UP]=OPEN;
				pNeighbour.walls[DOWN] = OPEN;

				break;

			case(DOWN):
				p.walls[DOWN]=OPEN;
				pNeighbour.walls[UP] = OPEN;

				break;

			default:
					break;
			}

		p.wallAmount--;
		pNeighbour.wallAmount--;

		if (p.wallAmount < 3 && deadEnds.Contains (p)) {
			deadEnds.Remove (p);
		}
		if (pNeighbour.wallAmount < 3 && deadEnds.Contains (pNeighbour)) {
			deadEnds.Remove (pNeighbour);
		}
	}
	

}
/// <summary>
/// Point.
/// has x,y (for grid index),wall, room, vector position, wallamount (everything starts with 4 walls), array of walls (1 corresponds to wall 0 corresponds to open, index is the dirrection of wall), and tunnel, 
/// </summary>
class Point{
	public int x,y;

	public GameObject wall;
	public GameObject room;
	public Vector3 vector;
	public int wallAmount = 4;
	public int[] walls = {1,1,1,1};
	public GameObject tunnel;
	public GameObject light;

	public Point(int px,int py){
		x = px;
		y = py;
		
	}
}