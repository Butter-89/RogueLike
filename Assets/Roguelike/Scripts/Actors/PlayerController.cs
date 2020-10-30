using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Camera cam;

    private GameActor playerActor;
    public bool InPlayerTurn { get; set; }
    public int jumpRadius = 2;
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        playerActor = GetComponent<GameActor>();
        if (playerActor == null)
            throw new System.Exception("Player Game Object Not Found");

        // bind the event of jumping to the button

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
                if(objectHit.GetComponent<AIController>() != null)
                {
                    EnemyClicked(objectHit);
                }
                else if (objectHit.GetComponent<Tile>() != null)
                {
                    TileClicked(objectHit);
                }
                else if(objectHit.GetComponent<Pickups>() || objectHit.GetComponent<Upgrades>())
                {
                    CollectibleClicked(objectHit);
                }
            }
        }
        else if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Restore();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            KillA();
        }
    }

    private void EnemyClicked(Transform i_hitObject)
    {
        TurnManager tm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TurnManager>();
        if (tm == null)
            throw new System.Exception("TurnManager is not found for actor: " + transform.name);

        GameActor ai_actor = i_hitObject.GetComponent<GameActor>();
        Tile tile = ai_actor.currentTile;
        if (tile.IsReachable())
        {
            if(tm.isPushing)
            {
                PushActorBack(ai_actor);
                playerActor.mana -= 3;
                tm.SwitchPushModeTo(false);
                tm.PlayerTurnEnd();
                return;
            }

            if (tm.throwingSpear)
            {
                tile.SpawnPickup(tm.GetComponent<GameplayManager>().pickupPrefab);
                playerActor.hasSpear = false;
            }
            playerActor.DealDamage(ai_actor, 1);
            tm.PlayerTurnEnd();
        }
    }

    private void TileClicked(Transform i_hitObject)
    {
        TurnManager tm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TurnManager>();
        if (tm == null)
            throw new System.Exception("TurnManager is not found for actor: " + transform.name);
        // get tile info
        Tile tile = i_hitObject.GetComponent<Tile>();
        //Debug.Log(tile.transform.position);
        // move player onto it 
        if (tile.IsReachable())
        {
            if(tm.isPushing)
            {
                GameActor enemy = tile.actorOnTile;
                if (enemy == null) throw new System.Exception("Can not push null actor");
                PushActorBack(enemy);
                playerActor.mana -= 3;
                tm.SwitchPushModeTo(false);
                tm.PlayerTurnEnd();
                return;
            }

            if(tm.throwingSpear)
            {
                tile.SpawnPickup(tm.GetComponent<GameplayManager>().pickupPrefab);
                if (tile.actorOnTile)
                    playerActor.DealDamage(tile.actorOnTile, 1);

                playerActor.hasSpear = false;
                tm.SwitchSpearModeTo(false);
                tm.PlayerTurnEnd();
                return;
            }
            playerActor.SetDestination(new Vector3(tile.room.originBoardPosition.x, 0, tile.room.originBoardPosition.y)
                + tile.transform.localPosition);
            playerActor.currentTile = tile;
            if (tm.isJumping)
            {
                playerActor.mana -= 2;
                tm.SwitchJumpModeTo(false);
            }
        }

        

        if (tile.GetComponent<Door>() != null)
        {
            tile.GetComponent<Door>().ToNextRoom(playerActor);
        }

        //tile.HighlightAdjacentTile();
    }

    private void CollectibleClicked(Transform i_hitObject)
    {
        Tile currentTile = null;
        if(i_hitObject.GetComponent<Pickups>())
        {
            currentTile = i_hitObject.GetComponent<Pickups>().currentTile;
        }
        else if(i_hitObject.GetComponent<Upgrades>())
        {
            currentTile = i_hitObject.GetComponent<Upgrades>().currentTile;
        }

        TileClicked(currentTile.transform);
    }

    private void PushActorBack(GameActor i_actor)
    {
        Vector2Int pushPosition = i_actor.currentTile.roomPosition + i_actor.currentTile.roomPosition - playerActor.currentTile.roomPosition;
        Tile tileToPush = i_actor.currentTile.room.GetTileAt(pushPosition);
        if(tileToPush.walkable)
        {
            i_actor.SetDestination(tileToPush.roomPosition + tileToPush.room.originBoardPosition);
        }
        else
        {
            playerActor.DealDamage(i_actor, 1);
        }    
    }

    private void Restore()
    {
        playerActor.HP = playerActor.MaxHP;
        playerActor.mana = playerActor.MaxMana;
        TurnManager tm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TurnManager>();
        tm.PlayerTurnEnd();
    }

    private void KillA()
    {
        TurnManager tm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TurnManager>();
        tm.KillAll();
    }
}
