using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Clinical.Data;
using Clinical.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace Clinical.Pages.Resolver
{
    public class ResolveTicketModel : PageModel
    {
        private readonly ClinicalContext _context;

        public ResolveTicketModel(ClinicalContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ticket Ticket { get; set; }

        [BindProperty]
        public string ResolutionNotes { get; set; }

        public IActionResult OnGet(int id)
        {
            Ticket = _context.Tickets.FirstOrDefault(t => t.TicketID == id);

            if (Ticket == null)
                return RedirectToPage("/Resolver/MyTickets");

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            var ticket = _context.Tickets.FirstOrDefault(t => t.TicketID == id);

            if (ticket == null)
                return RedirectToPage("/Resolver/MyTickets");

            string resolverName = User.FindFirstValue(ClaimTypes.Name);
            int resolverId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Update ticket
            ticket.Status = "Resolved";
            ticket.UpdatedDate = DateTime.Now;
            ticket.ClosedDate = DateTime.Now;

            // Audit Log
            _context.AuditLog.Add(new AuditLog
            {
                TicketID = id,
                ActionTaken = "Resolved: " + ResolutionNotes,
                ActionBy = resolverId,
                ActionDate = DateTime.Now
            });

            // Notification
            _context.Notification.Add(new Notification
            {
                TicketID = id,
                UserID = ticket.CreatedBy ?? 0,
                Message = $"Ticket #{id} resolved by {resolverName}",
                Status = "Sent",
                Timestamp = DateTime.Now
            });

            _context.SaveChanges();

            return RedirectToPage("/Resolver/MyTickets");
        }
    }
}
