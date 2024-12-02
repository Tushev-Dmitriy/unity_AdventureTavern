using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserDataController : MonoBehaviour
{
    public DatabaseManager databaseManager;
    public TMP_Text goldText;

    public void IncreaseResource(int numOfRes, bool isInrease, int count)
    {
        switch (numOfRes)
        {
            case 0:
                databaseManager.userGold = isInrease ? databaseManager.userGold + count : databaseManager.userGold - count;
                break;
            case 1:
                databaseManager.meat = isInrease ? databaseManager.meat + count : databaseManager.meat - count;                
                break;
            case 2:
                databaseManager.vegetables = isInrease ? databaseManager.vegetables + count : databaseManager.vegetables - count;
                break;
            case 3:
                databaseManager.potions = isInrease ? databaseManager.potions + count : databaseManager.potions - count;
                break;
        }

        UpdateAllText();
    }

    public void UpdateAllText()
    {
        goldText.text = databaseManager.userGold.ToString();
    }
}
