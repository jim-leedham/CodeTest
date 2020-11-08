using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPickUp : PickUp
{
    [SerializeField] private AbilityType abilityType;
    public AbilityType AbilityType { get { return abilityType; } }
}
