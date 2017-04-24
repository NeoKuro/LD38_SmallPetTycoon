using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddCritterItem : MonoBehaviour
{
    public Critter critter;

    public void OnCreateItem(Critter creature)
    {
        critter = creature;
    }

    public void OnToggle()
    {
        bool val = transform.GetChild(10).GetComponent<Toggle>().isOn;
        transform.parent.parent.parent.parent.GetComponent<AddCritterList>().OnToggle(val, critter);
    }

    public void OnSelected()
    {
        transform.parent.parent.parent.parent.GetComponent<AddCritterList>().OnSelected(gameObject);
    }
}
