using System;
using System.ComponentModel.DataAnnotations;

namespace Clinical.Models
{
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        public int TicketID { get; set; }
        public int UserID { get; set; }

        [StringLength(200)]
        public string Message { get; set; }

        [StringLength(20)]
        public string? Status { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}
