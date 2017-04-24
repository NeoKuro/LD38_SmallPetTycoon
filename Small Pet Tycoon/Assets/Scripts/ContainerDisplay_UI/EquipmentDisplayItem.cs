using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDisplayItem : MonoBehaviour
{
    public int eIndex = -999;

    public void OnItemClicked(GameObject obj)
    {
        ManageEquipment manager = transform.parent.parent.parent.parent.parent.GetComponent<ManageEquipment>();

        if (manager.selectedButton != null)
        {
            manager.selectedButton.GetComponent<Button>().interactable = true;
        }

        manager.selectedButton = gameObject;
        GetComponent<Button>().interactable = false;
        manager.OnItemSelected(eIndex);
    }
}
