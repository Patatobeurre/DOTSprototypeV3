/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using System.Threading;
using CodeMonkey.Utils;
using System.Linq;
using Unity.Physics.Systems;
using Unity.Physics;



public class ECS_RTSControls : MonoBehaviour {

    public static ECS_RTSControls instance;

    //[SerializeField] private CameraFollow cameraFollow;
    private Vector3 cameraFollowPosition;
    private float cameraFollowZoom;

    public Mesh quadMesh;
    public Mesh shadowMesh;
    public RectTransform selectionAreaTransform;
    public Mesh unitSelectedCircleMesh;
    public GameObject _selectionArea;
    public Camera _camera;
    public GameObject _Cubeplane;

    public float3 _startScreenPosition;
    public float3 _endScreenPosition;
    public float3 _lowerLeftScreenPosition;
    public float3 _upperRightScreenPosition;

    public float3 _startPosition;
    public float3 _endPosition;
    public float3 _lowerLeftPosition;
    public float3 _upperRightPosition;

    public UnityEngine.Ray _ray1;
    public UnityEngine.Ray _ray2;
    public UnityEngine.Ray _ray3;
    public UnityEngine.Ray _ray4;
    public UnityEngine.Ray _ray5;

    public bool _GetMouseButtonUp0;
    public bool _GetMouseButtonUp1;
    public bool _GetMouseButtonDown0;
    public bool _GetMouseButtonDown1;
    public bool _GetMouseButton;

    private void Awake() {
        instance = this;
    }

    private void Start() {


    }



    private void Update() 
    {
        _GetMouseButtonUp0 = Input.GetMouseButtonUp(0);
        _GetMouseButtonUp0 = Input.GetMouseButtonUp(1);
        _GetMouseButtonDown0 = Input.GetMouseButtonDown(0);
        _GetMouseButtonDown0 = Input.GetMouseButtonDown(1);

        var physicsWorldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
        var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;

        Debug.Log("JeamMichelPougnouf");

        NativeList<Unity.Physics.RaycastHit> _allHitso = new NativeList<Unity.Physics.RaycastHit>(Allocator.TempJob);

        RaycastInput _inputo = new RaycastInput()
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
        bool haveHit = collisionWorld.CastRay(_inputo, ref _allHitso);
        


        if (haveHit)
        {
            for (int i = 0; i < _allHitso.Length; i++)
            {
                Entity e = physicsWorldSystem.PhysicsWorld.Bodies[_allHitso[i].RigidBodyIndex].Entity;
                //Debug.Log(e + "_allHits[i].Position");
            }
            
        }


        if (Input.GetMouseButtonDown(0))
        {
            ECS_RTSControls.instance._selectionArea.SetActive(true);
            _startScreenPosition = Input.mousePosition;
            _ray1 = ECS_RTSControls.instance._camera.ScreenPointToRay(_startScreenPosition);
            Unity.Physics.RaycastHit _hit1 = new Unity.Physics.RaycastHit();
            RaycastInput _input = new RaycastInput()
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
            collisionWorld.CastRay(_input, out _hit1);
            _startPosition = _hit1.Position;
            ECS_RTSControls.instance.selectionAreaTransform.position = _ray1.origin + _ray1.direction*5;
        }

        if (Input.GetMouseButton(0))
        {
            float3 _currentPosition = Input.mousePosition;



            _ray5 = ECS_RTSControls.instance._camera.ScreenPointToRay(_currentPosition);

            float3 selectionAreaSize = _currentPosition - _startScreenPosition;
            ECS_RTSControls.instance.selectionAreaTransform.localScale = selectionAreaSize/13;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _endScreenPosition = Input.mousePosition;
            _lowerLeftScreenPosition = new float3(math.min(_startScreenPosition.x, _endScreenPosition.x), math.min(_startScreenPosition.y, _endScreenPosition.y), 0);
            _upperRightScreenPosition = new float3(math.max(_startScreenPosition.x, _endScreenPosition.x), math.max(_startScreenPosition.y, _endScreenPosition.y), 0);
            _ray2 = ECS_RTSControls.instance._camera.ScreenPointToRay(_lowerLeftScreenPosition);
            _ray3 = ECS_RTSControls.instance._camera.ScreenPointToRay(_upperRightScreenPosition);
            ECS_RTSControls.instance._selectionArea.SetActive(false);
            _ray4 = ECS_RTSControls.instance._camera.ScreenPointToRay(_endScreenPosition);
        }
        

        HandleCamera();

        if (_allHitso.IsCreated) { _allHitso.Dispose();}
    }

    private void HandleCamera() {
        Vector3 moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) { moveDir.y = +1f; }
        if (Input.GetKey(KeyCode.S)) { moveDir.y = -1f; }
        if (Input.GetKey(KeyCode.A)) { moveDir.x = -1f; }
        if (Input.GetKey(KeyCode.D)) { moveDir.x = +1f; }

        moveDir = moveDir.normalized;
        float cameraMoveSpeed = 300f;
        cameraFollowPosition += moveDir * cameraMoveSpeed * Time.deltaTime;

        float zoomSpeed = 1500f;
        if (Input.mouseScrollDelta.y > 0) cameraFollowZoom -= 1 * zoomSpeed * Time.deltaTime;
        if (Input.mouseScrollDelta.y < 0) cameraFollowZoom += 1 * zoomSpeed * Time.deltaTime;

        cameraFollowZoom = Mathf.Clamp(cameraFollowZoom, 20f, 200f);
    }

    public static float GetCameraShakeIntensity() {
        float intensity = Mathf.Clamp(.7f - instance.cameraFollowZoom / 170f, .0f, 2f);
        return intensity;
    }

}

public class MoveUnitSystem : SystemBase
{
    protected override void OnUpdate()
    {
        
    }
    public struct MoveJob : IJobChunk
    {
        public float deltaTime;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
         /*   NativeArray
    
        if (moveTo.move)
            {
                float reachedPositionDistance = 1f;
                if (math.distance(translation.Value, moveTo.position) > reachedPositionDistance)
                {
                    // Far from target position, Move to position
                    float3 moveDir = math.normalize(moveTo.position - translation.Value);
                    moveTo.lastMoveDir = moveDir;
                    translation.Value += moveDir * moveTo.moveSpeed * deltaTime;
                    skeletonPlayAnim.PlayAnim(ECS_UnitAnimType.TypeEnum.dMarine_Walk, moveDir, default);
                }
                else
                {
                    // Already there
                    skeletonPlayAnim.PlayAnim(ECS_UnitAnimType.TypeEnum.dMarine_Idle, moveTo.lastMoveDir, default);
                    moveTo.move = false;
                }
            }*/
        }
    }
}
// Unit go to Move Position



