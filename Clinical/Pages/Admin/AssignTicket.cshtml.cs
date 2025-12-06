using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Clinical.Data;
using Clinical.Models;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Clinical.Pages.Admin
{
    public class AssignTicketModel : PageModel
    {
        private readonly ClinicalContext _context;

        public AssignTicketModel(ClinicalContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ticket Ticket { get; set; }

        [BindProperty]
        public int SelectedResolverID { get; set; }

        public List<Users> Resolvers { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketID == id);

            if (Ticket == null)
                return RedirectToPage("/Admin/ViewTickets");

            // Load all resolvers
            Resolvers = await _context.Users
                .Where(u => u.Role == "Resolver")
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var existingTicket = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketID == Ticket.TicketID);

            if (existingTicket == null)
                return RedirectToPage("/Admin/ViewTickets");

            // Assign Resolver (ALWAYS int)
            existingTicket.AssignedTo = SelectedResolverID;
            existingTicket.Status = "Assigned to Resolver";
            existingTicket.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/ViewTickets");
        }
    }
}
