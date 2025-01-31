using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(InputSystemGroup))]
[BurstCompile]
public partial struct InputSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<InputComp>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        InputSystemManager input = InputSystemManager.Instance;

        var firstTouchPos = input.GetFirstTouchPos();
        var secTouchPos = input.GetSecondTouchPos();

        SystemAPI.SetSingleton(new InputComp() 
        { 
            isFirstTouchPress = input.isFirstTouchPress, 
            firstTouchPos = firstTouchPos,
            isSecTouchPress = input.isSecondTouchPress,
            secTouchPos = secTouchPos
        });
    }
}