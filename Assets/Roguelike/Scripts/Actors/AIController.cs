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
        // Calculate distance according to its attack range
        if(AI_actor.actorType == ActorType.Archer)
        {
            if (!CanShootActor(playerActor))
            {
                // Find shooting position
            }
            else
            {
                // Attack player in range
            }

        }
        else if(AI_actor.actorType == ActorType.Warrior)
        {
            if(GetDistanceToActor(playerActor) >= 1)
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

    private int GetDistanceToActor(GameActor i_actor)
    {
        return (Mathf.Abs((int)i_actor.CurrentPosition.x - (int)AI_actor.CurrentPosition.x)
                + Mathf.Abs((int)i_actor.CurrentPosition.z - (int)AI_actor.CurrentPosition.z)) - 1;
    }

    private bool CanShootActor(GameActor i_actor)
    {
        return false;
    }

    private void MoveCloserToTarget(GameActor i_actor)
    {
        Vector2Int targetPosition = new Vector2Int((int)i_actor.CurrentPosition.x, (int)i_actor.CurrentPosition.z) ;
        Vector2Int actorPosition = new Vector2Int((int)AI_actor.CurrentPosition.x, (int)AI_actor.CurrentPosition.z);
        Vector2Int moveToPosition = new Vector2Int(actorPosition.x, actorPosition.y);
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
        AI_actor.currentTile = AI_actor.currentTile.room.GetTileAt(moveToPosition);
        AI_actor.SetDestination(AI_actor.currentTile.roomPosition);
    }
}
