using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.UIElements;

public struct FourmiArchetype : IComponentData
{
    public float _HP;
    public Vector3 _position;
}
