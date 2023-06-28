using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum Player
{
    Player, IA_Player
}
public class UIShop : MonoBehaviour
{
    public List<UICard> allCards;
    public Text money;
    public Text level;
    public Text exp;

    public Player actualPlayer;

    private UnitDatabaseSO cachedDb;
    private int refreshCost = 1;
    private int levelUpCost = 2;

    private void Start()
    {
        cachedDb = GameManager.Instance.unitDatabase;
        GenerateCard();
        PlayerData.Instance.OnUpdate += Refresh;
        //Refresh();S
        
    }

    public void GenerateCard()
    {
        for (int i = 0; i < allCards.Count; i++)
        {
            if (!allCards[i].gameObject.activeSelf)
                allCards[i].gameObject.SetActive(true);
            allCards[i].Setup(cachedDb.allUnits[Random.Range(0, cachedDb.allUnits.Count)], this);
        }
        
    }

    public void OnCardClick(UICard card, UnitDatabaseSO.UnitData cardData)
    {
        if (actualPlayer == Player.Player)
        {
            if (GameManager.Instance.gameState == GameState.Decision && PlayerData.Instance.CanAfford(cardData.cost) && GameManager.Instance.team1BenchUnits.Count < 7)
            {
                PlayerData.Instance.SpendMoney(cardData.cost);
                card.gameObject.SetActive(false);
                GameManager.Instance.OnUnitBought(cardData, Player.Player);
            }
        }
        else if (actualPlayer == Player.IA_Player) {
            IAData.Instance.CanAfford(GameManager.Instance.team2BenchUnits.Count);
            if (GameManager.Instance.gameState == GameState.Decision && IAData.Instance.CanAfford(cardData.cost) && GameManager.Instance.team2BenchUnits.Count < 7)
            {
                IAData.Instance.SpendMoney(cardData.cost);
                card.gameObject.SetActive(false);
                GameManager.Instance.OnUnitBought(cardData,Player.IA_Player);
            }
        }
        
    }

    public void OnRefreshClick()
    {
        if (actualPlayer == Player.Player)
        {
            if (PlayerData.Instance.CanAfford(refreshCost))
            {
                PlayerData.Instance.SpendMoney(refreshCost);
                GenerateCard();
            }
        }
        else if (actualPlayer == Player.IA_Player)
        {
            if (IAData.Instance.CanAfford(refreshCost))
            {
                IAData.Instance.SpendMoney(refreshCost);
                GenerateCard();
            }
        }
    }

    public void RefreshEndRound() {
        GenerateCard();
    }

    public void OnLevelUpClick()
    {
        if (actualPlayer == Player.Player)
        {
            if (PlayerData.Instance.CanAfford(levelUpCost))
            {
                PlayerData.Instance.SpendMoney(levelUpCost);
                PlayerData.Instance.UpdateExp();

            }
        }
        else if (actualPlayer == Player.IA_Player)
        {
            if (IAData.Instance.CanAfford(levelUpCost))
            {
                IAData.Instance.SpendMoney(levelUpCost);
                IAData.Instance.UpdateExp();

            }
        }


    }
    public void Refresh()
    {
        if (actualPlayer == Player.Player)
        {
            money.text = "Money " + PlayerData.Instance.Money.ToString();
            level.text = "Level " + PlayerData.Instance.level.ToString();
            exp.text = "Exp " + PlayerData.Instance.exp.ToString() + "/" + PlayerData.Instance.expNeeded.ToString();
        }
        else if (actualPlayer == Player.IA_Player)
        {
            money.text = "Money " + IAData.Instance.Money.ToString();
            level.text = "Level " + IAData.Instance.level.ToString();
            exp.text = "Exp " + IAData.Instance.exp.ToString() + "/" + IAData.Instance.expNeeded.ToString();
        }
        
    }
}
