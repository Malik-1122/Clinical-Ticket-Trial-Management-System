using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System;
using System.IO;
using System.Security.Claims;

namespace Clinical.Pages.Reviewer
{
    public class ReviewTicketModel : PageModel
    {
        private readonly IConfiguration _config;
        public ReviewTicketModel(IConfiguration config) => _config = config;

        [BindProperty]
        public TicketViewModel Ticket { get; set; }

        public void OnGet(int id)
        {
            LoadTicket(id);
        }

        public IActionResult OnPost(int id)
        {
            // ✔ FIX: Correct reviewer ID from Claims
            string reviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (reviewerId == null) reviewerId = "0";

            // ✔ FIX: Convert StringValues to normal strings
            string comment = Request.Form["ReviewComment"].ToString();
            string status = Request.Form["Status"].ToString();

            string attachmentPath = null;

            // ✔ File upload
            var file = Request.Form.Files["ReviewAttachment"];
            if (file != null && file.Length > 0)
            {
                string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "review_attachments");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                string fileName = $"{Guid.NewGuid()}_{file.FileName}";
                string filePath = Path.Combine(uploads, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fs);
                }

                attachmentPath = "/review_attachments/" + fileName;
            }

            // ✔ FIX: Correct Connection String
            string conn = _config.GetConnectionString("ClinicalContext");

            using (SqlConnection c = new SqlConnection(conn))
            {
                c.Open();

                // ✔ Update Ticket Table
                string update = @"
                    UPDATE Tickets
                    SET ReviewerComment = @Comment,
                        ReviewAttachment = @Attach,
                        ReviewedDate = GETDATE(),
                        Status = @Status,
                        UpdatedDate = GETDATE()
                    WHERE TicketID = @ID";

                using (var cmd = new SqlCommand(update, c))
                {
                    cmd.Parameters.AddWithValue("@Comment", (object)comment ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Attach", (object)attachmentPath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }

                // ✔ Insert Audit Log
                using (var cmd = new SqlCommand(
                    "INSERT INTO AuditLog (TicketID, ActionTaken, ActionBy, ActionDate) VALUES (@t,@a,@b,GETDATE())", c))
                {
                    cmd.Parameters.AddWithValue("@t", id);
                    cmd.Parameters.AddWithValue("@a", $"Reviewed: {status}; Comment: {comment}");
                    cmd.Parameters.AddWithValue("@b", int.Parse(reviewerId));
                    cmd.ExecuteNonQuery();
                }

                // ✔ Fetch ticket creator
                int createdBy = 0;
                using (var cmd = new SqlCommand(
                    "SELECT CreatedBy FROM Tickets WHERE TicketID = @id", c))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    var res = cmd.ExecuteScalar();
                    if (res != null && res != DBNull.Value)
                        createdBy = Convert.ToInt32(res);
                }

                // ✔ Insert Notification
                using (var cmd = new SqlCommand(
                    "INSERT INTO Notification (TicketID, UserID, Message, Status, Timestamp) VALUES (@t,@u,@m,'Sent',GETDATE())",
                    c))
                {
                    cmd.Parameters.AddWithValue("@t", id);
                    cmd.Parameters.AddWithValue("@u", createdBy);
                    cmd.Parameters.AddWithValue("@m", $"Your ticket #{id} was reviewed. New status: {status}");
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/Reviewer/MyTickets");
        }

        private void LoadTicket(int id)
        {
            string conn = _config.GetConnectionString("ClinicalContext");

            using (SqlConnection c = new SqlConnection(conn))
            {
                string q = @"
                    SELECT TicketID, Title, Description, Category, Priority, Status, CreatedDate 
                    FROM Tickets 
                    WHERE TicketID = @id";

                using (var cmd = new SqlCommand(q, c))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    c.Open();

                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            Ticket = new TicketViewModel
                            {
                                TicketID = rdr.GetInt32(0),
                                Title = rdr.IsDBNull(1) ? "" : rdr.GetString(1),
                                Description = rdr.IsDBNull(2) ? "" : rdr.GetString(2),
                                Category = rdr.IsDBNull(3) ? "" : rdr.GetString(3),
                                Priority = rdr.IsDBNull(4) ? "" : rdr.GetString(4),
                                Status = rdr.IsDBNull(5) ? "" : rdr.GetString(5),
                                CreatedDate = rdr.IsDBNull(6) ? DateTime.MinValue : rdr.GetDateTime(6)
                            };
                        }
                    }
                }
            }
        }

        public class TicketViewModel
        {
            public int TicketID { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public string Priority { get; set; }
            public string Status { get; set; }
            public DateTime CreatedDate { get; set; }
        }
    }
}
