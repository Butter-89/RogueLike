using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera cam;

    private GameActor playerActor;
    public bool InPlayerTurn { get; set; }
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        playerActor = GetComponent<GameActor>();
        if (playerActor == null)
            throw new System.Exception("Player Game Object Not Found");

        InPlayerTurn = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!InPlayerTurn)
                return;

            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform.parent;
                if (objectHit.GetComponent<Tile>() != null)
                {
                    // get tile info
                    Tile tile = objectHit.GetComponent<Tile>();
                    //Debug.Log(tile.transform.position);
                    // move player onto it 
                    if(tile.IsReachable())
                    {
                        playerActor.SetDestination(tile.transform.localPosition);
                        playerActor.currentTile = tile;
                    }
                    
                    //tile.HighlightAdjacentTile();
                }
            }
        }
    }

    
}
