using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddEquipmentList : MonoBehaviour
{

    private List<Equipment> toAdd = new List<Equipment>();

    //Adds/Removes items from "ToAdd" list only
    //      ToAdd used at the end (on Confirm) to add entire list to container equipment
    public void OnToggle(bool val, Equipment equip)
    {
        //If true, then check if already present. If not, then add
        if (val)
        {
            for (int i = 0; i < toAdd.Count; i++)
            {
                if (toAdd[i].index == equip.index)
                {
                    //is not unique/already added, so break out of function
                    return;
                }
            }
            toAdd.Add(equip);
            return;
        }

        //if false, then check it exists in the list, then remove it if so
        for (int i = 0; i < toAdd.Count; i++)
        {
            if (toAdd[i].index == equip.index)
            {
                //is not unique/already added, so break out of function
                toAdd.RemoveAt(i);
            }
        }
    }

    public void OnSelected(GameObject selectedBtn)
    {
        if (transform.parent.GetComponent<ManageEquipment>().selectedButton != null)
        {
            transform.parent.GetComponent<ManageEquipment>().selectedButton.GetComponent<Button>().interactable = true;
        }

        transform.parent.GetComponent<ManageEquipment>().OnAddEquipmentSelected(selectedBtn);
        transform.parent.GetComponent<ManageEquipment>().selectedButton.GetComponent<Button>().interactable = false;
    }

    public void OnConfirmCancel(bool val)
    {
        transform.parent.GetComponent<ManageEquipment>().OnConfirmCancelAddEquip(val, new List<Equipment>(toAdd));
        toAdd.Clear();
    }
}
