using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static int indexPosition = 0;
    public static int secondsPassed = 0;
    public static float playerFunds = 1000.0f;
    public static Storage playerStorage = new Storage();
    public static Dictionary<int, ContainerSlot> slots = new Dictionary<int, ContainerSlot>();      //Int = Slot Index
    public static bool disableBGInput = false;

    public float lastTime = 0.0f;
    public float period = 1.0f;


    public static int GetIndex()
    {
        indexPosition++;
        return indexPosition;
    }

    public void Awake()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        //DontDestroyOnLoad(gameObject);
    }

    public void Update()
    {
        if(lastTime >= period)
        {
            lastTime = 0.0f;
            secondsPassed++;
        }

        lastTime += 1 * Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            indexPosition = 0;
            secondsPassed = 0;
            playerFunds = 1000.0f;
            foreach(KeyValuePair<int, ContainerSlot> slot in slots)
            {
                for(int i = 0; i < slot.Value.containers.Count; i++)
                {
                    slot.Value.containers[i].RemoveAllItems();
                    Destroy(slot.Value.containers[i].thisGameObject);
                }
            }
            slots.Clear();

            foreach (KeyValuePair<int, Container> cont in playerStorage.containerStorage)
            {
                Destroy(cont.Value.thisGameObject);
            }
            foreach (KeyValuePair<int, Equipment> cont in playerStorage.equipmentStorage)
            {
                Destroy(cont.Value.thisGameObject);
            }
            foreach (KeyValuePair<int, Critter> cont in playerStorage.critterStorage)
            {
                Destroy(cont.Value.thisGameObject);
            }
            foreach (Critter cont in playerStorage.allCritters)
            {
                Destroy(cont.thisGameObject);
            }
            playerStorage.containerStorage.Clear();
            playerStorage.equipmentStorage.Clear();
            playerStorage.critterStorage.Clear();
            playerStorage.allCritters.Clear();
            playerStorage = new Storage();

            disableBGInput = false;

            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

    }
}

public class Storage
{
    public Dictionary<int, Equipment> equipmentStorage = new Dictionary<int, Equipment>();
    public Dictionary<int, Critter> critterStorage = new Dictionary<int, Critter>();
    public Dictionary<int, Container> containerStorage = new Dictionary<int, Container>();
    public List<Critter> allCritters = new List<Critter>();
    public float fishFoodStocks = 0.0f;
    public float reptileFoodStocks = 0.0f;

    public Storage()
    {

    }

    public Storage(Storage s)
    {
        equipmentStorage = new Dictionary<int, Equipment>(s.equipmentStorage);
        critterStorage = new Dictionary<int, Critter>(s.critterStorage);
        containerStorage = new Dictionary<int, Container>(s.containerStorage);
        allCritters = new List<Critter>(s.allCritters);
        fishFoodStocks = s.fishFoodStocks;
        reptileFoodStocks = s.reptileFoodStocks;
    }

    public void AddEquipment(int index, Equipment equipment)
    {
        equipmentStorage.Add(index, equipment);
    }

    public void RemoveEquipment(int index)
    {
        equipmentStorage.Remove(index);
    }

    public void AddCritter(int index, Critter critter)
    {
        critterStorage.Add(index, critter);
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