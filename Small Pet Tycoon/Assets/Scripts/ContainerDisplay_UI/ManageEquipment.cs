using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageEquipment : MonoBehaviour
{
    public GameObject selectedButton;

    private int slotIndex;
    private int selectedEquipmentIndex = -999;
    private int lastSecond = 0;
    private int timerCount = 0;
    private bool displayInfo = false;

    private Container container;
    private Container originalContainer;
    private Equipment selectedEquipment;
    private List<GameObject> equipment = new List<GameObject>();
    private Storage originalStorage;

    private void Update()
    {
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
            LoadEquipmentInformation();
        }
    }

    public void OnManageEquipmentOpen(int index, Container c)
    {
        slotIndex = index;
        originalContainer = new Container(c);
        container = c;
        originalStorage = new Storage(GameManager.playerStorage);
        CheckSlotCount();
        LoadEquipment();
    }

    public void OnItemSelected(int equipIndex)
    {
        Dictionary<int, Equipment> containerEquipment = new Dictionary<int, Equipment>(container.equipmentList);
        foreach (KeyValuePair<int, Equipment> item in containerEquipment)
        {
            if (item.Value.index != equipIndex)
            {
                continue;
            }

            selectedEquipmentIndex = equipIndex;
            selectedEquipment = item.Value;

            LoadEquipmentInformation();
            if (equipment.Count > 1)
            {
                transform.GetChild(2).GetChild(2).GetComponent<Button>().interactable = true;
            }
            timerCount = 0;
            break;
        }

    }

    public void AddEquipment()
    {
        ToggleAddEquip(true);
        //Foreach equipment in inventory create list
        //Only show those valid for this Type
        GameObject equipListItem = Resources.Load("Prefabs/UI/ContainerDisplay/AddEquipItem") as GameObject;
        GameObject AddList = transform.GetChild(4).gameObject;
        foreach (KeyValuePair<int, Equipment> item in GameManager.playerStorage.equipmentStorage)
        {
            if (container.subType != SUB_TYPE.UNDEF)
            {
                if (item.Value.subType != container.subType)
                {
                    continue;
                }
            }

            GameObject itemListing = Instantiate(equipListItem);
            itemListing.transform.SetParent(AddList.transform.GetChild(0).GetChild(0).GetChild(0));
            itemListing.GetComponent<AddEquipItem>().equipment = item.Value;
            itemListing.transform.GetChild(0).GetComponent<Text>().text = item.Value.objName;
            if(item.Value.subType == SUB_TYPE.FISH)
            {
                itemListing.transform.GetChild(1).GetComponent<Text>().text = item.Value.filterRate.ToString();
                itemListing.transform.GetChild(2).GetComponent<Text>().text = "Filter Rate:";
                itemListing.transform.GetChild(3).GetComponent<Text>().text = item.Value.airSupply.ToString();
                itemListing.transform.GetChild(4).GetComponent<Text>().text = "Oxygenation Rate:";
            }
            else if (item.Value.subType == SUB_TYPE.REPTILE)
            {
                itemListing.transform.GetChild(1).GetComponent<Text>().text = item.Value.heatSupply.ToString();
                itemListing.transform.GetChild(2).GetComponent<Text>().text = "Heat Output:";
                itemListing.transform.GetChild(3).GetComponent<Text>().text = item.Value.humiditySup.ToString();
                itemListing.transform.GetChild(4).GetComponent<Text>().text = "Humidity Output:";
            }

            equipment.Add(itemListing);
        }

    }

    public void RemoveEquipment()
    {
        //Prevent removing all equipment...to do this, they must remove entire container
        if (equipment.Count > 1)
        {
            int oldIndex = selectedEquipment.slotIndex;
            //Reset slotIndex (not assigned anymore)
            selectedEquipment.slotIndex = -999;
            //Remove from container
            //Add equipment back to storage handled by the Container Method
            container.RemoveEquipment(oldIndex);
            equipment.Remove(selectedButton);
            Destroy(selectedButton);
            displayInfo = false;

            CheckSlotCount();
        }
    }

    public void OnAddEquipmentSelected(GameObject selectedBtn)
    {
        selectedButton = selectedBtn;
        selectedEquipment = selectedButton.GetComponent<AddEquipItem>().equipment;
        LoadEquipmentInformation();
    }

    public void OnConfirmCancelAddEquip(bool confirmed, List<Equipment> toAdd)
    {
        selectedButton = null;
        selectedEquipment = null;
        ToggleAddEquip(false);
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
                    Debug.Log("Error: Not same equipment types selected.");
                    return;
                }
            }
            //Check all entries unique (not already added)
            //Update the container equipment list
            for (int i = 0; i < toAdd.Count; i++)
            {
                if (container.equipmentList.ContainsKey(container.currSlot))
                {
                    continue;
                }

                toAdd[i].container = container;
                toAdd[i].slotIndex = container.currSlot;
                container.AddEquipment(toAdd[i], container.currSlot);
            }
        }

        CheckSlotCount();
        LoadEquipment();
        if (selectedButton == null)
        {
            transform.GetChild(2).GetChild(2).GetComponent<Button>().interactable = false;
        }
    }

    public void OnConfirmCancelChanges(bool confirmed)
    {
        if (!confirmed)
        {
            container = originalContainer;
            GameManager.playerStorage = originalStorage;
        }

        for (int i = 0; i < equipment.Count; i++)
        {
            Destroy(equipment[i]);
        }

        equipment.Clear();
        gameObject.SetActive(false);
        ContainerDisplay display = transform.parent.parent.GetComponent<ContainerDisplay>();
        display.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        display.SetupDisplay(display.slotIndex, container);
    }

    private void CheckSlotCount()
    {
        transform.GetChild(2).GetChild(1).GetComponent<Button>().interactable = false;
        if (container.equipmentSlots > container.usedSlots)
        {
            transform.GetChild(2).GetChild(1).GetComponent<Button>().interactable = true;
        }

        transform.GetChild(2).GetChild(2).GetComponent<Button>().interactable = false;
        if (equipment.Count > 1 && selectedButton != null)
        {
            transform.GetChild(2).GetChild(2).GetComponent<Button>().interactable = true;
        }
    }

    private void LoadEquipmentInformation()
    {
        transform.GetChild(1).GetChild(0).GetComponent<Text>().text = selectedEquipment.objName;
        transform.GetChild(1).GetChild(1).GetComponent<Text>().text = selectedEquipment.subType.ToString();
        if (selectedEquipment.container != null)
        {
            transform.GetChild(1).GetChild(2).GetComponent<Text>().text = selectedEquipment.container.equipmentSlots.ToString();
        }
        transform.GetChild(1).GetChild(3).GetComponent<Text>().text = selectedEquipment.size.ToString();
        transform.GetChild(1).GetChild(4).GetComponent<Text>().text = selectedEquipment.condition.ToString() + "%";
        transform.GetChild(1).GetChild(5).GetComponent<Text>().text = selectedEquipment.heatSupply.ToString();
        transform.GetChild(1).GetChild(6).GetComponent<Text>().text = selectedEquipment.humiditySup.ToString();
        transform.GetChild(1).GetChild(7).GetComponent<Text>().text = selectedEquipment.filterRate.ToString();
        transform.GetChild(1).GetChild(8).GetComponent<Text>().text = selectedEquipment.airSupply.ToString();
        transform.GetChild(1).GetChild(9).GetComponent<Text>().text = selectedEquipment.powerUsage.ToString() + "W";
    }

    private void LoadEquipment()
    {
        Dictionary<int, Equipment> containerEquipment = new Dictionary<int, Equipment>(container.equipmentList);
        GameObject listingObj = Resources.Load("Prefabs/UI/ContainerDisplay/EquipmentListing") as GameObject;

        foreach (KeyValuePair<int, Equipment> item in containerEquipment)
        {
            GameObject newItem = Instantiate(listingObj);
            newItem.transform.SetParent(transform.GetChild(3).GetChild(0).GetChild(0).GetChild(0));

            newItem.transform.GetChild(0).GetComponent<Text>().text = item.Value.objName;
            newItem.transform.GetChild(1).GetComponent<Text>().text = item.Value.size.ToString();
            newItem.transform.GetChild(2).GetComponent<Text>().text = item.Value.heatSupply.ToString();
            newItem.transform.GetChild(3).GetComponent<Text>().text = item.Value.humiditySup.ToString();
            newItem.transform.GetChild(4).GetComponent<Text>().text = item.Value.condition.ToString() + "%";
            newItem.transform.GetChild(5).GetComponent<Text>().text = item.Value.filterRate.ToString();
            newItem.transform.GetChild(6).GetComponent<Text>().text = item.Value.airSupply.ToString();

            newItem.GetComponent<EquipmentDisplayItem>().eIndex = item.Value.index;
            equipment.Add(newItem);
        }
    }

    private void ToggleAddEquip(bool showAddEquip)
    {
        //Clear the List
        for (int i = 0; i < equipment.Count; i++)
        {
            Destroy(equipment[i]);
        }
        equipment.Clear();

        //Hide the ScrollView section
        transform.GetChild(3).gameObject.SetActive(!showAddEquip);
        //Show "AddEquipment" section
        transform.GetChild(4).gameObject.SetActive(showAddEquip);
        //Disable Remove/Add/Confirm/Cancel buttons
        transform.GetChild(2).GetChild(1).GetComponent<Button>().interactable = !showAddEquip;
        transform.GetChild(2).GetChild(2).GetComponent<Button>().interactable = !showAddEquip;
        transform.GetChild(2).GetChild(3).GetComponent<Button>().interactable = !showAddEquip;
        transform.GetChild(2).GetChild(4).GetComponent<Button>().interactable = !showAddEquip;
        //  AddEquipment section containts "Confirm" and "Cancel" buttons
        //  Uses Toggle boxes like before

        //OnCancel discard changes

        //Clear List
        //Hide AddEquipment section
        //Show ScrollView section
        //Enable buttons

        //CheckSlotCount();
    }
}
