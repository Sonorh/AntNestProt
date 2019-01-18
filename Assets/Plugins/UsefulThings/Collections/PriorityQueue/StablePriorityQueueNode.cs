namespace PM.UsefulThings.Collections
{
    public interface IStablePriorityQueueNode : IFastPriorityQueueNode
    {
        /// <summary>
        /// Represents the order the node was inserted in
        /// </summary>
        long InsertionIndex { get; set; }
    }
}
