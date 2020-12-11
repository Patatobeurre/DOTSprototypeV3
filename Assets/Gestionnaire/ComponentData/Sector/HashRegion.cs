using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct HashRegion : ISharedComponentData
{
    public int _hashRegion;
}
