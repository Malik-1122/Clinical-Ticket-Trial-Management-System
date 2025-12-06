using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Clinical.Data;
using Clinical.Models;
using System.Linq;
using System.Security.Claims;

namespace Clinical.Pages.Resolver
{
    public class TicketDetailsModel : PageModel
    {
        private readonly ClinicalContext _context;

        public TicketDetailsModel(ClinicalContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ticket Ticket { get; set; }

        [BindProperty]
        public string? SelectedOfficerRole { get; set; }

        [BindProperty]
        public string? ResolutionNotes { get; set; }

        public string CategoryName { get; set; } = "";

        public IActionResult OnGet(int id)
        {
            Ticket = _context.Tickets.FirstOrDefault(t => t.TicketID == id);
            if (Ticket == null) return RedirectToPage("/Resolver/MyTickets");

            // Load category name from Category table
            if (Ticket.CategoryID != null)
            {
                CategoryName = _context.Category
                    .Where(c => c.CategoryID == Ticket.CategoryID)
                    .Select(c => c.CategoryName)
                    .FirstOrDefault() ?? "";
            }

            return Page();
        }

        // Save Officer Role
        public IActionResult OnPostSelectOfficer(int id)
        {
            var ticket = _context.Tickets.FirstOrDefault(t => t.TicketID == id);
            if (ticket == null) return RedirectToPage("/Resolver/MyTickets");

            ticket.OfficerRole = SelectedOfficerRole;
            ticket.UpdatedDate = DateTime.Now;

            _context.SaveChanges();

            return RedirectToPage("/Resolver/TicketDetails", new { id });
        }

        // Resolve Ticket
        public IActionResult OnPostResolve(int id)
        {
            var ticket = _context.Tickets.FirstOrDefault(t => t.TicketID == id);
            if (ticket == null) return RedirectToPage("/Resolver/MyTickets");

            ticket.Status = "Resolved";
            ticket.UpdatedDate = DateTime.Now;
            ticket.ClosedDate = DateTime.Now;

            // Save resolution notes if needed
            ticket.Description += "\n\n[Resolver Notes]: " + ResolutionNotes;

            _context.SaveChanges();

            return RedirectToPage("/Resolver/MyTickets");
        }
    }
}
