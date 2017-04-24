using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotSetupController : MonoBehaviour
{
    public int slotIndex = -999;
    public int slotSize = 0;
    public int maxContainers = 0;
    public int remainingSpace = 0;
    public Container currentContainer;
    public List<Container> selectedContainers = new List<Container>();
    public Dictionary<int, Dictionary<int, Equipment>> containerSetup = new Dictionary<int, Dictionary<int, Equipment>>();

    private int containerIndex = 0;
    private int containerUniqueIndex = 0;

    private GameObject containerInformation;
    private GameObject containerSelector;
    private GameObject equipmentSelector;
    private GameObject activeContainers;

    private List<GameObject> itemsList = new List<GameObject>();
    private Dictionary<int, GameObject> equipmentList = new Dictionary<int, GameObject>();  //Int = Equipment Index, GameObject = the List button
    //Int = Container Index, Dictionary<int, Equipment> = Int==Eq Index, Equipment

    public void SlotSetup(int index, int size, int maximum)
    {
        slotIndex = index;
        slotSize = size;
        maxContainers = maximum;
        remainingSpace = size;

        containerInformation = transform.GetChild(1).GetChild(1).gameObject;
        containerSelector = transform.GetChild(1).GetChild(2).gameObject;
        equipmentSelector = transform.GetChild(1).GetChild(3).gameObject;
        activeContainers = transform.GetChild(1).GetChild(4).gameObject;

        UpdateInfo();
    }

    public void OnContainerToggleChange()
    {
        IEnumerable<Toggle> active = transform.GetChild(1).GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetComponent<ToggleGroup>().ActiveToggles();
        Toggle toggle = null;

        foreach (Toggle tg in active)
        {
            toggle = tg;
            break;
        }

        if (toggle != null)
        {
            Color newCol = new Color(126.0f, 255.0f, 127.0f, 1.0f);
            containerSelector.transform.GetChild(2).GetComponent<Image>().color = newCol;
            containerSelector.transform.GetChild(2).GetComponent<Button>().interactable = true;
            currentContainer = toggle.gameObject.transform.parent.GetComponent<ContainerListItem>().container;
        }
        else
        {
            containerSelector.transform.GetChild(2).GetComponent<Image>().color = Color.white;
            containerSelector.transform.GetChild(2).GetComponent<Button>().interactable = false;
        }

    }

    public void OnSelectContainer()
    {
        ResetList();

        containerSelector.transform.GetChild(2).GetComponent<Button>().interactable = false;

        containerInformation.SetActive(false);
        containerSelector.SetActive(true);
        equipmentSelector.SetActive(false);

        GameObject containerListItem = Resources.Load("Prefabs/UI/ContainerSlotSetup/ContainerListItem") as GameObject;
        Dictionary<int, Container> containersList = GameManager.playerStorage.containerStorage;

        foreach (KeyValuePair<int, Container> container in containersList)
        {
            GameObject newItem = Instantiate(containerListItem);
            newItem.transform.GetChild(0).GetComponent<Text>().text = container.Value.objName;
            newItem.transform.GetChild(1).GetComponent<Text>().text = container.Value.size.ToString();
            newItem.transform.GetChild(2).GetComponent<Text>().text = container.Value.equipmentSlots.ToString();
            newItem.transform.SetParent(containerSelector.transform.GetChild(1).GetChild(0).GetChild(0).transform);
            newItem.GetComponent<ContainerListItem>().SetupItem(container.Value);

            itemsList.Add(newItem);
        }
    }

    public void OnSetupContainerPressed()
    {
        ResetList();

        containerInformation.SetActive(true);
        containerSelector.SetActive(false);
        equipmentSelector.SetActive(false);

        int remainingSlots = selectedContainers[containerIndex].equipmentSlots;

        containerInformation.transform.GetChild(0).GetComponent<Text>().text = selectedContainers[containerIndex].size.ToString();
        containerInformation.transform.GetChild(1).GetComponent<Text>().text = selectedContainers[containerIndex].equipmentSlots.ToString();

        GameObject equipmentListItem = Resources.Load("Prefabs/UI/ContainerSlotSetup/EquipItem") as GameObject;
        Dictionary<int, Equipment> equipmentStorageList = GameManager.playerStorage.equipmentStorage;

        if (!containerSetup.ContainsKey(containerUniqueIndex))
        {
            return;
        }

        foreach (KeyValuePair<int, Equipment> equipment in containerSetup[containerUniqueIndex])
        {
            GameObject newItem = Instantiate(equipmentListItem);
            newItem.transform.GetChild(0).GetComponent<Text>().text = equipment.Value.objName;

            GetTopTwoValues(newItem, equipment.Value);

            newItem.transform.SetParent(containerInformation.transform.GetChild(3).GetChild(0).GetChild(0).transform);
            newItem.GetComponent<EquipmentListItem>().SetupItem(equipment.Value);
            equipmentList.Add(equipment.Value.index, newItem);
            remainingSlots -= equipment.Value.size;
        }

        containerInformation.transform.GetChild(2).GetComponent<Text>().text = remainingSlots.ToString();
    }

    public void OnAddEquipmentPressed()
    {
        ResetList();

        containerInformation.SetActive(false);
        containerSelector.SetActive(false);
        equipmentSelector.SetActive(true);

        GameObject equipmentListItem = Resources.Load("Prefabs/UI/ContainerSlotSetup/EquipItemSelector") as GameObject;
        Dictionary<int, Equipment> equipmentStorageList = GameManager.playerStorage.equipmentStorage;

        foreach (KeyValuePair<int, Equipment> equipment in equipmentStorageList)
        {
            GameObject newItem = Instantiate(equipmentListItem);
            newItem.transform.GetChild(0).GetComponent<Text>().text = equipment.Value.objName;

            GetTopTwoValues(newItem, equipment.Value);


            newItem.transform.SetParent(equipmentSelector.transform.GetChild(1).GetChild(0).GetChild(0).transform);
            newItem.GetComponent<EquipmentListItem>().SetupItem(equipment.Value);
            newItem.GetComponent<EquipmentListItem>().AwakeCheck();

            equipmentList.Add(equipment.Value.index, newItem);
        }
    }

    public void OnConfirmContainer()
    {
        if (remainingSpace <= 0)
        {
            Debug.Log("Error: No room left. Try removing some containers first.");
            return;
        }

        for (int i = 0; i < selectedContainers.Count; i++)
        {
            if (selectedContainers[i].index == currentContainer.index)
            {
                Debug.Log("Error: Container already added!");
                return;
            }
        }

        if (remainingSpace >= currentContainer.size)
        {
            selectedContainers.Add(currentContainer);
            remainingSpace -= currentContainer.size;    //Reduce remaining space by the size of container
            transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<Text>().text = remainingSpace.ToString();
            containerIndex = selectedContainers.Count - 1;
            containerUniqueIndex = currentContainer.index;
            if (!containerSetup.ContainsKey(currentContainer.index))
            {
                containerSetup.Add(currentContainer.index, new Dictionary<int, Equipment>());   //On pickup, will remove from dictionary
            }
        }
        else
        {
            Debug.Log("Error: Container too big, not enough room!");
            return;
        }

        OnSetupContainerPressed();

    }

    public void OnConfirmEquipmentSelection(Dictionary<int, Equipment> selectedEquipment)
    {
        SUB_TYPE type = SUB_TYPE.UNDEF;
        foreach (KeyValuePair<int, Equipment> selected in selectedEquipment)
        {
            if (type == SUB_TYPE.UNDEF)
            {
                type = selected.Value.subType;
            }
            if (selected.Value.size > selectedContainers[containerIndex].size)
            {
                Debug.Log("Error: Incompatible equipment Size! Try a different one");
                return;
            }

            if (selected.Value.subType != type)
            {
                Debug.Log("Error: Incompatible equipment types! Subtypes must match. IE You cannot use Air Pumps in a Reptile habitat");
                return;
            }

        }

        if (selectedEquipment.Count > selectedContainers[containerIndex].equipmentSlots)
        {
            Debug.Log("Error: Too many equipment units! Try fewer units");
            return;
        }


        if (containerSetup.ContainsKey(containerUniqueIndex))
        {
            containerSetup[containerUniqueIndex] = selectedEquipment;
        }

        OnSetupContainerPressed();
    }

    public void OnConfirmContainerSetup()
    {
        //Add equipment to the Container Object (Items.cs)
        //      No need to check if enough slots etc. Done previously in OnConfirmEquipmentSelection() method
        // For each equipment, set the slot index
        int count = 0;
        foreach (KeyValuePair<int, Equipment> equipment in containerSetup[containerUniqueIndex])
        {
            bool alreadyAdded = false;
            for(int i = 0; i < selectedContainers[containerIndex].equipmentList.Count; i++)
            {
                if(selectedContainers[containerIndex].equipmentList[i].index == equipment.Value.index)
                {
                    alreadyAdded = true;
                }
            }

            if (!alreadyAdded)
            {
                equipment.Value.container = selectedContainers[containerIndex];
                selectedContainers[containerIndex].AddEquipment(equipment.Value, count);
                count++;
                Debug.Log("Slot: " + equipment.Value.slotIndex);
            }
        }

        //Place Container in Slot
        selectedContainers[containerIndex].PlaceContainer(slotIndex);

        //Only adds it if its 'new'...IE if press "EDIT" will not add again
        if (!GameManager.slots[slotIndex].containers.Contains(selectedContainers[containerIndex]))
        {
            //Add container to list in the ContainerSlot
            GameManager.slots[slotIndex].AddContainer(selectedContainers[containerIndex]);
            //Remove from storage
            GameManager.playerStorage.containerStorage.Remove(selectedContainers[containerIndex].index);
        }

        //Move Container to Slot location (containers will have gameobjects eventually)
        selectedContainers[containerIndex].thisGameObject.transform.SetParent(GameManager.slots[slotIndex].gameObject.transform);
        float scale = GameManager.slots[slotIndex].gameObject.GetComponent<BoxCollider>().size.x;
        float xPos = -(scale / 2) + (selectedContainers[containerIndex].thisGameObject.transform.localScale.x / 2);

        float previous = 0.0f;
        List<Container> cont = GameManager.slots[slotIndex].containers;
        for (int i = cont.Count - 2; i >= 0; i--)
        {
            previous += GetOffset(cont[i].size) + 0.5f;
        }

        //xPos += (-(scale / 2) + (selectedContainers[containerIndex].thisGameObject.transform.localScale.x / 2)) * GameManager.slots[slotIndex].containers.Count - 1;
        xPos += previous;
        Vector3 position = new Vector3(xPos, 0 + (selectedContainers[containerIndex].thisGameObject.transform.localScale.y / 2), 0);
        selectedContainers[containerIndex].thisGameObject.transform.localPosition = position;
        selectedContainers[containerIndex].thisGameObject.transform.localEulerAngles = Vector3.zero;
        Debug.Log("Done");

        //Add new window that shows present containers (along the top as a left/right button scroll?)

        if (activeContainers.activeInHierarchy)
        {
            activeContainers.GetComponent<ActiveContainers>().Setup();
        }

        //Close Window
        containerInformation.SetActive(false);
        containerSelector.SetActive(false);
        equipmentSelector.SetActive(false);
        activeContainers.SetActive(true);
        equipmentSelector.GetComponent<EquipmentSelector>().ResetData();
        ResetList();
        transform.GetChild(1).GetChild(0).GetChild(7).GetComponent<Button>().interactable = true;
    }

    public void OnConfirmSlotSetup()
    {
        GameManager.disableBGInput = false;
        ResetList();
        GameManager.slots[slotIndex].gameObject.GetComponent<Glow>().showOutline = false;
        selectedContainers.Clear();
        Destroy(gameObject);
    }

    public void OnRemoveEquipmentItem(int index)
    {
        if (equipmentList.ContainsKey(index))
        {
            Equipment thisEquip = containerSetup[containerUniqueIndex][index];
            GameManager.slots[slotIndex].containers[containerUniqueIndex].RemoveEquipment(thisEquip.slotIndex);
            Destroy(equipmentList[index]);
            equipmentList.Remove(index);
            containerSetup[containerUniqueIndex].Remove(index);
        }
        else
        {
            Debug.Log("ERROR: Does not exist?");
        }
    }

    public void ContainerRemoved(int tIndex)
    {
        for (int i = 0; i < selectedContainers.Count; i++)
        {
            if (selectedContainers[i].index == tIndex)
            {
                selectedContainers.RemoveAt(i);
            }
        }

        containerSetup.Remove(tIndex);

        UpdateInfo();
    }

    public void OnCancelEquipment()
    {
        remainingSpace += selectedContainers[selectedContainers.Count - 1].size;    //Reduce remaining space by the size of container
        transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<Text>().text = remainingSpace.ToString();
        containerIndex = selectedContainers.Count - 2;
        if (containerIndex < 0)
            containerIndex = 0;
        containerUniqueIndex = selectedContainers[containerIndex].index;
        if (containerSetup.ContainsKey(selectedContainers[selectedContainers.Count - 1].index))
        {
            containerSetup.Remove(selectedContainers[selectedContainers.Count - 1].index);   //On pickup, will remove from dictionary
        }
        selectedContainers.RemoveAt(selectedContainers.Count - 1);

        OnSelectContainer();
    }

    public void CloseBtn()
    {
        GameManager.disableBGInput = false;
        ResetList();
        for (int i = 0; i < GameManager.slots[slotIndex].containers.Count;)
        {
            GameManager.slots[slotIndex].containers[i].RemoveContainer(i);
            ContainerRemoved(i);
        }
        selectedContainers.Clear();
        Destroy(gameObject);
    }

    private void ResetList()
    {
        for (int i = 0; i < itemsList.Count; i++)
        {
            Destroy(itemsList[i]);
        }
        foreach (KeyValuePair<int, GameObject> equipment in equipmentList)
        {
            Destroy(equipment.Value);
        }
        itemsList.Clear();
        equipmentList.Clear();
    }

    private void UpdateInfo()
    {
        if (GameManager.slots.ContainsKey(slotIndex))
        {
            remainingSpace = GameManager.slots[slotIndex].remainingSpace;
            selectedContainers = new List<Container>(GameManager.slots[slotIndex].containers);
        }

        transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = slotSize.ToString();
        transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = maxContainers.ToString();
        transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<Text>().text = remainingSpace.ToString();

        if (selectedContainers.Count > 0)
        {
            activeContainers.SetActive(true);
        }
    }

    private void GetTopTwoValues(GameObject newItem, Equipment equipment)
    {
        float highestVal = 0.0f;
        string highestStr = "";
        float secondVal = 0.0f;
        string secondStr = "";

        switch (equipment.subType)
        {
            case SUB_TYPE.FISH:
                highestStr = "Air Pump";
                highestVal = equipment.airSupply;
                secondStr = "Water Pump";
                secondVal = equipment.filterRate;
                break;
            case SUB_TYPE.REPTILE:
                highestStr = "Heat Output";
                highestVal = equipment.heatSupply;
                secondStr = "Humidity";
                secondVal = equipment.humiditySup;
                break;
            case SUB_TYPE.INSECT:
                highestStr = "Heat Output";
                highestVal = equipment.heatSupply;
                secondStr = "Humidity";
                secondVal = equipment.humiditySup;
                break;
            case SUB_TYPE.ARACHNID:
                highestStr = "Heat Output";
                highestVal = equipment.heatSupply;
                secondStr = "Humidity";
                secondVal = equipment.humiditySup;
                break;
        }

        newItem.transform.GetChild(1).GetComponent<Text>().text = highestVal.ToString();
        newItem.transform.GetChild(2).GetComponent<Text>().text = highestStr;
        newItem.transform.GetChild(3).GetComponent<Text>().text = secondVal.ToString();
        newItem.transform.GetChild(4).GetComponent<Text>().text = secondStr;
    }

    private float GetOffset(int size)
    {
        switch (size)
        {
            case 1:
                return 1.0f;
            case 2:
                return 1.0f;
            case 3:
                return 2.0f;
            case 4:
                return 2.0f;
            case 5:
                return 3.0f;
            case 6:
                return 4.0f;
        }

        return 1.0f;
    }
}
