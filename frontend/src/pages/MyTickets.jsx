import Navbar from "../components/Navbar";

function MyTickets() {
    return (
        <div>

            <Navbar />

            <div className="container mt-4">

                <h2>My Tickets</h2>

                <div className="card shadow mt-3">

                    <div className="card-header bg-primary text-white">
                        Ticket History
                    </div>

                    <div className="card-body">

                        <table className="table table-hover">

                            <thead className="table-dark">

                                <tr>

                                    <th>ID</th>
                                    <th>Subject</th>
                                    <th>Status</th>
                                    <th>Priority</th>
                                    <th>Date</th>

                                </tr>

                            </thead>

                            <tbody>

                                <tr>

                                    <td>101</td>
                                    <td>Printer not working</td>

                                    <td>
                                        <span className="badge bg-danger">
                                            Open
                                        </span>
                                    </td>

                                    <td>High</td>

                                    <td>13 Jul 2026</td>

                                </tr>

                                <tr>

                                    <td>102</td>
                                    <td>Password Reset</td>

                                    <td>
                                        <span className="badge bg-warning text-dark">
                                            In Progress
                                        </span>
                                    </td>

                                    <td>Medium</td>

                                    <td>12 Jul 2026</td>

                                </tr>

                                <tr>

                                    <td>103</td>
                                    <td>Email Issue</td>

                                    <td>
                                        <span className="badge bg-success">
                                            Resolved
                                        </span>
                                    </td>

                                    <td>Low</td>

                                    <td>11 Jul 2026</td>

                                </tr>

                            </tbody>

                        </table>

                    </div>

                </div>

            </div>

        </div>
    );
}

export default MyTickets;