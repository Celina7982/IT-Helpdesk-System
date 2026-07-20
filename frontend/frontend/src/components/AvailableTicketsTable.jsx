function AvailableTicketsTable({

    tickets,
    loading,
    onView,
    onClaim

}) {

    return (

        <div className="card shadow mt-4">

            <div className="card-header bg-primary text-white">

                Available Tickets

            </div>

            <div className="card-body">

                {

                    loading ?

                        <div className="text-center">

                            <div className="spinner-border text-primary"></div>

                            <p className="mt-2">

                                Loading available tickets...

                            </p>

                        </div>

                        :

                        <table className="table table-striped table-hover">

                            <thead>

                                <tr>

                                    <th>ID</th>
                                    <th>Subject</th>
                                    <th>Customer</th>
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

                                                No available tickets.

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

                                                    {ticket.customerName}

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
                                                        className="btn btn-success btn-sm"
                                                        onClick={() =>
                                                            onClaim(ticket.ticketId)
                                                        }
                                                    >

                                                        Claim

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

export default AvailableTicketsTable;