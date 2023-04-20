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
    }

    public void OnStartDrag()
    {

        oldPosition = this.transform.position;
        oldSortingOrder = Renderer.sortingOrder;

        Renderer.sortingOrder = 20;
        IsDragging = true;
        previousTile = actualTile;
    }

    public void OnDragging()
    {
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

            //previousTile = tileUnder;
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
            actualTile = null;
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
        //Released over something!
        
        if (actualTile != null)
        {
            //It's a tile!
            BaseUnit thisEntity = GetComponent<BaseUnit>();
            Node candidateNode = GridManager.Instance.GetNodeForTile(actualTile);
            if (candidateNode != null && thisEntity != null)
            {
                if (!candidateNode.IsOccupied && actualTile.team == previousTile.team)
                {
                    //Let's move this unity to that node
                    thisEntity.CurrentNode.SetOccupied(false);
                    thisEntity.SetCurrentNode(candidateNode);
                    candidateNode.SetOccupied(true);
                    thisEntity.transform.position = candidateNode.worldPosition;

                    if (previousTile.isBench)
                    {
                        PlayerData.Instance.removeAtTile(candidateNode);
                    }
                    if (actualTile.isBench)
                    {
                        PlayerData.Instance.benchUnits.Add(thisEntity);
                        
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


}