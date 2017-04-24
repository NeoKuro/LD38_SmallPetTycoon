using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainmenuController : MonoBehaviour
{
    public List<GameObject> tutorials = new List<GameObject>();

    private int currIndex = -1;

    public void Update()
    {
        if(Input.anyKeyDown)
        {
            currIndex++;
            if(currIndex == 5)
            {
                //Switch Scenes
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
            }
            if(currIndex > 0 && currIndex < 5)
                tutorials[currIndex - 1].SetActive(false);
            if(currIndex < 5)
                tutorials[currIndex].SetActive(true);
        }
    }
}
