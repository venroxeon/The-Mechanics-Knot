using Unity.Entities;

public struct JumpToLevelComp : IComponentData
{
    public int targetLevelIndex;
}
