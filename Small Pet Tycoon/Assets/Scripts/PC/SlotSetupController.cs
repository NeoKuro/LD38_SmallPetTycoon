using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotSetupController : MonoBehaviour
{
    public int slotSize = 0;
    public int maxContainers = 0;
    public int remainingSpace = 0;
    public Container currentContainer;
    public Container[] selectedContainers;

    private int containerIndex = 0;
    private int containerUniqueIndex = 0;

    private GameObject containerInformation;
    private GameObject containerSelector;
    private GameObject equipmentSelector;

    private List<GameObject> itemsList = new List<GameObject>();
    private Dictionary<int, GameObject> equipmentList = new Dictionary<int, GameObject>();  //Int = Equipment Index, GameObject = the List button
    private Dictionary<int, Dictionary<int, Equipment>> containerSetup = new Dictionary<int, Dictionary<int, Equipment>>();
                                                                                            //Int = Container Index, Dictionary<int, Equipment> = Int==Eq Index, Equipment

    public void SlotSetup(int size, int maximum)
    {
        slotSize = size;
        maxContainers = maximum;
        remainingSpace = maximum;
        selectedContainers = new Container[maxContainers];

        containerInformation = transform.GetChild(1).GetChild(1).gameObject;
        containerSelector = transform.GetChild(1).GetChild(2).gameObject;
        equipmentSelector = transform.GetChild(1).GetChild(3).gameObject;

        transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = slotSize.ToString();
        transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = maxContainers.ToString();
        transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<Text>().text = remainingSpace.ToString();
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

        containerInformation.SetActive(false);
        containerSelector.SetActive(true);
        equipmentSelector.SetActive(false);

        GameObject containerListItem = Resources.Load("Prefabs/UI/ContainerListItem") as GameObject;
        Dictionary<int, Container> containersList = GameManager.playerStorage.containerStorage;

        foreach (KeyValuePair<int, Container> container in containersList)
        {
            GameObject newItem = Instantiate(containerListItem);
            newItem.transform.GetChild(0).GetComponent<Text>().text = container.Value.name;
            newItem.transform.GetChild(1).GetComponent<Text>().text = container.Value.size.ToString();
            newItem.transform.GetChild(2).GetComponent<Text>().text = ((container.Value.size - 1) * 2).ToString();
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

        containerInformation.transform.GetChild(0).GetComponent<Text>().text = selectedContainers[containerIndex].size.ToString();
        containerInformation.transform.GetChild(1).GetComponent<Text>().text = selectedContainers[containerIndex].equipmentSlots.ToString();
        containerInformation.transform.GetChild(2).GetComponent<Text>().text = (selectedContainers[containerIndex].equipmentSlots - selectedContainers[containerIndex].usedSlots).ToString();
        
        GameObject equipmentListItem = Resources.Load("Prefabs/UI/EquipItem") as GameObject;
        Dictionary<int, Equipment> equipmentStorageList = GameManager.playerStorage.equipmentStorage;

        if(!containerSetup.ContainsKey(selectedContainers[containerIndex].index))
        {
            return;
        }

        foreach (KeyValuePair<int, Equipment> equipment in containerSetup[selectedContainers[containerIndex].index])
        {
            GameObject newItem = Instantiate(equipmentListItem);
            newItem.transform.GetChild(0).GetComponent<Text>().text = equipment.Value.name;

            GetTopTwoValues(newItem, equipment.Value);


            newItem.transform.SetParent(containerInformation.transform.GetChild(3).GetChild(0).GetChild(0).transform);
            newItem.GetComponent<EquipmentListItem>().SetupItem(equipment.Value);

            equipmentList.Add(equipment.Value.index, newItem);
        }
    }

    public void OnAddEquipmentPressed()
    {
        ResetList();

        containerInformation.SetActive(false);
        containerSelector.SetActive(false);
        equipmentSelector.SetActive(true);

        GameObject equipmentListItem = Resources.Load("Prefabs/UI/EquipItemSelector") as GameObject;
        Dictionary<int, Equipment> equipmentStorageList = GameManager.playerStorage.equipmentStorage;

        foreach (KeyValuePair<int, Equipment> equipment in equipmentStorageList)
        {
            GameObject newItem = Instantiate(equipmentListItem);
            newItem.transform.GetChild(0).GetComponent<Text>().text = equipment.Value.name;

            GetTopTwoValues(newItem, equipment.Value);


            newItem.transform.SetParent(equipmentSelector.transform.GetChild(1).GetChild(0).GetChild(0).transform);
            newItem.GetComponent<EquipmentListItem>().SetupItem(equipment.Value);
            newItem.GetComponent<EquipmentListItem>().AwakeCheck();

            equipmentList.Add(equipment.Value.index, newItem);
        }
    }

    public void OnConfirmContainer()
    {
        for (int i = 0; i < selectedContainers.Length; i++)
        {
            if (selectedContainers[i] == null)
            {
                if (remainingSpace >= currentContainer.size)
                {
                    selectedContainers[i] = currentContainer;
                    remainingSpace -= currentContainer.size;    //Reduce remaining space by the size of container
                    transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<Text>().text = remainingSpace.ToString();
                    transform.GetChild(1).GetChild(0).GetChild(7).GetComponent<Button>().interactable = true;
                    containerIndex = i;
                    containerUniqueIndex = currentContainer.index;
                    if (!containerSetup.ContainsKey(currentContainer.index))
                    {
                        containerSetup.Add(currentContainer.index, new Dictionary<int, Equipment>());   //On pickup, will remove from dictionary
                    }

                    break;
                }
                else
                {
                    Debug.Log("Error: Container too big, not enough room!");
                    break;  //Break out since if not enough room first time, there won't be enough room for any other time
                }
            }
            else
            {
                Debug.Log("Error: Slot in use. Try removing a container first");
            }
        }
    }

    public void OnConfirmEquipmentSelection(Dictionary<int, Equipment> selectedEquipment)
    {
        SUB_TYPE type = SUB_TYPE.UNDEF;
        foreach(KeyValuePair<int, Equipment> selected in selectedEquipment)
        {
            if(type == SUB_TYPE.UNDEF)
            {
                type = selected.Value.subType;
            }
            if(selected.Value.size > selectedContainers[containerIndex].size)
            {
                Debug.Log("Error: Incompatible equipment Size! Try a different one");
                return;
            }

            if(selected.Value.subType != type)
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


        if(containerSetup.ContainsKey(containerIndex))
        {
            containerSetup[containerIndex] = selectedEquipment;
        }

        OnSetupContainerPressed();
    }

    public void CloseBtn()
    {
        GameManager.disableBGInput = false;
        Destroy(gameObject);
    }

    private void ResetList()
    {
        for(int i = 0; i < itemsList.Count; i++)
        {
            Destroy(itemsList[i]);
        }
        foreach(KeyValuePair<int, GameObject> equipment in equipmentList)
        {
            Destroy(equipment.Value);
        }
        itemsList.Clear();
        equipmentList.Clear();
    }

    private void GetTopTwoValues(GameObject newItem, Equipment equipment)
    {
        float highestVal = 0.0f;
        string highestStr = "";
        float secondVal = 0.0f;
        string secondStr = "";

        switch(equipment.subType)
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
}
