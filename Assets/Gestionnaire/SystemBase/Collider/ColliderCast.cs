using Unity.Entities;
using Unity.Collections;
using UnityEngine;
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






[DisableAutoCreation]
public class ColliderCast : SystemBase
{
    BeginSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        var _ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        
    }

    static public unsafe void _polygonColliderCast(CollisionFilter _filter, CollisionWorld _collisionWorld, NativeList<ColliderCastHit> _allHits,
       quaternion Orientation,
       float3 Start,
       float3 End,
       float3 vertex0, float3 vertex1, float3 vertex2, float3 vertex3)
    {

        BlobAssetReference<Unity.Physics.Collider> _polygonCollider = Unity.Physics.PolygonCollider.CreateQuad(vertex0, vertex1, vertex2, vertex3);

        Unity.Physics.ColliderCastInput colliderCastInput = new ColliderCastInput()
        {
            Collider = (Unity.Physics.Collider*)_polygonCollider.GetUnsafePtr(),
            Orientation = ECS_RTSControls.instance._camera.transform.rotation,
            Start = ECS_RTSControls.instance._camera.transform.position,
            End = ECS_RTSControls.instance._camera.transform.rotation * new Vector3(100, 0, 0)
        };

        ColliderCastJob _colliderCastJob = new ColliderCastJob();
        _colliderCastJob._collisionWorld = _collisionWorld;
        _colliderCastJob._allHits = _allHits;
        
        
        _polygonCollider.Dispose();
    }

    [BurstCompile]
    public struct ColliderCastJob : IJobChunk
    {
        [ReadOnly] public CollisionWorld _collisionWorld;
        public NativeList<ColliderCastHit> _allHits;
        public ColliderCastInput _colliderInput;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            _collisionWorld.CastCollider(_colliderInput, ref _allHits);
        }
    }

    public unsafe Entity SphereCast(float3 RayFrom, float3 RayTo, float radius)
    {
        var physicsWorldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
        var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;

        var filter = new CollisionFilter()
        {
            BelongsTo = ~0u,
            CollidesWith = ~0u, // all 1s, so all layers, collide with everything
            GroupIndex = 0
        };

        SphereGeometry sphereGeometry = new SphereGeometry() { Center = float3.zero, Radius = radius };
        BlobAssetReference<Unity.Physics.Collider> sphereCollider = Unity.Physics.SphereCollider.Create(sphereGeometry, filter);

        ColliderCastInput input = new ColliderCastInput()
        {
            Collider = (Unity.Physics.Collider*)sphereCollider.GetUnsafePtr(),
            Orientation = quaternion.identity,
            Start = RayFrom,
            End = RayTo
        };

        ColliderCastHit hit = new ColliderCastHit();
        bool haveHit = collisionWorld.CastCollider(input, out hit);
        if (haveHit)
        {
            // see hit.Position
            // see hit.SurfaceNormal
            Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
            return e;
        }

        sphereCollider.Dispose();

        return Entity.Null;




    }
}
