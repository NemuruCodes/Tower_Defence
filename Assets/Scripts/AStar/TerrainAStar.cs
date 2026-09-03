using UnityEngine;
using System.Collections.Generic;

//https://www.redblobgames.com/pathfinding/a-star/introduction.html - A* Algorithm (AStar)
//https://www.geeksforgeeks.org/dsa/a-search-algorithm/ 
//https://medium.com/@nanda.yugandhar/a-vs-dijkstra-a-visual-guide-to-why-a-sense-of-direction-matters-ef9378d71a53 - Research
//https://www.redblobgames.com/pathfinding/a-star/implementation.html - More on  A* Algorithm
//https://cp-algorithms.com/geometry/manhattan-distance.html - Manhatten Distance
public static class TerrainAStar
{
    //Main A* logic
    public static List<Vector2Int> FindPath(TerrainSurfaceGraph graph, Vector2Int start, Vector2Int goal) //Uses the Graph nodes
    {
        var frontier = new TerrainPriorityQueue<Vector2Int, double>(); // Creates the queue
        frontier.Enqueue(start, 0); //The cell where the A* starts from  which is  the lowest cost value

        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var costSoFar = new Dictionary<Vector2Int, double>();
        cameFrom[start] = start;
        costSoFar[start] = 0;

        while (frontier.Count > 0)  //Keep looping while searching
        {
            Vector2Int current = frontier.Dequeue(); // Gets the cell with the lowest priority from cells. This is how the A* finds the best path

            if (current == goal)
                break;

            foreach (Vector2Int next in graph.Neighbors(current)) //Checks for the cells that can be reached from the ppoint of view of the current cell.
            {
                double newCost = costSoFar[current] + graph.Cost(current, next);

                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    double priority = newCost + Heuristic(next, goal); // Finding the A* proirity from the cost
                    frontier.Enqueue(next, priority); // Addding cell/Node to queue
                    cameFrom[next] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(goal))  // Extra saftly incase goal / target is not found (Should never happen)
            return null; 

        return ReconstructPath(cameFrom, start, goal);
    }

    // Manhattan distance used for the remaining distance to  the goal / Target
    private static double Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int goal)
    {
        var path = new List<Vector2Int>();
        Vector2Int current = goal;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Add(start);
        path.Reverse();

        return path;
    }
}
