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
    public string name = "";
    public DATA_TYPE type;    //Container, Equipment, Critter
    public SUB_TYPE subType; //Fish, Reptile etc (Containers wil use this to IE when fill tank with water etc)
    public int size = 0;

    public Items(int i, string n, DATA_TYPE t, SUB_TYPE sType, int s)
    {
        index = i;
        name = n;
        type = t;
        subType = sType;
        size = s;
    }

    public Items(int i, string n, int s)
    {
        index = i;
        name = n;
        size = s;
    }

    public virtual IEnumerator RunCoroutine()
    {
        yield return null;
    }
}

public class Container : Items
{
    public int containerSlot = -1;  //Which container slot is this placed in in the room?
    public int maxCapacity = 0; //How many critters can be fit in here?
    public int currCapacity = 0;
    public int equipmentSlots = 0;  //How much room for equipment is there?
    public int usedSlots = 0;
    public float cleanliness = 0.0f;    //How clean (0-100) is the container?
    public float foodLevels = 0.0f;     //How much food is left in the container? 0 - 10
    public bool placed = false;

    public Dictionary<int, Equipment> equipmentList = new Dictionary<int, Equipment>();
    public List<Critter> critterList = new List<Critter>();

    public Container(int i, string n, int s) : base(i, n, s)
    {

        maxCapacity = size * 5;
        equipmentSlots = (size - 1) * 2;        //Size 1 has no equipment slots (bugs only) -- size 2 = 2, 3 = 4, 4 = 6, 5 = 8
        cleanliness = 50.0f;
        foodLevels = 2.5f;
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
        equipment.slotIndex = slot;
        equipmentList.Add(slot, equipment);
        GameManager.playerStorage.equipmentStorage.Remove(equipment.index);
    }

    public void RemoveEquipment(int slot)
    {
        GameManager.playerStorage.equipmentStorage.Add(equipmentList[slot].index, equipmentList[slot]);
        equipmentList.Remove(slot);
    }

    public void AddCritter(Critter critter)
    {
        critterList.Add(critter);
        GameManager.playerStorage.critterStorage.Remove(critter.index);
        critter.container = this;
    }

    public void RemoveCritter(Critter critter)
    {
        critterList.Remove(critter);
        critter.container = null;
        GameManager.playerStorage.critterStorage.Add(critter.index, critter);
    }

    public void PlaceContainer(int slot)
    {
        containerSlot = slot;
        placed = true;
    }

    public void RemoveContainer()
    {
        placed = false;
        for (int i = 0; i < critterList.Count; i++)
        {
            GameManager.playerStorage.critterStorage.Add(critterList[i].index, critterList[i]);
        }

        for (int i = 0; i < equipmentList.Count; i++)
        {
            GameManager.playerStorage.equipmentStorage.Add(equipmentList[i].index, equipmentList[i]);
        }

        critterList.Clear();
        equipmentList.Clear();
    }

    private IEnumerator RunCoroutine()
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

    private int lastSecond = 0;

    public Equipment(int i, string n, DATA_TYPE t, SUB_TYPE sType, int s, float power, float heat = 0.0f, float humid = 0.0f, float air = 0.0f, float filter = 0.0f) : base(i, n, t, sType, s)
    {
        heatSupply = heat;
        humiditySup = humid;
        powerUsage = power;
        airSupply = air;
        filterRate = filter;
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
            container.equipmentList.Remove(index);
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
    public float hunger = 0.0f;     //How hungry is this critter? (0-10, 10 = full)
    public float age = 0.0f;        //How old is critter?
    public float maxAge = 0.0f;     //When will this critter die of old age? (Cannot be changed, randomly generated based on parents?)
    public float happiness = 0.0f;  //How happy is this critter? (0-10) -- Happiness depends on environment, space etc
    public bool pregnant = false;   //Is this critter pregnant?
    public int children = 0;        //How many children has this critter had?
    public bool shiny = false;
    public bool sparkle = false;
    public bool glow = false;

    public Container container;


    public Critter(int i, string n, DATA_TYPE t, SUB_TYPE sType, int s, float mAge, bool shine, bool spark, bool glowy) : base(i, n, t, sType, s)
    {
        hunger = 5.0f;
        happiness = 5.0f;
        maxAge = mAge;
        shiny = shine;
        sparkle = spark;
        glow = glowy;

        //Add component to GameObject for AI behaviour (very simple, feed when hungry, randomly move, attack when pissed)
        
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
