import { useEffect, useState } from "react";
import StatisticCard from "../components/StatisticCard";
import RecentTicketsTable from "../components/RecentTicketsTable";
import EscalatedTicketsTable from "../components/EscalatedTicketsTable";
import dashboardService from "../services/dashboardService";

function AdminDashboard() {

    const [statistics, setStatistics] = useState({

        totalTickets: 0,
        openTickets: 0,
        inProgressTickets: 0,
        resolvedTickets: 0

    });

    const [loading, setLoading] = useState(true);

    const [error, setError] = useState("");

    useEffect(() => {

        const loadDashboard = async () => {

            try {

                const stats = await dashboardService.getStatistics();

                setStatistics(stats);

            }
            catch (err) {

                console.error(err);

                setError("Unable to load dashboard.");

            }
            finally {

                setLoading(false);

            }

        };

        loadDashboard();

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

        <div className="container mt-4">

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

            <div className="card shadow mt-4">

                <div className="card-header bg-primary text-white">

                    Recent Tickets

                </div>

                <div className="card-body">

                    <RecentTicketsTable />

                </div>

            </div>

            <EscalatedTicketsTable />

        </div>

    );

}

export default AdminDashboard;