using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShoppingBrowser : MonoBehaviour
{
    public float totalCost = 0.0f;

    public List<GameObject> basketList = new List<GameObject>();
    public List<ItemListing> items = new List<ItemListing>();
    public List<ItemListing> itemsInCart = new List<ItemListing>();
    public Dictionary<string, int> cartContents = new Dictionary<string, int>();

    public void Initialise(List<ItemListing> itemList)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if (items.Contains(itemList[i]))
            {
                continue;
            }
            items.Add(itemList[i]);
        }
    }

    public void ResetBrowser()
    {
        for (int i = 0; i < basketList.Count; i++)
        {
            Destroy(basketList[i]);
        }

        basketList.Clear();
    }

    public void LoadBasketItems()
    {
        foreach (KeyValuePair<string, int> item in cartContents)
        {
            GameObject newItem = Instantiate(Resources.Load("Prefabs/UI/Desktop/BasketObj")) as GameObject;
            string key = item.Key;
            ItemListing thisItem = GetItem(key);
            newItem.transform.GetChild(0).GetComponent<Text>().text = thisItem.name;
            newItem.transform.GetChild(1).GetComponent<Text>().text = thisItem.size;
            newItem.transform.GetChild(2).GetComponent<Text>().text = thisItem.price;
            newItem.transform.GetChild(3).GetComponent<Text>().text = item.Value.ToString();

            newItem.transform.SetParent(transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0));
            newItem.name = key + "_BacketItem";
            newItem.GetComponent<ShoppingItemBtn>().itemData = thisItem;
            basketList.Add(newItem);
        }
    }

    public void Checkout()
    {
        float totalCost = 0.0f;
        int totalObjCount = 0;
        int containerCount = 0;
        int equipmentCount = 0;
        int crittersCount = 0;

        foreach (KeyValuePair<string, int> item in cartContents)
        {
            ItemListing thisItem = GetItem(item.Key);
            float thisCost = thisItem.priceVal * item.Value;
            switch (thisItem.dataType)
            {
                case DATA_TYPE.CONTAINER:
                    containerCount++;
                    break;
                case DATA_TYPE.EQUIPMENT:
                    equipmentCount++;
                    break;
                case DATA_TYPE.CRITTER:
                    crittersCount++;
                    break;
            }
            totalObjCount += item.Value;
            totalCost += thisCost;
        }

        GameObject checkoutObj = transform.GetChild(3).gameObject;

        checkoutObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = totalObjCount.ToString();
        checkoutObj.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = containerCount.ToString();
        checkoutObj.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = equipmentCount.ToString();
        checkoutObj.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = crittersCount.ToString();
        checkoutObj.transform.GetChild(0).GetChild(4).GetComponent<Text>().text = "$" + totalCost.ToString();

        checkoutObj.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "$" + GameManager.playerFunds.ToString();
    }

    public void AddToCart(ItemListing item)
    {
        if (!itemsInCart.Contains(item))
        {
            itemsInCart.Add(item);
        }
        if (!cartContents.ContainsKey(item.name))
        {
            cartContents.Add(item.name, 1);
        }
        else
        {
            cartContents[item.name] += 1;
        }

        totalCost += item.priceVal;

        Debug.Log("totalCost: $" + totalCost);
    }

    public void Decrease(string name)
    {
        if (!cartContents.ContainsKey(name))
        {
            Debug.Log("Cart does not contain anymore of: " + name);
            return;
        }

        float count = 0.0f;

        count = cartContents[name] - 1;
        cartContents[name] -= 1;

        ItemListing item = GetItem(name);

        if (count == 0)
        {
            for (int i = 0; i < basketList.Count; i++)
            {
                if (basketList[i].GetComponent<ShoppingItemBtn>().itemData.name == name)
                {
                    Destroy(basketList[i]);
                    basketList.RemoveAt(i);
                }
            }
            cartContents.Remove(name);
            itemsInCart.Remove(item);
        }
        //Delete Entry in cart too
        totalCost -= item.priceVal;

        for (int i = 0; i < basketList.Count; i++)
        {
            if (basketList[i].GetComponent<ShoppingItemBtn>().itemData.name == item.name)
            {
                basketList[i].transform.GetChild(3).GetComponent<Text>().text = cartContents[item.name].ToString();
            }
        }

        Debug.Log("TotalCost: $" + totalCost);
    }

    public void Increase(string name)
    {
        if (!cartContents.ContainsKey(name))
        {
            Debug.Log("Cart does not contain anymore of: " + name);
            return;
        }

        cartContents[name] += 1;
        ItemListing item = GetItem(name);

        for (int i = 0; i < basketList.Count; i++)
        {
            if (basketList[i].GetComponent<ShoppingItemBtn>().itemData.name == item.name)
            {
                basketList[i].transform.GetChild(3).GetComponent<Text>().text = cartContents[item.name].ToString();
            }
        }

        //Delete Entry in cart too
        totalCost += (GetItem(name).priceVal);
        Debug.Log("TotalCost: $" + totalCost);
    }

    public void ConfirmPressed()
    {
        if (GameManager.playerFunds >= totalCost)
        {
            foreach (KeyValuePair<string, int> cartItem in cartContents)
            {
                for (int i = 0; i < cartItem.Value; i++)
                {
                    ItemListing item = GetItem(cartItem.Key);
                    if (item.dataType == DATA_TYPE.CONTAINER)
                    {
                        GameObject thisObj = Instantiate(Resources.Load("Prefabs/Containers/Container_Size" + item.size) as GameObject);
                        thisObj.transform.position = new Vector3(0, -1000.0f, 0);
                        thisObj.AddComponent<ItemHandler>();
                        thisObj.GetComponent<ItemHandler>().SetupItem(item);
                    }
                    else
                    {

                        GameObject thisObj = new GameObject("newItem");
                        thisObj.AddComponent<ItemHandler>();
                        thisObj.GetComponent<ItemHandler>().SetupItem(item);
                    }
                }
            }
            GameManager.playerFunds -= totalCost;
            totalCost = 0;
            transform.GetChild(3).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
            transform.parent.gameObject.SetActive(false);
            cartContents.Clear();
            itemsInCart.Clear();
            basketList.Clear();
            items.Clear();
        }
        else
        {
            Debug.Log("Insufficient Funds");
        }
    }

    private ItemListing GetItem(string name)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].name == name)
            {
                return items[i];
            }
        }
        return null;
    }
}
