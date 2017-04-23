using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSelector : MonoBehaviour
{
    public Dictionary<int, Equipment> selectedEquipment = new Dictionary<int, Equipment>();
    
    public void OnToggleChange(Equipment equipment, bool value)
    {
        //value == false, Deselected so remove from list
        if(value && !selectedEquipment.ContainsKey(equipment.index))
        {
            selectedEquipment.Add(equipment.index, equipment);
        }
        else if (!value)
        {
            if(selectedEquipment.ContainsKey(equipment.index))
            {
                selectedEquipment.Remove(equipment.index);
            }
        }

        transform.GetChild(2).GetComponent<Button>().interactable = true;

        if(selectedEquipment.Count <= 0)
        {
            transform.GetChild(2).GetComponent<Button>().interactable = false;
        }
    }

    public void OnConfirm()
    {
        transform.parent.parent.GetComponent<SlotSetupController>().OnConfirmEquipmentSelection(selectedEquipment);
    }

    public void OnCancel()
    {

    }
}
