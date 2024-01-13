using UnityEngine;
using System.Collections;

public class PseudoProceduralDunes : MonoBehaviour {
	
	public GameObject dunes;
	public GameObject drone;
	public float snapDist;
	public float duneHeight = 1;

	private float sizeOfPlaneX;
	private float sizeOfPlaneZ;
	
	private GameObject[,] planes = new GameObject[3, 3];
	
	// Use this for initialization
	void Start () {
		dunes.transform.localScale = new Vector3(1,duneHeight,1); 
		sizeOfPlaneX = snapDist;
		sizeOfPlaneZ = sizeOfPlaneX;
		for(int i = 0; i < 3; i++){
			for(int c = 0; c < 3; c++){
				if(i == 1 && c == 1){
					planes[i, c] = dunes;
					planes[i, c].name = i+","+c;
				} else {
					planes[i, c] = GameObject.Instantiate(dunes) as GameObject;
					planes[i, c].name = i+","+c;
				}	
			}
		}
				
		// Reposition all of the planes.
		for(int i = 0; i < 3; i++){
			for(int c = 0; c < 3; c++){
				// Adjust the x, z positions.
				Vector3 tempPos = new Vector3((float)sizeOfPlaneX*(i-1), 0, (float)sizeOfPlaneZ*((c-1)*-1));
				planes[i, c].transform.position += tempPos;
			}
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		var playerX = drone.transform.position.x;
		var playerZ = drone.transform.position.z;
		for (int i = 0; i < 3; i++){
			for (int c = 0; c < 3; c++){
				float planeXMin = planes[i, c].transform.position.x - (sizeOfPlaneX/2);
				float planeXMax = planeXMin + sizeOfPlaneX;
				float planeZMin = planes[i, c].transform.position.z - (sizeOfPlaneZ/2);
				float planeZMax = planeZMin + sizeOfPlaneZ;
				if(playerX > planeXMin && playerX < planeXMax && playerZ > planeZMin && playerZ < planeZMax){
					updatePlanes(i, c);	
				}
			}
		}
	}
	
	public void updatePlanes(int x, int z){
		if(x == 0){
			// Gone left;
			moveLeft();
		}
		else if(x == 2){
			 // Gone right;
			moveRight();
		}
		if(z == 0){
			// Gone up;
			moveUp();
		}
		else if(z == 2){
			 // Gone down;
			moveDown();
		}
	}
	
	public void moveLeft(){
		GameObject[,] newPlanes = new GameObject[3, 3];
		for(int i = 0; i < 3; i++){
			Vector3 addPos = new Vector3(-sizeOfPlaneX*3, 0, 0);
			planes[2, i].transform.position +=	addPos;
		}
		for(int i = 0; i < 3; i++){
			int c = i - 1;
			c = (c == -1)? 2 : c;
			newPlanes[i, 0] = planes[c, 0];
			newPlanes[i, 1] = planes[c, 1];
			newPlanes[i, 2] = planes[c, 2]; 
		}
		planes = newPlanes;
	}
	
	public void moveRight(){
		GameObject[,] newPlanes = new GameObject[3, 3];
		for(int i = 0; i < 3; i++){
			Vector3 addPos = new Vector3(sizeOfPlaneX*3, 0, 0);
			planes[0, i].transform.position += addPos;
		}
		for(int i = 0; i < 3; i++){
			int c = i + 1;
			c = (c == 3)? 0 : c;
			newPlanes[i, 0] = planes[c, 0];
			newPlanes[i, 1] = planes[c, 1];
			newPlanes[i, 2] = planes[c, 2]; 
		}
		planes = newPlanes;
	}
	
	public void moveUp(){
		
		GameObject[,] newPlanes = new GameObject[3, 3];
		for(int i = 0; i < 3; i++){
			Vector3 addPos = new Vector3(0, 0, sizeOfPlaneX*3);
			planes[i, 2].transform.position += addPos;
		}
		for(int i = 0; i < 3; i++){
			int c = i - 1;
			c = (c == -1)? 2 : c;
			newPlanes[0, i] = planes[0, c];
			newPlanes[1, i] = planes[1, c];
			newPlanes[2, i] = planes[2, c]; 
		}
		planes = newPlanes;
	}
	
	public void moveDown(){

		GameObject[,] newPlanes = new GameObject[3, 3];
		for(int i = 0; i < 3; i++){
			Vector3 addPos = new Vector3(0, 0, -sizeOfPlaneX*3);
			planes[i, 0].transform.position += addPos;
		}
		for(int i = 0; i < 3; i++){
			int c = i + 1;
			c = (c == 3)? 0 : c;
			newPlanes[0, i] = planes[0, c];
			newPlanes[1, i] = planes[1, c];
			newPlanes[2, i] = planes[2, c]; 
		}
		planes = newPlanes;
	}
}
