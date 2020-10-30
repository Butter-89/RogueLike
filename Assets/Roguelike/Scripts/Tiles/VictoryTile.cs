using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryTile : MonoBehaviour
{
    GameActor playerActor;

    private void Start()
    {
        playerActor = GameObject.FindGameObjectWithTag("Player").GetComponent<GameActor>();
        if (playerActor == null)
            throw new System.Exception("Player is not found for the victory room");
    }

    public void CheckWinning()
    {
        if(playerActor.currentTile == GetComponent<Tile>())
        {
            //Debug.Log("Winnnnn!");
            CanvasGroup cg = GameObject.Find("WinFailUI").GetComponent<CanvasGroup>();
            cg.gameObject.GetComponent<Text>().text = "You Win !";
            cg.alpha = 1;
            
        }
    }
}
