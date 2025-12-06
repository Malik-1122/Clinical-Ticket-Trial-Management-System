using Microsoft.AspNetCore.Mvc.RazorPages;
using Clinical.Data;
using System.Linq;

namespace Clinical.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly ClinicalContext _context;

        public DashboardModel(ClinicalContext context)
        {
            _context = context;
        }

        public int TotalTickets { get; set; }
        public int CreatedTickets { get; set; }
        public int AssignedTickets { get; set; }
        public int ResolvedTickets { get; set; }

        public void OnGet()
        {
            // Total tickets
            TotalTickets = _context.Tickets.Count();

            // Tickets when created -> Status = "Open"
            CreatedTickets = _context.Tickets.Count(t => t.Status == "Open");

            // Tickets assigned -> AssignedTo not null or status = Assigned
           
            AssignedTickets = _context.Tickets.Count(t =>
                   (t.AssignedTo != null) ||
                    t.Status == "Assigned"
            );


            // Tickets resolved -> Status = "Closed" or "Resolved"
            ResolvedTickets = _context.Tickets.Count(t =>
                t.Status == "Closed" ||
                t.Status == "Resolved"
            );
        }
    }
}
