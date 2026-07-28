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

        private readonly IJobCardPdfService _jobCardPdfService;
        public JobCardController(
         IJobCardService jobCardService,
         IJobCardPdfService jobCardPdfService)
        {
            _jobCardService = jobCardService;
            _jobCardPdfService = jobCardPdfService;
        }

        //---------------------------------------------------------
        // GET JOB CARD LIST
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpGet]
        public async Task<IActionResult> GetAll(
        [FromQuery] bool mine = false,
        [FromQuery] string? status = null,
        [FromQuery] int? assignedTo = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = "desc",
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        {
            //---------------------------------------------------------
            // Logged in User
            //---------------------------------------------------------

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) ||
                string.IsNullOrEmpty(role))
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim);

            var jobCards = await _jobCardService.GetJobCardListAsync(
            userId,
            role,
            mine,
            status,
            assignedTo,
            search,
            sortBy,
            sortDirection,
            pageNumber,
            pageSize);

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
        // EXPORT JOB CARD PDF
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> ExportPdf(int id)
        {
            var pdf = await _jobCardPdfService.GenerateJobCardPdfAsync(id);

            return File(
                pdf,
                "application/pdf",
                $"JobCard-{id}.pdf");
        }

        //---------------------------------------------------------
        // CREATE JOB CARD
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
        // ADD LABOUR
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

        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            await _jobCardService.CompleteJobCardAsync(id);

            return NoContent();
        }

        //---------------------------------------------------------
        // DELETE JOB CARD
        //---------------------------------------------------------

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _jobCardService.DeleteAsync(id);

            return NoContent();
        }
    }
}