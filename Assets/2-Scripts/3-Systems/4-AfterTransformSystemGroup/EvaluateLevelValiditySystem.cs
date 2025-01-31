using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(AfterTransformSystemGroup))]
[BurstCompile]
partial struct EvaluateLevelValiditySystem : ISystem
{
    EntityQuery levelQuery;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        levelQuery = QueryBuilder().WithAll<LevelManagerDataComp, LevelBuffer>().Build();
        
        state.RequireForUpdate(levelQuery);
        state.RequireForUpdate<CollidableComp>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var levelManagerComp = GetSingleton<LevelManagerDataComp>();

        var curLevelEntity = levelManagerComp.curLevelEntity;
        
        bool isValid = true;
        
        if(!HasComponent<DeadlockActiveTag>(curLevelEntity))
        {
            foreach(var (_nodeRotComp, evaluationComp, _collComp) in Query<RefRO<NodeRotatorComp>, RefRO<NodeEvaluationDataComp>, RefRO<CollidableComp>>())
            {
                var nodeRotComp = _nodeRotComp.ValueRO;
                var collComp = _collComp.ValueRO;

                if(nodeRotComp.rotAngleCurFrame == 0 || math.sign(nodeRotComp.rotAngleCurFrame) != math.sign(evaluationComp.ValueRO.targetRotDir))
                {
                    isValid = false;
                }
            }

        }
        else
        {
            isValid = false;
        }
        
        if(isValid)
        {
            state.EntityManager.AddComponent<LevelValidTag>(curLevelEntity);
        }
        else if(HasComponent<LevelValidTag>(curLevelEntity))
        {
            state.EntityManager.RemoveComponent<LevelValidTag>(curLevelEntity);
        }
    }
}
