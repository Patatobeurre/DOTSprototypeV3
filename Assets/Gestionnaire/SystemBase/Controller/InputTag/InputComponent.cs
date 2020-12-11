using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Transforms;

public struct InputComponent : IComponentData
{
    public bool _Z;
    public bool _A;
    public bool _Mouse_0_Down;
    public bool _Mouse_1_Down;
    public bool _Mouse_0_Up;
    public bool _Mouse_1_Up;
}
