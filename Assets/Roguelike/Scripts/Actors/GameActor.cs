using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum ActorType { Player, Warrior, Archer  };

public class GameActor : MonoBehaviour
{
    // Basic properties
    //public string actorName;    // Really Needed?
    public ActorType actorType;

    public int MaxHP;
    public int MaxMana;
    public int AttackRange;

    public int HP { get; set; }
    public int mana;

    public float moveTime;
    public int stunedTurns;
    private float timer;
    private Vector3 currentPosition;
    public Vector3 CurrentPosition => currentPosition;
    private Vector3 targetPosition;
    public Tile currentTile { get; set; }
    private bool turnStart;
    private bool actionComplete;
    public bool hasSpear;
    // Start is called before the first frame update

    void Awake()
    {
        timer = 0f;
        hasSpear = false;
        actionComplete = true;
        HP = MaxHP;
        mana = MaxMana;
        stunedTurns = 0;
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
        TurnManager tm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TurnManager>();
        if (tm == null)
            throw new System.Exception("TurnManager is not found for actor: " + transform.name);

        if (currentTile.GetComponent<Lava>())
        {
            ActorDeath();
            return;
        }
            

        if (actorType == ActorType.Player)
        {
            if(currentTile.GetComponent<VictoryTile>())
            {
                currentTile.GetComponent<VictoryTile>().CheckWinning();
            }
            if(hasSpear)
            {
                // Damage the enemy ahead if it's there
            }

            Pickups pickup = currentTile.PickupOnTile;
            Upgrades upgrade = currentTile.upgradeOnTile;
            if (pickup != null)
            {
                CollectPickup(pickup);
            }
            else if(upgrade != null)
            {
                CollectUpgrade(upgrade);
            }
            // Player turn ends
            tm.PlayerTurnEnd();
        }
        else if (actorType == ActorType.Warrior || actorType == ActorType.Archer)
        {
            //tm.EnemyTurnEnd();
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
        targetPosition = new Vector3(currentTile.room.originBoardPosition.x + i_destination.x, 0, currentTile.room.originBoardPosition.y + i_destination.y);
        if (currentPosition != targetPosition)
            actionComplete = false;
    }

    // Attack
    public void DealDamage(GameActor i_target, int i_dmg)
    {
        i_target.HP = Mathf.Clamp(i_target.HP - i_dmg, 0, 100);
        StartCoroutine(HitFlash(i_target));
    }

    IEnumerator HitFlash(GameActor i_target)
    {
        Material prevMat = i_target.GetComponentInChildren<Renderer>().material;
        Color prevColoer = prevMat.color;
        i_target.GetComponentInChildren<Renderer>().material = null;
        i_target.GetComponentInChildren<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        i_target.GetComponentInChildren<Renderer>().material = prevMat;
        i_target.GetComponentInChildren<Renderer>().material.color = prevColoer;
        if (i_target.HP == 0)
            i_target.ActorDeath();
    }

    public void ActorDeath()
    {
        if(actorType != ActorType.Player)
        {
            // De-register from actor's manager
            currentTile.room.TurnManager.DeregistorActor(this);
            currentTile.Occupied = false;
            // Play SFX & VFX and destroy game object
            Destroy(this.gameObject);
        }
        else
        {
            CanvasGroup cg = GameObject.Find("WinFailUI").GetComponent<CanvasGroup>();
            cg.gameObject.GetComponent<Text>().text = "GAME OVER!";
            cg.alpha = 1;
            Destroy(this.gameObject);
        }
    }

    public void CollectPickup(Pickups i_pickup)
    {
        //Pickups pickup = currentTile.PickupOnTile;
        if(i_pickup.type == PickupType.Spear)
        {
            hasSpear = true;
        }
        i_pickup.OnPickUp();
    }

    public void CollectUpgrade(Upgrades i_upgrade)
    {
        i_upgrade.ShowMenu();
    }
}
