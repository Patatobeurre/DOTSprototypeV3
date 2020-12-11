using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct Resistance : IComponentData
{
    public float _explosionResist;
    public float _fireResist;
}