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
using System;
using UnityEditor;
using UnityEngine.Assertions;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Rendering;

[AlwaysUpdateSystem]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class UnitSelection : SystemBase
{

    BeginSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    EndFixedStepSimulationEntityCommandBufferSystem m_EntityCommandBufferSystemo;
    EndFramePhysicsSystem m_EndFramePhysicsSystem;
    EndSimulationEntityCommandBufferSystem m_endSimulationEntityCommandBufferSystem;
    LateSimulationSystemGroup m_lateSimulationSystemGroup;
    BeginPresentationEntityCommandBufferSystem m_beginPresentationEntityCommandBufferSystem;

    BlobAssetReference<Unity.Physics.Collider> _polygonCollider;

    EntityQuery _query;
    EntityQuery _queryInput;
    EntityQuery _dalleQuery;
    EntityQuery _dalleSelectedQuery;

    EntityQueryDesc desc1;
    EntityQueryDesc desc2;


    float3 _startPosition;
    float3 _endPosition;
    float3 _lowerLeftPosition;
    float3 _upperRightPosition;

    public NativeList<ColliderCastHit> _allHits;
    public BlobAssetReference<Unity.Physics.Collider> _sphereCollider;

    Entity _inputEntity;
    bool _inputReady;
    bool Input_Mouse_0_GetUp;
    bool Input_Mouse_0_GetDown;
    bool Input_Mouse_0_Up;
    bool Input_Mouse_1_GetUp;
    bool Input_Mouse_1_GetDown;
    bool Input_Mouse_1_Up;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        m_EntityCommandBufferSystemo = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
        m_EndFramePhysicsSystem = World.GetExistingSystem<EndFramePhysicsSystem>();
        m_endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        m_beginPresentationEntityCommandBufferSystem = World.GetExistingSystem<BeginPresentationEntityCommandBufferSystem>();

        _query = GetEntityQuery(typeof(MoveStats));
        _queryInput = GetEntityQuery(typeof(InputComponent));
        _dalleQuery = GetEntityQuery(typeof(Selectable), typeof(Player1));
        _dalleSelectedQuery = GetEntityQuery(typeof(Selectable), typeof(Player1), typeof(UnitSelected), typeof(GrisTag));
    }
    protected unsafe override void OnUpdate()
    {
        

        var _ecb = m_EntityCommandBufferSystem.CreateCommandBuffer();
        var _ecbo = m_EntityCommandBufferSystemo.CreateCommandBuffer();
        var _ecbEndSimilation = m_endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        var _ecbBeginPresentation = m_beginPresentationEntityCommandBufferSystem.CreateCommandBuffer();

        var _physicsWorldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
        CollisionWorld _collisionWorld = _physicsWorldSystem.PhysicsWorld.CollisionWorld;
        var _physicsWorld = _physicsWorldSystem.PhysicsWorld;

        float3 _startPosition = ECS_RTSControls.instance._startScreenPosition;
        float3 _endPosition = ECS_RTSControls.instance._endScreenPosition;
        float3 _lowerLeftPosition = ECS_RTSControls.instance._lowerLeftScreenPosition;
        float3 _upperRightPosition = ECS_RTSControls.instance._upperRightScreenPosition;

        Dependency = JobHandle.CombineDependencies(Dependency, m_EndFramePhysicsSystem.GetOutputDependency());

        if (_inputReady)
        {
            if (Input_Mouse_0_Up == true && Input_Mouse_0_Up != EntityManager.HasComponent<Input_Mouse_0_Up>(_inputEntity))
            {
                Input_Mouse_0_GetDown = true;
                Input_Mouse_0_Up = false;
            }

            if (Input_Mouse_0_Up == false && (Input_Mouse_0_Up != EntityManager.HasComponent<Input_Mouse_0_Up>(_inputEntity)))
            {
                Input_Mouse_0_GetUp = true;
                Input_Mouse_0_Up = true;
            }

            if (Input_Mouse_1_Up == true && (Input_Mouse_1_Up != EntityManager.HasComponent<Input_Mouse_1_Up>(_inputEntity)))
            {
                Input_Mouse_1_GetDown = true;
                Input_Mouse_1_Up = false;
            }
            if (Input_Mouse_1_Up == false && (Input_Mouse_1_Up != EntityManager.HasComponent<Input_Mouse_1_Up>(_inputEntity)))
            {
                Input_Mouse_1_GetUp = true;
                Input_Mouse_1_Up = true;
            }
        }
        else
        {
            if (_queryInput.CalculateEntityCount() > 0)
            {
                _inputEntity = _queryInput.GetSingletonEntity();
                _inputReady = true;
                Input_Mouse_0_Up = EntityManager.HasComponent<Input_Mouse_0_Up>(_inputEntity);
                Input_Mouse_1_Up = EntityManager.HasComponent<Input_Mouse_1_Up>(_inputEntity);
            }
        }

        if (Time.ElapsedTime > 4)
        {
           
           /*this.Dependency =  Entities.WithoutBurst().ForEach((in Translation _translation, in UnitSelected _unitSelected, in Selectable _selectable, in GrisTag _grisTag, in Entity _entity) =>
            {
                var e = _ecbEndSimilation.Instantiate(PrefabConvert._DalleVerte);
                _ecbEndSimilation.SetComponent<Translation>(e, _translation);
                _ecbEndSimilation.DestroyEntity(_entity);
                Debug.Log("Unity3d : 'va te faire foutre'");

            }).Schedule(this.Dependency);*/



        }

        if (Input_Mouse_0_GetUp == true)
        {
            bool selectOnlyOneEntity = false;
            float selectionAreaMinSize = 10f;
            float selectionAreaSize = math.distance(_lowerLeftPosition, _upperRightPosition);
            if (selectionAreaSize < selectionAreaMinSize)
            {
                // Selection area too small
                _lowerLeftPosition += new float3(-1, -1, 0) * (selectionAreaMinSize - selectionAreaSize) * .5f;
                _upperRightPosition += new float3(+1, +1, 0) * (selectionAreaMinSize - selectionAreaSize) * .5f;
                selectOnlyOneEntity = true;
            }

            // Deselect all selected Entities
            this.Dependency = Entities.WithAll<UnitSelected>().ForEach((Entity entity) => {
                _ecb.RemoveComponent<UnitSelected>(entity);
            }).Schedule(this.Dependency);




            Unity.Physics.RaycastHit _hit1 = new Unity.Physics.RaycastHit();
            Unity.Physics.RaycastHit _hit2 = new Unity.Physics.RaycastHit();
            Unity.Physics.RaycastHit _hit3 = new Unity.Physics.RaycastHit();
            Unity.Physics.RaycastHit _hit4 = new Unity.Physics.RaycastHit();
            Unity.Physics.RaycastHit _hit5 = new Unity.Physics.RaycastHit();

            var _ray1 = ECS_RTSControls.instance._ray1;
            var _ray2 = ECS_RTSControls.instance._ray2;
            var _ray3 = ECS_RTSControls.instance._ray3;
            var _ray4 = ECS_RTSControls.instance._ray4;
            var _ray5 = ECS_RTSControls.instance._ray5;

            /*RaycastInput _inputo = new RaycastInput()
            {
                Start = _ray1.origin,
                End = _ray1.origin + _ray1.direction * 1000,
                Filter = new CollisionFilter()
                {
                    BelongsTo = (uint)10 << 10,
                    CollidesWith = (uint)7 << 8,
                    GroupIndex = 8
                }
            };*/

            GetRaycastHit(_collisionWorld, _ray1, _ray2, _ray3, _ray4, _ray5, out _hit1, out _hit2, out _hit3, out _hit4, out _hit5);


            var _filter = new CollisionFilter()
            {
                BelongsTo = ~1u,
                CollidesWith = ~1u, // all 1s, so all layers, collide with everything
                GroupIndex = 0
            };
            
            NativeList<ColliderCastHit> _allHits = new NativeList<ColliderCastHit>(Allocator.TempJob);

            
            _polygonCollider = Unity.Physics.PolygonCollider.CreateQuad(_hit1.Position, _hit2.Position, _hit3.Position, _hit4.Position, _filter);
            /*SphereGeometry sphereGeometry = new SphereGeometry() { Center = float3.zero, Radius = 3 };
            _sphereCollider = Unity.Physics.SphereCollider.Create(sphereGeometry, _filter);*/
            /*
            Unity.Physics.ColliderCastInput colliderCastInput = new ColliderCastInput()
            {
                Collider = (Unity.Physics.Collider*)_sphereCollider.GetUnsafePtr(),
                Orientation = ECS_RTSControls.instance._camera.transform.rotation,
                Start = ECS_RTSControls.instance._camera.transform.position,
                End = _ray1.origin + _ray1.direction * 1000
            };*/
            var _moyPoly = new float3((_hit1.Position.x + _hit2.Position.x + _hit3.Position.x + _hit4.Position.x) / 4,
                (_hit1.Position.y + _hit2.Position.y + _hit3.Position.y + _hit4.Position.y) / 4,
                (_hit1.Position.z + _hit2.Position.z + _hit3.Position.z + _hit4.Position.z) / 4);



            Debug.Log(_moyPoly + "_moyPoly");

            //var  _pouitCollider = BlobAssetReference<Unity.Physics.Collider>.Create(_polygonCollider.GetUnsafePtr(), 1);
            /* this.Dependency = Job.WithoutBurst().WithCode(() =>
             {
                 var e = _ecb.Instantiate(PrefabConvert._CubeRouge);
                 _ecb.SetComponent<Translation>(e, new Translation { Value = _hit1.Position + new float3(0, 5, 0) });
                 e = _ecb.Instantiate(PrefabConvert._CubeRouge);
                 _ecb.SetComponent<Translation>(e, new Translation { Value = _hit2.Position + new float3(0, 5, 0) });
                 e = _ecb.Instantiate(PrefabConvert._CubeRouge);
                 _ecb.SetComponent<Translation>(e, new Translation { Value = _hit3.Position + new float3(0, 5, 0) });
                 e = _ecb.Instantiate(PrefabConvert._CubeRouge);
                 _ecb.SetComponent<Translation>(e, new Translation { Value = _hit4.Position + new float3(0, 5, 0) });
             }).Schedule(this.Dependency);
             Debug.Log(_dalleSelectedQuery.CalculateEntityCount() + "_dalleSelectedQueryCount");
             Debug.Log(_dalleQuery.CalculateEntityCount() + "_dalleQueryCount");

             /*Mesh _mesh = new Mesh { }

             RenderMesh huiaef = new RenderMesh { mesh = }

             this.Dependency.Complete();*/

            ColliderCastJob _colliderCastJob = new ColliderCastJob();
            _colliderCastJob.CollectAllHits = true;
            _colliderCastJob.Collider = (Unity.Physics.Collider*)_polygonCollider.GetUnsafePtr();
            _colliderCastJob.ColliderCastHits = _allHits;
            _colliderCastJob.End = new float3(0, -2, 0);// _moyPoly;
            _colliderCastJob.Orientation = quaternion.identity; //ECS_RTSControls.instance._camera.transform.rotation;
            _colliderCastJob.Start = /*_moyPoly +*/ new float3(0,5,0);
            _colliderCastJob.World = _physicsWorld;
            Dependency = _colliderCastJob.ScheduleSingle(_query, this.Dependency);

            //World.GetOrCreateSystem<StepPhysicsWorld>().FinalSimulationJobHandle.Complete();

            this.Dependency.Complete();

            /*Debug.Log(_query.CalculateEntityCount() + "_queryMovestats");
            Debug.Log(_allHits.Length + "_allHits.Length");*/
            this.Dependency = Job.WithoutBurst().WithDisposeOnCompletion(_allHits).WithCode(() =>
            {
                for (int i = 0; i < _allHits.Length; i++)
                {
                    _ecbEndSimilation.AddComponent(_allHits[i].Entity, new UnitSelected());
                    
                }
                
                  Debug.Log(  _allHits.Length + "entityHits");
                
                
            }).Schedule(this.Dependency);

            this.Dependency.Complete();


            
        }

        if (Input_Mouse_1_Up == false)
        {

            if (Time.ElapsedTime > 2)
            {
                Debug.Log("yoloZoulou");
                this.Dependency = Entities.WithoutBurst().WithAll<PlaneLayer>().WithAll<Player1>().ForEach((Entity entity) => {
                    _ecb.DestroyEntity(entity);//.RemoveComponent<UnitSelected>(entity);
                }).Schedule(this.Dependency);
            }

            //Debug.Log(Input.GetMouseButtonDown(1));
            // Right mouse button down
            float3 targetPosition = UtilsClass.GetMouseWorldPosition();
            List<float3> movePositionList = GetPositionListAround(targetPosition, new float[] { 10f, 20f, 30f }, new int[] { 5, 10, 20 });
            int positionIndex = 0;
            Entities.WithAll<UnitSelected>().ForEach((Entity entity, ref MoveStats _moveStats) => {
                /*moveTo.position = movePositionList[positionIndex];
                positionIndex = (positionIndex + 1) % movePositionList.Count;
                moveTo.move = true;*/
            }).Schedule();
        }
        //m_EntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        World.GetOrCreateSystem<StepPhysicsWorld>().FinalSimulationJobHandle.Complete();
        this.Dependency.Complete();
        m_EntityCommandBufferSystemo.AddJobHandleForProducer(this.Dependency);
        m_endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        m_EntityCommandBufferSystemo.AddJobHandleForProducer(this.Dependency);

        if (_polygonCollider.IsCreated)
        {
            _polygonCollider.Dispose();
        }
        if (_sphereCollider.IsCreated)
        {
            _sphereCollider.Dispose();
        }
        Input_Mouse_0_GetUp = false;
        Input_Mouse_0_GetDown = false;
    }

   // [BurstCompile]
    public unsafe struct ColliderCastJob : IJobChunk
    {
        [NativeDisableUnsafePtrRestriction] public Unity.Physics.Collider* Collider;
        public quaternion Orientation;
        public float3 Start;
        public float3 End;
        public NativeList<ColliderCastHit> ColliderCastHits;
        public bool CollectAllHits;
        [ReadOnly] public PhysicsWorld World;


        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            ColliderCastInput colliderCastInput = new ColliderCastInput
            {
                Collider = Collider,
                Orientation = Orientation,
                Start = Start,
                End = End
            };

            if (CollectAllHits)
            {
                World.CastCollider(colliderCastInput, ref ColliderCastHits);
            }
            else if (World.CastCollider(colliderCastInput, out ColliderCastHit hit))
            {
                ColliderCastHits.Add(hit);
            }
        }
    }

    private void GetRaycastHit(CollisionWorld _collisionWorld, UnityEngine.Ray _ray1, UnityEngine.Ray _ray2, UnityEngine.Ray _ray3, UnityEngine.Ray _ray4, UnityEngine.Ray _ray5,
        out Unity.Physics.RaycastHit _hit1, out Unity.Physics.RaycastHit _hit2, out Unity.Physics.RaycastHit _hit3, out Unity.Physics.RaycastHit _hit4, out Unity.Physics.RaycastHit _hit5)
    {
        RaycastInput _input1 = new RaycastInput()
        {
            Start = _ray1.origin,
            End = _ray1.origin + _ray1.direction * 1000,
            Filter = new CollisionFilter()
            {
                BelongsTo = (uint)10 << 10,
                CollidesWith = (uint)7 << 8,
                GroupIndex = 8
            }
        };
        RaycastInput _input2 = new RaycastInput()
        {
            Start = _ray2.origin,
            End = _ray2.origin + _ray2.direction * 1000,
            Filter = new CollisionFilter()
            {
                BelongsTo = (uint)10 << 10,
                CollidesWith = (uint)7 << 8,
                GroupIndex = 8
            }
        };
        RaycastInput _input3 = new RaycastInput()
        {
            Start = _ray3.origin,
            End = _ray3.origin + _ray3.direction * 1000,
            Filter = new CollisionFilter()
            {
                BelongsTo = (uint)10 << 10,
                CollidesWith = (uint)7 << 8,
                GroupIndex = 8
            }
        };
        RaycastInput _input4 = new RaycastInput()
        {
            Start = _ray4.origin,
            End = _ray4.origin + _ray4.direction * 1000,
            Filter = new CollisionFilter()
            {
                BelongsTo = (uint)10 << 10,
                CollidesWith = (uint)7 << 8,
                GroupIndex = 8
            }
        };
        RaycastInput _input5 = new RaycastInput()
        {
            Start = _ray5.origin,
            End = _ray5.origin + _ray5.direction * 1000,
            Filter = new CollisionFilter()
            {
                BelongsTo = (uint)10 << 10,
                CollidesWith = (uint)7 << 8,
                GroupIndex = 8
            }
        };
        var _rayd1 = _collisionWorld.CastRay(_input1, out _hit1);
        var _rayd2 = _collisionWorld.CastRay(_input2, out _hit2);
        var _rayd3 = _collisionWorld.CastRay(_input3, out _hit3);
        var _rayd4 = _collisionWorld.CastRay(_input4, out _hit4);
        var _rayd5 = _collisionWorld.CastRay(_input5, out _hit5);
        /*Debug.Log(_hit1 + "_hit1");
        Debug.Log(_hit2 + "_hit2");
        Debug.Log(_hit3 + "_hit3");
        Debug.Log(_hit4 + "_hit4");*/
        //Debug.Log(_hit5);
    }


    private List<float3> GetPositionListAround(float3 startPosition, float[] ringDistance, int[] ringPositionCount)
        {
            List<float3> positionList = new List<float3>();
            positionList.Add(startPosition);
            for (int ring = 0; ring < ringPositionCount.Length; ring++)
            {
                List<float3> ringPositionList = GetPositionListAround(startPosition, ringDistance[ring], ringPositionCount[ring]);
                positionList.AddRange(ringPositionList);
            }
            return positionList;
        }

        private List<float3> GetPositionListAround(float3 startPosition, float distance, int positionCount)
        {
            List<float3> positionList = new List<float3>();
            for (int i = 0; i < positionCount; i++)
            {
                int angle = i * (360 / positionCount);
                float3 dir = ApplyRotationToVector(new float3(0, 1, 0), angle);
                float3 position = startPosition + dir * distance;
                positionList.Add(position);
            }
            return positionList;
        }

        private float3 ApplyRotationToVector(float3 vec, float angle)
        {
            return Quaternion.Euler(0, 0, angle) * vec;
        }

    




    

}

/*public class UnitSelectedRenderer : ComponentSystem
{

    protected override void OnUpdate()
    {
        Entities.WithAll<UnitSelected>().ForEach((ref Translation translation) => {
            float3 position = translation.Value + new float3(0, -3f, +5f);
            Graphics.DrawMesh(
                ECS_RTSControls.instance.unitSelectedCircleMesh,
                position,
                Quaternion.identity,
                ECS_RTSControls.instance.unitSelectedCircleMaterial,
                0
            );
        });
    }

}*/

