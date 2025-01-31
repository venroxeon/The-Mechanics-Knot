using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public partial struct RopeResetJob : IJobEntity
{
    [BurstCompile]
    public void Execute(ref DynamicBuffer<RopeNodeEntityBuffer> nodeBuffer)
    {
        nodeBuffer.Clear();
    }
}
