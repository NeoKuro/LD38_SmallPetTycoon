using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerListItem : MonoBehaviour
{
    public Container container;

    public void SetupItem(Container thisContainer)
    {
        container = thisContainer;
        transform.GetChild(3).GetComponent<Toggle>().group = transform.parent.GetComponent<ToggleGroup>();
    }

    public void ToggleChanged()
    {
        transform.parent.parent.parent.parent.parent.parent.GetComponent<SlotSetupController>().OnContainerToggleChange();
    }
}
