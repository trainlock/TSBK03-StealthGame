using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour{
    #region Variables
    public Transform    seeker;

    private Vector3     m_target;
    private Grid        m_grid;
    #endregion

    void Awake(){
        Debug.Log("PATHFINDING: Awake");
        m_grid = GetComponent<Grid>();
    }

    void Start(){
        Debug.Log("PATHFINDING: Start");
        UpdateTarget(transform.position);
    }

    public void UpdateTarget(Vector3 targetPos){
        FindPath(seeker.position, targetPos);
    }

    #region Getters and Setters
    public Grid GetGrid(){
        return m_grid;
    }

    public List<Node> GetPath(){
        return m_grid.path;
    }
    #endregion

    // Not in use
    public void StartFindPath(Vector3 startPos, Vector3 targetPos){
    //    StopAllCoroutines();
    //    StartCoroutine(FindPath(startPos, targetPos));
    }

    void FindPath(Vector3 startPos, Vector3 targetPos){
        // Get starting node and target node
        Node startNode = m_grid.NodeFromWorldPoint(startPos);
        Node targetNode = m_grid.NodeFromWorldPoint(targetPos);

        // Create the open and closed set
        Heap<Node> openSet = new Heap<Node>(m_grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        // Add first node to open set
        openSet.Add(startNode);

        while (openSet.Count > 0){
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);
            if (currentNode == targetNode){
                RetracePath(startNode, targetNode);
                return;
            }

            // Add the neighbours to the openset
            foreach (Node neighbour in m_grid.GetNeighbours(currentNode)){
                if (!neighbour.walkable || closedSet.Contains(neighbour)){
                    continue;
                }

                // Update h and g cost
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)){
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    // Check if neighbour exists in the openset, if not add it
                    if (!openSet.Contains(neighbour)){
                        openSet.Add(neighbour);
                    }
                    else{
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode){
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode){
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        m_grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB){
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY){
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX);
    }
}