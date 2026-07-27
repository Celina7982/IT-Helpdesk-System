//----------------------------------------------------
// API Instance
//----------------------------------------------------

import api from "./api";

//----------------------------------------------------
// Get All Job Cards
//----------------------------------------------------
//
// Retrieves the Job Cards that the logged-in user
// is allowed to view.
//
// Administrators:
//     Receive all Job Cards.
//
// Technicians:
//     Receive only their assigned Job Cards.
//
//----------------------------------------------------

const getAll = async () => {

    const response = await api.get("/JobCard");

    return response.data;

};

//----------------------------------------------------
// Get Job Card Details
//----------------------------------------------------
//
// Retrieves the full details of a single Job Card.
//
//----------------------------------------------------

const getDetails = async (id) => {

    const response = await api.get(`/JobCard/${id}`);

    return response.data;

};

//----------------------------------------------------
// Create Job Card From Ticket
//----------------------------------------------------
//
// Creates a new Job Card using an existing Ticket.
//
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
//
// Updates an existing Job Card.
//
//----------------------------------------------------

const update = async (id, jobCard) => {

    await api.put(`/JobCard/${id}`, jobCard);

};

//----------------------------------------------------
// Complete Job Card
//----------------------------------------------------
//
// Marks a Job Card as completed.
//
// API Endpoint:
// PUT /api/JobCard/{id}/complete
//
//----------------------------------------------------

const completeJobCard = async (jobCardId) => {

    await api.put(`/JobCard/${jobCardId}/complete`);

};

//----------------------------------------------------
// Add Labour Entry
//----------------------------------------------------
//
// Adds a Labour Entry to the specified Job Card.
//
//----------------------------------------------------

const addLabourEntry = async (jobCardId, labourEntry) => {

    await api.post(
        `/JobCard/${jobCardId}/labour`,
        labourEntry
    );

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

    addLabourEntry

};

export default jobCardService;