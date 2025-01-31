using Unity.Mathematics;
using Unity.Burst;
using Unity.Entities;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using Unity.Physics;

namespace CustomUtilities
{
    [BurstCompile]
    public struct RopePhysicsUtilities
    {
        [BurstCompile]
        public static bool CheckDist(in float segRadius, in float colRadius, in float2 colCenter, in float2 segPos, out float dist, out float2 delta)
        {
            delta = segPos - colCenter;

            dist = math.distance(segPos, colCenter);
            delta /= dist;

            if (dist < (colRadius + segRadius + 0.001f))
                return true;
            else
                return false;
        }

        [BurstCompile]
        public static bool CheckDistAndRespond(in bool isTrigger, in float segRadius, in float3 _colCenter, in float colRadius, ref TempLocalTransform tempTrfm, ref RopeSegCollisionComp segCollComp)
        {
            var colCenter = new float2(_colCenter.x, _colCenter.y);
            var segPos = new float2(tempTrfm.Position.x, tempTrfm.Position.y);

            if (CheckDist(segRadius, colRadius, colCenter, segPos, out var dist, out var delta))
            {
                if(!isTrigger)
                {
                    var newPos = colCenter + (delta * (colRadius + segRadius));
                    tempTrfm.Position.x = newPos.x;
                    tempTrfm.Position.y = newPos.y;
                    //segPhysComp.prevPos = segPhysComp.curPos;

                    segCollComp.isCol = true;
                }

                segCollComp.hasEntered = true;

                return true;
            }
            else
            {
                //segPhysComp.prevPos = segPhysComp.curPos;
                segCollComp.isCol = false;
                segCollComp.hasEntered = false;
            }

            return false;
        }        
        
        [BurstCompile]
        public static bool CheckDistAndRespond(in bool isTrigger, out float segToColDist, in float segRadius, in float3 _colCenter, in float colRadius, ref TempLocalTransform tempTrfm, ref RopeSegCollisionComp segCollComp)
        {
            var colCenter = new float2(_colCenter.x, _colCenter.y);
            var segPos = new float2(tempTrfm.Position.x, tempTrfm.Position.y);

            if (CheckDist(segRadius, colRadius, colCenter, segPos, out var dist, out var delta))
            {
                if(!isTrigger)
                {
                    var newPos = colCenter + (delta * (colRadius + segRadius));
                    tempTrfm.Position.x = newPos.x;
                    tempTrfm.Position.y = newPos.y;
                    //segPhysComp.prevPos = segPhysComp.curPos;

                    segCollComp.isCol = true;
                }

                segCollComp.hasEntered = true;

                segToColDist = dist;
                return true;
            }
            else
            {
                //segPhysComp.prevPos = segPhysComp.curPos;

                segCollComp.isCol = false;
                segCollComp.hasEntered = false;
            }

            segToColDist = dist;
            return false;
        }
    }

    [BurstCompile]
    public struct Utilities
    {
        [BurstCompile]
        public static void UpdateLinkedEntityGroup(ref DynamicBuffer<LinkedEntityGroup> linkedBuffer, in EntityStorageInfoLookup storageLookup)
        {
            for (int i = linkedBuffer.Length - 1; i >= 0; i--)
            {
                if (!storageLookup.Exists(linkedBuffer[i].Value))
                {
                    linkedBuffer.RemoveAt(i);
                }
            }
        }

        [BurstCompile]
        public static bool DoIntersect(in float3 p1, in float3 q1, in float3 p2, in float3 q2)
        {
            int o1 = Orientation(p1, q1, p2);
            int o2 = Orientation(p1, q1, q2);
            int o3 = Orientation(p2, q2, p1);
            int o4 = Orientation(p2, q2, q1);

            if (o1 != o2 && o3 != o4)
                return true;

            return false;
        }

        [BurstCompile]
        static int Orientation(in float3 p, in float3 q, in float3 r)
        {
            float val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);
            if (val == 0)
                return 0; // collinear
            return (val > 0) ? 1 : -1; // clock or counterclock wise
        }

        [BurstCompile]
        public static bool Raycast(in float3 RayFrom, in float3 RayTo, out Entity hitEntity, in CollisionWorld colWorld, in uint layer = ~0u)
        {
            RaycastInput input = new RaycastInput()
            {
                Start = RayFrom,
                End = RayTo,
                Filter = new CollisionFilter()
                {
                    BelongsTo = ~0u,
                    CollidesWith = layer,
                    GroupIndex = 0
                }
            };

            if (colWorld.CastRay(input, out var hit))
            {
                hitEntity = hit.Entity;
                return true;
            }
            else
            {
                hitEntity = default;
                return false;
            }
        }

        //[BurstCompile]
        //public static bool Raycast(in float3 RayFrom, in float3 RayTo, in Entity ignoreEntity, out Entity hitEntity, in CollisionWorld colWorld, in uint layer = ~0u)
        //{
        //    RaycastInput input = new RaycastInput()
        //    {
        //        Start = RayFrom,
        //        End = RayTo,
        //        Filter = new CollisionFilter()
        //        {
        //            BelongsTo = ~0u,
        //            CollidesWith = layer,
        //            GroupIndex = 0
        //        }
        //    };

        //    IgnoreEntityRaycast<RaycastHit> collector = new(ignoreEntity);

        //    if (colWorld.CastRay(input, ref collector))
        //    {
        //        hitEntity = collector.hitEntity;
        //        return true;
        //    }
        //    else
        //    {
        //        hitEntity = default;
        //        return false;
        //    }
        //}
    }
}
