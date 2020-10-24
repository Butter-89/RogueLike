using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActorType { Player, Warrior, Archer  };

public class GameActor : MonoBehaviour
{
    // Basic properties
    //public string actorName;    // Really Needed?
    public ActorType actorType;

    public int MaxHP;
    public int MaxMana;
    public int AttackRange;

    private int HP { get; set; }
    private int mana;

    public float moveTime;
    private float timer;
    private Vector3 currentPosition;
    public Vector3 CurrentPosition => currentPosition;
    private Vector3 targetPosition;
    public Tile currentTile { get; set; }
    private bool turnStart;
    private bool actionComplete;
    private bool hasSpear;
    // Start is called before the first frame update

    void Start()
    {
        timer = 0f;
        hasSpear = false;
        actionComplete = true;
        HP = MaxHP;
        mana = MaxMana;
    }

    void Update()
    {
        if(!actionComplete)
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
        actionComplete = true;
        if(actorType == ActorType.Player)
        {
            if(hasSpear)
            {
                // Damage the enemy ahead if it's there
            }

            TurnManager tm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TurnManager>();
            if (tm == null)
                throw new System.Exception("TurnManager is not found for actor: " + transform.name);

            // Player turn ends
            tm.PlayerTurnEnd();
        }
    }

    public void SetCurrentPosition(Vector3 i_position)
    {
        currentPosition = i_position;
    }

    public void SetDestination(Vector3 i_destination)
    {
        targetPosition = i_destination;
        if (currentPosition != targetPosition)
            actionComplete = false;
    }

    public void SetDestination(Vector2Int i_destination)
    {
        targetPosition = new Vector3(i_destination.x, 0, i_destination.y);
        if (currentPosition != targetPosition)
            actionComplete = false;
    }

    // Attack
    public void DealDamage(GameActor i_actor, int i_dmg)
    {
        // TODO: play some VFX and SFX

        // Deal actual damage
        i_actor.HP = Mathf.Clamp(i_actor.HP - i_dmg, 0, 100);
        if (i_actor.HP == 0)
            ActorDeath();
    }

    private void ActorDeath()
    {
        if(actorType != ActorType.Player)
        {
            // De-register from actor's manager
            currentTile.room.TurnManager.DeregistorActor(this);
            // Play SFX & VFX and destroy game object
            Destroy(this.gameObject);
        }

    }
}
