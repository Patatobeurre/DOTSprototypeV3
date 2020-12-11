using System;
using Unity.Physics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;
using Unity.Transforms;
using Collider = Unity.Physics.Collider;


public class ColliderAsset : MonoBehaviour
{
    public void AddSphereDynamicCollider(SphereDynamicColliderStats _sphereDynamicColliderStats, GameObject _sphereDynamicColliderprefab, EntityManager _entityManager)
    {
        Entity _sphereCollider = GameObjectConversionUtility.ConvertGameObjectHierarchy(_sphereDynamicColliderprefab, new GameObjectConversionSettings() { DestinationWorld = World.DefaultGameObjectInjectionWorld });

        BlobAssetReference<Collider> sourceCollider = _entityManager.GetComponentData<PhysicsCollider>(_sphereCollider).Value;

        _entityManager.AddComponentData(_sphereDynamicColliderStats._entity, new Translation { Value = _sphereDynamicColliderStats._position });
        _entityManager.AddComponentData(_sphereDynamicColliderStats._entity, new Rotation { Value = _sphereDynamicColliderStats._orientation });
        _entityManager.AddComponentData(_sphereDynamicColliderStats._entity, new PhysicsCollider { Value = sourceCollider });
        _entityManager.AddComponentData(_sphereDynamicColliderStats._entity, new PhysicsDamping() { Linear = _sphereDynamicColliderStats.linearDamping, Angular = _sphereDynamicColliderStats.angularDamping });
        _entityManager.AddComponentData(_sphereDynamicColliderStats._entity, new PhysicsVelocity() { Linear = _sphereDynamicColliderStats.linearVelocity, Angular = _sphereDynamicColliderStats.angularVelocity });
        _entityManager.SetComponentData(_sphereDynamicColliderStats._entity, new PhysicsMass() {Transform = _sphereDynamicColliderStats.massTransform, InverseMass = _sphereDynamicColliderStats.inverseMass, InverseInertia = _sphereDynamicColliderStats.inverseInertiaMass, AngularExpansionFactor = _sphereDynamicColliderStats.angularExpansionFactorMass });

    }

    public struct SphereDynamicColliderStats
    {
        public Entity _entity;

        public float3 _position;
        public quaternion _orientation;

        public float3 linearVelocity;
        public float3 angularVelocity;

        public float mass;

        public float linearDamping;
        public float angularDamping;

        public RigidTransform massTransform;
        public float inverseMass;
        public float3 inverseInertiaMass;
        public float angularExpansionFactorMass;
    }

    public void AddSphereCollider(SphereDynamicColliderStats _sphereColliderStats, GameObject _sphereColliderprefab, EntityManager _entityManager)
    {
        Entity _sphereCollider = GameObjectConversionUtility.ConvertGameObjectHierarchy(_sphereColliderprefab, new GameObjectConversionSettings() { DestinationWorld = World.DefaultGameObjectInjectionWorld });

        BlobAssetReference<Collider> sourceCollider = _entityManager.GetComponentData<PhysicsCollider>(_sphereCollider).Value;

        _entityManager.AddComponentData(_sphereColliderStats._entity, new Translation { Value = _sphereColliderStats._position });
        _entityManager.AddComponentData(_sphereColliderStats._entity, new Rotation { Value = _sphereColliderStats._orientation });
        _entityManager.AddComponentData(_sphereColliderStats._entity, new PhysicsCollider { Value = sourceCollider });
    }

    public struct SphereColliderStats
    {
        public Entity _entity;

        public float3 _position;
        public quaternion _orientation;
    }


    public void AddBoxDynamicCollider(BoxDynamicColliderStats _boxDynamicColliderStats, GameObject _boxDynamicColliderprefab, EntityManager _entityManager)
    {
        Entity _boxCollider = GameObjectConversionUtility.ConvertGameObjectHierarchy(_boxDynamicColliderprefab, new GameObjectConversionSettings() { DestinationWorld = World.DefaultGameObjectInjectionWorld });


        BlobAssetReference<Collider> sourceCollider = _entityManager.GetComponentData<PhysicsCollider>(_boxCollider).Value;

        _entityManager.AddComponentData(_boxDynamicColliderStats._entity, new Translation { Value = _boxDynamicColliderStats._position });
        _entityManager.AddComponentData(_boxDynamicColliderStats._entity, new Rotation { Value = _boxDynamicColliderStats._orientation });
        _entityManager.AddComponentData(_boxDynamicColliderStats._entity, new PhysicsCollider { Value = sourceCollider });
        _entityManager.AddComponentData(_boxDynamicColliderStats._entity, new PhysicsDamping() { Linear = _boxDynamicColliderStats.linearDamping, Angular = _boxDynamicColliderStats.angularDamping });
        _entityManager.AddComponentData(_boxDynamicColliderStats._entity, new PhysicsVelocity() { Linear = _boxDynamicColliderStats.linearVelocity, Angular = _boxDynamicColliderStats.angularVelocity });
        _entityManager.SetComponentData(_boxDynamicColliderStats._entity, new PhysicsMass() { Transform = _boxDynamicColliderStats.massTransform, InverseMass = _boxDynamicColliderStats.inverseMass, InverseInertia = _boxDynamicColliderStats.inverseInertiaMass, AngularExpansionFactor = _boxDynamicColliderStats.angularExpansionFactorMass });

    }
    public struct BoxDynamicColliderStats
    {
        public Entity _entity;

        public float3 _position;
        public quaternion _orientation;

        public float3 linearVelocity;
        public float3 angularVelocity;

        public float mass;

        public float linearDamping;
        public float angularDamping;

        public RigidTransform massTransform;
        public float inverseMass;
        public float3 inverseInertiaMass;
        public float angularExpansionFactorMass;
    }

    public void AddBoxCollider(BoxDynamicColliderStats _boxColliderStats, GameObject _boxColliderprefab, EntityManager _entityManager)
    {
        Entity _boxCollider = GameObjectConversionUtility.ConvertGameObjectHierarchy(_boxColliderprefab, new GameObjectConversionSettings() { DestinationWorld = World.DefaultGameObjectInjectionWorld });


        BlobAssetReference<Collider> sourceCollider = _entityManager.GetComponentData<PhysicsCollider>(_boxCollider).Value;

        _entityManager.AddComponentData(_boxColliderStats._entity, new Translation { Value = _boxColliderStats._position });
        _entityManager.AddComponentData(_boxColliderStats._entity, new Rotation { Value = _boxColliderStats._orientation });
        _entityManager.AddComponentData(_boxColliderStats._entity, new PhysicsCollider { Value = sourceCollider });
    }

    public struct BoxColliderStats
    {
        public Entity _entity;

        public float3 _position;
        public quaternion _orientation;
    }
}
