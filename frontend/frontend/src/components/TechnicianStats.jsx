function TechnicianStats({ availableTickets, myTickets }) {

    const resolvedToday =
        myTickets.filter(ticket => ticket.status === "Resolved").length;

    return (

        <div className="row mb-4">

            <div className="col-md-4">

                <div className="card shadow text-center">

                    <div className="card-body">

                        <h6 className="text-muted">

                            Available Tickets

                        </h6>

                        <h2 className="text-primary">

                            {availableTickets.length}

                        </h2>

                    </div>

                </div>

            </div>

            <div className="col-md-4">

                <div className="card shadow text-center">

                    <div className="card-body">

                        <h6 className="text-muted">

                            My Tickets

                        </h6>

                        <h2 className="text-success">

                            {myTickets.length}

                        </h2>

                    </div>

                </div>

            </div>

            <div className="col-md-4">

                <div className="card shadow text-center">

                    <div className="card-body">

                        <h6 className="text-muted">

                            Resolved

                        </h6>

                        <h2 className="text-warning">

                            {resolvedToday}

                        </h2>

                    </div>

                </div>

            </div>

        </div>

    );

}

export default TechnicianStats;