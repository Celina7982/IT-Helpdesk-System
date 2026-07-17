import { useEffect, useState } from "react";
import dashboardService from "../services/dashboardService";
import StatusBadge from "./StatusBadge";
import PriorityBadge from "./PriorityBadge";


function RecentTicketsTable() {

    const [tickets, setTickets] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    /**
     * Load recent tickets from the API.
     */
    const loadRecentTickets = async () => {

        try {

            const data = await dashboardService.getRecentTickets();

            setTickets(data);

        }
        catch (err) {

            console.error(err);
            setError("Unable to load recent tickets.");

        }
        finally {

            setLoading(false);

        }

    };

    useEffect(() => {

        loadRecentTickets();

    }, []);

    if (loading) {

        return (

            <div className="text-center p-4">

                <div className="spinner-border text-primary"></div>

            </div>

        );

    }

    if (error) {

        return (

            <div className="alert alert-danger">

                {error}

            </div>

        );

    }

    return (

        <table className="table table-striped table-hover">

            <thead>

                <tr>

                    <th>Ticket #</th>
                    <th>Subject</th>
                    <th>Status</th>
                    <th>Priority</th>

                </tr>

            </thead>

            <tbody>

                {tickets.map(ticket => (

                    <tr key={ticket.ticketId}>

                        <td>{ticket.ticketId}</td>

                        <td>{ticket.subject}</td>

                        <td><StatusBadge status={ticket.status} /></td>

                        <td><PriorityBadge priority={ticket.priority} /></td>

                    </tr>

                ))}

            </tbody>

        </table>

    );

}

export default RecentTicketsTable;