using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinical.Models
{
    public class TicketAllocation
    {
        [Key]
        public int AllocationID { get; set; }

        [ForeignKey("Ticket")]
        public int TicketID { get; set; }
        public Ticket? Ticket { get; set; }

        [ForeignKey("AssignedToUser")]
        public int AssignedTo { get; set; }
        public Users? AssignedToUser { get; set; }

        [ForeignKey("AssignedByUser")]
        public int AssignedBy { get; set; }
        public Users? AssignedByUser { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.Now;

        public string? Remarks { get; set; }
    }
}
