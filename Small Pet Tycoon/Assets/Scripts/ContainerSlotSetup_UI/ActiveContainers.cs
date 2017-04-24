using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveContainers : MonoBehaviour
{
    private int currIndex = 0;
    private List<Container> placedContainers = new List<Container>();

    private void OnEnable()
    {
        Setup();
    }

    public void OnCycleBtnPressed(bool isNext)
    {
        if (isNext)
        {
            currIndex++;
            if (currIndex >= placedContainers.Count)
            {
                currIndex = 0;
            }
        }
        else
        {
            currIndex--;
            if (currIndex < 0)
            {
                currIndex = placedContainers.Count - 1;
            }
        }
        UpdateStats();
    }

    public void OnEditContainer()
    {
        SlotSetupController controller = transform.parent.parent.GetComponent<SlotSetupController>();
        Dictionary<int, Equipment> equipment = new Dictionary<int, Equipment>();

        for (int i = 0; i < placedContainers[currIndex].equipmentList.Count; i++)
        {
            equipment.Add(placedContainers[currIndex].equipmentList[i].index, placedContainers[currIndex].equipmentList[i]);
        }

        if (!controller.containerSetup.ContainsKey(placedContainers[currIndex].index))
        {
            controller.containerSetup.Add(placedContainers[currIndex].index, new Dictionary<int, Equipment>());
        }

        controller.containerSetup[placedContainers[currIndex].index] = equipment;
        controller.OnSetupContainerPressed();
    }

    public void OnRemoveContainerPressed()
    {
        int tIndex = placedContainers[currIndex].index;
        placedContainers[currIndex].RemoveContainer(currIndex);
        placedContainers.RemoveAt(currIndex);
        transform.parent.parent.GetComponent<SlotSetupController>().ContainerRemoved(tIndex);
        if (placedContainers.Count <= 0)
        {
            gameObject.SetActive(false);
            return;
        }
        OnCycleBtnPressed(true);
        UpdateStats();
    }

    public void Setup()
    {
        placedContainers = transform.parent.parent.GetComponent<SlotSetupController>().selectedContainers;
        
        transform.GetChild(5).GetComponent<Button>().interactable = false;
        transform.GetChild(6).GetComponent<Button>().interactable = false;

        if (placedContainers.Count > 1)
        {
            //Enable left/right buttons
            transform.GetChild(5).GetComponent<Button>().interactable = true;
            transform.GetChild(6).GetComponent<Button>().interactable = true;
        }
        UpdateStats();
    }

    private void UpdateStats()
    {
        Container selected = placedContainers[currIndex];
        transform.GetChild(0).GetComponent<Text>().text = selected.objName;
        transform.GetChild(1).GetComponent<Text>().text = selected.size.ToString();
        transform.GetChild(2).GetComponent<Text>().text = selected.equipmentList.Count.ToString();
        transform.GetChild(3).GetComponent<Text>().text = selected.equipmentSlots.ToString();
        transform.GetChild(7).GetComponent<Text>().text = selected.subType.ToString();
        transform.GetChild(8).GetComponent<Text>().text = selected.critterList.Count.ToString();

    }
}
