using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Clinical.Data;
using Clinical.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Clinical.Pages.Admin
{
    public class ViewTicketsModel : PageModel
    {
        private readonly ClinicalContext _context;

        public ViewTicketsModel(ClinicalContext context)
        {
            _context = context;
        }

        public List<TicketView> Tickets { get; set; }

        public async Task OnGetAsync()
        {
            Tickets = await (
                from t in _context.Tickets
                join u in _context.Users on t.AssignedTo equals u.UserID into userJoin
                from assigned in userJoin.DefaultIfEmpty()
                select new TicketView
                {
                    TicketID = t.TicketID,
                    Title = t.Title,
                    Description = t.Description,
                    Category = t.Category,
                    Priority = t.Priority,
                    Status = t.Status,
                    AssignedTo = assigned != null ? assigned.Name : "—",
                    CreatedDate = t.CreatedDate
                }
            )
            .OrderByDescending(t => t.CreatedDate)
            .ToListAsync();
        }

        public class TicketView
        {
            public int TicketID { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public string Priority { get; set; }
            public string Status { get; set; }
            public string AssignedTo { get; set; }
            public DateTime? CreatedDate { get; set; }
        }
    }
}
