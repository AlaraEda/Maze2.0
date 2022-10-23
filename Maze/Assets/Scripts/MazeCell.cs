using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell
{
    //Are cells visited or not by the algorithm?
    public bool Visited = false;

    //The walls surrounding the cell, and also the walls that the algorithm is going to delete. 
    public GameObject UpWall;
    public GameObject DownWall;
    public GameObject LeftWall;
    public GameObject RightWall;

}
