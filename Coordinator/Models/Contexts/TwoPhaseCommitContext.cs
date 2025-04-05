using Microsoft.EntityFrameworkCore;

namespace Coordinator.Models.Contexts
{
    public class TwoPhaseCommitContext : DbContext
    {
        public TwoPhaseCommitContext(DbContextOptions<TwoPhaseCommitContext> options) : base(options)
        {
        }


        public DbSet<Node> Nodes { get; set; }
        public DbSet<NodeState> NodeStates { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Node>().HasData(

                   new Node("ORDER.API") { NodeID = Guid.NewGuid() },
                   new Node("STOCK.API") { NodeID = Guid.NewGuid() },
                   new Node("PAYMENT.API") { NodeID = Guid.NewGuid() }
               );
        }

    }
}
