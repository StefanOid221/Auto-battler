using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
    public List<UICard> allCards;
    public Text money;

    private UnitDatabaseSO cachedDb;
    private int refreshCost = 1;

    private void Start()
    {
        cachedDb = GameManager.Instance.unitDatabase;
        GenerateCard();
        PlayerData.Instance.OnUpdate += Refresh;
        Refresh();
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
        if (PlayerData.Instance.CanAfford(cardData.cost) && PlayerData.Instance.benchUnits.Count < 7)
        {
            PlayerData.Instance.SpendMoney(cardData.cost);
            card.gameObject.SetActive(false);
            GameManager.Instance.OnUnitBought(cardData);
        }
    }

    public void OnRefreshClick()
    {
        //Decrease money 
        if (PlayerData.Instance.CanAfford(refreshCost))
        {
            PlayerData.Instance.SpendMoney(refreshCost);
            GenerateCard();
        }
    }

    void Refresh()
    {
        money.text = PlayerData.Instance.Money.ToString();
    }
}
