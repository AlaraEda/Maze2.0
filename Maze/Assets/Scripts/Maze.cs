using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   //Voor toggle
using TMPro;

public class Maze : MonoBehaviour
{
    [Header("Maze")]
    public int Rows = 2;
    public int Columns = 2;

    public GameObject Wall;
    public GameObject Floor;
    public GameObject FinnishProp;
    public GameObject StartProp;

    [Header("UI-Elements")]
    public GameObject HeightField;
    public GameObject WidthField;
    public GameObject Version1;
    public GameObject Version2;

    [Header("Materials")]
    public Material Moss;
    public Material Medieval;
    public Material Cobblestone;
    public Material Default;

    //Create the grid? Two-Dimensional Array
    private MazeCell[,] grid;
    private int currentRow = 0;
    private int currentColumn = 0;

    private GameObject startprop;
    private GameObject finnishprop;

    private bool scanComplete = false;

    // Start is called before the first frame update
    void Start(){

        Debug.Log("When in game: Use W/A/S/D to zoom in and out of the maze. Use O/P to switch between the maze overview and the playground.");

        //Default Maze Material
        Wall.GetComponent<MeshRenderer>().material = Default;
        Floor.GetComponent<MeshRenderer>().material = Default;

        //GenerateMaze
        GenerateGrid();
    }

    void GenerateGrid(){
        //destroy all the children of this transform object.
        foreach (Transform transform in transform){
            Destroy(transform.gameObject);
        }

        //Grid with all the walls & floors
        //Dimensions should be at least 2x2
        CreateGrid();

        ChangeCameraPosition();

        currentRow = 0;
        currentColumn = 0;
        scanComplete = false;

        //Algorithm carves path from top left to bottom right
        HuntAndKill();
    }

    void CreateGrid(){
        float size = Wall.transform.localScale.x;
        grid = new MazeCell[Rows, Columns]; //Initalize the MazeCell

        //Create the maze grid itself
        for (int i = 0; i < Rows; i++){
            for (int j = 0; j < Columns; j++){

                //This is gonna go for all the columns and rows of the game, so 4x4=16 times and will create 16 floors..
                GameObject floor = Instantiate(Floor, new Vector3(j * size, 0, -i * size), Quaternion.identity);
                floor.name = "Floor_" + i + "_" + j;    //Rename the floor obj so you can see where they are in the board. 

                //Add Walls
                GameObject upWall = Instantiate(Wall, new Vector3(j * size, 1.75f, -i * size + 1.25f), Quaternion.identity);
                upWall.name = "UpWall_" + i + "_" + j;

                GameObject downWall = Instantiate(Wall, new Vector3(j * size, 1.75f, -i * size - 1.25f), Quaternion.identity);
                downWall.name = "DownWall_" + i + "_" + j;

                GameObject leftWall = Instantiate(Wall, new Vector3(j * size - 1.25f, 1.75f, -i * size), Quaternion.Euler(0, 90, 0)); //Rotate 90*
                leftWall.name = "LeftWall_" + i + "_" + j;

                GameObject rightWall = Instantiate(Wall, new Vector3(j * size + 1.25f, 1.75f, -i * size), Quaternion.Euler(0, 90, 0));
                rightWall.name = "LeftWall_" + i + "_" + j;

                //Create all the MazeCells / the grid
                grid[i, j] = new MazeCell();

                grid[i, j].UpWall = upWall;
                grid[i, j].DownWall = downWall;
                grid[i, j].LeftWall = leftWall;
                grid[i, j].RightWall = rightWall;


                //Make all the new gameobjects a child of Maze-object
                floor.transform.parent = transform;
                upWall.transform.parent = transform;
                downWall.transform.parent = transform;
                leftWall.transform.parent = transform;
                rightWall.transform.parent = transform;

                //Destroy the entrance (starting point) and exit-point walls of the maze
                if (i == 0 & j == 0){
                    Destroy(leftWall);

                    startprop = Instantiate(StartProp, new Vector3(-1.2f, 0.3f, -i * size), Quaternion.Euler(0, 90, 0));
                    startprop.name = "StartProp_" + i + "_" + j;
                }

                if (i == Rows -1 && j == Columns -1){
                    Destroy(rightWall);

                    //GameObject finnishprop = Instantiate(FinnishProp, new Vector3(28.3f, 0.3f, -i * size), Quaternion.Euler(0, 90, 0));
                    finnishprop = Instantiate(FinnishProp, new Vector3(j * size + 1.25f, 0.3f, -i * size), Quaternion.Euler(0, 90, 0));
                    finnishprop.name = "FinnishProp_" + i + "_" + j;
                }
            }
        }
    }

    //Change Camera Position, so you can see the whole grid everytime. 
    void ChangeCameraPosition(){
        float size = Wall.transform.localScale.x;
        Vector3 cameraPosition = Camera.main.transform.position;
        Quaternion cameraRotation = Camera.main.transform.rotation;
        
        cameraPosition.x = Mathf.Round(Columns/2) * size;
        cameraPosition.y = Mathf.Max(13, Mathf.Max(Rows, Columns) * 3.5f);     //Minimum zoom out is 13. 
        cameraPosition.z = -Mathf.Round(Rows/2) * size;

        Camera.main.transform.position = cameraPosition;
        //Camera.main.transform.rotation = cameraRotation;
        Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    void HuntAndKill(){
        //Mark the first cell of the random walk as visited.
        grid[currentRow, currentColumn].Visited = true;

        while (!scanComplete){
            Walk();
            Hunt();    
        }                             
    }

    void Walk(){
        while(AreThereUnvistedNeighbors()){
            //Choose a random direction to walk
            int direction = Random.Range(0,4);                                                      

            //Check up
            if(direction == 0){

                //check if row-up is visited
                if (IsCellUnvisitedAndWithinBoundaries(currentRow -1, currentColumn)){
                // if (currentRow > 0 && !grid[currentRow - 1, currentColumn].Visited){     //This is the same as the line above. 
                    
                    //Destroy if there is a wall above the cell.
                    if(grid[currentRow, currentColumn].UpWall){
                        //mark the visited cell and destroy the (up) wall between the two cells
                        Destroy(grid[currentRow, currentColumn].UpWall);
                    }

                    currentRow--; //We are going one row up. 
                    grid[currentRow, currentColumn].Visited = true;

                    //Destroy if there is a wall beneath the cell.
                    if(grid[currentRow, currentColumn].DownWall){
                        //mark the visited cell and destroy the (down) wall between the two cells
                        Destroy(grid[currentRow, currentColumn].DownWall);
                    }
                }
            }
            //Check down
            else if(direction == 1){
                if (IsCellUnvisitedAndWithinBoundaries(currentRow + 1, currentColumn)){             //if (currentRow < Rows - 1 && !grid[currentRow + 1, currentColumn].Visited){

                    if(grid[currentRow, currentColumn].DownWall){
                        Destroy(grid[currentRow, currentColumn].DownWall);
                    }

                    currentRow++; //We are going one row down. 
                    grid[currentRow, currentColumn].Visited = true;

                    
                    if(grid[currentRow, currentColumn].UpWall){
                        Destroy(grid[currentRow, currentColumn].UpWall);
                    }
                }
            }
            //Check left
            else if(direction == 2){
                if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn - 1)){         //if (currentColumn > 0 && !grid[currentRow, currentColumn - 1].Visited){
                    
                    if(grid[currentRow, currentColumn].LeftWall){
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                    }

                    currentColumn--; //We are going one row up. 
                    grid[currentRow, currentColumn].Visited = true;

                    if(grid[currentRow, currentColumn].RightWall){
                        Destroy(grid[currentRow, currentColumn].RightWall);
                    }
                }
            }
            //Check right
            else if(direction == 3){
                if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn +1)){         //if (currentColumn < Columns -1 && !grid[currentRow, currentColumn +1].Visited){

                    if(grid[currentRow, currentColumn].RightWall){
                        Destroy(grid[currentRow, currentColumn].RightWall);
                    }

                    currentColumn++; //We are going one row down. 
                    grid[currentRow, currentColumn].Visited = true;

                    if(grid[currentRow, currentColumn].LeftWall){
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                    }
                }
            }
        }
    }

    //Scan the grid looking for an unvisited cell that is adjacented to a visited cell
    void Hunt(){

        scanComplete = true;

        for (int i = 0; i<Rows; i++){
            for (int j = 0; j < Columns; j++){
                
                //Scan the grid until you find a cell that is unvisited and has an unvisited neighbor
                if(!grid[i,j].Visited && AreThereVisitedNeighbors(i,j)){

                    //if an unvisited neighbor has been found, do another random walk from that cell. 
                    scanComplete = false;

                    currentRow = i;
                    currentColumn = j;
                    grid[currentRow, currentColumn].Visited = true;
                    //Randomly destroy a wall
                    DestroyAdjacentWall();
                    return;
                }
            }
        }
    }

    void DestroyAdjacentWall(){
        bool destroyed = false;

        while (!destroyed){
            int direction = Random.Range(0,4);

            //Check up
            if(direction == 0){
                if(currentRow > 0 && grid[currentRow -1, currentColumn].Visited){

                    //Debug.Log("Destroyed down wall." + (currentRow - 1)+ " " + currentColumn);

                    if (grid[currentRow, currentColumn].UpWall){
                        Destroy(grid[currentRow, currentColumn].UpWall);
                    }

                    if (grid[currentRow-1, currentColumn].DownWall){
                        Destroy(grid[currentRow - 1, currentColumn].DownWall);
                    }

                    destroyed = true;
                }
            }

            //Check down
            else if (direction ==1){
                if(currentRow < Rows -1 && grid[currentRow +1, currentColumn].Visited){
                    
                    //Debug.Log("Destroyed up wall." + (currentRow + 1)+ " " + currentColumn);

                    if (grid[currentRow, currentColumn].DownWall){
                        Destroy(grid[currentRow, currentColumn].DownWall);
                    }

                    if (grid[currentRow + 1, currentColumn].UpWall){
                        Destroy(grid[currentRow + 1, currentColumn].UpWall);
                    }
                    
                    destroyed = true;
                }
            }

            //Check left
            else if (direction ==2){
                if(currentColumn > 0 && grid[currentRow, currentColumn -1].Visited){
                    
                    //Debug.Log("Destroyed right wall." + currentRow + " " + (currentColumn -1));

                    if (grid[currentRow, currentColumn].LeftWall){
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                    }

                    if (grid[currentRow, currentColumn -1].RightWall){
                        Destroy(grid[currentRow, currentColumn -1].RightWall);
                    }

                    
                    destroyed = true;
                }
            }

            //Check right
            else if (direction ==3){
                if(currentColumn < Columns -1 && grid[currentRow, currentColumn + 1].Visited){
                    
                    //Debug.Log("Destroyed left wall." + currentRow + " " + (currentColumn -1));
                    
                    if (grid[currentRow, currentColumn].RightWall){
                        Destroy(grid[currentRow, currentColumn].RightWall);
                    }

                    if (grid[currentRow, currentColumn +1].LeftWall){
                        Destroy(grid[currentRow, currentColumn +1].LeftWall);
                    }
                    destroyed = true;
                }
            }
        }
    }

    //Check if there are unvisted neighbors
    bool AreThereUnvistedNeighbors(){
        
        //Check up
        if (IsCellUnvisitedAndWithinBoundaries(currentRow -1, currentColumn)){
            return true;
        }

        //Check down
        if (IsCellUnvisitedAndWithinBoundaries(currentRow +1, currentColumn)){
            return true;
        }

        //Check left
        if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn + 1)){
            return true;
        }

        //Check right
        if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn - 1)){
            return true;
        }

        //Dead-end? Return false. 
        return false;
    }

    public bool AreThereVisitedNeighbors(int row, int column){

        //Check Up
        if( row > 0 && grid[row -1, column].Visited){
            return true;
        }

        //Check down
        if(row < Rows -1 && grid[row +1, column].Visited){
            return true;
        }

        //Check left
        if(column > 0 && grid[row, column -1].Visited){
            return true;
        }

        //Check Right
        if(column < Columns -1 && grid[row, column +1].Visited){
            return true;
        }

        return false;
    }

    //Do Boundary Check and unVisited Check
    bool IsCellUnvisitedAndWithinBoundaries(int row, int column){
        if (row >=0 && row < Rows && column >=0 && column < Columns && !grid[row, column].Visited){
            return true;
        }

        return false;
    }

    public void Regenerate(){

        int rows = 2;
        int columns = 2;

        if (int.TryParse(HeightField.GetComponent<TMP_InputField>().text, out rows)){

            //Minimum Rows is 10, if rows is 0, than it will still select 10.
            //Mathf returns the largest number between 10 and whatever the other one is. 
            if (rows < 10){
                Rows = Mathf.Max(10, rows);
            }     
            
            if (rows > 250){
                Rows = Mathf.Min(250, rows);
            }
            
            if (rows >= 10 && rows <= 250){
                Rows = rows;
            }           
            
        }

        //Destroy Start-line & Finnish-line
        Destroy(startprop);
        Destroy(finnishprop);

        if (int.TryParse(WidthField.GetComponent<TMP_InputField>().text, out columns)){
            //Minimum Columns is 10, if rows is 0, than it will still select 10.
            
            if (columns < 10){
                Columns = Mathf.Max(10, columns);
            }
            
            else if (columns > 250){
                Columns = Mathf.Min(250, columns);
            }
            
            if (columns >= 10 && columns <= 250) {
                Columns = columns;
            }
        }


        //Material Options: Moss, Cobblestone, Medieval, Default
        if (Version1.GetComponent<Toggle>().isOn){
            Wall.GetComponent<MeshRenderer>().material = Cobblestone;
            Floor.GetComponent<MeshRenderer>().material = Moss;
            
        }
        
        else if (Version2.GetComponent<Toggle>().isOn){
            Wall.GetComponent<MeshRenderer>().material = Medieval;
            Floor.GetComponent<MeshRenderer>().material = Cobblestone;
        }
        
        else{
            //Wall.GetComponent<Renderer>().material = null;
            Wall.GetComponent<MeshRenderer>().material = Default;
            Floor.GetComponent<MeshRenderer>().material = Default;
        }

        GenerateGrid();
    }
}
