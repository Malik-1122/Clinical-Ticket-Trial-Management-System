using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Clinical.Models;
using System.Data.SqlClient;
using System;

using Microsoft.Extensions.Configuration;

namespace Clinical.Pages.Admin
{
    public class CreateTicketModel : PageModel
    {
        private readonly string _connString;

        
        public CreateTicketModel(IConfiguration config)
        {
            Console.WriteLine("hooo");
            _connString = config.GetConnectionString("ClinicalContext");

        }

        [BindProperty]
        public Ticket Ticket { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //    return Page();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    Console.WriteLine("hiii");
                    string sql = @"
                        INSERT INTO Tickets 
                        (Title, Description, CategoryID, PriorityID, Status, CreatedBy, CreatedDate, AdminRole, Category, Priority)
                        VALUES 
                        (@Title, @Description, @CategoryID, @PriorityID, @Status, @CreatedBy, @CreatedDate, @AdminRole, @Category, @Priority)
                    ";

                    Console.WriteLine(sql);

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        // Add parameters like normal SQL
                        cmd.Parameters.AddWithValue("@Title", Ticket.Title);
                        cmd.Parameters.AddWithValue("@Description", Ticket.Description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CategoryID", Ticket.CategoryID ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PriorityID", Ticket.PriorityID ?? (object)DBNull.Value);

                        // System generated values
                        cmd.Parameters.AddWithValue("@Status", "Open");
                        cmd.Parameters.AddWithValue("@CreatedBy", 1);   // TEMP FIX (Replace with logged-in UserID)
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@AdminRole", "Admin");

                        // Convert ID to text values
                        cmd.Parameters.AddWithValue("@Category",
                            Ticket.CategoryID == 1 ? "Safety" :
                            Ticket.CategoryID == 2 ? "Data" :
                            Ticket.CategoryID == 3 ? "Technical" :
                            (object)DBNull.Value);

                        cmd.Parameters.AddWithValue("@Priority",
                            Ticket.PriorityID == 1 ? "Low" :
                            Ticket.PriorityID == 2 ? "Medium" :
                            Ticket.PriorityID == 3 ? "High" :
                            (object)DBNull.Value);

                        conn.Open();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return RedirectToPage("/Admin/ViewTickets");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "ERROR: " + ex.Message);
                return Page();
            }
        }
    }
}
