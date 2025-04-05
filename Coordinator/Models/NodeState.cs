using Coordinator.Enums;

namespace Coordinator.Models
{
    public record NodeState(Guid TransactionID)
    {
        public Guid NodeStateID { get; set; }

        /// <summary>
        /// 1. aşama durumunu belirler.
        /// </summary>
        public ReadyType IsReady { get; set; } 

        /// <summary>
        ///  2.aşama neticesinde işlemin tamamlanıp tamamlanmadığını ifade eder.
        /// </summary>
        public TransactionStateType TransactionState { get; set; }

        public Node Node { get; set; } 


    }
}
