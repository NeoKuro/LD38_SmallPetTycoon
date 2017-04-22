using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShoppingItemBtn : MonoBehaviour
{
    public ItemListing itemData;
    public bool isCritters = false;
    public bool maleSelected = true;

    public void AddToCartPressed()
    {
        if (itemData.name == null || itemData.name == "")
        {
            Debug.Log("Error: ItemData not set: " + gameObject.name);
            return;
        }

        if(isCritters)
        {
            CritterListing cListing = (CritterListing)itemData;
            cListing.gender = "Female";
            if (maleSelected)
                cListing.gender = "Male";

            itemData = cListing;
        }

        transform.parent.parent.parent.parent.parent.GetComponent<ShoppingBrowser>().AddToCart(itemData);
    }

    public void CheckBoxChanged(Toggle toggleBtn)
    {
        maleSelected = toggleBtn.isOn;
        Debug.Log(maleSelected);
    }

    public void AddQuanity()
    {
        transform.parent.parent.parent.parent.parent.GetComponent<ShoppingBrowser>().Increase(itemData.name);
    }

    public void RemoveQuantity()
    {
        transform.parent.parent.parent.parent.parent.GetComponent<ShoppingBrowser>().Decrease(itemData.name);
    }
}
