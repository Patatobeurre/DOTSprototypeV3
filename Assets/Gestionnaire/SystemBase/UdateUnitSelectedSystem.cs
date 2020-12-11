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
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateBefore(typeof(BeginInitializationEntityCommandBufferSystem))]
public class UdateUnitSelectedSystem : SystemBase
{

    BeginInitializationEntityCommandBufferSystem m_beginSimulationEntityCommandBufferSystem;

    EntityQuery _queryGris;
    EntityQuery _queryVert;
    EntityQueryDesc _queryDesc;

    float4 _vert;
    float4 _gris;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_beginSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

        _queryDesc = new EntityQueryDesc {
            Any = new ComponentType[] { typeof(UnitSelected) },
            All = new ComponentType[]{ typeof(Selectable) ,typeof(FloorTag), typeof(CustomColor)}
        };
        _queryGris = GetEntityQuery(_queryDesc);
        //_queryVert = GetEntityQuery(_queryDesc); 

        _vert = new float4 { x = 0.01179f, y = 0.5f, z = 0.1044553f, w = 0f };
        _gris = new float4 { x = 0.3985849f, y = 0.4400764f, z = 0.5f, w = 0.7960784f };
    }

    protected override void OnUpdate()
    {
        if (Time.ElapsedTime > 4)
        {
            EntityCommandBuffer _ecbBeginPresentation = m_beginSimulationEntityCommandBufferSystem.CreateCommandBuffer();

            var _eGris = PrefabConvert._DalleGrize;
      //      var _dalleGrisColor = EntityManager.GetComponentData<MaterialColor>(_eGris).Value;
     
            var _eVert = PrefabConvert._DalleVerte;
       //     var _dalleVertColor = EntityManager.GetComponentData<MaterialColor>(_eVert).Value;

           // _queryGris.SetSharedComponentFilter<RenderMesh>(_sharedComponentIndexGris);
            Debug.Log(_queryGris.CalculateEntityCount() + "_queryDALLE SELECTUPDATE.CalculateEntityCount()");

            //   _queryVert.SetSharedComponentFilter<RenderMesh>(_sharedComponentIndexVert);
            // Debug.Log(_queryVert.CalculateEntityCount() + "_queryDALLE SELECTUPDATE.CalculateEntityCount()");






            /* this.Dependency = Entities.WithoutBurst().ForEach((in Translation _translation, in Selectable _selectable, in VertTag _vertTag, in Entity _entity) =>
             {
                 var e = _ecbBeginPresentation.Instantiate(PrefabConvert._DalleGrize);
                 _ecbBeginPresentation.SetComponent<Translation>(e, _translation);
                 _ecbBeginPresentation.DestroyEntity(_entity);
                 _ecbBeginPresentation.
                 Debug.Log("Unity3d : 'va te faire foutre LeRetour'");

             }).Schedule(this.Dependency);*/
            this.Dependency =   Entities.ForEach((ref CustomColor color, in FloorTag _fT) =>
            {
                color.Value = new float4 { x = 0.3985849f, y = 0.4400764f, z = 0.5f, w = 0.7960784f };
            })
            .Schedule(this.Dependency);

            this.Dependency =    Entities.ForEach((ref CustomColor color, in UnitSelected unitSelected, in FloorTag _fT) =>
            {
                color.Value = new float4 { x = 0.01179f, y = 0.5f, z = 0.1044553f, w = 0f };
            })
            .Schedule(this.Dependency);




            if (_queryGris.CalculateEntityCount() > 0)
        {

           /* PresentationJob _presentationJobGris = new PresentationJob();
            _presentationJobGris._ecbBeginPresentation = _ecbBeginPresentation;
            _presentationJobGris._entityTypeHandle = GetEntityTypeHandle();
            _presentationJobGris._unitSelectedHandle = GetComponentTypeHandle<UnitSelected>();
                _presentationJobGris._CustomColorTypeHandle = GetComponentTypeHandle<CustomColor>();
                _presentationJobGris._grisColor = _vert ;// _dalleGrisColor;
                _presentationJobGris._vertColor = _gris;// _dalleVertColor;

                this.Dependency =  _presentationJobGris.ScheduleSingle(_queryGris, this.Dependency);*/

        }

        }

        m_beginSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
    }
}

public struct PresentationJob : IJobChunk
{
     public  EntityCommandBuffer _ecbBeginPresentation;
     [ReadOnly] public EntityTypeHandle _entityTypeHandle;
     public ComponentTypeHandle<CustomColor> _CustomColorTypeHandle;
    [ReadOnly] public ComponentTypeHandle<UnitSelected> _unitSelectedHandle;
    public float4 _grisColor;
    public float4 _vertColor;

    void IJobChunk.Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
    {
        NativeArray<Entity> _entityArray = chunk.GetNativeArray(_entityTypeHandle);
        NativeArray<CustomColor> _customColorArray = chunk.GetNativeArray(_CustomColorTypeHandle);


        if (chunk.Has<UnitSelected>(_unitSelectedHandle) == true)
        {

            for (int i = 0; i < chunk.Count; i++)
            {
                if (!_customColorArray[i].Value.Equals(_vertColor))
                {
                   _customColorArray[i] = new CustomColor { Value = _vertColor };
                }
                
            }
        }
        if (chunk.Has<UnitSelected>(_unitSelectedHandle) == false)
        {

            for (int i = 0; i < chunk.Count; i++)
            {
                if (!_customColorArray[i].Value.Equals(_grisColor))
                {
                    _customColorArray[i] = new CustomColor { Value = _grisColor };
                }
            }

        }
    }
}
