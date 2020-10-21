using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    List<GameActor> enemies;
    private GameActor player;

    [SerializeField]
    private bool isPlayerTurn;

    private void Start()
    {
        isPlayerTurn = true;
    }

    public void RoomSetup()
    {
        
    }

    public void SwitchTurn()
    {
        if(isPlayerTurn)
        {
            PlayerTurnEnd();
            isPlayerTurn = false;
            EnemyTurnStart();
        }
        else
        {
            EnemyTurnEnd();
            isPlayerTurn = true;
            PlayerTurnStart();
        }
    }

    public void RegisterEnemy()
    {

    }

    public void PlayerTurnStart()
    {

    }

    public void PlayerTurnEnd()
    {

    }

    public void EnemyTurnStart()
    {

    }

    public void EnemyTurnEnd()
    {

    }
}
