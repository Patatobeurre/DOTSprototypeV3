using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct AbilityPerform : IComponentData
{
    public int _SkillOne;
    public float _couldownSkillOne;
    public int _SkillTwo;
    public float _couldownSkillTwo;
    public int _SkillThree;
    public float _couldownSkillThree;
    public int _SkillFour;
    public float _couldownSkillFour;
    public int _SkillFive;
    public float _couldownSkillFive;
}

