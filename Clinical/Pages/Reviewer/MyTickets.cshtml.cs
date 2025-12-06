using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Security.Claims;

namespace Clinical.Pages.Reviewer
{
    public class MyTicketsModel : PageModel
    {
        private readonly IConfiguration _config;
        public MyTicketsModel(IConfiguration config) => _config = config;

        public List<TicketRow> Tickets { get; set; } = new();

        public void OnGet()
        {
            string reviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(reviewerId)) return;

            string conn = _config.GetConnectionString("ClinicalContext");

            using (SqlConnection c = new SqlConnection(conn))
            {
                string q = @"
                    SELECT TicketID, Title, Category, Priority, Status, CreatedDate
                    FROM Tickets
                    WHERE AssignedTo = @r
                    ORDER BY TicketID DESC";

                using (var cmd = new SqlCommand(q, c))
                {
                    cmd.Parameters.AddWithValue("@r", reviewerId);
                    c.Open();

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            Tickets.Add(new TicketRow
                            {
                                TicketID = rdr.GetInt32(0),
                                Title = rdr.IsDBNull(1) ? "" : rdr.GetString(1),
                                Category = rdr.IsDBNull(2) ? "" : rdr.GetString(2),
                                Priority = rdr.IsDBNull(3) ? "" : rdr.GetString(3),
                                Status = rdr.IsDBNull(4) ? "" : rdr.GetString(4),
                                CreatedDate = rdr.IsDBNull(5) ? System.DateTime.MinValue : rdr.GetDateTime(5)
                            });
                        }
                    }
                }
            }
        }

        public class TicketRow
        {
            public int TicketID { get; set; }
            public string Title { get; set; }
            public string Category { get; set; }
            public string Priority { get; set; }
            public string Status { get; set; }
            public System.DateTime CreatedDate { get; set; }
        }
    }
}
