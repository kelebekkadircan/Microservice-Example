namespace Coordinator.Models
{
    public record Node(string Name)
    {
        public Guid NodeID { get; set; }

        public ICollection<NodeState> NodeStates { get; set; }

        //public string Name { get; set; } = string.Empty;




    }
}
