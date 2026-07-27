using IThelpdesk.DTOs.JobCard;
using IThelpdesk.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IThelpdesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobCardController : ControllerBase
    {
        private readonly IJobCardService _jobCardService;

        public JobCardController(IJobCardService jobCardService)
        {
            _jobCardService = jobCardService;
        }


        //---------------------------------------------------------
        // GET JOB CARD LIST
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //---------------------------------------------------------
            // Retrieve the logged-in user's ID and Role from the JWT.
            //---------------------------------------------------------

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(role))
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim);

            //---------------------------------------------------------
            // Administrators receive all Job Cards.
            // Technicians receive only their assigned Job Cards.
            //---------------------------------------------------------

            var jobCards = await _jobCardService.GetJobCardListAsync(userId, role);

            return Ok(jobCards);
        }

        //---------------------------------------------------------
        // GET JOB CARD DETAILS
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var jobCard = await _jobCardService.GetDetailsAsync(id);

            if (jobCard == null)
                return NotFound();

            return Ok(jobCard);
        }

        //---------------------------------------------------------
        // CREATE JOB CARD FROM TICKET
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpPost("create-from-ticket")]
        public async Task<IActionResult> CreateFromTicket(CreateJobCardDto request)
        {
            var jobCard =
                await _jobCardService.CreateFromTicketAsync(request.TicketId);

            return Ok(jobCard);
        }

        //---------------------------------------------------------
        // ADD LABOUR ENTRY
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpPost("{id}/labour")]
        public async Task<IActionResult> AddLabourEntry(
    int id,
    AddLabourEntryDto dto)
        {
            await _jobCardService.AddLabourEntryAsync(id, dto);

            return Ok();
        }
        //---------------------------------------------------------
        // UPDATE JOB CARD
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateJobCardDto dto)
        {
            await _jobCardService.UpdateJobCardAsync(id, dto);

            return NoContent();
        }

        //---------------------------------------------------------
        // COMPLETE JOB CARD
        //---------------------------------------------------------
        //
        // Marks the specified Job Card as completed.
        //
        // This endpoint is called when the technician clicks the
        // "Complete" button in the React application.
        //
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            //---------------------------------------------------------
            // Call the Service Layer
            //---------------------------------------------------------

            await _jobCardService.CompleteJobCardAsync(id);

            //---------------------------------------------------------
            // Return HTTP 204 (No Content)
            //---------------------------------------------------------

            return NoContent();
        }


        

        //---------------------------------------------------------
        // DELETE JOB CARD
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _jobCardService.DeleteAsync(id);

            return NoContent();
        }
    }
}