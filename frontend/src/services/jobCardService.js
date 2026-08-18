import api from "./api";

//----------------------------------------------------
// Get Job Cards
//----------------------------------------------------

const getAll = async (filters = {}) => {
    const params = {};

    if (filters.mine !== undefined) params.mine = filters.mine;
    if (filters.status) params.status = filters.status;
    if (filters.assignedTo) params.assignedTo = filters.assignedTo;
    if (filters.search) params.search = filters.search;
    if (filters.sortBy) params.sortBy = filters.sortBy;
    if (filters.sortDirection) params.sortDirection = filters.sortDirection;

    params.pageNumber = filters.pageNumber ?? 1;
    params.pageSize = filters.pageSize ?? 10;

    const response = await api.get("/JobCard", { params });
    return response.data;
};

//----------------------------------------------------
// Get Job Card Details
//----------------------------------------------------

const getDetails = async (id) => {
    const response = await api.get(`/JobCard/${id}`);
    return response.data;
};

//----------------------------------------------------
// Get Job Card By Ticket ID
//----------------------------------------------------

const getByTicketId = async (ticketId) => {
    const response = await api.get(`/JobCard/by-ticket/${ticketId}`);
    return response.data;
};

//----------------------------------------------------
// Create Job Card
//----------------------------------------------------

const createFromTicket = async (ticketId) => {
    const response = await api.post("/JobCard/create-from-ticket", { ticketId });
    return response.data;
};

//----------------------------------------------------
// Update Job Card
//----------------------------------------------------

const update = async (id, jobCard) => {
    await api.put(`/JobCard/${id}`, jobCard);
};

//----------------------------------------------------
// Complete Job Card
//----------------------------------------------------

const completeJobCard = async (jobCardId) => {
    await api.put(`/JobCard/${jobCardId}/complete`);
};

//----------------------------------------------------
// Labour Entries
//----------------------------------------------------

const addLabourEntry = async (jobCardId, labourEntry) => {
    await api.post(`/JobCard/${jobCardId}/labour`, labourEntry);
};

const updateLabourEntry = async (jobCardId, labourId, labourEntry) => {
    await api.put(`/JobCard/${jobCardId}/labour/${labourId}`, labourEntry);
};

const deleteLabourEntry = async (jobCardId, labourId) => {
    await api.delete(`/JobCard/${jobCardId}/labour/${labourId}`);
};

//----------------------------------------------------
// Parts
//----------------------------------------------------

const getParts = async (jobCardId) => {
    const response = await api.get(`/JobCard/${jobCardId}/parts`);
    return response.data;
};

const addPart = async (jobCardId, part) => {
    await api.post(`/JobCard/${jobCardId}/parts`, part);
};

const updatePart = async (jobCardId, partId, part) => {
    await api.put(`/JobCard/${jobCardId}/parts/${partId}`, part);
};

const deletePart = async (jobCardId, partId) => {
    await api.delete(`/JobCard/${jobCardId}/parts/${partId}`);
};

//----------------------------------------------------
// Get Audit History
//----------------------------------------------------

const getAuditHistory = async (jobCardId) => {
    const response = await api.get(`/JobCard/${jobCardId}/audit`);
    return response.data;
};

//----------------------------------------------------
// Print / Download PDF
//----------------------------------------------------

const downloadPdf = async (jobCardId) => {
    const response = await api.get(`/JobCard/${jobCardId}/pdf`, {
        responseType: "blob"
    });
    return response.data;
};

//----------------------------------------------------
// Export Service
//----------------------------------------------------

const jobCardService = {
    getAll,
    getDetails,
    getByTicketId,
    createFromTicket,
    update,
    completeJobCard,
    addLabourEntry,
    updateLabourEntry,
    deleteLabourEntry,
    getParts,
    addPart,
    updatePart,
    deletePart,
    getAuditHistory,
    downloadPdf
};

export default jobCardService;