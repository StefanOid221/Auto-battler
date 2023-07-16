using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    public LayerMask releaseMask;
    

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
        if(GameManager.Instance.gameState != GameState.Fight) {
            oldPosition = this.transform.position;
            oldSortingOrder = Renderer.sortingOrder;
            previousTile = actualTile;
            Renderer.sortingOrder = 20;
            IsDragging = true;

        }
            }
    public void OnDragging()
    {
        if (!IsDragging || GameManager.Instance.gameState == GameState.Fight)
            return;

        Tile tileUnder = GetTileUnder();
        if (tileUnder != null)
        {
            Vector3 newPosition = tileUnder.transform.position;
            this.transform.position = newPosition;
            actualTile = tileUnder;
        }
        
    }
    public void OnEndDrag()
    {
        if (!IsDragging || GameManager.Instance.gameState == GameState.Fight)
            return;
        if (!TryRelease())
        {
            //Nothing was found, return to original position.
            this.transform.position = oldPosition;
            actualTile = previousTile;
        }
        if (previousTile != null)
        {
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
                if (!candidateNode.IsOccupied && actualTile.team == previousTile.team)
                {
                    if (previousTile.isBench && !actualTile.isBench)
                    {
                        if (GameManager.Instance.team1BoardUnits.Count < PlayerData.Instance.level && GameManager.Instance.gameState == GameState.Decision)
                        {
                            GameManager.Instance.removeAtTile(candidateNode);
                            thisUnit.isBenched = false;
                            thisUnit.previousFightTile = actualTile;
                            GameManager.Instance.team1BoardUnits.Add(thisUnit);
                            GameManager.Instance.team1BenchUnits.Remove(thisUnit);
                            moveUnit(thisUnit, candidateNode);
                            return true;
                        }
                        else return false;
                    } 
                    else if (actualTile.isBench && !previousTile.isBench && GameManager.Instance.gameState == GameState.Decision)
                    {

                        GameManager.Instance.team1BenchUnits.Add(thisUnit);
                        thisUnit.isBenched = true;
                        GameManager.Instance.team1BoardUnits.Remove(thisUnit);
                        moveUnit(thisUnit, candidateNode);
                        return true;
                    }
                    if (actualTile.isBench && previousTile.isBench)
                    {
                        moveUnit(thisUnit, candidateNode);
                        return true;
                    }
                    else if (!actualTile.isBench && !previousTile.isBench && GameManager.Instance.gameState == GameState.Decision)
                    {
                        moveUnit(thisUnit, candidateNode);
                        thisUnit.previousFightTile = actualTile;
                    }       
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
            Vector3 pointOnBoard = hit.point;
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