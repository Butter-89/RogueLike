using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private GameActor AI_actor;
    private GameActor playerActor;
    private void Start()
    {
        AI_actor = GetComponent<GameActor>();
        if (AI_actor == null)
            throw new System.Exception(transform.name + "'s AI controller failed to get actor component");

        playerActor = GameObject.FindGameObjectWithTag("Player").GetComponent<GameActor>();
        if (playerActor == null)
            throw new System.Exception(transform.name + "'s AI controller failed to get Player's actor component");
    }

    public void MakeDecision()
    {
        if(AI_actor.stunedTurns == 0)
        {
            // Calculate distance according to its attack range
            if (AI_actor.actorType == ActorType.Archer)
            {
                if (!CanShootActor(playerActor))
                {
                    // Find shooting position
                    MoveCloserToTarget(playerActor);
                }
                else
                {
                    // Attack player in range
                    AI_actor.DealDamage(playerActor, 1);
                }

            }
            else if (AI_actor.actorType == ActorType.Warrior)
            {
                if (GetDistanceToActor(playerActor) >= 1)
                {
                    // Move closer to player
                    MoveCloserToTarget(playerActor);
                }
                else
                {
                    // Perform melee attack
                    AI_actor.DealDamage(playerActor, 1);
                }
            }
        }
        else
        {
            AI_actor.stunedTurns -= 1;
        }
        
    }

    private int GetDistanceToActor(GameActor i_actor)
    {
        return (Mathf.Abs(i_actor.currentTile.roomPosition.x - AI_actor.currentTile.roomPosition.x)
                + Mathf.Abs(i_actor.currentTile.roomPosition.y - AI_actor.currentTile.roomPosition.y)) - 1;
    }

    private bool CanShootActor(GameActor i_actor)
    {
        bool aligned = false;
        if(i_actor.currentTile.roomPosition.x == AI_actor.currentTile.roomPosition.x)
        {
            int tileNum = Math.Abs(i_actor.currentTile.roomPosition.y - AI_actor.currentTile.roomPosition.y) - 1;
            int sign = i_actor.currentTile.roomPosition.y - AI_actor.currentTile.roomPosition.y > 0 ? 1 : -1;
            for (int i = 1; i <= tileNum; i++)
            {
                Tile tile = AI_actor.currentTile.room.GetTileAt(AI_actor.currentTile.roomPosition + new Vector2Int(0, sign * i));
                if (tile == null)
                    break;
            }
            aligned = true;
        }
        else if(i_actor.currentTile.roomPosition.y == AI_actor.currentTile.roomPosition.y)
        {
            int tileNum = Math.Abs(i_actor.currentTile.roomPosition.x - AI_actor.currentTile.roomPosition.x) - 1;
            int sign = i_actor.currentTile.roomPosition.x - AI_actor.currentTile.roomPosition.x > 0 ? 1 : -1;
            for (int i = 1; i <= tileNum; i++)
            {
                Tile tile = AI_actor.currentTile.room.GetTileAt(AI_actor.currentTile.roomPosition + new Vector2Int(sign * i, 0));
                if (tile == null)
                    break;
            }
            aligned = true;
        }
        return GetDistanceToActor(i_actor) >= 2 && aligned;
    }

    private void MoveCloserToTarget(GameActor i_actor)
    {
        Vector2Int targetPosition = new Vector2Int(i_actor.currentTile.roomPosition.x, i_actor.currentTile.roomPosition.y) ;
        Vector2Int actorPosition = new Vector2Int(AI_actor.currentTile.roomPosition.x, AI_actor.currentTile.roomPosition.y);
        Vector2Int moveToPosition = actorPosition; //new Vector2Int(actorPosition.x, actorPosition.y);
        int diff_x = Mathf.Abs(targetPosition.x - actorPosition.x);
        int diff_y = Mathf.Abs(targetPosition.y - actorPosition.y);
        // Try to be in a line with i_actor
        if (diff_x < diff_y)
        {
            // Go for x direction
            if(diff_x == 0)
            {
                if (targetPosition.y >= actorPosition.y)
                    moveToPosition += new Vector2Int(0, 1);
                else
                    moveToPosition += new Vector2Int(0, -1);
            }
            else
            {
                if (targetPosition.x >= actorPosition.x)
                    moveToPosition += new Vector2Int(1, 0);
                else
                    moveToPosition += new Vector2Int(-1, 0);
            }
            
        }
        else
        {
            // Go for y direction
            if(diff_y == 0)
            {
                if (targetPosition.x >= actorPosition.x)
                    moveToPosition += new Vector2Int(1, 0);
                else
                    moveToPosition += new Vector2Int(-1, 0);
            }
            else
            {
                if (targetPosition.y >= actorPosition.y)
                    moveToPosition += new Vector2Int(0, 1);
                else
                    moveToPosition += new Vector2Int(0, -1);
            }
            
        }
        //Debug.Log(transform.name + " moves to " + moveToPosition);
        Tile targetTile = AI_actor.currentTile.room.GetTileAt(moveToPosition);
        if (targetTile.IsReachable() && !targetTile.Occupied)
        {
            
        }
        AI_actor.currentTile = AI_actor.currentTile.room.GetTileAt(moveToPosition);
        AI_actor.SetDestination(AI_actor.currentTile.roomPosition);
    }
}
