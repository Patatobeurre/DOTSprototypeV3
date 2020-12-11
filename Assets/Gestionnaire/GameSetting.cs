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
using Collider = Unity.Physics.Collider;

[AlwaysUpdateSystem]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(Decision))]
public class GameSetting : SystemBase
{
    public static int HASH_REGION_NUM = 10;
    public static int MAP_WIDTH;
    public static int MAP_HEIGHT;

    EntityManager _entityManager;

    public static EntityArchetype _herosArchetype;
    Entity _fullBodyEntity;
    EntityArchetype _entityHerosArchetype;
    EntityArchetype _fullBodyArchetype;

    EntityQuery _heroesQuery;

    bool _JobIsDone;

    EntityQuery _query;

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    BeginSimulationEntityCommandBufferSystem m_BeginSimulationBufferSystem;

    EntityCommandBufferSystem _customEntityCommandBuffer;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();


        m_BeginSimulationBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

       /* World.CreateSystem<EntityCommandBuffer>(new EntityCommandBuffer());*/

        _entityHerosArchetype = EntityManager.CreateArchetype(
        typeof(Translation),
        typeof(HashRegion),
        typeof(MoveStats),
        typeof(AbilityStats),
        typeof(AbilityCouldownStats),
        typeof(AbilityCouldownPerform),
        typeof(MovePerformForColliderJob),
        typeof(AbilityPerform),
        typeof(SouffleDeFeuColliderForJob),
        typeof(SouffleDeFeuStats),
        typeof(ColliderSphere),
        typeof(HerosArchetype),
        typeof(DamageCount),
        typeof(HealthPoint));


        //_query = GetEntityQuery(typeof(LocalToWorld), typeof(Spawner_FromEntity));
        _query = GetEntityQuery(typeof(SouffleDeFeuTemp), typeof(HashRegion));

        _heroesQuery = EntityManager.CreateEntityQuery(typeof(MoveStats));
        //var ah = Resources.Load<GameObject>("Heros").GetComponent<MoveStats>();
    }

    protected override void OnUpdate()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var _ecb = m_BeginSimulationBufferSystem.CreateCommandBuffer();


        if (Time.ElapsedTime > 2 && _JobIsDone == false)
        {

            if (Time.ElapsedTime < 5)
            {
                _fullBodyEntity = EntityManager.Instantiate(PrefabConvert._FullBody);

                EntityManager.AddComponent<MoveStats>(_fullBodyEntity);
                var _pouet = EntityManager.GetChunk(_fullBodyEntity);
                _herosArchetype = _pouet.Archetype;
                //_entityManager.DestroyEntity(_fullBodyEntity);

                for (int i = -75; i < 75; i++)
                {
                    for (int j = -75; j < 75; j++)
                    {
                        var e = EntityManager.Instantiate(PrefabConvert._DalleGrize);
                        EntityManager.SetComponentData<Translation>(e, new Translation { Value = new float3(i, 3, j) });
                        EntityManager.AddComponentData<CustomColor>(e, new CustomColor());
                        EntityManager.SetComponentData<CustomColor>(e, new CustomColor { Value = new float4 { x = 0.01179f, y = 0.5f, z = 0.1044553f, w = 0f } });
                    }
                }

                _JobIsDone = true;
            }
        }


        if (Time.ElapsedTime > 2)
        {
            if (_heroesQuery.CalculateEntityCount() < 2)
            {
                var _pouet = EntityManager.Instantiate(_fullBodyEntity);
                EntityManager.SetComponentData(_pouet, new LocalToWorld
                {
                    Value = new float4x4(rotation: quaternion.identity, translation: new float3(1, 5, 3))
                });
                EntityManager.SetComponentData(_pouet, new Translation { Value = new float3(0,5,0) });
            }
        }

        CollisionFilter _filter = new CollisionFilter()
        {
            BelongsTo = (uint)9 << 8,
            CollidesWith = (uint)17 << 10,
            GroupIndex = 7
        };

        BoxGeometry _boxGeometry = new BoxGeometry() { Center = float3.zero, Size = new float3(10000, 10, 10000), Orientation = quaternion.identity };
        BlobAssetReference<Collider> _boxCollider = Unity.Physics.BoxCollider.Create(_boxGeometry, _filter);


        this.Dependency =  Entities.
            ForEach((ref PhysicsCollider _physicsCollider, in Entity _entity, in PlaneLayer _planeLayer) =>
            {
                _physicsCollider.Value = _boxCollider;

            }).Schedule(this.Dependency);

        this.Dependency.Complete();


        /*if (_query.CalculateEntityCount() < 10)
        {
            //EntityManager.CreateEntity(_spawner_FromEntity, 10);
            //EntityManager.CreateEntity(typeof(LocalToWorld), typeof(Spawner_FromEntity), typeof(Translation));
            EntityManager.CreateEntity(typeof(SouffleDeFeuTemp), typeof(HashRegion));

            Debug.Log("spawn!!");
        }*/

        if (_heroesQuery.CalculateEntityCount() < 10)
        {
            
            /*_herosEntity = EntityManager.CreateEntity(_entityHerosArchetype);
            
            _entityManager.SetComponentData(_herosEntity, new Translation { Value = new float3(0, 0, 0) });
            _entityManager.SetSharedComponentData(_herosEntity, new HashRegion { _hashRegion = 1 });
            _entityManager.SetComponentData(_herosEntity, new HealthPoint { _healthPoint = 100000 });
            _entityManager.SetComponentData(_herosEntity, new MoveStats { _canMove = 1, _position = new float3(0, 0, 0), _newPosition = new Vector3(0, 0, 0), _mouveSpeed = 1 });
            _entityManager.SetComponentData(_herosEntity, new AbilityStats { _SkillOne = 1, _SkillTwo = 0, _SkillThree = 0, _SkillFour = 0, _SkillFive = 0 });
            _entityManager.SetComponentData(_herosEntity, new AbilityCouldownStats { _couldownSkillOne = 1, _couldownSkillTwo = 1, _couldownSkillThree = 1, _couldownSkillFour = 1, _couldownSkillFive = 1 });
            _entityManager.SetComponentData(_herosEntity, new AbilityPerform { _SkillOne = 0, _SkillTwo = 1, _SkillThree = 1, _SkillFour = 1, _SkillFive = 1, });
            _entityManager.SetComponentData(_herosEntity, new AbilityCouldownPerform { _couldownSkillOne = 1, _couldownSkillTwo = 1, _couldownSkillThree = 1, _couldownSkillFour = 1, _couldownSkillFive = 1 });
            _entityManager.SetComponentData(_herosEntity, new MovePerformForColliderJob { _radius = 1, _layerMask = 1, _newPosition = new float3(0, 0, 0) });
            _entityManager.SetComponentData(_herosEntity, new SouffleDeFeuColliderForJob { _position = new float3(0, 0, 0), _layerMask = new LayerMask(), _radius = 1, _damageExplosion = 1 });
            _entityManager.SetComponentData(_herosEntity, new SouffleDeFeuStats { _position = new float3(0, 0, 0), _team = 1, _damage = 1, _radius = 1 });
            _entityManager.SetComponentData(_herosEntity, new ColliderSphere { _radius = 1, _layerMask = 1, _position = new float3(0, 0, 0) });*/
            /*i++;
            Job.WithoutBurst().WithCode(() =>
            {
                //EntityManager.CreateEntity(_souffleDeFeu, _souffleTempArray.Length, Allocator.TempJob);
                _herosEntity = _ecb.CreateEntity(_entityHerosArchetype);

                _ecb.SetComponent(_herosEntity, new Translation { Value = new float3(0, 0, 0) });
                _ecb.SetSharedComponent(_herosEntity, new HashRegion { _hashRegion = 0 });
                _ecb.SetComponent(_herosEntity, new MoveStats { _canMove = 1, _position = new float3(0, 0, 0), _newPosition = new Vector3(0, 0, 0), _mouveSpeed = 1 });
                _ecb.SetComponent(_herosEntity, new AbilityStats { _SkillOne = 1, _SkillTwo = 0, _SkillThree = 0, _SkillFour = 0, _SkillFive = 0 });
                _ecb.SetComponent(_herosEntity, new AbilityCouldownStats { _couldownSkillOne = 1, _couldownSkillTwo = 1, _couldownSkillThree = 1, _couldownSkillFour = 1, _couldownSkillFive = 1 });
                _ecb.SetComponent(_herosEntity, new AbilityPerform { _SkillOne = 1, _SkillTwo = 1, _SkillThree = 1, _SkillFour = 1, _SkillFive = 1, });
                _ecb.SetComponent(_herosEntity, new AbilityCouldownPerform { _couldownSkillOne = 1, _couldownSkillTwo = 1, _couldownSkillThree = 1, _couldownSkillFour = 1, _couldownSkillFive = 1 });
                _ecb.SetComponent(_herosEntity, new MovePerformForColliderJob { _radius = 1, _layerMask = 1, _newPosition = new float3(0, 0, 0) });
                _ecb.SetComponent(_herosEntity, new SouffleDeFeuColliderForJob { _position = new float3(0, 0, 0), _layerMask = new LayerMask(), _radius = 1, _damageExplosion = 1 });
                _ecb.SetComponent(_herosEntity, new SouffleDeFeuStats { _position = new float3(0, 0, 0), _team = 1, _damage = 1, _radius = 1 });
                _ecb.SetComponent(_herosEntity, new ColliderSphere { _radius = 1, _layerMask = 1, _position = new float3(0, 0, 0) });
                i++;
            }).Run();*/
        }


        m_BeginSimulationBufferSystem.AddJobHandleForProducer(this.Dependency);

        BuildPhysicsWorld _physicsWorldSystem = Unity.Entities.World.DefaultGameObjectInjectionWorld.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>(); _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    /*
     *                 _ecb.SetComponent(i, _herosEntity, new Translation { Value = new float3(0, 0, 0) });
                _ecb.SetSharedComponent(i, _herosEntity, new HashRegion { _hashRegion = 0 });
                _ecb.SetComponent(i, _herosEntity, new MoveStats { _canMove = 1, _position = new float3(0, 0, 0), _newPosition = new Vector3(0, 0, 0), _mouveSpeed = 1 });
                _ecb.SetComponent(i, _herosEntity, new AbilityStats { _SkillOne = 1, _SkillTwo = 0, _SkillThree = 0, _SkillFour = 0, _SkillFive = 0 });
                _ecb.SetComponent(i, _herosEntity, new AbilityCouldownStats { _couldownSkillOne = 1, _couldownSkillTwo = 1, _couldownSkillThree = 1, _couldownSkillFour = 1, _couldownSkillFive = 1 });
                _ecb.SetComponent(i, _herosEntity, new AbilityPerform { _SkillOne = 1, _SkillTwo = 1, _SkillThree = 1, _SkillFour = 1, _SkillFive = 1, });
                _ecb.SetComponent(i, _herosEntity, new AbilityCouldownPerform { _couldownSkillOne = 1, _couldownSkillTwo = 1, _couldownSkillThree = 1, _couldownSkillFour = 1, _couldownSkillFive = 1 });
                _ecb.SetComponent(i, _herosEntity, new MovePerformForColliderJob { _radius = 1, _layerMask = 1, _newPosition = new float3(0, 0, 0) });
                _ecb.SetComponent(i, _herosEntity, new SouffleDeFeuColliderForJob { _position = new float3(0, 0, 0), _layerMask = new LayerMask(), _radius = 1, _damageExplosion = 1 });
                _ecb.SetComponent(i, _herosEntity, new SouffleDeFeuStats { _position = new float3(0, 0, 0), _team = 1, _damage = 1, _radius = 1 });
                _ecb.SetComponent(i, _herosEntity, new ColliderSphere { _radius = 1, _layerMask = 1, _position = new float3(0, 0, 0) });

    
    struct SpawnEntity : IJobChunk
    {
        public EntityArchetype _entityHerosArchetype;
        public Entity _herosEntity;
        public EntityCommandBuffer.ParallelWriter _ecb;
        public int i;

        void IJobChunk.Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            if (i < 200)
            {
                _herosEntity = _ecb.CreateEntity(chunkIndex, _entityHerosArchetype);

                _ecb.SetComponent(chunkIndex, _herosEntity, new Translation { Value = new float3(0, 0, 0) });
                _ecb.SetSharedComponent(chunkIndex, _herosEntity, new HashRegion { _hashRegion = 0 });
                _ecb.SetComponent(chunkIndex, _herosEntity, new MoveStats { _canMove = 1, _position = new float3(0, 0, 0), _newPosition = new Vector3(0, 0, 0), _mouveSpeed = 1 });
                _ecb.SetComponent(chunkIndex, _herosEntity, new AbilityStats { _SkillOne = 1, _SkillTwo = 0, _SkillThree = 0, _SkillFour = 0, _SkillFive = 0 });
                _ecb.SetComponent(chunkIndex, _herosEntity, new AbilityCouldownStats { _couldownSkillOne = 1, _couldownSkillTwo = 1, _couldownSkillThree = 1, _couldownSkillFour = 1, _couldownSkillFive = 1 });
                _ecb.SetComponent(chunkIndex, _herosEntity, new AbilityPerform { _SkillOne = 1, _SkillTwo = 1, _SkillThree = 1, _SkillFour = 1, _SkillFive = 1, });
                _ecb.SetComponent(chunkIndex, _herosEntity, new AbilityCouldownPerform { _couldownSkillOne = 1, _couldownSkillTwo = 1, _couldownSkillThree = 1, _couldownSkillFour = 1, _couldownSkillFive = 1 });
                _ecb.SetComponent(chunkIndex, _herosEntity, new MovePerformForColliderJob { _radius = 1, _layerMask = 1, _newPosition = new float3(0, 0, 0) });
                _ecb.SetComponent(chunkIndex, _herosEntity, new SouffleDeFeuColliderForJob { _position = new float3(0, 0, 0), _layerMask = new LayerMask(), _radius = 1, _damageExplosion = 1 });
                _ecb.SetComponent(chunkIndex, _herosEntity, new SouffleDeFeuStats { _position = new float3(0, 0, 0), _team = 1, _damage = 1, _radius = 1 });
                _ecb.SetComponent(chunkIndex, _herosEntity, new ColliderSphere { _radius = 1, _layerMask = 1, _position = new float3(0, 0, 0) });
                i++;
            }
        }
    }*/
}
