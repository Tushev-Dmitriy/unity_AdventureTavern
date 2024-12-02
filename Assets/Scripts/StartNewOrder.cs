using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartNewOrder : MonoBehaviour
{
    public GameObject newPerson;
    public Sprite[] personSprite;
    public TMP_Text personName;
    public TMP_Text personOrder;

    private Image personImg;
    private Color tempColor = new Color(255, 255, 255, 0);
    private Color mainColor = new Color(255, 255, 255, 255);
    private List<string> persons = new List<string>();
    private List<string> personsTypes = new List<string>();

    private void Awake()
    {
        personImg = newPerson.transform.GetChild(0).GetComponent<Image>();
        personImg.color = tempColor;
    }

    public void SetNewPerson(Dictionary<string, string> adventurersDictionary)
    {
        persons = new List<string>(adventurersDictionary.Keys);
        personsTypes = new List<string>(adventurersDictionary.Values);

        int rndPersonNum = Random.Range(0, persons.Count);
        string rndPersonName = persons[rndPersonNum];
        string rndPersonType = personsTypes[rndPersonNum];

        personName.text = $"{rndPersonName} ({rndPersonType})";
    }
}
