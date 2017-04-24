using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerSlot : MonoBehaviour
{
    public int index = -999;   //Which slot in the game are they(?)
    public int slotSize = 2;
    public int maxContainers = 1;
    public int remainingSpace = 2;
    public int remainingContainer = 1;

    public List<Container> containers = new List<Container>();

    private void Start()
    {
        remainingContainer = slotSize;
        remainingSpace = slotSize;
        GameManager.slots.Add(index, this);
    }

    private void OnMouseDown()
    {
        if(GetComponent<Glow>().showOutline == false)
        {
            return;
        }

        if(GameManager.disableBGInput)
        {
            return;
        }
        //Open Slot Setup Mode
        GameObject slotSetupObj = Resources.Load("Prefabs/UI/ContainerSlotSetup") as GameObject;
        

        GameObject screen = Instantiate(slotSetupObj);
        screen.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
        screen.transform.localPosition = Vector2.zero;
        screen.GetComponent<SlotSetupController>().SlotSetup(index,slotSize, maxContainers);
        GameManager.disableBGInput = true;
    }

    public void AddContainer(Container c)
    {
        containers.Add(c);
        remainingContainer--;
        remainingSpace -= c.size;
    }

    public void UpdateContainer(int index, Container c)
    {
        containers[index] = c;      //Index of the containers held in this slot will not be the same as the index of the container
                                    //  Because if one is being replaced (IE index 1 being replaced with a new container) etc
    }

    public void RemoveContainer(int targetIndex)
    {
        GameManager.playerStorage.containerStorage.Add(containers[targetIndex].index, containers[targetIndex]);     //Add container back to storage
        containers[targetIndex].thisGameObject.transform.position = new Vector3(0, -1000.0f, 0);                    //Move off screen   -- Allows degredation to continue
        remainingSpace += containers[targetIndex].size;                                                          //Remove the container's contents (reset it)
        remainingContainer++;
        containers.RemoveAt(targetIndex);
    }
}
