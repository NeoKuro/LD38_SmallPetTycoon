using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddCritterList : MonoBehaviour
{
    private List<Critter> toAdd = new List<Critter>();

    public void OnToggle(bool val, Critter critter)
    {
        //If true, then check if already present. If not, then add
        if (val)
        {
            for (int i = 0; i < toAdd.Count; i++)
            {
                if (toAdd[i].index == critter.index)
                {
                    //is not unique/already added, so break out of function
                    return;
                }
            }
            toAdd.Add(critter);
            return;
        }

        //if false, then check it exists in the list, then remove it if so
        for (int i = 0; i < toAdd.Count; i++)
        {
            if (toAdd[i].index == critter.index)
            {
                //is not unique/already added, so break out of function
                toAdd.RemoveAt(i);
            }
        }
    }

    public void OnSelected(GameObject selectedBtn)
    {
        if (transform.parent.GetComponent<ManageCritters>().selectedButton != null)
        {
            transform.parent.GetComponent<ManageCritters>().selectedButton.GetComponent<Button>().interactable = true;
        }

        transform.parent.GetComponent<ManageCritters>().OnAddCritterSelected(selectedBtn);
        transform.parent.GetComponent<ManageCritters>().selectedButton.GetComponent<Button>().interactable = false;
    }

    public void OnConfirmCancel(bool val)
    {
        transform.parent.GetComponent<ManageCritters>().OnConfirmCancelAddCritter(val, new List<Critter>(toAdd));
        toAdd.Clear();
    }
}
