function TechnicianStats({

    availableTickets,
    myTickets,
    selectedCard,
    onCardClick

}) {

    const availableCount = availableTickets.length;

    const myCount = myTickets.length;

    const inProgressCount =
        myTickets.filter(ticket => ticket.status === "In Progress").length;

    const resolvedCount =
        myTickets.filter(ticket => ticket.status === "Resolved").length;

    return (

        <div className="row mb-4">

            {/* Available Tickets */}

            <div className="col-lg-3 col-md-6 mb-3">

                <div
                    className={`card shadow-sm h-100 border-2 ${
                        selectedCard === "Available"
                            ? "border-primary"
                            : ""
                    }`}
                    style={{ cursor: "pointer" }}
                    onClick={() => onCardClick("Available")}
                >

                    <div className="card-body text-center">

                        <h6 className="text-muted">

                            Available Tickets

                        </h6>

                        <h2 className="text-primary">

                            {availableCount}

                        </h2>

                    </div>

                </div>

            </div>

            {/* My Tickets */}

            <div className="col-lg-3 col-md-6 mb-3">

                <div
                    className={`card shadow-sm h-100 border-2 ${
                        selectedCard === "Mine"
                            ? "border-success"
                            : ""
                    }`}
                    style={{ cursor: "pointer" }}
                    onClick={() => onCardClick("Mine")}
                >

                    <div className="card-body text-center">

                        <h6 className="text-muted">

                            My Tickets

                        </h6>

                        <h2 className="text-success">

                            {myCount}

                        </h2>

                    </div>

                </div>

            </div>

            {/* In Progress */}

            <div className="col-lg-3 col-md-6 mb-3">

                <div
                    className={`card shadow-sm h-100 border-2 ${
                        selectedCard === "In Progress"
                            ? "border-warning"
                            : ""
                    }`}
                    style={{ cursor: "pointer" }}
                    onClick={() => onCardClick("In Progress")}
                >

                    <div className="card-body text-center">

                        <h6 className="text-muted">

                            In Progress

                        </h6>

                        <h2 className="text-warning">

                            {inProgressCount}

                        </h2>

                    </div>

                </div>

            </div>

            {/* Resolved */}

            <div className="col-lg-3 col-md-6 mb-3">

                <div
                    className={`card shadow-sm h-100 border-2 ${
                        selectedCard === "Resolved"
                            ? "border-success"
                            : ""
                    }`}
                    style={{ cursor: "pointer" }}
                    onClick={() => onCardClick("Resolved")}
                >

                    <div className="card-body text-center">

                        <h6 className="text-muted">

                            Resolved

                        </h6>

                        <h2 className="text-success">

                            {resolvedCount}

                        </h2>

                    </div>

                </div>

            </div>

        </div>

    );

}

export default TechnicianStats;