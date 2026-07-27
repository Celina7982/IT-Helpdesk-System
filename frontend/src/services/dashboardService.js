import api from "./api";

/**
 * Dashboard Service
 * -----------------
 * Handles all Dashboard API requests.
 */

/**
 * Get dashboard statistics.
 */
const getStatistics = async () => {

    const response = await api.get("/Dashboard/statistics");

    return response.data;
};

/**
 * Get recent tickets.
 */
const getRecentTickets = async () => {

    const response = await api.get("/Dashboard/recent-tickets");

    return response.data;
};

const dashboardService = {

    getStatistics,
    getRecentTickets

};

export default dashboardService;