using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SUB_TYPE
{
    FISH,
    REPTILE,
    INSECT,
    ARACHNID,
    UNDEF
}

public class Items : MonoBehaviour
{
    public int index = -999;    //Unique index. Whenever a new critter, or item is born/bought, a global value raises
    public string objName = "";
    public DATA_TYPE type;    //Container, Equipment, Critter
    public SUB_TYPE subType; //Fish, Reptile etc (Containers wil use this to IE when fill tank with water etc)
    public int size = 0;

    public Items(int i, string n, DATA_TYPE t, SUB_TYPE sType, int s)
    {
        index = i;
        objName = n;
        type = t;
        subType = sType;
        size = s;
    }

    //Container constructor
    public Items(int i, string n, int s)
    {
        index = i;
        objName = n;
        size = s;
        type = DATA_TYPE.CONTAINER;
        subType = SUB_TYPE.UNDEF;
    }

    public virtual IEnumerator RunCoroutine()
    {
        yield return null;
    }
}

public class Container : Items
{
    public string customName = "";
    public int containerSlot = -1;  //Which container slot is this placed in in the room?
    public int maxCapacity = 0; //How many critters can be fit in here?
    public int currCapacity = 0;
    public int equipmentSlots = 0;  //How much room for equipment is there?
    public int usedSlots = 0;
    public int equipUniqueIndex = 0;            //Each time equip added, this will increase. Never decreases (ensures unique slot ID all the time)
    public float cleanliness = 0.0f;    //How clean (0-100) is the container?
    public float foodLevels = 0.0f;     //How much food is left in the container? 0 - 10
    public bool placed = false;
    public GameObject thisGameObject;
    public Dictionary<int, Equipment> equipmentList = new Dictionary<int, Equipment>();
    public List<Critter> critterList = new List<Critter>();

    public Container(int i, string n, int s, GameObject go) : base(i, n, s)
    {
        maxCapacity = size * 5;                 //Size 1 = 5x size 1 critters
        equipmentSlots = (size - 1) * 2;        // size 1 = 1, 2 = 2, 3 = 4, 4 = 6, 5 = 8
        if (equipmentSlots <= 0)
            equipmentSlots = 1;                 //Ensure there is at minimum 1 slot
        cleanliness = 50.0f;
        foodLevels = 2.5f;
        thisGameObject = go;
    }

    //Make a new copy
    public Container(Container c) : base(c.index, c.objName, c.size)
    {
        type = c.type;
        subType = c.subType;
        customName = new string(c.customName.ToCharArray());
        containerSlot = c.containerSlot;
        maxCapacity = c.maxCapacity;
        currCapacity = c.currCapacity;
        equipmentSlots = c.equipmentSlots;
        usedSlots = c.usedSlots;
        equipUniqueIndex = c.equipUniqueIndex;
        cleanliness = c.cleanliness;
        foodLevels = c.foodLevels;
        placed = c.placed;
        thisGameObject = c.thisGameObject;
        equipmentList = new Dictionary<int, Equipment>(c.equipmentList);
        critterList = new List<Critter>(c.critterList);
    }

    public void SetCustomName(string newName)
    {
        customName = newName;
    }

    public void FeedCritters(float toAdd)
    {
        foodLevels += toAdd;
        if(foodLevels > 10.0f)
        {
            foodLevels = 10.0f;
        }
    }

    //Returns 'freeze' time
    public float CleanContainer()
    {
        cleanliness = 100.0f;
        float freeze = size * (5 + size);
        return freeze;
    }

    public void AddEquipment(Equipment equipment, int slot)
    {
        // equipment.slotIndex = slot;
        subType = equipment.subType;
        equipment.container = this;
        equipment.slotIndex = slot;
        equipmentList.Add(slot, equipment);
        usedSlots++;
        equipUniqueIndex++;
        GameManager.playerStorage.equipmentStorage.Remove(equipment.index);
    }

    public void RemoveEquipment(int slot)
    {
        GameManager.playerStorage.equipmentStorage.Add(equipmentList[slot].index, equipmentList[slot]);
        equipmentList[slot].slotIndex = -999;
        equipmentList[slot].container = null;
        equipmentList.Remove(slot);
        usedSlots--;
    }

    public void AddCritter(Critter critter)
    {
        critterList.Add(critter);
        GameManager.playerStorage.critterStorage.Remove(critter.index);
        critter.container = this;
        currCapacity++;
    }

    public void RemoveCritter(Critter critter)
    {
        critterList.Remove(critter);
        critter.container = null;
        GameManager.playerStorage.critterStorage.Add(critter.index, critter);
        currCapacity--;
    }

    public void PlaceContainer(int slot)
    {
        containerSlot = slot;
        placed = true;
    }

    public void RemoveContainer(int slotIndex)
    {
        placed = false;
        thisGameObject.transform.SetParent(null);
        GameManager.slots[containerSlot].RemoveContainer(slotIndex);
        RemoveAllItems();
    }

    public void RemoveAllItems()
    {
        for (int i = equipmentList.Count - 1; i >= 0; i--)
        {
            RemoveEquipment(i);
        }

        for (int i = critterList.Count - 1; i >= 0; i--)
        {
            RemoveCritter(critterList[i]);
        }

        critterList.Clear();
        equipmentList.Clear();
    }

    public override IEnumerator RunCoroutine()
    {
        while (placed)
        {
            cleanliness -= (critterList.Count * 0.5f) + (foodLevels * 0.25f);  //Will reduce cleanliness by 0.5 * critters in container every second.
                                                                                //Also food levels. So keeping completely full food stores means get dirtier faster
            yield return new WaitForSeconds(1.0f);
        }
    }
}

public class Equipment : Items
{
    public int slotIndex = -1;      //Which equipment slot is this equipment item in?
    public float heatSupply = 0.0f; //How much heat is supplied by this equipment
    public float humiditySup = 0.0f;//How much humidity is supplied by this equipment
    public float powerUsage = 0.0f; //How much power is used each minute?
    public float condition = 10.0f;  //State of repair (0-10, at 0 irreparable, can repair but gets more expensive with age + degradation?)
    public float maxCondition = 10.0f;  //Max state of repair (0-10, each repair reduces it)
    public int age = 0;              //Age of the item since bought (in seconds)
    public float airSupply = 0.0f;  //Air supplied to water tank 
    public float filterRate = 0.0f; //Amount of water processed each minute
    public Container container;
    public GameObject thisGameObject;

    private int lastSecond = 0;

    public Equipment(int i, string n, DATA_TYPE t, SUB_TYPE sType, int s, float power, float heat, float humid, float air, float filter, GameObject go) : base(i, n, t, sType, s)
    {
        heatSupply = heat;
        humiditySup = humid;
        powerUsage = power;
        airSupply = air;
        filterRate = filter;
        thisGameObject = go;
    }

    public override IEnumerator RunCoroutine()
    {
        while (condition > 0.0f)
        {
            age++;
            yield return new WaitForSeconds(1.0f);
        }

        DestroyEquipment();
    }

    public float Repair()
    {
        float deltaR = maxCondition - condition;
        condition = maxCondition;
        maxCondition -= deltaR * 0.5f;

        if (condition <= 0)
        {
            DestroyEquipment();
        }

        return deltaR;
    }

    private void DestroyEquipment()
    {
        if (container != null)
        {
            container.equipmentList.Remove(slotIndex);
        }
        else if (GameManager.playerStorage.equipmentStorage.ContainsKey(index))
        {
            GameManager.playerStorage.equipmentStorage.Remove(index);
        }
        //Delete equipment
    }
}

public class Critter : Items
{
    public string customName = "";
    public string sex = "";
    public float hunger = 0.0f;     //How hungry is this critter? (0-10, 10 = full)
    public float age = 0.0f;        //How old is critter?
    public float maxAge = 0.0f;     //When will this critter die of old age? (Cannot be changed, randomly generated based on parents?)
    public float hygiene = 0.0f;    //How healthy/clean is a creature? 0-10. As the container cleanliness goes down, so too does the hygiene
    public float happiness = 0.0f;  //How happy is this critter? (0-10) -- Happiness depends on environment, space etc
    public float price = 0.0f;      //Player set price of the critter
    public float actualValue = 0.0f;//Actual value of this critter as the AI/NPC system sees it (Basic Economy system --- age)
    public int children = 0;        //How many children has this critter had?
    public bool pregnant = false;   //Is this critter pregnant?
    public bool shiny = false;
    public bool sparkle = false;
    public bool glow = false;

    public Container container;

    public GameObject thisGameObject;


    public Critter(int i, string n, DATA_TYPE t, SUB_TYPE sType, int s, float mAge, bool shine, bool spark, bool glowy, string sx, GameObject go) : base(i, n, t, sType, s)
    {
        sex = sx;
        hunger = 5.0f;
        happiness = 5.0f;
        maxAge = mAge;
        shiny = shine;
        sparkle = spark;
        glow = glowy;
        thisGameObject = go;

        GameManager.playerStorage.allCritters.Add(this);

        //Add component to GameObject for AI behaviour (very simple, feed when hungry, randomly move, attack when pissed)

    }

    public void SetCustomName(string newName)
    {
        customName = newName;
    }
    
    public void Eat()
    {
        if(container.foodLevels > 0)
        {
            float food = 10 - hunger;       //Needed food
            float available = container.foodLevels * 2.0f;
            if(available >= food)
            {
                hunger = 10.0f;
                container.foodLevels -= (food / 2);     //Divide by 2 because 1 food unit restores 2 hunger points
            }
            else
            {
                hunger += available;
                container.foodLevels = 0;
            }
        }
    }

    //If angry (chance between 0-3.3 happiness --- 33% chance)
    //This only performs, does not handle logic (IE checking whether alone or not etc)
    public void Attack()
    {
        int rand = Random.Range(0, container.critterList.Count - 1);
        while(container.critterList[rand].index == index)
        {
            rand = Random.Range(0, container.critterList.Count - 1);
        }

        container.critterList[rand].KillCritter();
    }

    public override IEnumerator RunCoroutine()
    {
        while (age < maxAge)
        {
            age++;
            Debug.Log("Age : " + age);
            yield return new WaitForSeconds(1.0f);
        }

        //If too old, kill this critter
        KillCritter();
    }

    //Public so that if another critter attacks, then it can call this function. No HP checking
    public void KillCritter()
    {
        if (container != null)
            container.critterList.Remove(this);
        else if (GameManager.playerStorage.critterStorage.ContainsKey(index))
        {
            GameManager.playerStorage.critterStorage.Remove(index);
        }

        Destroy(gameObject);
        //Destroy critter
    }
}
