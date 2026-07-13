import Sidebar from "../components/Sidebar";

function ClientDashboard() {
    return (
        <div className="d-flex">

            <Sidebar />

            <div className="container-fluid p-4">

                <h2>Welcome, Client</h2>

                <p className="text-muted">
                    IT Helpdesk Dashboard
                </p>

                <div className="row mt-4">

                    <div className="col-md-4">

                        <div className="card shadow border-0">

                            <div className="card-body text-center">

                                <h5>Total Tickets</h5>

                                <h1 className="text-primary">5</h1>

                            </div>

                        </div>

                    </div>

                    <div className="col-md-4">

                        <div className="card shadow border-0">

                            <div className="card-body text-center">

                                <h5>Open Tickets</h5>

                                <h1 className="text-danger">3</h1>

                            </div>

                        </div>

                    </div>

                    <div className="col-md-4">

                        <div className="card shadow border-0">

                            <div className="card-body text-center">

                                <h5>Resolved Tickets</h5>

                                <h1 className="text-success">2</h1>

                            </div>

                        </div>

                    </div>

                </div>

                <div className="card shadow mt-5">

                    <div className="card-header bg-primary text-white">

                        Recent Tickets

                    </div>

                    <div className="card-body">

                        <table className="table table-hover">

                            <thead className="table-dark">

                                <tr>
                                    <th>ID</th>
                                    <th>Subject</th>
                                    <th>Status</th>
                                    <th>Priority</th>
                                </tr>

                            </thead>

                            <tbody>

                                <tr>
                                    <td>101</td>
                                    <td>Printer Offline</td>
                                    <td><span className="badge bg-danger">Open</span></td>
                                    <td>High</td>
                                </tr>

                                <tr>
                                    <td>102</td>
                                    <td>Password Reset</td>
                                    <td><span className="badge bg-warning text-dark">In Progress</span></td>
                                    <td>Medium</td>
                                </tr>

                                <tr>
                                    <td>103</td>
                                    <td>Email Issue</td>
                                    <td><span className="badge bg-success">Resolved</span></td>
                                    <td>Low</td>
                                </tr>

                            </tbody>

                        </table>

                    </div>

                </div>

            </div>

        </div>
    );
}

export default ClientDashboard;