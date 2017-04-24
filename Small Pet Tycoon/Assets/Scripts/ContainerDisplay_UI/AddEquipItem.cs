using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddEquipItem : MonoBehaviour
{
    public Equipment equipment;

    public void OnCreateItem(Equipment equip)
    {
        equipment = equip;
    }

    public void OnToggle()
    {
        bool val = transform.GetChild(8).GetComponent<Toggle>().isOn;
        transform.parent.parent.parent.parent.GetComponent<AddEquipmentList>().OnToggle(val, equipment);
    }

    public void OnSelected()
    {
        transform.parent.parent.parent.parent.GetComponent<AddEquipmentList>().OnSelected(gameObject);
    }
}
