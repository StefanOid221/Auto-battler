using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_Manager : Manager<IA_Manager>
{
    public UIShop shopRef;
    public GameObject shop;
    public IA_Utility_Units utility;

    public void Start()
    {
        shopRef = shop.GetComponent<UIShop>();
        utility = shop.GetComponent<IA_Utility_Units>();
    }

    public void buyCard()
    {
        shopRef.OnLevelUpClick();
        if (GameManager.Instance.team2Units.Count < 5)
        {
            
            //shopRef.OnCardClick(shopRef.allCards[0], shopRef.allCards[0].myData);
            //shopRef.OnCardClick(shopRef.allCards[1], shopRef.allCards[1].myData);
            DecideWhatToBuy(); 
        }
        
        
    }

    public void DecideWhatToBuy()
    {
        int tankCount = 0;
        int warriorCount = 0;
        int rangedCount = 0;
        int assasinCount = 0;

        int tankShopCount = 0;
        int warriorShopCount = 0;
        int rangedShopCount = 0;
        int assasinShopCount = 0;

        int mostExpensive = 2;

        

        foreach (BaseUnit unit in GameManager.Instance.team2Units)
        {
            if (unit.unitType == 1) //Tank
                tankCount += 1;
            else if (unit.unitType == 2) //Warrior
                warriorCount += 1;
            else if (unit.unitType == 3) //Assasin
                assasinCount += 1;
            else rangedCount += 1; // Ranged
        }

        foreach (UICard unit in shopRef.allCards)
        {
            if (unit.myData.prefab.unitType == 1)
            { //Tank
                tankShopCount += 1;
                mostExpensive = (int)unit.myData.prefab.cost;
            }
            else if (unit.myData.prefab.unitType == 2)
            { //Warrior
                warriorShopCount += 1;
                if (tankCount == 0 && assasinCount == 0) mostExpensive = (int)unit.myData.prefab.cost;
            }
            else if (unit.myData.prefab.unitType == 3)
            { //Assasin
                assasinShopCount += 1;
                if (tankCount == 0) mostExpensive = (int)unit.myData.prefab.cost;
            }
            else rangedShopCount += 1; // Ranged
        }

        while (IAData.Instance.CanAfford(mostExpensive))
        {
            if (tankShopCount == 3 && IAData.Instance.CanAfford(15))
            {
                BuyThreeUnits(1);
                break;
            }

            else if (warriorShopCount >= 3 && IAData.Instance.CanAfford(9))
            {
                BuyThreeUnits(2);
                break;
            }
            else if (rangedCount >= 3 && IAData.Instance.CanAfford(6))
            {
                BuyThreeUnits(4);
                break;
            }
            else if (assasinCount >= 3 && IAData.Instance.CanAfford(12))
            {
                BuyThreeUnits(3);
                break;
            }

            else
            {
                List<float> values = new List<float>();

                for(int i =0;i < shopRef.allCards.Count; i++)
                {
                    if (shopRef.allCards[i].myData.prefab.unitType == 1 && shopRef.allCards[i].isActiveAndEnabled)
                    {
                        values.Add(utility.CalculateUtility(shopRef.allCards[i].myData.prefab, tankShopCount, tankCount));
                    }
                    else if (shopRef.allCards[i].myData.prefab.unitType == 2 && shopRef.allCards[i].isActiveAndEnabled)
                    {
                        values.Add(utility.CalculateUtility(shopRef.allCards[i].myData.prefab, warriorShopCount, warriorCount));
                    }
                    else if (shopRef.allCards[i].myData.prefab.unitType == 3 && shopRef.allCards[i].isActiveAndEnabled)
                    {
                        values.Add(utility.CalculateUtility(shopRef.allCards[i].myData.prefab, assasinShopCount, assasinShopCount));
                    }
                    else if (shopRef.allCards[i].myData.prefab.unitType == 4 && shopRef.allCards[i].isActiveAndEnabled)
                        values.Add(utility.CalculateUtility(shopRef.allCards[i].myData.prefab, rangedShopCount, rangedShopCount));
                    else values.Add(0);
                }

                int index = getMaxPosition(values);
                shopRef.OnCardClick(shopRef.allCards[index], shopRef.allCards[index].myData);
            }
        }


    }

    public void BuyThreeUnits(int type)
    {
        for (int i =0; i< shopRef.allCards.Count; i++)
        {
            if (shopRef.allCards[i].myData.prefab.unitType == type && shopRef.allCards[i].isActiveAndEnabled)
            {             
                shopRef.OnCardClick(shopRef.allCards[i], shopRef.allCards[i].myData);
                break;
            }
        }
        for (int i = 0; i < shopRef.allCards.Count; i++)
        {
            if (shopRef.allCards[i].myData.prefab.unitType == type && shopRef.allCards[i].isActiveAndEnabled)
            {
                shopRef.OnCardClick(shopRef.allCards[i], shopRef.allCards[i].myData);
                break;
            }
        }
        for (int i = 0; i < shopRef.allCards.Count; i++)
        {
            if (shopRef.allCards[i].myData.prefab.unitType == type && shopRef.allCards[i].isActiveAndEnabled)
            {
                shopRef.OnCardClick(shopRef.allCards[i], shopRef.allCards[i].myData);
                break;
            }
        }
    }

    public int getMaxPosition(List<float> list)
    {
        int index = -1;
        float max = 0;
        for (int i=0;i<list.Count; i++)
        {
            if (list[i] > max)
            {
                max = list[i];
                index = i;
            }
        }
        return index;
    }
}
