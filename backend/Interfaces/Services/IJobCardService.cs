using IThelpdesk.DTOs.Common;
using IThelpdesk.DTOs.JobCard;
using IThelpdesk.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IThelpdesk.Interfaces.Services
{
    public interface IJobCardService
    {
        //--------------------------------------------------
        // Job Cards
        //--------------------------------------------------

        Task<IEnumerable<JobCard>> GetAllAsync();

        Task<PagedResultDto<JobCardListDto>> GetJobCardListAsync(
            int userId,
            string role,
            bool mine,
            string? status,
            int? assignedTo,
            string? search,
            string? sortBy,
            string? sortDirection,
            int pageNumber,
            int pageSize);

        Task<JobCard?> GetByIdAsync(int id);

        Task<JobCardDetailsDto?> GetDetailsAsync(int id);

        Task<JobCard> CreateFromTicketAsync(int ticketId, int performedByUserId);


        //--------------------------------------------------
        // Create Paper Job Card
        // Ticket is optional
        //--------------------------------------------------

        Task<JobCard> CreatePaperJobCardAsync(
            CreatePaperJobCardDto dto,
            int performedByUserId);

        //--------------------------------------------------
        // Paper Job Card - Technician Assignment
        //--------------------------------------------------

        Task AssignPaperJobCardTechnicianAsync(
            int jobCardId,
            int technicianId,
            bool isLeadTechnician,
            int performedByUserId);

        Task UpdateAsync(JobCard jobCard);

        Task UpdateJobCardAsync(
    int id,
    UpdateJobCardDto dto,
    int performedByUserId);


        //--------------------------------------------------
        // Paper Job Card - Overall Time Tracking
        //--------------------------------------------------

        Task UpdatePaperJobTimeAsync(
            int jobCardId,
            UpdatePaperJobTimeDto dto,
            int performedByUserId);


        Task CompleteJobCardAsync(int id, int performedByUserId);

        Task DeleteAsync(int id);

        //--------------------------------------------------
        // Labour
        //--------------------------------------------------

        Task AddLabourEntryAsync(int jobCardId, AddLabourEntryDto dto, int performedByUserId);

        Task<List<JobCardLabourEntryDto>> GetLabourEntriesAsync(int jobCardId);

        Task UpdateLabourEntryAsync(int jobCardId, int labourId, UpdateLabourEntryDto dto, int userId, string role);

        Task DeleteLabourEntryAsync(int labourId, int performedByUserId, string role);


        //--------------------------------------------------
        // Paper Job Card - Work Completed
        //--------------------------------------------------

        Task AddPaperWorkEntryAsync(
            int jobCardId,
            AddPaperWorkEntryDto dto,
            int performedByUserId);

        //--------------------------------------------------
        // Parts
        //--------------------------------------------------

        Task AddPartAsync(int jobCardId, AddPartDto dto, int performedByUserId, string role);


        //--------------------------------------------------
        // Paper Job Card - Parts Used
        //--------------------------------------------------

        Task AddPaperPartAsync(
            int jobCardId,
            AddPaperPartDto dto,
            int performedByUserId);


        Task<List<JobCardPartDto>> GetPartsAsync(int jobCardId);

        Task UpdatePartAsync(int jobCardId, int partId, UpdatePartDto dto, int performedByUserId, string role);

        Task DeletePartAsync(int partId, int performedByUserId, string role);

        //--------------------------------------------------

        Task<JobCard?> GetByTicketIdAsync(int ticketId);
    }
}