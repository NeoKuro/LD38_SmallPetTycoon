using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentListItem : MonoBehaviour
{
    public Equipment equipment;

    public void SetupItem(Equipment thisContainer)
    {
        equipment = thisContainer;
    }

    public void AwakeCheck()
    {
        Dictionary<int, Equipment> eList = transform.parent.parent.parent.parent.GetComponent<EquipmentSelector>().selectedEquipment;

        if(eList.Count <= 0)
        {
            return;
        }

        foreach(KeyValuePair<int, Equipment> listing in eList)
        {
            if(listing.Value.index == equipment.index)
            {
                transform.GetChild(7).GetComponent<Toggle>().isOn = true;
            }
        }
    }

    public void RemoveEquipment()
    {
        transform.parent.parent.parent.parent.parent.parent.GetComponent<SlotSetupController>().OnRemoveEquipmentItem(equipment.index); 
    }

    public void OnToggleChange(Toggle tg)
    {
        transform.parent.parent.parent.parent.GetComponent<EquipmentSelector>().OnToggleChange(equipment, tg.isOn);
    }
}
