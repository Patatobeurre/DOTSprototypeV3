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



    public class InputUpdate : MonoBehaviour
    {
        EntityManager _entityManager;

        EntityQuery _query;
        Entity _inputEntity;

        BeginInitializationEntityCommandBufferSystem _beginInitializationEntityCommandBufferSystem;


        void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _inputEntity = _entityManager.CreateEntity(typeof(InputComponent));
            _query = _entityManager.CreateEntityQuery(typeof(InputComponent));

            _beginInitializationEntityCommandBufferSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }


        void Update()
        {
            EntityCommandBuffer _ecb = _beginInitializationEntityCommandBufferSystem.CreateCommandBuffer();


            miniJob _miniJob = new miniJob
            {
                Input_A = Input.GetKey(KeyCode.A),
                Input_Z = Input.GetKey(KeyCode.Z),
                Input_Mouse_0_Down = Input.GetMouseButton(0),
                Input_Mouse_1_Down = Input.GetMouseButton(1),
                Input_Mouse_0_Up = !Input.GetMouseButton(0),
                Input_Mouse_1_Up = !Input.GetMouseButton(1),
                _ecb = _ecb,
                _entityManager = _entityManager,
                _inputEntity = _inputEntity
            };

            JobHandle _handle = _miniJob.ScheduleSingle(_query);
            _handle.Complete();

        _beginInitializationEntityCommandBufferSystem.AddJobHandleForProducer(_handle);
        }

        


    }
    public struct miniJob : IJobChunk
    {
        public bool Input_Z;
        public bool Input_A;
        public bool Input_Mouse_0_Down;
        public bool Input_Mouse_1_Down;
        public bool Input_Mouse_0_Up;
        public bool Input_Mouse_1_Up;
        public EntityCommandBuffer _ecb;
        public EntityManager _entityManager;
        public Entity _inputEntity;


        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {

                if (Input_Mouse_0_Down && !_entityManager.HasComponent<Input_Mouse_0_Down>(_inputEntity))
                {
                    _ecb.AddComponent(_inputEntity, new Input_Mouse_0_Down());
                }
                if (!Input_Mouse_0_Down && _entityManager.HasComponent<Input_Mouse_0_Down>(_inputEntity))
                {
                    _ecb.RemoveComponent(_inputEntity, typeof(Input_Mouse_0_Down));
                }
            


                if (Input_Mouse_1_Down && !_entityManager.HasComponent<Input_Mouse_1_Down>(_inputEntity))
                {
                    _ecb.AddComponent(_inputEntity, new Input_Mouse_1_Down());
                }
                if (!Input_Mouse_1_Down && _entityManager.HasComponent<Input_Mouse_1_Down>(_inputEntity))
                {
                    _ecb.RemoveComponent(_inputEntity, typeof(Input_Mouse_1_Down));
                }
            


                if (Input_Mouse_0_Up && !_entityManager.HasComponent<Input_Mouse_0_Up>(_inputEntity))
                {
                    _ecb.AddComponent(_inputEntity, new Input_Mouse_0_Up());
                }
                if (!Input_Mouse_0_Up && _entityManager.HasComponent<Input_Mouse_0_Up>(_inputEntity))
                {
                    _ecb.RemoveComponent(_inputEntity, typeof(Input_Mouse_0_Up));
                }
            


                if (Input_Mouse_1_Up && !_entityManager.HasComponent<Input_Mouse_1_Up>(_inputEntity))
                {
                    _ecb.AddComponent(_inputEntity, new Input_Mouse_1_Up());
                }
                if (!Input_Mouse_1_Up && _entityManager.HasComponent<Input_Mouse_1_Up>(_inputEntity))
                {
                    _ecb.RemoveComponent(_inputEntity, typeof(Input_Mouse_1_Up));
                }
            


                if (Input_A && !_entityManager.HasComponent<Input_A>(_inputEntity))
                {
                    _ecb.AddComponent(_inputEntity, new Input_A());
                }
                if (!Input_A && _entityManager.HasComponent<Input_A>(_inputEntity))
                {
                    _ecb.RemoveComponent(_inputEntity, typeof(Input_A));
                }
            


                if (Input_Z && !_entityManager.HasComponent<Input_Z>(_inputEntity))
                {
                    _ecb.AddComponent(_inputEntity, new Input_Z());
                }
                if (!Input_Z && _entityManager.HasComponent<Input_Z>(_inputEntity))
                {
                    _ecb.RemoveComponent(_inputEntity, typeof(Input_Z));
                }
            
        }
    }

