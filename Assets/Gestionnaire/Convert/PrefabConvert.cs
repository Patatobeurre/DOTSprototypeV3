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

[AlwaysUpdateSystem]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateBefore(typeof(InitDecision))]
public class PrefabConvert : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public static PrefabConvert instance;

    public GameObject _fullBody;
    public GameObject _meshFilterAndRenderer;
    public GameObject _cubeRouge;
    public GameObject _dalleGrize;
    public GameObject _dalleVerte;

    public static Entity _FullBody;
    public static Entity _MeshFilterAndRenderer;
    public static Entity _CubeRouge;
    public static Entity _DalleGrize;
    public static Entity _DalleVerte;

    private void Awake()
    {
        instance = this;
    }
    void IDeclareReferencedPrefabs.DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(_fullBody);
        referencedPrefabs.Add(_meshFilterAndRenderer);
        referencedPrefabs.Add(_cubeRouge);
        referencedPrefabs.Add(_dalleGrize);
        referencedPrefabs.Add(_dalleVerte);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //Get primary entity of the prefab asset, to assign to primary entity converted from this component.
        _FullBody = conversionSystem.GetPrimaryEntity(_fullBody);
        _MeshFilterAndRenderer = conversionSystem.GetPrimaryEntity(_meshFilterAndRenderer);
        _CubeRouge = conversionSystem.GetPrimaryEntity(_cubeRouge);
        _DalleGrize = conversionSystem.GetPrimaryEntity(_dalleGrize);
        _DalleVerte = conversionSystem.GetPrimaryEntity(_dalleVerte);
        //FullBody = prefabAssetEntity;
        //Remember it, etc.
    }

}
