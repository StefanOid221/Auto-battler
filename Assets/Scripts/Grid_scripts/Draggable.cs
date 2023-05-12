using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    public LayerMask releaseMask;
    public Vector3 dragOffset = new Vector3(0, -0.4f, 0);

    private Camera cam;

    private Renderer Renderer;

    private Vector3 oldPosition;
    private int oldSortingOrder;
    private Tile previousTile = null;
    private Tile actualTile = null;

    public bool IsDragging = false;
    

    private void Start()
    {
        cam = Camera.main;
        Renderer = GetComponent<Renderer>();
        BaseUnit thisUnit = GetComponent<BaseUnit>();
        actualTile = GridManager.Instance.GetTileForNode(thisUnit.CurrentNode);
        previousTile = actualTile;

    }

    public void OnStartDrag()
    {

        oldPosition = this.transform.position;
        oldSortingOrder = Renderer.sortingOrder;
        previousTile = actualTile;
        Renderer.sortingOrder = 20;
        IsDragging = true;
        Debug.Log("1");
    }

    public void OnDragging()
    {
        //Debug.Log("2");
        if (!IsDragging)
            return;

        Tile tileUnder = GetTileUnder();
        if (tileUnder != null)
        {
            tileUnder.SetHighlight(true, !GridManager.Instance.GetNodeForTile(tileUnder).IsOccupied);

            if (previousTile != null && tileUnder != previousTile)
            {
                //We are over a different tile.
                previousTile.SetHighlight(false, false);
            }

            
            Vector3 newPosition = tileUnder.transform.position;
            this.transform.position = newPosition;
            actualTile = tileUnder;

        }
        
    }

    public void OnEndDrag()
    {
        if (!IsDragging)
            return;



        if (!TryRelease())
        {
            //Nothing was found, return to original position.
            this.transform.position = oldPosition;
            actualTile = previousTile;
        }

        if (previousTile != null)
        {
            previousTile.SetHighlight(false, false);
            previousTile = null;
        }

        Renderer.sortingOrder = oldSortingOrder;

        IsDragging = false;
    }

    private bool TryRelease()
    {      
        if (actualTile != null)
        {
            BaseUnit thisUnit = GetComponent<BaseUnit>();
            Node candidateNode = GridManager.Instance.GetNodeForTile(actualTile);
            if (candidateNode != null && thisUnit != null)  
            {
                Debug.Log("3");
                Debug.Log(thisUnit);
                if (!candidateNode.IsOccupied && actualTile.team == previousTile.team)
                {
                    Debug.Log("5");
                    if (previousTile.isBench && !actualTile.isBench)
                    {
                        if (GameManager.Instance.team1BoardUnits.Count < PlayerData.Instance.level)
                        {
                            Debug.Log("5.1");
                            GameManager.Instance.removeAtTile(candidateNode);
                            thisUnit.isBenched = false;
                            thisUnit.previousFightTile = actualTile;
                            GameManager.Instance.team1BoardUnits.Add(thisUnit);
                            moveUnit(thisUnit, candidateNode);
                            return true;
                        }
                        else return false;
                    } 
                    else if (actualTile.isBench && !previousTile.isBench)
                    {
                        Debug.Log("5.2");
                        GameManager.Instance.team1BenchUnits.Add(thisUnit);
                        thisUnit.isBenched = true;
                        GameManager.Instance.team1BoardUnits.Remove(thisUnit);
                        moveUnit(thisUnit, candidateNode);
                        return true;
                    }
                    if (actualTile.isBench && previousTile.isBench)
                    {
                        Debug.Log("5.3");
                        moveUnit(thisUnit, candidateNode);
                        return true;
                    }
                    else if (!actualTile.isBench && !previousTile.isBench)
                    {
                        moveUnit(thisUnit, candidateNode);
                        thisUnit.previousFightTile = actualTile;
                    }
                        
                    Debug.Log("5.4");
                    previousTile = actualTile;
                    return true;
                }
            }
        }
        return false;
    }

    public Tile GetTileUnder()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, releaseMask))
        {
            // Get the exact point on the board where the ray intersects
            Vector3 pointOnBoard = hit.point;

            // Find the tile that contains the point
            Tile[] tiles = FindObjectsOfType<Tile>();
            foreach (Tile tile in tiles)
            {
                Collider tileCollider = tile.GetComponent<Collider>();
                if (tileCollider.bounds.Contains(pointOnBoard))
                {

                    return tile;
                }
            }
        }

        return null;
    }
    public void moveUnit(BaseUnit unit, Node node)
    {
        unit.CurrentNode.SetOccupied(false);
        unit.SetCurrentNode(node);
        node.SetOccupied(true);
        unit.transform.position = node.worldPosition;
    }


}