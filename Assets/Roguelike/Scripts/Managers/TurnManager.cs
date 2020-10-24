using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    List<GameActor> enemies = new List<GameActor>();
    //public GameActor Player;
    public GameActor Player { get; set; }

    [SerializeField]
    private bool isPlayerTurn;

    private void Start()
    {
        isPlayerTurn = true;
    }

    public void RoomSetup()
    {
        // for each tile, if there is an enemy object or pickup, register it
    }

    public void SwitchTurn()
    {
        if(isPlayerTurn)
        {
            isPlayerTurn = false;
            EnemyTurnStart();
        }
        else
        {
            isPlayerTurn = true;
            PlayerTurnStart();
        }
    }

    public void RegisterEnemy(GameActor i_actor)
    {
        if (!enemies.Contains(i_actor))
            enemies.Add(i_actor);
        else
            Debug.LogWarning("Actor " + i_actor.transform.name + "is already registered");
    }

    public void DeregistorActor(GameActor i_actor)
    {
        if (enemies.Contains(i_actor))
            enemies.Remove(i_actor);
        else
            Debug.LogWarning("Actor not found in the enemy list");
    }

    public void PlayerTurnStart()
    {
        Debug.Log("Player turn starts");
        // Enable player input from mouse click
        ToggleControllerInput(true);
        Player.currentTile.HighlightAdjacentTile();
    }

    public void PlayerTurnEnd()
    {
        Debug.Log("Player turn ends");
        // Disable player input from mouse click
        ToggleControllerInput(false);
        // Unhighlight or reset the tiles
        Player.currentTile.room.ResetHighlightedTiles();
        SwitchTurn();
    }

    public void EnemyTurnStart()
    {
        Debug.Log("Enemy turn starts");
        // TODO: Perform enemy actions using functions like TurnUpdate()
        foreach(GameActor enemyActor in enemies)
        {
            enemyActor.GetComponent<AIController>().MakeDecision();
        }
        EnemyTurnEnd();
    }

    public void EnemyTurnEnd()
    {
        Debug.Log("Player turn ends");
        // Call SwitchTurn() after updating all the enemies
        SwitchTurn();
    }

    private void ToggleControllerInput(bool i_enable)
    {
        PlayerController controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (controller == null)
            throw new System.Exception("Player controller not found");

        controller.InPlayerTurn = i_enable;
    }
}
