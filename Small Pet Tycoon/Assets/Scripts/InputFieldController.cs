using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputFieldController : MonoBehaviour
{
    private string customName = "";
    private bool changeName = false;

    public void Clicked()
    {
        if(changeName)
        {
            return;
        }

        changeName = true;
        transform.parent.GetChild(1).gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(transform.parent.GetChild(1).gameObject, null);
    }
    
    private void Update()
    {
        if(changeName)
        {
            if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                customName = transform.parent.GetChild(1).GetComponent<InputField>().text;
                GetComponent<Text>().text = customName;
                transform.parent.GetChild(1).gameObject.SetActive(false);
                changeName = false;
                if (transform.parent.parent.name.ToLower().Contains("default"))
                {
                    transform.parent.parent.parent.parent.GetComponent<ContainerDisplay>().container.customName = customName;
                }
                else if (transform.parent.parent.name.ToLower().Contains("critters"))
                {
                    transform.parent.parent.GetComponent<ManageCritters>().selectedCritter.customName= customName;
                }
            }
        }
    }
}
