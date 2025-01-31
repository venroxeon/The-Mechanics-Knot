//using System;
//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Mathematics;
//using Unity.Profiling;
//using UnityEditor.PackageManager;
//using static Unity.Entities.SystemAPI;

//[BurstCompile]
//partial struct Test1 : ISystem
//{
//    int dir, speed;
//    [BurstCompile]
//    public void OnCreate(ref SystemState state)
//    {
//        check = false;
//        speed = 70;
//        dir = 1;
//        state.RequireForUpdate<NodeRotatorComp>();
//    }
//    bool check;
//    [BurstCompile]
//    public void OnUpdate(ref SystemState state)
//    {
//        InputComp input = GetSingleton<InputComp>();
//        if (!input.isFirstTouchPress)
//        {
//            check = false;
//            return;
//        }
//        if (check)
//            return;
//        dir *= -1;
//        speed += 10;
//        check = true;
//        foreach (var nodeRot in Query<RefRW<NodeRotatorComp>>())
//        {
//            if (nodeRot.ValueRO.type == CollType.Main)
//                nodeRot.ValueRW.rotAngleUnscaled = math.radians(speed) * dir;
//        }
//    }
//}