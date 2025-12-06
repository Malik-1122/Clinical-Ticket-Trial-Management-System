using Microsoft.AspNetCore.Mvc.RazorPages;
using Clinical.Data;
using System.Linq;
using System.Security.Claims;

namespace Clinical.Pages.Resolver
{
    public class DashboardModel : PageModel
    {
        private readonly ClinicalContext _context;

        public DashboardModel(ClinicalContext context)
        {
            _context = context;
        }

        public int TotalAssignedTickets { get; set; }
        public int OpenTickets { get; set; }
        public int ResolvedByMe { get; set; }
        public int TotalResolved { get; set; }

        public void OnGet()
        {
            // Get logged-in Resolver ID from claims
            string resolverIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(resolverIdString))
                return;

            int resolverId = Convert.ToInt32(resolverIdString);

            // Tickets assigned to this resolver
            TotalAssignedTickets = _context.Tickets.Count(t =>
                t.AssignedTo == resolverId
            );

            // Tickets assigned to resolver and still open
            OpenTickets = _context.Tickets.Count(t =>
                t.AssignedTo == resolverId &&
                t.Status == "Open"
            );

            // Tickets resolved by this resolver
            ResolvedByMe = _context.Tickets.Count(t =>
                t.AssignedTo == resolverId &&
                (t.Status == "Closed" || t.Status == "Resolved")
            );

            // Total resolved tickets (system-wide)
            TotalResolved = _context.Tickets.Count(t =>
                t.Status == "Closed" || t.Status == "Resolved"
            );
        }
    }
}
