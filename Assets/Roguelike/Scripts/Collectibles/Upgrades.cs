using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeType { RestoreHP, IncMaxHP, IncMaxMana, IncMaxJump}

public class Upgrades : MonoBehaviour
{
    //public UpgradeType upgradeType;
    public List<UpgradeType> types;
    public Tile currentTile;
    private CanvasGroup group;
    private GameObject option1;
    private GameObject option2;
    private GameObject option3;
    private GameActor player;
    // Start is called before the first frame update
    private void Start()
    {
        GenerateUpgradeContent();
        SetUpUI();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<GameActor>();
    }

    private void GenerateUpgradeContent()
    {
        types.Add(UpgradeType.IncMaxHP);
        types.Add(UpgradeType.RestoreHP);
        types.Add(UpgradeType.IncMaxMana);
        types.Add(UpgradeType.IncMaxJump);

        int toRemove = Random.Range(0, 4);
        types.RemoveAt(toRemove);
    }

    private void SetUpUI()
    {
        // Bind the buttons in the canvas
        option1 = GameObject.Find("Option1");
        option2 = GameObject.Find("Option2");
        option3 = GameObject.Find("Option3");
        group = option1.transform.parent.GetComponent<CanvasGroup>();


        option1.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = types[0].ToString();
        option1.GetComponentInChildren<Text>().text = GetDescription(types[0]);
        option1.GetComponentInChildren<Button>().onClick.AddListener(() => Upgrade(player, 0));

        option2.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = types[1].ToString();
        option2.GetComponentInChildren<Text>().text = GetDescription(types[1]);
        option2.GetComponentInChildren<Button>().onClick.AddListener(() => Upgrade(player, 1));

        option3.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = types[2].ToString();
        option3.GetComponentInChildren<Text>().text = GetDescription(types[2]);
        option3.GetComponentInChildren<Button>().onClick.AddListener(() => Upgrade(player, 2));

        // Bind event

    }

    public void ShowMenu()
    {
        //upgradeMenu.gameObject.SetActive(true);
        group.alpha = 1;
        group.blocksRaycasts = true;
    }

    public void CloseMenu()
    {
        group.alpha = 0;
        group.blocksRaycasts = false;
    }

    public void Upgrade(GameActor i_player, int i_option)
    {
        UpgradeType chosenType = types[i_option];
        switch(chosenType)
        {
            case UpgradeType.RestoreHP:
                i_player.HP = i_player.MaxHP;
                break;
            case UpgradeType.IncMaxHP:
                i_player.MaxHP += 1;
                i_player.HP += 1;
                break;
            case UpgradeType.IncMaxMana:
                i_player.MaxMana += 1;
                i_player.HP += 1;
                break;
            case UpgradeType.IncMaxJump:
                PlayerController controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
                controller.jumpRadius += 1;
                break;
        }
        CloseMenu();
        OnComplete();
        TurnManager tm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TurnManager>();
        tm.PlayerTurnEnd();
    }

    public void OnComplete()
    {
        TurnManager tm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TurnManager>();
        // deregister upgrade
        tm.DeregisterUpgrade(this);
        Destroy(gameObject);
    }

    private string GetDescription(UpgradeType type)
    {
        string description = "";
        switch(type)
        {
            case UpgradeType.RestoreHP:
                description = "Restore player's HP";
                break;
            case UpgradeType.IncMaxHP:
                description = "Increase player's max HP";
                break;
            case UpgradeType.IncMaxMana:
                description = "Increase player's max mana";
                break;
            case UpgradeType.IncMaxJump:
                description = "Increase player's max jump radius";
                break;
        }
        return description;
    }
}
