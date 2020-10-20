using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActorType { Player, Warrior, Archer  };

public class GameActor : MonoBehaviour
{
    // Basic properties
    public string actorName;    // Really Needed?
    public ActorType actorType;

    [SerializeField]
    private int maxHP;
    public int MaxHP => maxHP;
    [SerializeField]
    private int maxMana;
    public int MaxMana => maxMana;

    public float moveTime;
    private float timer;
    private Vector3 currentPosition;
    private Vector3 targetPosition;
    private bool turnStart;
    private bool moveComplete;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
    }

    void Update()
    {
        if (currentPosition != targetPosition)
        {
            timer += Time.deltaTime;
            MoveToTile();
        }
        else
        {
            OnMoveComplete();
        }
            
        //if(turnStart)
        //{
        //    MoveToTile();
        //}
    }

    private void MoveToTile()
    {
        float t = timer / moveTime;
        transform.position = Vector3.Lerp(currentPosition, targetPosition, t);
    }

    private void OnMoveComplete()
    {
        //currentPosition = targetPosition;
        moveComplete = true;
    }

    public void SetDestination(Vector3 i_destination)
    {
        targetPosition = i_destination;
    }
}
