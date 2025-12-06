using System;
using System.ComponentModel.DataAnnotations;

namespace Clinical.Models
{
    public class AuditLog
    {
        [Key]
        public int LogID { get; set; }

        public int TicketID { get; set; }

        [StringLength(200)]
        public string? ActionTaken { get; set; }

        public int? ActionBy { get; set; }

        public DateTime? ActionDate { get; set; }
    }
}
