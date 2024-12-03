using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartNewOrder : MonoBehaviour
{
    [Header("Scripts")]
    public UserDataController userDataController;

    [Header("Obj in scene")]
    public GameObject dialogObj;
    public GameObject newPerson;
    public Sprite[] personSprite;
    public TMP_Text personName;
    public TMP_Text personOrder;

    private bool userDo = false;
    private Image personImg;
    private GameObject personBtn;
    private Color tempColor = new Color(255, 255, 255, 0);
    private Color mainColor = new Color(255, 255, 255, 255);
    private List<string> persons = new List<string>();
    private List<string> personsTypes = new List<string>();
    private List<string> resources = new List<string>();
    private List<int> resourcesCount = new List<int>();
    private int rndOrderNum = -1;
    private int rndResourcesCountNum = -1;

    private void Awake()
    {
        personImg = newPerson.transform.GetChild(0).GetComponent<Image>();
        personBtn = newPerson.transform.GetChild(1).gameObject;
        personBtn.SetActive(false);
        personImg.color = tempColor;
    }

    public void GetPersonLists(Dictionary<string, string> adventurersDictionary, Dictionary<string, int> resourcesDictionary)
    {
        persons = new List<string>(adventurersDictionary.Keys);
        personsTypes = new List<string>(adventurersDictionary.Values);
        resources = new List<string>(resourcesDictionary.Keys);
        resourcesCount = new List<int>(resourcesDictionary.Values);
        StartCoroutine(GetNewPersonInTavern(5));
    }

    IEnumerator GetNewPersonInTavern(float delay)
    {
        personBtn.SetActive(false);
        yield return new WaitForSeconds(delay);
        SetNewPerson();
    }

    public void SetNewPerson()
    {
        personBtn.SetActive(true);

        int rndPersonNum = Random.Range(0, persons.Count);
        string rndPersonName = persons[rndPersonNum];
        string rndPersonType = personsTypes[rndPersonNum];
        personName.text = $"{rndPersonName} ({rndPersonType})";

        int numOfSprite = -1;
        switch (rndPersonType)
        {
            case "Knight":
                numOfSprite = 2;
                break;
            case "Mage":
                numOfSprite = 0;
                break;
            case "Rogue":
                numOfSprite = 1;
                break;
        }
        personImg.sprite = personSprite[numOfSprite];
        personImg.color = mainColor;

        rndOrderNum = Random.Range(0, resources.Count);
        rndResourcesCountNum = Random.Range(1, 11);
        string rndResourceName = resources[rndOrderNum];
        personOrder.text = $"My order is: {rndResourcesCountNum} {rndResourceName}";
    }

    public void SetUserComplete(bool userDoing)
    {
        userDo = userDoing;
    }

    public void CheckOrderComplete()
    {
        if (userDo && CanComplete())
        {
            userDataController.ProgressController(true);
            dialogObj.SetActive(false);
            userDataController.UpdateAllText();
            personImg.color = tempColor;
            StartCoroutine(GetNewPersonInTavern(5));
        } else
        {
            userDataController.ProgressController(false);
            dialogObj.SetActive(false);
            personImg.color = tempColor;
            StartCoroutine(GetNewPersonInTavern(5));
        }  
    }

    private bool CanComplete()
    {
        switch (rndOrderNum)
        {
            case 0:
                if (userDataController.databaseManager.meat >= rndResourcesCountNum)
                {
                    userDataController.databaseManager.meat -= rndResourcesCountNum;
                    resourcesCount[rndOrderNum] -= rndResourcesCountNum;
                    return true;
                }
                break;
            case 1:
                if (userDataController.databaseManager.iron >= rndResourcesCountNum)
                {
                    userDataController.databaseManager.iron -= rndResourcesCountNum;
                    resourcesCount[rndOrderNum] -= rndResourcesCountNum;
                    return true;
                }
                break;
            case 2:
                if (userDataController.databaseManager.herbs >= rndResourcesCountNum)
                {
                    userDataController.databaseManager.herbs -= rndResourcesCountNum;
                    resourcesCount[rndOrderNum] -= rndResourcesCountNum;
                    return true;
                }
                break;
        }
        return false;
    }

}
