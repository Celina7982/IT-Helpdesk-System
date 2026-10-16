import api from "./api";

const slaTicketService = {

    //----------------------------------------------------
    // Get All SLA Reports
    //----------------------------------------------------

    getAll: async () => {

        const response = await api.get("/SlaTicket");

        return response.data;
    },


    //----------------------------------------------------
    // Get SLA Report By ID
    //----------------------------------------------------

    getById: async (id) => {

        const response = await api.get(`/SlaTicket/${id}`);

        return response.data;
    },


    //----------------------------------------------------
    // Get SLA Report By Ticket ID
    //----------------------------------------------------

    getByTicketId: async (ticketId) => {

        const response = await api.get(
            `/SlaTicket/ticket/${ticketId}`
        );

        return response.data;
    },


    //----------------------------------------------------
    // Create SLA Report From Resolved Ticket
    //----------------------------------------------------

    createFromTicket: async (ticketId) => {

        const response = await api.post(
            `/SlaTicket/create-from-ticket/${ticketId}`
        );

        return response.data;
    },


    //----------------------------------------------------
    // Update SLA Report
    //----------------------------------------------------

    update: async (id, slaReport) => {

        const response = await api.put(
            `/SlaTicket/${id}`,
            slaReport
        );

        return response.data;
    },

    //----------------------------------------------------
// Download SLA Report PDF
//----------------------------------------------------

downloadPdf: async (id) => {

    const response = await api.get(
        `/SlaTicket/${id}/pdf`,
        {
            responseType: "blob"
        }
    );

    return response.data;
},

    //----------------------------------------------------
    // Delete SLA Report
    //----------------------------------------------------

    delete: async (id) => {

        const response = await api.delete(
            `/SlaTicket/${id}`
        );

        return response.data;
    }
};

export default slaTicketService;