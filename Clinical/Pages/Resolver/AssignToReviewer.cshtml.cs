using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Clinical.Models;

namespace Clinical.Pages.Resolver
{
    public class AssignToReviewerModel : PageModel
    {
        private readonly IConfiguration _config;

        public AssignToReviewerModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public int TicketID { get; set; }

        public List<UserModel> Reviewers { get; set; } = new();

        public void OnGet(int id)
        {
            TicketID = id;
            LoadReviewers();
        }

        public IActionResult OnPost(int id)
        {
            int reviewerID = int.Parse(Request.Form["ReviewerID"]);
            string conn = _config.GetConnectionString("ClinicalContext");  // FIXED

            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();

                string query = @"
                    UPDATE Tickets
                    SET AssignedTo = @ReviewerID,
                        Status = 'Assigned to Reviewer'
                    WHERE TicketID = @TicketID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ReviewerID", reviewerID);
                cmd.Parameters.AddWithValue("@TicketID", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToPage("/Resolver/MyTickets");
        }

        private void LoadReviewers()
        {
            string conn = _config.GetConnectionString("ClinicalContext");  // FIXED

            using (SqlConnection con = new SqlConnection(conn))
            {
                string query = "SELECT UserID, Name, Email FROM Users WHERE Role = 'Reviewer'";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Reviewers.Add(new UserModel
                    {
                        UserID = rdr.GetInt32(0),
                        Name = rdr.GetString(1),
                        Email = rdr.GetString(2)
                    });
                }
            }
        }

        public class UserModel
        {
            public int UserID { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
        }
    }
}
