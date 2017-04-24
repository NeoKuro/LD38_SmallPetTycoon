using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CritterDisplayItem : MonoBehaviour
{
    public int cIndex = -999;

    public void OnItemClicked()
    {
        ManageCritters manager = transform.parent.parent.parent.parent.parent.GetComponent<ManageCritters>();

        if (manager.selectedButton != null)
        {
            manager.selectedButton.GetComponent<Button>().interactable = true;
        }

        manager.selectedButton = gameObject;
        GetComponent<Button>().interactable = false;
        manager.OnItemSelected(cIndex);
    }
}
