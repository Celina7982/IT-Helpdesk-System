import { useNavigate } from "react-router-dom";
import Sidebar from "../components/Sidebar";

function TechnicianDashboard() {
    const navigate = useNavigate();

    return (
        <div className="d-flex">

            {/* Sidebar */}
            <Sidebar />

            {/* Main Content */}
            <div className="container-fluid p-4">

                <h2>Welcome, Technician</h2>

                <p className="text-muted">
                    Manage assigned support tickets.
                </p>

                {/* Dashboard Cards */}
                <div className="row mt-4">

                    <div className="col-md-4">

                        <div className="card shadow border-0">

                            <div className="card-body text-center">

                                <h5>Assigned Tickets</h5>

                                <h1 className="text-primary">8</h1>

                            </div>

                        </div>

                    </div>

                    <div className="col-md-4">

                        <div className="card shadow border-0">

                            <div className="card-body text-center">

                                <h5>In Progress</h5>

                                <h1 className="text-warning">3</h1>

                            </div>

                        </div>

                    </div>

                    <div className="col-md-4">

                        <div className="card shadow border-0">

                            <div className="card-body text-center">

                                <h5>Resolved Today</h5>

                                <h1 className="text-success">5</h1>

                            </div>

                        </div>

                    </div>

                </div>

                {/* Assigned Tickets Table */}
                <div className="card shadow mt-5">

                    <div className="card-header bg-primary text-white">

                        Assigned Tickets

                    </div>

                    <div className="card-body">

                        <table className="table table-hover">

                            <thead className="table-dark">

                                <tr>

                                    <th>ID</th>
                                    <th>Client</th>
                                    <th>Issue</th>
                                    <th>Status</th>
                                    <th>Priority</th>
                                    <th>Action</th>

                                </tr>

                            </thead>

                            <tbody>

                                <tr>

                                    <td>101</td>
                                    <td>John Smith</td>
                                    <td>Printer Offline</td>

                                    <td>
                                        <span className="badge bg-danger">
                                            Open
                                        </span>
                                    </td>

                                    <td>High</td>

                                    <td>
                                        <button
                                            className="btn btn-sm btn-primary"
                                            onClick={() => navigate("/ticket-details")}
                                        >
                                            View
                                        </button>
                                    </td>

                                </tr>

                                <tr>

                                    <td>102</td>
                                    <td>Sarah Jones</td>
                                    <td>Password Reset</td>

                                    <td>
                                        <span className="badge bg-warning text-dark">
                                            In Progress
                                        </span>
                                    </td>

                                    <td>Medium</td>

                                    <td>
                                        <button
                                            className="btn btn-sm btn-primary"
                                            onClick={() => navigate("/ticket-details")}
                                        >
                                            View
                                        </button>
                                    </td>

                                </tr>

                                <tr>

                                    <td>103</td>
                                    <td>Mike Brown</td>
                                    <td>Email Issue</td>

                                    <td>
                                        <span className="badge bg-success">
                                            Resolved
                                        </span>
                                    </td>

                                    <td>Low</td>

                                    <td>
                                        <button
                                            className="btn btn-sm btn-primary"
                                            onClick={() => navigate("/ticket-details")}
                                        >
                                            View
                                        </button>
                                    </td>

                                </tr>

                                <tr>

                                    <td>104</td>
                                    <td>Jane Doe</td>
                                    <td>Network Connection</td>

                                    <td>
                                        <span className="badge bg-danger">
                                            Open
                                        </span>
                                    </td>

                                    <td>High</td>

                                    <td>
                                        <button
                                            className="btn btn-sm btn-primary"
                                            onClick={() => navigate("/ticket-details")}
                                        >
                                            View
                                        </button>
                                    </td>

                                </tr>

                            </tbody>

                        </table>

                    </div>

                </div>

            </div>

        </div>
    );
}

export default TechnicianDashboard;