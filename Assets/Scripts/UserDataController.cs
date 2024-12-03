using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserDataController : MonoBehaviour
{
    public DatabaseManager databaseManager;
    public TMP_Text goldText;
    public TMP_Text meatText;
    public TMP_Text ironText;
    public TMP_Text herbsText;
    public Slider progressBar;

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
                databaseManager.iron = isInrease ? databaseManager.iron + count : databaseManager.iron - count;
                break;
            case 3:
                databaseManager.herbs = isInrease ? databaseManager.herbs + count : databaseManager.herbs - count;
                break;
        }

        UpdateAllText();
    }

    public void UpdateAllText()
    {
        goldText.text = databaseManager.userGold.ToString();
        meatText.text = databaseManager.meat.ToString();
        ironText.text = databaseManager.iron.ToString();
        herbsText.text = databaseManager.herbs.ToString();
    }

    public void ProgressController(bool isIncrease)
    {
        if (progressBar.value != 10)
        {
            progressBar.value = isIncrease ? progressBar.value + 1 : progressBar.value - 1;
        } else
        {
            Debug.Log(1);
        }
    }
}
