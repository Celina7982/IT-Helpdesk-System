function MyTicketsTable({

    tickets,
    loading,
    onView,
    onResolve,
    onEscalate

}) {

    return (

        <div className="card shadow mt-4">

            <div className="card-header bg-success text-white">

                My Tickets

            </div>

            <div className="card-body">

                {

                    loading ?

                        <div className="text-center">

                            <div className="spinner-border text-success"></div>

                            <p className="mt-2">

                                Loading your tickets...

                            </p>

                        </div>

                        :

                        <table className="table table-striped table-hover">

                            <thead>

                                <tr>

                                    <th>ID</th>
                                    <th>Subject</th>
                                    <th>Status</th>
                                    <th>Priority</th>
                                    <th>Actions</th>

                                </tr>

                            </thead>

                            <tbody>

                                {

                                    tickets.length === 0 ?

                                        <tr>

                                            <td
                                                colSpan="5"
                                                className="text-center"
                                            >

                                                You have no assigned tickets.

                                            </td>

                                        </tr>

                                        :

                                        tickets.map(ticket => (

                                            <tr key={ticket.ticketId}>

                                                <td>

                                                    {ticket.ticketId}

                                                </td>

                                                <td>

                                                    {ticket.subject}

                                                </td>

                                                <td>

                                                    <span
                                                        className={
                                                            ticket.status === "Resolved"
                                                                ? "badge bg-success"
                                                                : ticket.status === "In Progress"
                                                                    ? "badge bg-info"
                                                                    : "badge bg-warning text-dark"
                                                        }
                                                    >

                                                        {ticket.status}

                                                    </span>

                                                </td>

                                                <td>

                                                    <span
                                                        className={
                                                            ticket.priority === "High"
                                                                ? "badge bg-danger"
                                                                : ticket.priority === "Medium"
                                                                    ? "badge bg-warning text-dark"
                                                                    : "badge bg-success"
                                                        }
                                                    >

                                                        {ticket.priority}

                                                    </span>

                                                </td>

                                                <td>

                                                    <button
                                                        className="btn btn-outline-primary btn-sm me-2"
                                                        onClick={() =>
                                                            onView(ticket.ticketId)
                                                        }
                                                    >
                                                        View
                                                    </button>

                                                    <button
                                                        className="btn btn-success btn-sm me-2"
                                                        onClick={() =>
                                                            onResolve(ticket.ticketId)
                                                        }
                                                    >
                                                        Resolve
                                                    </button>

                                                    <button
                                                        className="btn btn-warning btn-sm"
                                                        onClick={() =>
                                                            onEscalate(ticket.ticketId)
                                                        }
                                                    >
                                                        Escalate
                                                    </button>

                                                </td>

                                            </tr>

                                        ))

                                }

                            </tbody>

                        </table>

                }

            </div>

        </div>

    );

}

export default MyTicketsTable;