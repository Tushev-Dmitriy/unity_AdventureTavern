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
    public GameObject winObj;

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
        if (progressBar.value + 1 != 10)
        {
            progressBar.value = isIncrease ? progressBar.value + 1 : progressBar.value - 1;
        } else
        {
            winObj.SetActive(true);
            progressBar.value++;
            Time.timeScale = 0;
        }
    }

    public void ByNewItem(int numOfItem)
    {
        switch (numOfItem)
        {
            case 0:
                if (databaseManager.userGold - 15 >= 0)
                {
                    databaseManager.meat++;
                    databaseManager.userGold -= 15;
                }
                break;
            case 1:
                if (databaseManager.userGold - 10 >= 0)
                {
                    databaseManager.iron++;
                    databaseManager.userGold -= 10;
                }
                break;
            case 2:
                if (databaseManager.userGold - 5 >= 0)
                {
                    databaseManager.herbs++;
                    databaseManager.userGold -= 5;
                }
                break;
        }

        UpdateAllText();
    }

    public void QuitGame()
    {
        Application.Quit(0);
    }

    public void ResetStat()
    {
        databaseManager.ResetToDefaultValues(databaseManager.dbConnection);
        UpdateAllText();
        progressBar.value = databaseManager.userProgress;
    }
}
