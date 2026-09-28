import api from "./api";

const ticketService = {
    //-------------------------------------------------------
    // Get All Tickets (Admin)
    //-------------------------------------------------------
    getAllTickets: async (pageNumber = 1, pageSize = 10) => {
        const response = await api.get(`/Ticket?pageNumber=${pageNumber}&pageSize=${pageSize}`, {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        });
        return response.data;
    },

    // Create Ticket
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


    getAvailableTickets: async () => {
        const response = await api.get("/Ticket/available", {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        });
        return response.data;
    },

    getMyTickets: async (pageNumber = 1, pageSize = 10) => {
        const response = await api.get(
            `/Ticket/my?pageNumber=${pageNumber}&pageSize=${pageSize}`,
            {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                }
            }
        );
        return response.data;
    },

    getEscalatedTickets: async (pageNumber = 1, pageSize = 10) => {
        const response = await api.get(
            `/Ticket/escalated?pageNumber=${pageNumber}&pageSize=${pageSize}`,
            {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        });
        return response.data;
    },

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

   getArchivedTickets: async (pageNumber = 1, pageSize = 10) => {
    const response = await api.get(
        `/Ticket/archived?pageNumber=${pageNumber}&pageSize=${pageSize}`,
        {
            headers: { Authorization: `Bearer ${localStorage.getItem("token")}` }
        }
    );
    return response.data;
},

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

    escalateTicket: async (ticketId, escalationReason) => {
        await api.put(
            `/Ticket/${ticketId}/escalate`,
            { escalationReason },
            {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                }
            }
        );
    },

    assignTicket: async (ticketId, assignedToUserId) => {
        await api.put(
            `/Ticket/${ticketId}/assign`,
            { assignedToUserId },
            {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                }
            }
        );
    },

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
    },

    updateComment: async (ticketId, commentId, message) => {
        const response = await api.put(
            `/tickets/${ticketId}/comments/${commentId}`,
            { message },
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