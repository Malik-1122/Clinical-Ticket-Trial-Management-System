using System;
using System.ComponentModel.DataAnnotations;

namespace Clinical.Models
{
    public class Ticket
    {
        [Key]
        public int TicketID { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }

        public int? CategoryID { get; set; }
        public int? PriorityID { get; set; }

        public string? Category { get; set; }
        public string? Priority { get; set; }

        public string? Status { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }

        public string? AdminRole { get; set; }

        // FIXED
        public int? AssignedTo { get; set; }

        public string? OfficerRole { get; set; }
    }
}
