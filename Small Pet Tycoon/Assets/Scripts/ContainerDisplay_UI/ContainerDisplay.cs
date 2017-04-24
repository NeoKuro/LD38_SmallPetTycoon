using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerDisplay : MonoBehaviour
{
    public int slotIndex;
    public Container container;

    private GameObject defaultDisplay;
    private GameObject manageEquipment;
    private GameObject manageCritters;

    private int counter = 0;
    private int lastSecond = 0;

    public void SetupDisplay(int index, Container thisContainer)
    {
        slotIndex = index;
        container = thisContainer;

        defaultDisplay = transform.GetChild(1).GetChild(0).gameObject;
        manageEquipment = transform.GetChild(1).GetChild(1).gameObject;
        manageCritters = transform.GetChild(1).GetChild(2).gameObject;

        UpdateData();
    }

    private void Update()
    {
        if(lastSecond != GameManager.secondsPassed)
        {
            counter++;
            lastSecond = GameManager.secondsPassed;
        }

        if(counter >= 10)
        {
            counter = 0;
            UpdateData();
        }        
    }

    private void UpdateData()
    {
        string name = container.customName;
        string type = container.subType.ToString();
        string size = container.size.ToString();
        string critterCount = container.critterList.Count.ToString();
        string maxCritters = container.maxCapacity.ToString();           //Size 1 = 5x Size 1 critters (children = size 0)
        string food = container.foodLevels.ToString();
        string cleanliness = container.cleanliness.ToString();

        //Logicy stuff here
        string children = "0";
        string happiness = "0";
        string temperature = "0";
        string humidity = "0";

        int tempChild = 0;
        float tempHappy = 0.0f;
        for (int i = 0; i < container.critterList.Count; i++)
        {
            if(container.critterList[i].size <= 1)
            {
                //if Size 0, then they are child
                tempChild++;
            }
            tempHappy += container.critterList[i].happiness;
        }

        children = tempChild.ToString();
        happiness = (tempHappy / container.critterList.Count).ToString();

        if(container.critterList.Count == 0)
        {
            happiness = 0.0f.ToString();
        }

        float tempTemperature = 21.0f;
        float heatOut = 0.0f;

        if (container.subType == SUB_TYPE.FISH)
        {
            tempTemperature = 15.0f;
        }

        for(int i = 0; i < container.equipmentList.Count; i++)
        {
            heatOut += container.equipmentList[i].heatSupply;
        }

        float heat = heatOut / Mathf.Pow(container.size, 2);
        tempTemperature += (heat + Random.Range(-3.0f, 3.0f));

        float tempHumidity = Random.Range(40.0f, 50.0f);
        float humid = 0.0f;

        if (container.subType == SUB_TYPE.FISH)
        {
            humid = 100.0f;
        }
        
        for (int i = 0; i < container.equipmentList.Count; i++)
        {
            humid += container.equipmentList[i].humiditySup;
        }

        float tHumid = humid / Mathf.Pow(container.size, 2);
        tempHumidity += tHumid;

        temperature = tempTemperature.ToString();
        humidity = tempHumidity.ToString();

        if (container.customName == "")
        {
            name = "Click to set name, then press enter";
        }

        defaultDisplay.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = name;
        defaultDisplay.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = type;
        defaultDisplay.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = size;
        defaultDisplay.transform.GetChild(0).GetChild(4).GetComponent<Text>().text = critterCount;
        defaultDisplay.transform.GetChild(0).GetChild(5).GetComponent<Text>().text = maxCritters;
        defaultDisplay.transform.GetChild(0).GetChild(6).GetComponent<Text>().text = food;
        defaultDisplay.transform.GetChild(0).GetChild(7).GetComponent<Text>().text = cleanliness;
        defaultDisplay.transform.GetChild(0).GetChild(8).GetComponent<Text>().text = children;
        defaultDisplay.transform.GetChild(0).GetChild(9).GetComponent<Text>().text = happiness;
        defaultDisplay.transform.GetChild(0).GetChild(10).GetComponent<Text>().text = temperature;
        defaultDisplay.transform.GetChild(0).GetChild(11).GetComponent<Text>().text = humidity;

    }
}
