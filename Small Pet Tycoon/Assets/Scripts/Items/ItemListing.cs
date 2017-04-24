using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DATA_TYPE
{
    CONTAINER,
    EQUIPMENT,
    CRITTER
}

public class ItemListing
{
    public string name;
    public string price;
    public float priceVal;
    public string size;
    public DATA_TYPE dataType;


}

public class ContainerListing : ItemListing
{

    public ContainerListing(string n, string s, string p, float val, DATA_TYPE dType)
    {
        name = n;
        size = s;
        price = p;
        priceVal = val;
        dataType = dType;
    }
}

public class EquipmentListing : ItemListing
{
    public float powerCost;
    public string description;
    public string type;
    public float heat;
    public float humidity;
    public float airPump;
    public float waterFilter;

    public EquipmentListing(string n, string s, string p, float val, float pwr, string desc, string t, DATA_TYPE dType, float h, float hum, float air, float water)
    {
        name = n;
        size = s;
        price = p;
        priceVal = val;
        powerCost = pwr;
        description = desc;
        type = t;
        dataType = dType;
        heat = h;
        humidity = hum;
        airPump = air;
        waterFilter = water;
    }
}

public class CritterListing : ItemListing
{
    public string breed = "";
    public string gender = "Male";
    public string type;
    public string description;

    public CritterListing(string n, string s, string p, float val, string t, string desc, DATA_TYPE dType)
    {
        name = n;
        size = s;
        price = p;
        priceVal = val;
        type = t;
        description = desc;
        dataType = dType;
    }

    public CritterListing(CritterListing c)
    {
        name = c.name;
        size = c.size;
        price = c.price;
        priceVal = c.priceVal;
        dataType = c.dataType;

        breed = c.breed;
        gender = c.gender;
        type = c.type;
        description = c.description;
    }
}