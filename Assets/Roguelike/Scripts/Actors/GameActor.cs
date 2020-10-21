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
    public Tile currentTile;
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
        float t = timer / moveTime; // lerp too fast - is t wrong ?
        transform.position = Vector3.Lerp(currentPosition, targetPosition, t);
        currentPosition = transform.position;
    }

    private void OnMoveComplete()
    {
        //currentPosition = targetPosition;
        timer = 0f;
        moveComplete = true;
        if(actorType == ActorType.Player)
        {

        }
    }

    public void SetCurrentPosition(Vector3 i_position)
    {
        currentPosition = i_position;
    }

    public void SetDestination(Vector3 i_destination)
    {
        targetPosition = i_destination;
    }
}
