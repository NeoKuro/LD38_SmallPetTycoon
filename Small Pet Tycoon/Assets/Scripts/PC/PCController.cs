using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCController : MonoBehaviour
{

    private void OnMouseDown()
    {
        if (!GameManager.disableBGInput)
        {
            GameObject pcDesktop = Resources.Load("Prefabs/UI/PCPanel_Border") as GameObject;

            GameObject desktop = Instantiate(pcDesktop);
            desktop.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
            desktop.transform.localPosition = Vector2.zero;
            GameManager.disableBGInput = true;
            //Pause Game(?)
        }
    }

}
