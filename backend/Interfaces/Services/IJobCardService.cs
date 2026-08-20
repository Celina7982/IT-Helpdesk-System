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

        Task<JobCard> CreateFromTicketAsync(int ticketId);

        Task UpdateAsync(JobCard jobCard);

        Task UpdateJobCardAsync(int id, UpdateJobCardDto dto);

        Task CompleteJobCardAsync(int id);

        Task DeleteAsync(int id);

        //--------------------------------------------------
        // Labour
        //--------------------------------------------------

        Task AddLabourEntryAsync(int jobCardId, AddLabourEntryDto dto);

        Task<List<JobCardLabour>> GetLabourEntriesAsync(int jobCardId);

        //--------------------------------------------------
        // Parts
        //--------------------------------------------------

        Task AddPartAsync(int jobCardId, AddPartDto dto, int performedByUserId, string role);

        Task<List<JobCardPartDto>> GetPartsAsync(int jobCardId);

        Task UpdatePartAsync(int jobCardId, int partId, UpdatePartDto dto, int performedByUserId, string role);

        Task DeletePartAsync(int partId, int performedByUserId, string role);

        //--------------------------------------------------

        Task<JobCard?> GetByTicketIdAsync(int ticketId);
    }
}
