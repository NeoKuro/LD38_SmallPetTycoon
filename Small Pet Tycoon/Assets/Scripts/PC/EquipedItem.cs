using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipedItem : MonoBehaviour
{
    public Container container;

    public void SetItem(Container thisContainer)
    {
        container = thisContainer;
    }

    public void OnRemoveClicked()
    {
        //Remove this entry from the list and then delete it
    }
}
