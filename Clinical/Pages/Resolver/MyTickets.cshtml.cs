using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;

namespace Clinical.Pages.Resolver
{
    public class MyTicketsModel : PageModel
    {
        private readonly IConfiguration _config;

        public MyTicketsModel(IConfiguration config)
        {
            _config = config;
        }

        public List<TicketViewModel> MyTickets { get; set; } = new();

        public void OnGet()
        {
            // Get Resolver ID from Claims (NOT Session)
            string resolverId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(resolverId))
                return;

            string connString = _config.GetConnectionString("ClinicalContext"); // FIXED

            using (SqlConnection con = new SqlConnection(connString))
            {
                string query = @"
                    SELECT TicketID, Title, Status, Priority, CreatedDate
                    FROM Tickets
                    WHERE AssignedTo = @ResolverID
                    ORDER BY TicketID DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ResolverID", int.Parse(resolverId));

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    MyTickets.Add(new TicketViewModel
                    {
                        TicketID = rdr.GetInt32(0),
                        Title = rdr.GetString(1),
                        Status = rdr.GetString(2),
                        Priority = rdr.GetString(3),
                        CreatedDate = rdr.GetDateTime(4)
                    });
                }
            }
        }

        public class TicketViewModel
        {
            public int TicketID { get; set; }
            public string Title { get; set; }
            public string Status { get; set; }
            public string Priority { get; set; }
            public System.DateTime CreatedDate { get; set; }
        }
    }
}
