using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    public Items itemData;

    private void OnMouseDown()
    {
        if(itemData.type != DATA_TYPE.CONTAINER)
        {
            return;
        }

        Container data = (Container)itemData;
        GameObject instGO = Resources.Load("Prefabs/UI/ContainerDisplay") as GameObject;

        GameObject display = Instantiate(instGO);
        display.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
        display.transform.localPosition = Vector2.zero;
        display.GetComponent<ContainerDisplay>().SetupDisplay(data.containerSlot, data);
        GameManager.disableBGInput = true;
    }

    public void SetupItem(ItemListing item)
    {
        int index = GameManager.GetIndex();

        gameObject.name = item.name + "_" + index;

        switch (item.dataType)
        {
            case DATA_TYPE.CONTAINER:
                itemData = new Container(index, item.name, System.Convert.ToInt32(item.size), gameObject);
                GameManager.playerStorage.containerStorage.Add(index, (Container)itemData);
                break;
            case DATA_TYPE.EQUIPMENT:
                EquipmentListing eItem = (EquipmentListing)item;
                itemData = new Equipment(index, item.name, DATA_TYPE.EQUIPMENT, GetSubType(eItem.type),
                                                                                        System.Convert.ToInt32(eItem.size), eItem.powerCost,
                                                                                        eItem.heat, eItem.humidity, eItem.airPump, eItem.waterFilter);
                GameManager.playerStorage.equipmentStorage.Add(index, (Equipment)itemData);
                break;
            case DATA_TYPE.CRITTER:
                CritterListing cItem = (CritterListing)item;
                float randAge = Random.Range(300, 600);    //Max age between 5 minutes and 10 minutes
                bool shiny = RandBool(1);   //0.1% chance
                bool glow = RandBool(1);    //0.1% chance
                bool sparkle = RandBool(1); //0.1% chance
                itemData = new Critter(index, item.name, DATA_TYPE.CRITTER, GetSubType(cItem.type),
                                                                                    System.Convert.ToInt32(cItem.size), randAge, shiny, sparkle, glow);
                GameManager.playerStorage.critterStorage.Add(index, (Critter)itemData);
                StartCoroutine(itemData.RunCoroutine());
                break;
        }
    }

    //Chance to return true
    private bool RandBool(float chance)
    {
        int num = Random.Range(0, 1000);
        if (num >= 1000 - chance)
        {
            return true;
        }
        return false;
    }

    private SUB_TYPE GetSubType(string s)
    {
        switch(s.ToLower())
        {
            case "fish":
                return SUB_TYPE.FISH;
            case "reptile":
                return SUB_TYPE.REPTILE;
            case "insect":
                return SUB_TYPE.INSECT;
            case "arachnid":
                return SUB_TYPE.ARACHNID;
        }

        return SUB_TYPE.REPTILE;
    }
}
