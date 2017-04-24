using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ManageCritters : MonoBehaviour
{
    public GameObject selectedButton;
    public Critter selectedCritter;

    private int slotIndex;
    private int selectedCritterIndex = -999;
    private int lastSecond = 0;
    private int timerCount = 0;
    private bool displayInfo = false;
    private bool changePrice = false;

    private GameObject critterInfoObj;
    private GameObject critterOptionsObj;
    private GameObject critterList;
    private GameObject addCritterList;

    private Container container;
    private Container originalContainer;
    private List<GameObject> crittersObj = new List<GameObject>();
    private Dictionary<int, float> originalPrice = new Dictionary<int, float>();
    private Storage originalStorage;

    private void Update()
    {
        if (changePrice)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (!originalPrice.ContainsKey(selectedCritter.index))
                {
                    originalPrice.Add(selectedCritter.index, selectedCritter.price);
                }
                selectedCritter.price = float.Parse(critterOptionsObj.transform.GetChild(6).GetComponent<InputField>().text);                
                changePrice = false;
                critterOptionsObj.transform.GetChild(6).gameObject.SetActive(false);
                selectedButton.transform.GetChild(5).GetComponent<Text>().text = "$" + selectedCritter.price.ToString();
            }
        }


        if (!displayInfo)
        {
            return;
        }

        if (lastSecond != GameManager.secondsPassed)
        {
            lastSecond = GameManager.secondsPassed;
            timerCount++;
        }

        if (timerCount >= 5)
        {
            timerCount = 0;
            LoadCritterInformation();
        }
    }

    public void OnManageCritterOpen(int index, Container c)
    {
        slotIndex = index;
        originalContainer = new Container(c);
        container = c;
        originalStorage = new Storage(GameManager.playerStorage);

        critterInfoObj = transform.GetChild(1).gameObject;
        critterOptionsObj = transform.GetChild(2).gameObject;
        critterList = transform.GetChild(3).gameObject;
        addCritterList = transform.GetChild(4).gameObject;

        CheckContainerCapacity();
        LoadCritters();
    }

    public void AddCritters()
    {
        ToggleAddCritter(true);
        //Foreach equipment in inventory create list
        //Only show those valid for this Type
        GameObject critterListItem = Resources.Load("Prefabs/UI/ContainerDisplay/AddCritterListing") as GameObject;
        foreach (KeyValuePair<int, Critter> item in GameManager.playerStorage.critterStorage)
        {
            if (container.subType != SUB_TYPE.UNDEF)
            {
                if (item.Value.subType != container.subType)
                {
                    continue;
                }
            }

            GameObject itemListing = Instantiate(critterListItem);
            itemListing.transform.SetParent(addCritterList.transform.GetChild(0).GetChild(0).GetChild(0));
            itemListing.GetComponent<AddCritterItem>().critter = item.Value;
            itemListing.transform.GetChild(0).GetComponent<Text>().text = item.Value.customName;
            itemListing.transform.GetChild(1).GetComponent<Text>().text = item.Value.objName;
            itemListing.transform.GetChild(2).GetComponent<Text>().text = item.Value.age.ToString();
            itemListing.transform.GetChild(3).GetComponent<Text>().text = GetSexChar(item.Value.sex);
            itemListing.transform.GetChild(4).GetComponent<Text>().text = item.Value.size.ToString();

            crittersObj.Add(itemListing);
        }

    }

    public void RemoveCritter()
    {
        if (selectedCritter == null)
        {
            return;
        }

        //Remove from container
        //Add equipment back to storage handled by the Container Method
        container.RemoveCritter(selectedCritter);
        crittersObj.Remove(selectedButton);
        Destroy(selectedButton);
        displayInfo = false;
        selectedCritter = null;
        selectedButton = null;

        CheckContainerCapacity();

    }

    public void PriceChange()
    {
        changePrice = true;
        critterOptionsObj.transform.GetChild(6).gameObject.SetActive(true);
        critterOptionsObj.transform.GetChild(6).GetComponent<InputField>().text = selectedCritter.price.ToString();
        EventSystem.current.SetSelectedGameObject(critterOptionsObj.transform.GetChild(6).gameObject, null);
    }

    public void OnItemSelected(int critIndex)
    {
        List<Critter> containerCritters = new List<Critter>(container.critterList);
        for (int i = 0; i < containerCritters.Count; i++)
        {
            if (containerCritters[i].index != critIndex)
            {
                continue;
            }

            selectedCritterIndex = critIndex;
            selectedCritter = containerCritters[i];
            displayInfo = true;

            LoadCritterInformation();
            if (crittersObj.Count > 0)
            {
                critterOptionsObj.transform.GetChild(2).GetComponent<Button>().interactable = true;
            }
            critterOptionsObj.transform.GetChild(3).GetComponent<Button>().interactable = true;
            timerCount = 0;
            break;
        }
    }

    public void OnAddCritterSelected(GameObject selectedBtn)
    {
        selectedButton = selectedBtn;
        selectedCritter = selectedButton.GetComponent<AddCritterItem>().critter;
        LoadCritterInformation();
    }

    public void OnConfirmCancelAddCritter(bool confirmed, List<Critter> toAdd)
    {
        selectedButton = null;
        selectedCritter = null;
        ToggleAddCritter(false);
        //Only confirm does this
        if (confirmed)
        {
            SUB_TYPE tempType = SUB_TYPE.UNDEF;
            for (int i = 0; i < toAdd.Count; i++)
            {
                if (tempType == SUB_TYPE.UNDEF)
                {
                    tempType = toAdd[i].subType;
                    continue;
                }

                if (tempType != toAdd[i].subType)
                {
                    Debug.Log("Error: Not same Critter types selected.");
                    return;
                }
            }
            //Check all entries unique (not already added)
            //Update the container equipment list
            for (int i = 0; i < toAdd.Count; i++)
            {
                if (container.equipmentList.ContainsKey(container.equipUniqueIndex))
                {
                    continue;
                }

                toAdd[i].container = container;
                container.AddCritter(toAdd[i]);
            }
        }

        CheckContainerCapacity();
        LoadCritters();
        if (selectedButton == null)
        {
            critterOptionsObj.transform.GetChild(2).GetComponent<Button>().interactable = false;
        }

    }

    public void OnConfirmCancelChanges(bool confirmed)
    {
        if (!confirmed)
        {
            container = originalContainer;
            GameManager.playerStorage = originalStorage;
            foreach (KeyValuePair<int, float> oldPrices in originalPrice)
            {
                if (GameManager.playerStorage.critterStorage.ContainsKey(oldPrices.Key))
                {
                    GameManager.playerStorage.critterStorage[oldPrices.Key].price = oldPrices.Value;
                }
                else
                {
                    for(int i = 0; i < container.critterList.Count; i++)
                    {
                        if(container.critterList[i].index == oldPrices.Key)
                        {
                            container.critterList[i].price = oldPrices.Value;
                        }
                    }
                }
            }
        }

        for (int i = 0; i < crittersObj.Count; i++)
        {
            Destroy(crittersObj[i]);
        }

        displayInfo = false;
        crittersObj.Clear();
        originalPrice.Clear();
        gameObject.SetActive(false);
        ContainerDisplay display = transform.parent.parent.GetComponent<ContainerDisplay>();
        display.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        display.SetupDisplay(display.slotIndex, container);
    }

    private void CheckContainerCapacity()
    {
        //Turn on Add Critter
        //Turn off Remove Critter
        //Turn off Set Price
        critterOptionsObj.transform.GetChild(1).GetComponent<Button>().interactable = true;
        critterOptionsObj.transform.GetChild(2).GetComponent<Button>().interactable = false;
        critterOptionsObj.transform.GetChild(3).GetComponent<Button>().interactable = false;
        if (container.critterList.Count >= container.maxCapacity)
        {
            //Turn off "Add Critter"
            critterOptionsObj.transform.GetChild(1).GetComponent<Button>().interactable = false;
        }

        if (selectedButton != null)
        {
            //Turn on Remove Critter
            //Turn on Set Price
            critterOptionsObj.transform.GetChild(2).GetComponent<Button>().interactable = true;
            critterOptionsObj.transform.GetChild(3).GetComponent<Button>().interactable = true;
        }
    }

    private void LoadCritters()
    {
        List<Critter> containerCritters = new List<Critter>(container.critterList);
        GameObject listingObj = Resources.Load("Prefabs/UI/ContainerDisplay/CritterListing") as GameObject;

        for (int i = 0; i < containerCritters.Count; i++)
        {
            GameObject newItem = Instantiate(listingObj);
            newItem.transform.SetParent(critterList.transform.GetChild(0).GetChild(0).GetChild(0));

            newItem.transform.GetChild(0).GetComponent<Text>().text = containerCritters[i].customName;
            newItem.transform.GetChild(1).GetComponent<Text>().text = containerCritters[i].objName;
            newItem.transform.GetChild(2).GetComponent<Text>().text = containerCritters[i].age.ToString();
            newItem.transform.GetChild(3).GetComponent<Text>().text = GetSexChar(containerCritters[i].sex);
            newItem.transform.GetChild(4).GetComponent<Text>().text = containerCritters[i].size.ToString();
            newItem.transform.GetChild(5).GetComponent<Text>().text = "$" + containerCritters[i].price.ToString();
            newItem.transform.GetChild(6).GetComponent<Text>().text = containerCritters[i].children.ToString();

            newItem.GetComponent<CritterDisplayItem>().cIndex = containerCritters[i].index;
            crittersObj.Add(newItem);
        }
    }

    private void LoadCritterInformation()
    {
        critterInfoObj.transform.GetChild(0).GetComponent<Text>().text = selectedCritter.customName;
        critterInfoObj.transform.GetChild(2).GetComponent<Text>().text = selectedCritter.subType.ToString();
        critterInfoObj.transform.GetChild(3).GetComponent<Text>().text = selectedCritter.objName;
        critterInfoObj.transform.GetChild(4).GetComponent<Text>().text = selectedCritter.size.ToString();
        critterInfoObj.transform.GetChild(5).GetComponent<Text>().text = selectedCritter.age.ToString();
        critterInfoObj.transform.GetChild(6).GetComponent<Text>().text = selectedCritter.sex;
        critterInfoObj.transform.GetChild(7).GetComponent<Text>().text = selectedCritter.hunger.ToString();
        critterInfoObj.transform.GetChild(8).GetComponent<Text>().text = selectedCritter.hygiene.ToString();
        critterInfoObj.transform.GetChild(9).GetComponent<Text>().text = selectedCritter.happiness.ToString();
        critterInfoObj.transform.GetChild(10).GetComponent<Text>().text = selectedCritter.children.ToString();
    }

    private void ToggleAddCritter(bool showAddCritter)
    {
        //Clear the List
        for (int i = 0; i < crittersObj.Count; i++)
        {
            Destroy(crittersObj[i]);
        }
        crittersObj.Clear();

        //Hide the ScrollView section
        critterList.SetActive(!showAddCritter);
        //Show "AddCritters" section
        addCritterList.SetActive(showAddCritter);
        //Toggle Remove/Add/SetPrice/Confirm/Cancel buttons
        critterOptionsObj.transform.GetChild(1).GetComponent<Button>().interactable = !showAddCritter;
        critterOptionsObj.transform.GetChild(2).GetComponent<Button>().interactable = !showAddCritter;
        critterOptionsObj.transform.GetChild(3).GetComponent<Button>().interactable = !showAddCritter;
        critterOptionsObj.transform.GetChild(4).GetComponent<Button>().interactable = !showAddCritter;
        critterOptionsObj.transform.GetChild(5).GetComponent<Button>().interactable = !showAddCritter;
    }

    private string GetSexChar(string sex)
    {
        switch (sex.ToLower())
        {
            case "male":
                return "M";
            case "female":
                return "F";
        }
        return "?";
    }
}
