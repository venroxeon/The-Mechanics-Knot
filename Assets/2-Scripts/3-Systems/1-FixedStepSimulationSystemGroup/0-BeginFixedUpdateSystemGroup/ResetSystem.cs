using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(BeginFixedUpdateSystemGroup))]
[BurstCompile]
partial struct ResetSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if(DeadlockCautionSingleton.Instance != null)
            DeadlockCautionSingleton.Instance.gameObject.SetActive(false);

        ScheduleJobs(ref state);
    }

    [BurstCompile]
    public void ScheduleJobs(ref  SystemState state)
    {
        RopeResetJob ropeResetJob = new();
        state.Dependency = ropeResetJob.ScheduleParallel(state.Dependency);

        RopeSegResetJob segResetJob = new();
        state.Dependency = segResetJob.ScheduleParallel(state.Dependency);

        NodeRotCompResetJob nodeRoCompResetJob = new();
        state.Dependency = nodeRoCompResetJob.ScheduleParallel(state.Dependency);

        NodeBufferResetJob nodeBufferResetJob = new();
        state.Dependency = nodeBufferResetJob.ScheduleParallel(state.Dependency);
    }
}
