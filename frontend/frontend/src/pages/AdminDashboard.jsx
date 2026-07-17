import { useEffect, useState } from "react";
import StatisticCard from "../components/StatisticCard";
import RecentTicketsTable from "../components/RecentTicketsTable";
import dashboardService from "../services/dashboardService";
import ticketService from "../services/ticketService";

function AdminDashboard() {

    const [statistics, setStatistics] = useState({
        totalTickets: 0,
        openTickets: 0,
        inProgressTickets: 0,
        resolvedTickets: 0
    });

    const [escalatedTickets, setEscalatedTickets] = useState([]);

    const [loading, setLoading] = useState(true);

    const [error, setError] = useState("");

    const loadDashboard = async () => {

        try {

            const stats = await dashboardService.getStatistics();
            const escalated = await ticketService.getEscalatedTickets();

            setStatistics(stats);
            setEscalatedTickets(escalated);

        }
        catch (err) {

            console.error(err);
            setError("Unable to load dashboard.");

        }
        finally {

            setLoading(false);

        }

    };

    useEffect(() => {

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

            {/* Statistics */}

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

            {/* Recent Tickets */}

            <div className="card shadow mt-4">

                <div className="card-header bg-primary text-white">

                    Recent Tickets

                </div>

                <div className="card-body">

                    <RecentTicketsTable />

                </div>

            </div>

            {/* Escalated Tickets */}

            <div className="card shadow mt-4">

                <div className="card-header bg-danger text-white">

                    Escalated Tickets

                </div>

                <div className="card-body">

                    {

                        escalatedTickets.length === 0 ?

                            <p className="text-muted">

                                No escalated tickets.

                            </p>

                            :

                            <table className="table table-striped table-hover">

                                <thead>

                                    <tr>

                                        <th>ID</th>
                                        <th>Subject</th>
                                        <th>Priority</th>
                                        <th>Reason</th>
                                        <th>Status</th>

                                    </tr>

                                </thead>

                                <tbody>

                                    {

                                        escalatedTickets.map(ticket => (

                                            <tr key={ticket.ticketId}>

                                                <td>{ticket.ticketId}</td>

                                                <td>{ticket.subject}</td>

                                                <td>

                                                    <span className="badge bg-warning text-dark">

                                                        {ticket.priority}

                                                    </span>

                                                </td>

                                                <td>{ticket.escalationReason}</td>

                                                <td>

                                                    <span className="badge bg-danger">

                                                        {ticket.status}

                                                    </span>

                                                </td>

                                            </tr>

                                        ))

                                    }

                                </tbody>

                            </table>

                    }

                </div>

            </div>

        </div>

    );

}

export default AdminDashboard;