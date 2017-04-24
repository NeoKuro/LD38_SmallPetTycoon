using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicEconomy : MonoBehaviour
{
    private int lastSecond = 0;
    private int timerCount = 0;
    private int timerCount2 = 0;

    private void Update()
    {
        GetComponent<Text>().text = "$" + GameManager.playerFunds.ToString();

        if (lastSecond != GameManager.secondsPassed)
        {
            lastSecond = GameManager.secondsPassed;
            timerCount++;
            timerCount2++;
        }

        if (timerCount >= 15)
        {
            //Check the prices of all items (Determine value based on age) --- In this version, older = more valuable
            CheckPrices();
            timerCount = 0;
        }

        if (timerCount2 >= 30)
        {
            //Check whether prices are less than or equal to the desired (actual) price
            CheckToPurchase();
            timerCount2 = 0;
        }
    }

    private void CheckPrices()
    {
        List<Critter> critters = GameManager.playerStorage.allCritters;
        for (int i = 0; i < critters.Count; i++)
        {
            critters[i].actualValue = critters[i].age / 2;
        }
    }

    private void CheckToPurchase()
    {
        List<Critter> critters = new List<Critter>(GameManager.playerStorage.allCritters);
        foreach(Critter c in critters)
        {
            if(c.age >= 60)
            {
                if(c.price <= c.actualValue)
                {
                    foreach (KeyValuePair<int, ContainerSlot> slot in GameManager.slots)
                    {
                        for (int j = 0; j < slot.Value.containers.Count; j++)
                        {
                            slot.Value.containers[j].RemoveCritter(c);
                        }
                    }
                    GameManager.playerStorage.critterStorage.Remove(c.index);
                    GameManager.playerFunds += c.price;
                    GameManager.playerStorage.allCritters.Remove(c);
                    Destroy(c.thisGameObject);
                }
            }
        }
    }
}
