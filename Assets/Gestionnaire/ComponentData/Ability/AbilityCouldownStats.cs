using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.UIElements;

[GenerateAuthoringComponent]
public struct AbilityCouldownStats : IComponentData
{
    public float _couldownSkillOne;
    public float _couldownSkillTwo;
    public float _couldownSkillThree;
    public float _couldownSkillFour;
    public float _couldownSkillFive;
}
