using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSelector : MonoBehaviour
{
    public Dictionary<int, Equipment> selectedEquipment = new Dictionary<int, Equipment>();
    public Dictionary<int, Equipment> newEquip = new Dictionary<int, Equipment>();

    private void OnEnable()
    {
        newEquip = new Dictionary<int, Equipment>(selectedEquipment);
    }

    public void OnToggleChange(Equipment equipment, bool value)
    {
        //value == false, Deselected so remove from list

        if(value && !newEquip.ContainsKey(equipment.index))
        {
            newEquip.Add(equipment.index, equipment);
        }
        else if (!value)
        {
            if(newEquip.ContainsKey(equipment.index))
            {
                newEquip.Remove(equipment.index);
            }
        }

        transform.GetChild(2).GetComponent<Button>().interactable = true;

        if(newEquip.Count <= 0)
        {
            transform.GetChild(2).GetComponent<Button>().interactable = false;
        }
    }

    public void ResetData()
    {
        selectedEquipment.Clear();
    }

    public void OnConfirm()
    {
        UpdateData();
        transform.parent.parent.GetComponent<SlotSetupController>().OnConfirmEquipmentSelection(selectedEquipment);
    }

    public void OnCancel()
    {
        newEquip.Clear();
        transform.parent.parent.GetComponent<SlotSetupController>().OnSetupContainerPressed();
    }

    private void UpdateData()
    {
        selectedEquipment = new Dictionary<int, Equipment>(newEquip);
    }
}
