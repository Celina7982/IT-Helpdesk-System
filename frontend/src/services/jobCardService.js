//----------------------------------------------------
// API Instance
//----------------------------------------------------
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

  // Sorting
  if (filters.sortBy) params.sortBy = filters.sortBy;
  if (filters.sortDirection) params.sortDirection = filters.sortDirection;

  // Pagination
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
// Labour Entries API Calls (ADDED & FIXED)
//----------------------------------------------------
const getLabourEntries = async (jobCardId) => {
  const response = await api.get(`/JobCard/${jobCardId}/labour`);
  return response.data;
};

const addLabourEntry = async (jobCardId, labourEntry) => {
  const response = await api.post(`/JobCard/${jobCardId}/labour`, labourEntry);
  return response.data;
};

const updateLabourEntry = async (jobCardId, entryId, labourEntry) => {
  const response = await api.put(`/JobCard/${jobCardId}/labour/${entryId}`, labourEntry);
  return response.data;
};

const deleteLabourEntry = async (entryId) => {
  const response = await api.delete(`/JobCard/labour/${entryId}`);
  return response.data;
};

//----------------------------------------------------
// Get Parts
//----------------------------------------------------
const getParts = async (jobCardId) => {
  const response = await api.get(`/JobCard/${jobCardId}/parts`);
  return response.data;
};

//----------------------------------------------------
// Add Part
//----------------------------------------------------
const addPart = async (jobCardId, part) => {
  const response = await api.post(`/JobCard/${jobCardId}/parts`, part);
  return response.data;
};

//----------------------------------------------------
// Update Part
//----------------------------------------------------
const updatePart = async (jobCardId, partId, updateDto) => {
  const response = await api.put(`/JobCard/${jobCardId}/parts/${partId}`, updateDto);
  return response.data;
};

//----------------------------------------------------
// Delete Part
//----------------------------------------------------
const deletePart = async (partId) => {
  const response = await api.delete(`/JobCard/parts/${partId}`);
  return response.data;
};

//----------------------------------------------------
// Get Audit History
//----------------------------------------------------
const getAuditHistory = async (jobCardId) => {
  const response = await api.get(`/JobCard/${jobCardId}/audit`);
  return response.data;
};

//----------------------------------------------------
// Print Job Card
//----------------------------------------------------
const downloadPdf = async (jobCardId) => {
  const response = await api.get(`/JobCard/${jobCardId}/pdf`, {
    responseType: "blob",
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
  getLabourEntries,    // Exported
  addLabourEntry,      // Exported
  updateLabourEntry,   // Exported
  deleteLabourEntry,   // Exported
  getParts,
  addPart,
  updatePart,
  deletePart,
  getAuditHistory,
  downloadPdf,
};

export default jobCardService;