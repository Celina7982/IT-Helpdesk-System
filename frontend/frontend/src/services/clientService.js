import api from "./api";

const clientService = {

    // Get tickets created by the logged-in client
    getMyTickets: async () => {

        const response = await api.get(
            "/Ticket/mytickets",
            {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                }
            }
        );

        return response.data;
    },

    // Create a new ticket
    createTicket: async (ticket) => {

        const response = await api.post(
            "/Ticket",
            ticket,
            {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                }
            }
        );

        return response.data;
    }

};

export default clientService;