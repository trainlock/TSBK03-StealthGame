using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour{
    public bool displayGridGizmos;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Awake(){
        Debug.Log("GRID: Awake.");
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    // Max size of grid
    public int MaxSize{
        get{
            return gridSizeX * gridSizeY;
        }
    }

    // Create grid and add nodes
    void CreateGrid(){
        grid = new Node[gridSizeX, gridSizeY];
        // Bottom left corner
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++){
            for (int y = 0; y < gridSizeY; y++){
                // Get world point
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                // Collision check for world point within a given radius
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                // Populate grid with nodes
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    // Finds the neighbours to the node and returns them in a list
    public List<Node> GetNeighbours(Node node){
        List<Node> neighbours = new List<Node>();

        // Loop through the nodes that are adjacent to the current node
        for (int x = -1; x <= 1; x++){
            for (int y = -1; y <= 1; y++){
                // Check if we are at the current node
                if (x == 0 && y == 0){
                    continue;
                }
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY){
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    // Convert world point to grid point
    public Node NodeFromWorldPoint(Vector3 worldPosition){
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        // Should always be clamped between 0 and 1
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // Get x and y indices of grid array
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<Node> path;
    void OnDrawGizmos(){
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (displayGridGizmos){
            if (path != null){
                foreach (Node n in path){
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
        else{
            if (grid != null){
                foreach (Node n in grid){
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    if (path != null){
                        if (path.Contains(n)){
                            Gizmos.color = Color.black;
                        }
                    } 
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
    }
}