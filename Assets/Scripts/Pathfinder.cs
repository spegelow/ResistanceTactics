using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    public class PathfindingNode
    {
        public MapTile tile;
        public int movementToReachNode;

        public PathfindingNode(MapTile m, int movementToReachNode)
        {
            this.tile = m;
            this.movementToReachNode = movementToReachNode;
        }
    }
    public static List<MapTile> GetMoveableTiles(Unit unit)
    {
        //Initialize open and closed sets
        List<PathfindingNode> openSet = new List<PathfindingNode>();
        List<PathfindingNode> nodes = new List<PathfindingNode>();

        //Get our starting point
        openSet.Add(new PathfindingNode(unit.currentTile, 0));
        nodes.Add(new PathfindingNode(unit.currentTile, 0));

        PathfindingNode currentNode;
        while (openSet.Count > 0)
        {
            //TODO Get lowest movement node
            //Grab the next node to look at
            currentNode = openSet[0];
            openSet.Remove(currentNode);

            //Go through each neighbor of the current node
            List<MapTile> neighbors = currentNode.tile.GetNeighbors();
            foreach(MapTile neighbor in neighbors)
            {
                //Check the cost to move to this tile from the current node
                int costToMove = GetCostToMoveBetweenTiles(unit, currentNode.tile, neighbor);

                if (costToMove == -1)
                {
                    //We can't move to this tile from here
                    continue;
                }

                //Is this tile in range?
                int totalMoveCost = currentNode.movementToReachNode + costToMove;
                if (totalMoveCost > unit.movementRange)
                {
                    //Tile is out of range, so skip it
                    continue;
                }

                //Check if this tile is already a node
                PathfindingNode neighborNode = nodes.Find(n => n.tile == neighbor);
                if(neighborNode == null)
                {
                    //No existing node, so make a pathfinding node for this tile and add it to the openset
                    neighborNode = new PathfindingNode(neighbor, totalMoveCost);
                    nodes.Add(neighborNode);
                    openSet.Add(neighborNode);
                }
                else
                {
                    //Check if we need to update the existing node based on movement cost
                    if(neighborNode.movementToReachNode > totalMoveCost)
                    {
                        //Update this node's move cost and readd it to the openset if it isn't there
                        neighborNode.movementToReachNode = totalMoveCost;
                        if(!openSet.Contains(neighborNode))
                        {
                            openSet.Add(neighborNode);
                        }
                    }
                }
            }
        }

        List<MapTile> moveableTiles = new List<MapTile>();
        nodes.ForEach(n => {
            if (n.tile.occupant == null || n.tile.occupant == unit)
            { 
                moveableTiles.Add(n.tile); 
            }
        });
        return moveableTiles;
    }

    public static int GetCostToMoveBetweenTiles(Unit u, MapTile t1, MapTile t2)
    {
        if(t2.tileHeight - t1.tileHeight > u.maxVerticalMovement)
        {
            //Too big a difference in heights, cannot move
            return -1;
        }

        if(t2.occupant != null && t2.occupant.team != u.team)
        {
            //Cannot move through an enemy
            return -1;
        }

        //TODO Add terrains for move cost

        return 1;
    }
}
