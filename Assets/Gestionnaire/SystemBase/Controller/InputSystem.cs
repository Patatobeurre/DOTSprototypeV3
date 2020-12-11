using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine.Jobs;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using System.Linq;
using Unity.Physics.Systems;
using Unity.Physics;


[UpdateInGroup(typeof(InitializationSystemGroup))]
[AlwaysUpdateSystem]
public class InputSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        /*GetEntityQuery(typeof(HerosArchetype), ComponentType.ReadOnly<MoveStats>(),  ComponentType.ReadWrite<AbilityStats>());
        EntityManager.CreateEntity(typeof(InputComponent));*/
    }
    protected override void OnUpdate()
    {
        
        /*Entities.ForEach((ref InputComponent _input) =>
        {
            _input._A = Input.GetKey(KeyCode.A);
            _input._Z = Input.GetKey(KeyCode.Z);
            //Debug.Log("repouet");
            //Debug.Log(_input._Z);
        }).Run();*/
    }


}
