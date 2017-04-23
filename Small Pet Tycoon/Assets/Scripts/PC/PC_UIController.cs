using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC_UIController : MonoBehaviour
{
    public GameObject startMenuObj;
    public GameObject screenObj;

    private bool startMenuShown = false;
    private ItemListController listControllerReference;

    public void StartPressed()
    {
        if (startMenuObj == null)
        {
            Debug.Log("Error: startMenu not found/Assigned");
            return;
        }

        StartCoroutine(SlideWindow(124, startMenuObj, 2.5f, -1.0f));
    }

    public void ShoppingPressed()
    {
        if (!ShowMainScreen())
        {
            return;
        }
        startMenuShown = false;
        screenObj.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void FinancePressed()
    {
        if (!ShowMainScreen())
        {
            return;
        }
        Debug.Log("Finance");
    }

    public void MarketingPressed()
    {
        if (!ShowMainScreen())
        {
            return;
        }
        Debug.Log("Marketing");
    }

    public void ContainersPressed()
    {
        if (listControllerReference == null)
        {
            listControllerReference = screenObj.AddComponent<ItemListController>();
        }
        screenObj.GetComponent<ItemListController>().InitialiseList(0);
    }

    public void EquipmentPressed()
    {
        if (listControllerReference == null)
        {
            listControllerReference = screenObj.AddComponent<ItemListController>();
        }
        screenObj.GetComponent<ItemListController>().InitialiseList(1);
    }

    public void CrittersPressed()
    {
        if (listControllerReference == null)
        {
            listControllerReference = screenObj.AddComponent<ItemListController>();
        }
        screenObj.GetComponent<ItemListController>().InitialiseList(2);
    }

    public void ShoppingBasketPressed()
    {
        if (listControllerReference == null)
        {
            listControllerReference = screenObj.AddComponent<ItemListController>();
            screenObj.GetComponent<ItemListController>().ResetList();
        }

        screenObj.GetComponent<ItemListController>().InitialiseList(3);
        screenObj.transform.GetChild(0).GetComponent<ShoppingBrowser>().LoadBasketItems();
    }

    public void CheckoutPressed()
    {
        if (listControllerReference == null)
        {
            listControllerReference = screenObj.AddComponent<ItemListController>();
            screenObj.GetComponent<ItemListController>().ResetList();
        }

        screenObj.GetComponent<ItemListController>().InitialiseList(4);
        screenObj.transform.GetChild(0).GetComponent<ShoppingBrowser>().Checkout();
    }

    public void CloseBtnPressed()
    {
        screenObj.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void LogoutBtn()
    {
        Destroy(gameObject);
        Debug.Log("Logged Out");
        GameManager.disableBGInput = false;
    }

    private IEnumerator SlideWindow(float deltaY, GameObject obj, float rate, float defHeight, bool slideUp = true)
    {
        float currentDelta = 0.0f;
        if (slideUp)
        {
            while (currentDelta < deltaY)
            {
                obj.transform.localPosition += Vector3.up * rate;
                currentDelta += 1.0f * rate;
                yield return new WaitForEndOfFrame();
            }

            if (currentDelta > deltaY)
            {
                Vector3 position = obj.transform.localPosition;
                position.y = defHeight;
                obj.transform.localPosition = position;
            }

            startMenuShown = true;
        }
        else
        {
            startMenuShown = false;
            currentDelta = obj.transform.localPosition.y;
            while (currentDelta > deltaY)
            {
                obj.transform.localPosition -= Vector3.up * rate;
                currentDelta -= 1.0f * rate;
                yield return new WaitForEndOfFrame();
            }

            if (currentDelta < deltaY)
            {
                Vector3 position = obj.transform.localPosition;
                position.y = defHeight;
                obj.transform.localPosition = position;
            }
        }
    }

    private bool ShowMainScreen()
    {
        if (!startMenuShown || screenObj == null)
        {
            Debug.Log("ScreenObject may not be assigned, or Start Menu has not finished coming into position");
            return false;
        }
        StartCoroutine(SlideWindow(-199.0f, startMenuObj, 2.5f, -200.0f, false));
        screenObj.SetActive(true);
        return true;
    }
}
