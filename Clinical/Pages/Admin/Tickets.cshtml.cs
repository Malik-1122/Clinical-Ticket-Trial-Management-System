using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Clinical.Data;
using Clinical.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinical.Pages.Admin
{
    public class TicketsModel : PageModel
    {
        private readonly ClinicalContext _context;

        public TicketsModel(ClinicalContext context)
        {
            _context = context;
        }

        public IList<Ticket> Ticket { get; set; }

        public async Task OnGetAsync()
        {
            Ticket = await _context.Tickets
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .ToListAsync();
        }
    }
}
