using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerGender : Villager
{
    public enum Gender {
        Child, // Can't work and other things
        Woman,
        Man
    };

    public Gender currentGender;


    protected override void UpdateVillager()
    {
        if (currentGender == Gender.Child) Debug.Log("<color=red> KID </color>"); // TODO: Follow one parent
        base.UpdateVillager();
    }
}
