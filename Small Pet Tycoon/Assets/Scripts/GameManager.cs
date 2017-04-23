using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int indexPosition = 0;
    public static int secondsPassed = 0;
    public static float playerFunds = 2500.0f;
    public static Storage playerStorage = new Storage();
    public static bool disableBGInput = false;

    public float lastTime = 0.0f;
    public float period = 1.0f;


    public static int GetIndex()
    {
        indexPosition++;
        return indexPosition - 1;
    }

    public void Awake()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
    }

    public void Update()
    {
        if(lastTime >= period)
        {
            lastTime = 0.0f;
            secondsPassed++;
        }

        lastTime += 1 * Time.deltaTime;
    }
}

public class Storage
{
    public Dictionary<int, Equipment> equipmentStorage = new Dictionary<int, Equipment>();
    public Dictionary<int, Critter> critterStorage = new Dictionary<int, Critter>();
    public Dictionary<int, Container> containerStorage = new Dictionary<int, Container>();
    public float fishFoodStocks = 0.0f;
    public float reptileFoodStocks = 0.0f;

    public void AddEquipment(int index, Equipment equipment)
    {
        equipmentStorage.Add(index, equipment);
    }

    public void RemoveEquipment(int index)
    {
        equipmentStorage.Remove(index);
    }

    public void AddCritter(int index, Critter equipment)
    {
        critterStorage.Add(index, equipment);
    }

    public void RemoveCritter(int index)
    {
        critterStorage.Remove(index);
    }

    public void AddContainer(int index, Container container)
    {
        containerStorage.Add(index, container);
    }

    public void RemoveContainer(int index)
    {
        containerStorage.Remove(index);
    }
}