using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TurnManager : MonoBehaviour
{
    List<GameActor> enemies = new List<GameActor>();
    List<Pickups> pickups = new List<Pickups>();
    List<Upgrades> upgrades = new List<Upgrades>();
    //public GameActor Player;
    public GameActor Player { get; set; }
    public Button jumpButton;
    public Button spearButton;
    public Button pushButton;
    [SerializeField]
    private bool isPlayerTurn;
    public bool isJumping;
    public bool throwingSpear;
    public bool isPushing;

    private Text hp;
    private Text mana;
    private Text inventory;

    private void Start()
    {
        isPlayerTurn = true;
        isJumping = false;
        throwingSpear = false;

        jumpButton = GameObject.Find("Button_Jump").GetComponent<Button>();
        if (jumpButton == null) throw new System.Exception("Jump button not found");
        jumpButton.onClick.AddListener(ToggleJumpMode);

        spearButton = GameObject.Find("Button_Spear").GetComponent<Button>();
        if(spearButton == null) throw new System.Exception("Spear button not found");
        spearButton.onClick.AddListener(ToggleSpearMode);

        pushButton = GameObject.Find("Button_Push").GetComponent<Button>();
        if (pushButton == null) throw new System.Exception("Push button not found");
        pushButton.onClick.AddListener(TogglePushMode);

        hp = GameObject.Find("HP").GetComponent<Text>();
        mana = GameObject.Find("Mana").GetComponent<Text>();
        inventory = GameObject.Find("Inventory").GetComponent<Text>();

        //UpdateUI();
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

    public void RegisterPickup(Pickups i_pickup)
    {
        if (!pickups.Contains(i_pickup))
            pickups.Add(i_pickup);
        else
            Debug.LogWarning("Pickup " + i_pickup.transform.name + "is already registered");
    }

    public void RegisterUpgrade(Upgrades i_upgrade)
    {
        if (!upgrades.Contains(i_upgrade))
            upgrades.Add(i_upgrade);
        else
            Debug.LogWarning("Upgrade " + i_upgrade.transform.name + "is already registered");
    }

    public void DeregistorActor(GameActor i_actor)
    {
        if (enemies.Contains(i_actor))
            enemies.Remove(i_actor);
        else
            Debug.LogWarning("Actor not found in the enemy list");
    }

    public void DeregisterPickup(Pickups i_pickup)
    {
        if (pickups.Contains(i_pickup))
            pickups.Remove(i_pickup);
        else
            Debug.LogWarning("Pickup " + i_pickup.transform.name + "not registered");
    }

    public void DeregisterUpgrade(Upgrades i_upgrade)
    {
        if (upgrades.Contains(i_upgrade))
            upgrades.Remove(i_upgrade);
    }

    public void PlayerTurnStart()
    {
        //Debug.Log("Player turn starts");
        // Enable player input from mouse click
        ToggleControllerInput(true);
        Player.currentTile.HighlightAdjacentTile();
        UpdateUI();
    }

    public void PlayerTurnEnd()
    {
        //Debug.Log("Player turn ends");
        // Disable player input from mouse click
        ToggleControllerInput(false);
        // Unhighlight or reset the tiles
        Player.currentTile.room.ResetHighlightedTiles();
        UpdateUI();
        SwitchTurn();
        
    }

    public void EnemyTurnStart()
    {
        //Debug.Log("Enemy turn starts");
        // TODO: Perform enemy actions using functions like TurnUpdate()
        foreach(GameActor enemyActor in enemies)
        {
            enemyActor.GetComponent<AIController>().MakeDecision();
        }
        UpdateUI();
        EnemyTurnEnd();
    }

    public void EnemyTurnEnd()
    {
        //Debug.Log("Player turn ends");
        // Call SwitchTurn() after updating all the enemies
        UpdateUI();
        SwitchTurn();
    }

    private void ToggleControllerInput(bool i_enable)
    {
        PlayerController controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (controller == null)
            throw new System.Exception("Player controller not found");

        controller.InPlayerTurn = i_enable;
    }

    public void KillAll()
    {
        for(int i = enemies.Count - 1; i >=0; i--)
        {
            enemies[i].ActorDeath();
        }
    }

    public void ClearCollectibles()
    {
        for (int i = pickups.Count - 1; i >= 0; i--)
        {
            pickups[i].OnPickUp();
        }
    }

    public void ClearUpgrades()
    {
        for (int i = upgrades.Count - 1; i >= 0; i--)
        {
            upgrades[i].OnComplete();
        }
    }

    private void ToggleJumpMode()
    {
        SwitchSpearModeTo(false);
        SwitchPushModeTo(false);
        if (Player.mana - 2 >= 0)
        {
            PlayerController controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            if (controller == null)
                throw new System.Exception("Player controller not found");

            int radius = controller.jumpRadius;
            if (!isJumping)
            {
                isJumping = true;
                Player.currentTile.room.ResetHighlightedTiles();
                jumpButton.GetComponentInChildren<Text>().text = "Quit Jump";
                // highlight range 2 tiles
                Player.currentTile.HighlightTilesInRange(radius);
            }
            else
            {
                isJumping = false;
                Player.currentTile.room.ResetHighlightedTiles();
                jumpButton.GetComponentInChildren<Text>().text = "Jump";
                // highlight range 1 (adjacent) tiles
                Player.currentTile.HighlightTilesInRange(1);
            }
        }
        else
            return;
    }

    private void ToggleSpearMode()
    {
        SwitchJumpModeTo(false);
        SwitchPushModeTo(false);
        if (Player.hasSpear)
        {
            PlayerController controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            if (controller == null)
                throw new System.Exception("Player controller not found");

            if(!throwingSpear)
            {
                throwingSpear = true;
                Player.currentTile.room.ResetHighlightedTiles();
                spearButton.GetComponentInChildren<Text>().text = "Quit Spear";
                Player.currentTile.HighlightTilesInRange(1);
                Player.currentTile.HighlightTilesInRange(2);
                Player.currentTile.HighlightTilesInRange(3);
            }
            else
            {
                throwingSpear = false;
                Player.currentTile.room.ResetHighlightedTiles();
                spearButton.GetComponentInChildren<Text>().text = "Spear";
                Player.currentTile.HighlightTilesInRange(1);
            }
        }
    }

    private void TogglePushMode()
    {
        SwitchJumpModeTo(false);
        SwitchSpearModeTo(false);
        if(Player.mana - 3 >= 0)
        {
            PlayerController controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            if (controller == null)
                throw new System.Exception("Player controller not found");

            if (!isPushing)
            {
                isPushing = true;
                //Player.currentTile.room.ResetHighlightedTiles();
                pushButton.GetComponentInChildren<Text>().text = "Quit Push";
                // highlight range 2 tiles
                //Player.currentTile.HighlightTilesInRange(1);
            }
            else
            {
                isPushing = false;
                //Player.currentTile.room.ResetHighlightedTiles();
                jumpButton.GetComponentInChildren<Text>().text = "Push";
                // highlight range 1 (adjacent) tiles
                //Player.currentTile.HighlightTilesInRange(1);
            }
        }
    }

    public void SwitchJumpModeTo(bool i_on)
    {
        if(!i_on)
        {
            isJumping = false;
            jumpButton.GetComponentInChildren<Text>().text = "Jump";
        }
    }

    public void SwitchSpearModeTo(bool i_on)
    {
        if(!i_on)// deactivate
        {
            throwingSpear = false;
            spearButton.GetComponentInChildren<Text>().text = "Spear";
        }
    }

    public void SwitchPushModeTo(bool i_on)
    {
        if(!i_on)
        {
            isPushing = false;
            pushButton.GetComponentInChildren<Text>().text = "Push";
        }
    }

    public void UpdateUI()
    {
        hp.text = "HP - " + Player.HP;
        mana.text = "Mana - " + Player.mana;
        if(Player.hasSpear)
        {
            inventory.text = "Spear in hand";
        }
        else
        {
            inventory.text = "";
        }
        if (Player.mana - 2 < 0)
            jumpButton.interactable = false;
        else
            jumpButton.interactable = true;
    }
}
