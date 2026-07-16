import Navbar from "../components/Navbar";

function TicketDetails() {
    return (
        <div>

            <Navbar />

            <div className="container mt-4">

                <h2>Ticket Details</h2>

                <div className="card shadow">

                    <div className="card-body">

                        <p><strong>Ticket ID:</strong> 101</p>

                        <p><strong>Client:</strong> John Smith</p>

                        <p><strong>Subject:</strong> Printer Offline</p>

                        <p><strong>Category:</strong> Hardware</p>

                        <p><strong>Priority:</strong> High</p>

                        <p>
                            <strong>Status:</strong>
                            <span className="badge bg-danger ms-2">
                                Open
                            </span>
                        </p>

                        <hr />

                        <h5>Description</h5>

                        <p>
                            The office printer is offline and no one can print.
                        </p>

                        <hr />

                        <h5>Technician Actions</h5>

                        <div className="mb-3">

                            <label className="form-label">
                                Update Status
                            </label>

                            <select className="form-select">

                                <option>Open</option>
                                <option>In Progress</option>
                                <option>Resolved</option>

                            </select>

                        </div>

                        <button className="btn btn-primary me-2">
                            Save Changes
                        </button>

                        <button className="btn btn-success">
                            Mark as Resolved
                        </button>

                    </div>

                </div>

            </div>

        </div>
    );
}

export default TicketDetails;