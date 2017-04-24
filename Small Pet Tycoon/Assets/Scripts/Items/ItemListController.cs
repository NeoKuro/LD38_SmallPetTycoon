using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class ItemListController : MonoBehaviour
{
    public GameObject itemListingObj;
    public List<GameObject> itemObjects = new List<GameObject>();
    public List<ItemListing> items = new List<ItemListing>();

    private TextAsset textFile = null;

    public void InitialiseList(int listType)
    {
        ResetList();
        itemListingObj = transform.GetChild(0).GetChild(2).gameObject;
        transform.GetChild(0).GetChild(3).gameObject.SetActive(false);

        itemListingObj.SetActive(true);
        switch (listType)
        {
            case 0:
                itemListingObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<GridLayoutGroup>().constraintCount = 2;
                itemListingObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<GridLayoutGroup>().cellSize = new Vector2(270, 100);
                ReadContainerTxtFile("Data/ContainersList");
                LoadContainerItems();
                break;
            case 1:
                itemListingObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<GridLayoutGroup>().constraintCount = 1;
                itemListingObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<GridLayoutGroup>().cellSize = new Vector2(550, 100);
                ReadEquipmentTxtFile("Data/EquipmentList");
                LoadEquipmentItems();
                break;
            case 2:
                itemListingObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<GridLayoutGroup>().constraintCount = 1;
                itemListingObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<GridLayoutGroup>().cellSize = new Vector2(550, 100);
                ReadCritterTxtFile("Data/CrittersList");
                LoadCritterItems();
                break;
            case 3:
                itemListingObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<GridLayoutGroup>().constraintCount = 2;
                itemListingObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<GridLayoutGroup>().cellSize = new Vector2(270, 100);
                transform.GetChild(0).GetComponent<ShoppingBrowser>().ResetBrowser();
                break;
            case 4:
                transform.GetChild(0).GetComponent<ShoppingBrowser>().ResetBrowser();
                transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
                itemListingObj.SetActive(false);
                break;
        }
    }

    public void ResetList()
    {
        for (int i = 0; i < itemObjects.Count; i++)
        {
            Destroy(itemObjects[i]);
        }

        items.Clear();
        itemObjects.Clear();
    }

    private void ReadContainerTxtFile(string file)
    {
        textFile = Resources.Load(file) as TextAsset;
        string text = textFile.text;
        string[] textLines = GetLines(text);

        for (int i = 0; i < textLines.Length; i++)
        {
            string[] lineEntries = Regex.Split(textLines[i], ",");
            ContainerListing item = new ContainerListing(lineEntries[0], lineEntries[1], lineEntries[2], System.Convert.ToInt32(lineEntries[2]), DATA_TYPE.CONTAINER);
            items.Add(item);
        }
    }

    private void ReadEquipmentTxtFile(string file)
    {
        textFile = Resources.Load(file) as TextAsset;
        string text = textFile.text;
        string[] textLines = GetLines(text);

        for (int i = 0; i < textLines.Length; i++)
        {
            string[] lineEntries = Regex.Split(textLines[i], ",");
            EquipmentListing item = new EquipmentListing(lineEntries[0], lineEntries[1], lineEntries[2], float.Parse(lineEntries[2]),
                                                        float.Parse(lineEntries[3]), lineEntries[4], lineEntries[5], DATA_TYPE.EQUIPMENT, 
                                                        float.Parse(lineEntries[6]), float.Parse(lineEntries[7]), float.Parse(lineEntries[8]), float.Parse(lineEntries[9]));
            items.Add(item);
        }
    }

    private void ReadCritterTxtFile(string file)
    {
        textFile = Resources.Load(file) as TextAsset;
        string text = textFile.text;
        string[] textLines = GetLines(text);

        for (int i = 0; i < textLines.Length; i++)
        {
            string[] lineEntries = Regex.Split(textLines[i], ",");
            CritterListing item = new CritterListing(lineEntries[0], lineEntries[1], lineEntries[2], System.Convert.ToInt32(lineEntries[2]), lineEntries[3], lineEntries[4], DATA_TYPE.CRITTER);
            item.breed = item.name;
            items.Add(item);
        }
    }

    private void LoadContainerItems()
    {
        GameObject containerItem = Resources.Load("Prefabs/UI/Desktop/ContainersObj") as GameObject;
        List<ContainerListing> cListing = new List<ContainerListing>();
        for (int i = 0; i < items.Count; i++)
        {
            cListing.Add((ContainerListing)items[i]);
        }

        for (int i = 0; i < items.Count; i++)
        {
            GameObject newitem = Instantiate(containerItem);
            newitem.transform.GetChild(0).GetComponent<Text>().text = cListing[i].name;
            newitem.transform.GetChild(1).GetComponent<Text>().text = cListing[i].size;
            newitem.transform.GetChild(2).GetComponent<Text>().text = cListing[i].price;
            //newitem.transform.GetChild(3).GetComponent<Text>().text = cListing[i].img;   //URL to the image (name of image in an 'Img' directory in resources?)

            newitem.transform.SetParent(itemListingObj.transform.GetChild(0).GetChild(0).GetChild(0));
            newitem.name = items[i].name + "_ItemListing";
            newitem.GetComponent<ShoppingItemBtn>().itemData = cListing[i];
            itemObjects.Add(newitem);
        }

        transform.GetChild(0).GetComponent<ShoppingBrowser>().Initialise(items);
    }

    private void LoadEquipmentItems()
    {
        GameObject equipmentItem = Resources.Load("Prefabs/UI/Desktop/EquipmentObj") as GameObject;

        if (equipmentItem == null)
        {
            Debug.Log("Error: equipmentObj not found");
            return;
        }

        List<EquipmentListing> eListing = new List<EquipmentListing>();
        for (int i = 0; i < items.Count; i++)
        {
            eListing.Add((EquipmentListing)items[i]);
        }

        for (int i = 0; i < items.Count; i++)
        {
            GameObject newitem = Instantiate(equipmentItem);
            newitem.transform.GetChild(0).GetComponent<Text>().text = eListing[i].name;
            newitem.transform.GetChild(1).GetComponent<Text>().text = eListing[i].size;
            newitem.transform.GetChild(2).GetComponent<Text>().text = eListing[i].price;
            newitem.transform.GetChild(3).GetComponent<Text>().text = eListing[i].powerCost.ToString() + "W";
            newitem.transform.GetChild(4).GetComponent<Text>().text = eListing[i].type;
            newitem.transform.GetChild(5).GetComponent<Text>().text = eListing[i].description;
            //newitem.transform.GetChild(3).GetComponent<Text>().text = cListing[i].img;   //URL to the image (name of image in an 'Img' directory in resources?)

            newitem.transform.SetParent(itemListingObj.transform.GetChild(0).GetChild(0).GetChild(0));
            newitem.name = items[i].name + "_ItemListing";
            newitem.GetComponent<ShoppingItemBtn>().itemData = eListing[i];
            itemObjects.Add(newitem);
        }

        transform.GetChild(0).GetComponent<ShoppingBrowser>().Initialise(items);

    }

    private void LoadCritterItems()
    {
        GameObject critterItem = Resources.Load("Prefabs/UI/Desktop/CritterObj") as GameObject;

        if (critterItem == null)
        {
            Debug.Log("Error: equipmentObj not found");
            return;
        }

        List<CritterListing> crListing = new List<CritterListing>();
        for (int i = 0; i < items.Count; i++)
        {
            crListing.Add((CritterListing)items[i]);
        }

        for (int i = 0; i < items.Count; i++)
        {
            GameObject newitem = Instantiate(critterItem);
            newitem.transform.GetChild(0).GetComponent<Text>().text = crListing[i].name;
            newitem.transform.GetChild(1).GetComponent<Text>().text = crListing[i].size;
            newitem.transform.GetChild(2).GetComponent<Text>().text = crListing[i].price;
            newitem.transform.GetChild(3).GetComponent<Text>().text = crListing[i].type;
            newitem.transform.GetChild(4).GetComponent<Text>().text = crListing[i].description;
            //newitem.transform.GetChild(3).GetComponent<Text>().text = cListing[i].img;   //URL to the image (name of image in an 'Img' directory in resources?)

            newitem.transform.SetParent(itemListingObj.transform.GetChild(0).GetChild(0).GetChild(0));
            newitem.name = items[i].name + "_ItemListing";
            newitem.GetComponent<ShoppingItemBtn>().itemData = crListing[i];
            newitem.GetComponent<ShoppingItemBtn>().isCritters = true;
            itemObjects.Add(newitem);
        }

        transform.GetChild(0).GetComponent<ShoppingBrowser>().Initialise(items);

    }


    private string[] GetLines(string text)
    {

        string[] tLines = Regex.Split(text, "\n|\r|\r\n");
        List<string> lines = new List<string>();
        foreach (string line in tLines)
        {
            if (line != "")
            {
                lines.Add(line);
            }
        }
        string[] textLines = new string[lines.Count];

        for (int i = 0; i < lines.Count; i++)
        {
            textLines[i] = lines[i];
        }

        return textLines;
    }
}
