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
using System.Collections.Generic;
using CodeMonkey.Utils;
using System.Collections;
using Unity.Rendering;
using System.Threading;


[UpdateInGroup(typeof(TransformSystemGroup))]
public class ColliderCastTest : SystemBase
{
    EntityQuery _query;

    EndFixedStepSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();

        _query = GetEntityQuery(typeof(HashRegion));
    }

    protected unsafe override void OnUpdate()
    {
        var _physicsWorldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
        var _collisionWorld = _physicsWorldSystem.PhysicsWorld.CollisionWorld;

        var _ecb = m_EntityCommandBufferSystem.CreateCommandBuffer();

        var _filter = new CollisionFilter()
        {
            BelongsTo = ~0u,
            CollidesWith = ~0u, // all 1s, so all layers, collide with everything
            GroupIndex = 0
        };

        SphereGeometry sphereGeometry = new SphereGeometry() { Center = float3.zero, Radius = 100 };
        BlobAssetReference<Unity.Physics.Collider> sphereCollider = Unity.Physics.SphereCollider.Create(sphereGeometry, _filter);
        /*Vector3
        Unity.Physics.ColliderCastInput input = new Unity.Physics.ColliderCastInput()
        {
            Collider = (Unity.Physics.Collider*)sphereCollider.GetUnsafePtr(),
            Orientation = quaternion.identity,
            Start = RayFrom,
            End = RayTo
        };*/

        /*Entities.
            ForEach((ref Translation _translation, in Entity _entity) =>
            {
                _ecb.AddComponent(_entity, new UnitSelected());

            }).Schedule();*/
    }

}
