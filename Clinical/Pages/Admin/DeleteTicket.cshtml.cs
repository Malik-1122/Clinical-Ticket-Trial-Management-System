using Clinical.Data;
using Clinical.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Clinical.Pages.Admin
{
    public class DeleteTicketModel : PageModel
    {
        private readonly ClinicalContext _context;

        public DeleteTicketModel(ClinicalContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ticket Ticket { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketID == id);
            if (Ticket == null)
                return RedirectToPage("/Admin/ViewTickets");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return RedirectToPage("/Admin/ViewTickets");

            // 🔥 Delete CHILD TABLES FIRST
            var logs = _context.AuditLog.Where(a => a.TicketID == id);
            _context.AuditLog.RemoveRange(logs);

            var notes = _context.Notification.Where(n => n.TicketID == id);
            _context.Notification.RemoveRange(notes);

            await _context.SaveChangesAsync();

            // 🔥 Now delete the ticket safely
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/ViewTickets");
        }
    }
}
