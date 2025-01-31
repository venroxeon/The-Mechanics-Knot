//using Unity.Burst;
//using Unity.Collections;
//using Unity.Collections.LowLevel.Unsafe;
//using Unity.Entities;

//[BurstCompile]
//public partial class CustomCommandBufferSystems : EntityCommandBufferSystem
//{
//    [BurstCompile]
//    public unsafe struct Singleton : IComponentData, IECBSingleton
//    {
//        internal UnsafeList<EntityCommandBuffer>* pendingBuffers;
//        internal AllocatorManager.AllocatorHandle allocator;

//        public EntityCommandBuffer CreateCommandBuffer(WorldUnmanaged world)
//        {
//            return EntityCommandBufferSystem
//                .CreateCommandBuffer(ref *pendingBuffers, allocator, world);
//        }

//        public void SetPendingBufferList(ref UnsafeList<EntityCommandBuffer> buffers)
//        {
//            var ptr = UnsafeUtility.AddressOf(ref buffers);
//            pendingBuffers = (UnsafeList<EntityCommandBuffer>*)ptr;
//        }

//        public void SetAllocator(Allocator allocatorIn)
//        {
//            allocator = allocatorIn;
//        }

//        public void SetAllocator(AllocatorManager.AllocatorHandle allocatorIn)
//        {
//            allocator = allocatorIn;
//        }
//    }

//    protected override void OnCreate()
//    {
//        base.OnCreate();

//        this.RegisterSingleton<Singleton>(ref PendingBuffers, World.Unmanaged);
//    }
//}
