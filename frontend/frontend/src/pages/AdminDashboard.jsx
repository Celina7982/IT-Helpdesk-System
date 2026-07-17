import { useEffect, useState } from "react";
import StatisticCard from "../components/StatisticCard";
import dashboardService from "../services/dashboardService";
import RecentTicketsTable from "../components/RecentTicketsTable";

function AdminDashboard() {

    // Dashboard statistics
    const [statistics, setStatistics] = useState({
        totalTickets: 0,
        openTickets: 0,
        inProgressTickets: 0,
        resolvedTickets: 0
    });

    // Loading indicator
    const [loading, setLoading] = useState(true);

    // Error message
    const [error, setError] = useState("");

    /**
     * Loads dashboard statistics from the API.
     */
    const loadStatistics = async () => {

        try {

            const data = await dashboardService.getStatistics();

            setStatistics(data);

        }
        catch (err) {

            console.error(err);
            setError("Unable to load dashboard statistics.");

        }
        finally {

            setLoading(false);

        }

    };

    // Load dashboard once when page opens
    useEffect(() => {

        loadStatistics();

    }, []);

    if (loading) {

        return (

            <div className="text-center mt-5">

                <div className="spinner-border text-primary"></div>

                <p className="mt-3">
                    Loading dashboard...
                </p>

            </div>

        );

    }

    if (error) {

        return (

            <div className="alert alert-danger mt-4">

                {error}

            </div>

        );

    }

    return (

        <div>

            <h2 className="mb-4">
                IT Helpdesk Admin Dashboard
            </h2>

            <div className="row">

                <StatisticCard
                    title="Total Tickets"
                    value={statistics.totalTickets}
                    color="primary"
                />

                <StatisticCard
                    title="Open Tickets"
                    value={statistics.openTickets}
                    color="warning"
                />

                <StatisticCard
                    title="In Progress"
                    value={statistics.inProgressTickets}
                    color="info"
                />

                <StatisticCard
                    title="Resolved"
                    value={statistics.resolvedTickets}
                    color="success"
                />

            </div>

            <div className="card body">

                    <RecentTicketsTable />

                </div>

            </div>

        

    );

}

export default AdminDashboard;