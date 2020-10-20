using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera cam;

    private GameActor player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<GameActor>();
        if (player == null)
            throw new System.Exception("Player Game Object Not Found");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform.parent;
                if (objectHit.GetComponent<Tile>() != null)
                {
                    // get tile info
                    Tile tile = objectHit.GetComponent<Tile>();

                    // move player onto it 
                    player.SetDestination(tile.transform.localPosition);
                }
            }
        }
    }

    
}
