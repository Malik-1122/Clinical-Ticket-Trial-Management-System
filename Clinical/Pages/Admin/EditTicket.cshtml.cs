using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Clinical.Data;
using Clinical.Models;
using System.Threading.Tasks;

namespace Clinical.Pages.Admin
{
    public class EditTicketModel : PageModel
    {
        private readonly ClinicalContext _context;

        public EditTicketModel(ClinicalContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ticket Ticket { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketID == id);

            if (Ticket == null)
            {
                return RedirectToPage("/Admin/ViewTickets");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var existing = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketID == Ticket.TicketID);

            if (existing == null)
                return RedirectToPage("/Admin/ViewTickets");

            existing.Title = Ticket.Title;
            existing.Description = Ticket.Description;
            existing.Category = Ticket.Category;
            existing.Priority = Ticket.Priority;
            existing.UpdatedDate = System.DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/ViewTickets");
        }
    }
}
