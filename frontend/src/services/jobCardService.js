//----------------------------------------------------
// API Instance
//----------------------------------------------------

import api from "./api";



//----------------------------------------------------
// Get Job Cards
//----------------------------------------------------
//
// Optional filters:
//
// mine=true
// status=Open
// assignedTo=5
// search=laptop
//
//----------------------------------------------------



//----------------------------------------------------
// Get Job Cards
//----------------------------------------------------

const getAll = async (filters = {}) => {

    const params = {};

    if (filters.mine !== undefined)
        params.mine = filters.mine;

    if (filters.status)
        params.status = filters.status;

    if (filters.assignedTo)
        params.assignedTo = filters.assignedTo;

    if (filters.search)
        params.search = filters.search;

    //----------------------------------------
    // Sorting
    //----------------------------------------

    if (filters.sortBy)
        params.sortBy = filters.sortBy;

    if (filters.sortDirection)
        params.sortDirection = filters.sortDirection;

    //----------------------------------------
    // Pagination
    //----------------------------------------

    params.pageNumber = filters.pageNumber ?? 1;
    params.pageSize = filters.pageSize ?? 10;

    const response = await api.get("/JobCard", {
        params
    });

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
// Create Job Card
//----------------------------------------------------

const createFromTicket = async (ticketId) => {

    const response = await api.post(
        "/JobCard/create-from-ticket",
        {
            ticketId
        }
    );

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
// Add Labour Entry
//----------------------------------------------------

const addLabourEntry = async (jobCardId, labourEntry) => {

    await api.post(
        `/JobCard/${jobCardId}/labour`,
        labourEntry
    );

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

    await api.post(
        `/JobCard/${jobCardId}/parts`,
        part
    );
};

//----------------------------------------------------
// Delete Part
//----------------------------------------------------

const deletePart = async (partId) => {

    await api.delete(`/JobCard/parts/${partId}`);
};


//----------------------------------------------------
// Get Audit History
//----------------------------------------------------

const getAuditHistory = async (jobCardId) => {

    const response = await api.get(`/JobCard/${jobCardId}/audit`);

    return response.data;

};

//----------------------------------------------------
// print Job Card
//----------------------------------------------------

const downloadPdf = async (jobCardId) => {
    const response = await api.get(
        `/JobCard/${jobCardId}/pdf`,
        {
            responseType: "blob"
        }
    );

    return response.data;
};



//----------------------------------------------------
// Export Service
//----------------------------------------------------

const jobCardService = {

    getAll,
    getDetails,
    createFromTicket,
    update,
    completeJobCard,

    addLabourEntry,

    getParts,
    addPart,
    deletePart,

    getAuditHistory,
    downloadPdf

};



export default jobCardService;