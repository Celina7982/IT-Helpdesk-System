import { useEffect, useState } from "react";
import ticketService from "../services/ticketService";

function TicketDetailsModal({ show, onClose, ticketId }) {

    const [ticket, setTicket] = useState(null);
    const [loading, setLoading] = useState(false);

    useEffect(() => {

        if (!show || !ticketId)
            return;

        loadTicket();

    }, [show, ticketId]);

    const loadTicket = async () => {

        setLoading(true);

        try {

            const data = await ticketService.getTicketDetails(ticketId);

            setTicket(data);

        }
        catch (error) {

            console.error(error);
            alert("Unable to load ticket details.");

        }
        finally {

            setLoading(false);

        }

    };

    if (!show)
        return null;

    return (

        <div
            className="modal fade show"
            style={{
                display: "block",
                backgroundColor: "rgba(0,0,0,0.5)"
            }}
        >

            <div className="modal-dialog modal-lg">

                <div className="modal-content">

                    <div className="modal-header">

                        <h5 className="modal-title">

                            Ticket #{ticket?.ticketId}

                        </h5>

                        <button
                            className="btn-close"
                            onClick={onClose}
                        ></button>

                    </div>

                    <div className="modal-body">

                        {

                            loading ?

                                <p>Loading...</p>

                                :

                                ticket &&

                                <table className="table">

                                    <tbody>

                                        <tr>
                                            <th>Subject</th>
                                            <td>{ticket.subject}</td>
                                        </tr>

                                        <tr>
                                            <th>Description</th>
                                            <td>{ticket.description}</td>
                                        </tr>

                                        <tr>
                                            <th>Customer</th>
                                            <td>{ticket.customerName}</td>
                                        </tr>

                                        <tr>
                                            <th>Company</th>
                                            <td>{ticket.companyName}</td>
                                        </tr>

                                        <tr>
                                            <th>Category</th>
                                            <td>{ticket.category}</td>
                                        </tr>

                                        <tr>
                                            <th>Priority</th>
                                            <td>{ticket.priority}</td>
                                        </tr>

                                        <tr>
                                            <th>Status</th>
                                            <td>{ticket.status}</td>
                                        </tr>

                                        <tr>
                                            <th>Assigned Technician</th>
                                            <td>{ticket.assignedTechnician}</td>
                                        </tr>

                                        <tr>
                                            <th>Created</th>
                                            <td>
                                                {new Date(ticket.createdDate).toLocaleString()}
                                            </td>
                                        </tr>

                                        <tr>
                                            <th>Escalated</th>
                                            <td>
                                                {ticket.isEscalated ? "Yes" : "No"}
                                            </td>
                                        </tr>

                                        {
                                            ticket.isEscalated &&

                                            <tr>

                                                <th>Escalation Reason</th>

                                                <td>{ticket.escalationReason}</td>

                                            </tr>

                                        }

                                    </tbody>

                                </table>

                        }

                    </div>

                    <div className="modal-footer">

                        <button
                            className="btn btn-secondary"
                            onClick={onClose}
                        >
                            Close
                        </button>

                    </div>

                </div>

            </div>

        </div>

    );

}

export default TicketDetailsModal;