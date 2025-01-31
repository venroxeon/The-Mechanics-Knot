using Unity.Entities;
using static Unity.Entities.SystemAPI;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct FixedStepInitializationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (TryGetSingleton<FixedStepSimulationParamComp>(out var fixedStepComp))
        {
            state.Enabled = false;
            state.World.GetExistingSystemManaged<FixedStepSimulationSystemGroup>().Timestep = fixedStepComp.fixedDeltaTime;
        }
    }
}