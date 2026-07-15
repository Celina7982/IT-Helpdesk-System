using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public DashboardController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult GetDashboard()
    {
        DashboardSummary dashboard = new DashboardSummary();

        using (SqlConnection con = new SqlConnection(
            _configuration.GetConnectionString("DefaultConnection")))
        {
            con.Open();

            string sql = @"
            SELECT
            (SELECT COUNT(*) FROM Tickets) AS TotalTickets,
            (SELECT COUNT(*) FROM Tickets WHERE Status='Open') AS OpenTickets,
            (SELECT COUNT(*) FROM Tickets WHERE Status='Closed') AS ClosedTickets,
            (SELECT COUNT(*) FROM Tickets WHERE Status='In Progress') AS InProgressTickets,
            (SELECT COUNT(*) FROM Tickets WHERE Priority='High' OR Priority='Critical') AS HighPriorityTickets,
            (SELECT COUNT(*) FROM Users) AS TotalUsers,
            (SELECT COUNT(*) FROM Employees) AS TotalTechnicians,
            (SELECT COUNT(*) FROM Departments) AS TotalDepartments,
            (SELECT COUNT(*) FROM Assets) AS TotalAssets";

            SqlCommand cmd = new SqlCommand(sql, con);

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                dashboard.TotalTickets = Convert.ToInt32(reader["TotalTickets"]);
                dashboard.OpenTickets = Convert.ToInt32(reader["OpenTickets"]);
                dashboard.ClosedTickets = Convert.ToInt32(reader["ClosedTickets"]);
                dashboard.InProgressTickets = Convert.ToInt32(reader["InProgressTickets"]);
                dashboard.HighPriorityTickets = Convert.ToInt32(reader["HighPriorityTickets"]);
                dashboard.TotalUsers = Convert.ToInt32(reader["TotalUsers"]);
                dashboard.TotalTechnicians = Convert.ToInt32(reader["TotalTechnicians"]);
                dashboard.TotalDepartments = Convert.ToInt32(reader["TotalDepartments"]);
                dashboard.TotalAssets = Convert.ToInt32(reader["TotalAssets"]);
            }
        }
         return Ok(dashboard);
    }
}