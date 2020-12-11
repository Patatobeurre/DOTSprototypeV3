using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SouffleDeFeuTemp : IComponentData
{
    public float _radius;
    public float _damage;
    public int _team;
    public Vector3 _position;
}
