using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Clinical.Pages.Reviewer
{
    public class CloseTicketModel : PageModel
    {
        private readonly IConfiguration _config;

        public CloseTicketModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public int TicketID { get; set; }

        public void OnGet(int id)
        {
            TicketID = id;
        }

        public IActionResult OnPost(int id)
        {
            string reviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string conn = _config.GetConnectionString("ClinicalContext");

            using (SqlConnection c = new SqlConnection(conn))
            {
                c.Open();

                // Update ticket status
                string update = @"
                    UPDATE Tickets 
                    SET Status = 'Closed', 
                        ClosedDate = GETDATE(),
                        UpdatedDate = GETDATE()
                    WHERE TicketID = @id";

                using (var cmd = new SqlCommand(update, c))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                // Audit Log
                using (var cmd = new SqlCommand(
                    "INSERT INTO AuditLog (TicketID, ActionTaken, ActionBy, ActionDate) VALUES (@t,'Closed',@b,GETDATE())", c))
                {
                    cmd.Parameters.AddWithValue("@t", id);
                    cmd.Parameters.AddWithValue("@b", reviewerId);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/Reviewer/MyTickets");
        }
    }
}
