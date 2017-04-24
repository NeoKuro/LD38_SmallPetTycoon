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
            CritterListing cListing = new CritterListing((CritterListing)itemData);
            cListing.gender = "";
            if (maleSelected)
            { 
                cListing.gender = "Male";
            }
            else
            {
                cListing.gender = "Female";
            }

            cListing.name = cListing.breed + "_" + cListing.gender;

            itemData = cListing;
        }
        transform.parent.parent.parent.parent.parent.GetComponent<ShoppingBrowser>().AddToCart(itemData);
    }

    public void CheckBoxChanged(Toggle toggleBtn)
    {
        maleSelected = toggleBtn.isOn;
    }

    public void AddQuanity()
    {
        transform.parent.parent.parent.parent.parent.GetComponent<ShoppingBrowser>().Increase(itemData);
    }

    public void RemoveQuantity()
    {
        transform.parent.parent.parent.parent.parent.GetComponent<ShoppingBrowser>().Decrease(itemData);
    }
}
