using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace Clinical.Pages.Reviewer
{
    public class DashboardModel : PageModel
    {
        private readonly IConfiguration _config;
        public DashboardModel(IConfiguration config) => _config = config;

        public int AssignedCount { get; set; }
        public int ReviewedCount { get; set; }
        public int ClosedCount { get; set; }

        public void OnGet()
        {
            // ✔ Get reviewer ID from Claims
            string reviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(reviewerId)) return;

            // ✔ Correct Connection String
            string conn = _config.GetConnectionString("ClinicalContext");

            using (SqlConnection c = new SqlConnection(conn))
            {
                c.Open();

                // Assigned but not closed
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Tickets WHERE AssignedTo = @r AND Status <> 'Closed'", c))
                {
                    cmd.Parameters.AddWithValue("@r", int.Parse(reviewerId));
                    AssignedCount = (int)cmd.ExecuteScalar();
                }

                // Reviewed tickets
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Tickets WHERE AssignedTo = @r AND Status = 'Reviewed'", c))
                {
                    cmd.Parameters.AddWithValue("@r", int.Parse(reviewerId));
                    ReviewedCount = (int)cmd.ExecuteScalar();
                }

                // Closed tickets
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Tickets WHERE AssignedTo = @r AND Status = 'Closed'", c))
                {
                    cmd.Parameters.AddWithValue("@r", int.Parse(reviewerId));
                    ClosedCount = (int)cmd.ExecuteScalar();
                }
            }
        }
    }
}
