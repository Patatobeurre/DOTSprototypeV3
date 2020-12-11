using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct ControlledUnitShared : IComponentData
{
    public int _playerControl;
}
