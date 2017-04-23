using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotSetup : MonoBehaviour
{
    public int slotSize = 2;
    public int maxContainers = 1;

    private void OnMouseDown()
    {
        if(GameManager.disableBGInput)
        {
            return;
        }
        //Open Slot Setup Mode
        GameObject slotSetupObj = Resources.Load("Prefabs/UI/ContainerSlotSetup") as GameObject;
        

        GameObject screen = Instantiate(slotSetupObj);
        screen.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
        screen.transform.localPosition = Vector2.zero;
        screen.GetComponent<SlotSetupController>().SlotSetup(slotSize, maxContainers);
        GameManager.disableBGInput = true;
    }
}
