import api from "./api";

const ticketService = {

    getAvailableTickets: async () => {

        const response = await api.get("/Ticket/available", {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        });

        return response.data;
    },

    getMyTickets: async () => {

        const response = await api.get("/Ticket/my", {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        });

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

    getEscalatedTickets: async () => {

        const response = await api.get(
            "/Ticket/escalated",
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