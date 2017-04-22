using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCController : MonoBehaviour
{
    public bool desktopOpen = false;

    private void OnMouseDown()
    {
        if (!desktopOpen)
        {
            GameObject pcDesktop = Resources.Load("Prefabs/UI/PCPanel_Border") as GameObject;

            GameObject desktop = Instantiate(pcDesktop);
            desktop.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
            desktop.transform.localPosition = Vector2.zero;
            desktopOpen = true; //Remember to close it on "Logout"
                                //Pause Game(?)
        }
    }

}
