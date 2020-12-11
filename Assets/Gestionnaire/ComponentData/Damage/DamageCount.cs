using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct DamageCount : IComponentData
{
    public float _heal;
    public float _damageExplosionCount;
    public float _damageFireCount;
}
