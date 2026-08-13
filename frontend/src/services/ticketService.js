import api from "./api";

const ticketService = {

    //-------------------------------------------------------
    // Get All Tickets (Admin)
    //-------------------------------------------------------

    getAllTickets: async () => {
        const response = await api.get("/Ticket", {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        });
        return response.data;
    },
    //-------------------------------------------------------
// Create Ticket
//-------------------------------------------------------

createTicket: async (ticketData) => {

    const response = await api.post(

        "/Ticket",

        ticketData,

        {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        }

    );

    return response.data;

},
    //-------------------------------------------------------
    // Available Tickets
    //-------------------------------------------------------

    getAvailableTickets: async () => {
        const response = await api.get("/Ticket/available", {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        });
        return response.data;
    },

    //-------------------------------------------------------
    // My Tickets
    //-------------------------------------------------------

    getMyTickets: async () => {
        const response = await api.get("/Ticket/my", {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        });
        return response.data;
    },

    //-------------------------------------------------------
    // Escalated Tickets
    //-------------------------------------------------------

    getEscalatedTickets: async () => {
        const response = await api.get("/Ticket/escalated", {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        });
        return response.data;
    },

    //-------------------------------------------------------
    // Ticket Details
    //-------------------------------------------------------

    getTicketDetails: async (ticketId) => {
        const response = await api.get(
            `/Ticket/${ticketId}`,
            {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                }
            }
        );
        return response.data;
    },



    //-------------------------------------------------------
// Archived Tickets
//-------------------------------------------------------

getArchivedTickets: async () => {
    const response = await api.get(
        "/Ticket/archived",
        {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        }
    );

    return response.data;
},
    //-------------------------------------------------------
    // Claim Ticket
    //-------------------------------------------------------

    claimTicket: async (ticketId) => {
        await api.put(
            `/Ticket/${ticketId}/claim`,
            {},
            {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                }
            }
        );
    },

    //-------------------------------------------------------
    // Resolve Ticket
    //-------------------------------------------------------

    resolveTicket: async (ticketId) => {
        await api.put(
            `/Ticket/${ticketId}/resolve`,
            {},
            {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                }
            }
        );
    },


    //-------------------------------------------------------
// Archive Ticket
//-------------------------------------------------------

archiveTicket: async (ticketId) => {
    await api.put(
        `/Ticket/${ticketId}/archive`,
        {},
        {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        }
    );
},

    //-------------------------------------------------------
    // Escalate Ticket
    //-------------------------------------------------------

    escalateTicket: async (ticketId, escalationReason) => {
        await api.put(
            `/Ticket/${ticketId}/escalate`,
            {
                escalationReason
            },
            {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                }
            }
        );
    },

    //-------------------------------------------------------
    // Assign Ticket
    //-------------------------------------------------------

    assignTicket: async (ticketId, assignedToUserId) => {
        await api.put(
            `/Ticket/${ticketId}/assign`,
            {
                assignedToUserId
            },
            {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                }
            }
        );
    },

    //-------------------------------------------------------
    // Delete Ticket
    //-------------------------------------------------------

    deleteTicket: async (ticketId) => {
        await api.delete(
            `/Ticket/${ticketId}`,
            {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                }
            }
        );
    },

    //-------------------------------------------------------
    // Get Ticket Comments
    // Matches GET: api/tickets/{ticketId}/comments
    //-------------------------------------------------------

    getComments: async (ticketId) => {
        const response = await api.get(
            `/tickets/${ticketId}/comments`,
            {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                }
            }
        );
        return response.data;
    },

    //-------------------------------------------------------
    // Add Comment to Ticket
    // Matches POST: api/tickets/{ticketId}/comments
    // Payload expects CreateCommentDto: { message: "..." }
    //-------------------------------------------------------

    addComment: async (ticketId, commentText) => {
        const response = await api.post(
            `/tickets/${ticketId}/comments`,
            { message: commentText },
            {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                }
            }
        );
        return response.data;
    }

};

export default ticketService;