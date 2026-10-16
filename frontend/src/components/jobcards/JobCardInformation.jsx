
function JobCardInformation({ jobCard }) {

    //--------------------------------------------------
    // Identify Paper Job Cards
    //--------------------------------------------------

    const isPaperJobCard = jobCard.jobCardType === 1;

    //--------------------------------------------------
    // Paper Job Card Information
    //--------------------------------------------------

    if (isPaperJobCard) {

        const assignedTechnicians =
            jobCard.assignedTechnicians || [];

        return (

            <>

                <div className="row mb-4">

                    <div className="col-md-4">

                        <strong>Status</strong>

                        <br />

                        <span
                            className={
                                jobCard.status === "Completed"
                                    ? "badge bg-success"
                                    : jobCard.status === "Open"
                                        ? "badge bg-warning text-dark"
                                        : "badge bg-info"
                            }
                        >
                            {jobCard.status}
                        </span>

                    </div>

                    <div className="col-md-4">

                        <strong>Customer</strong>

                        <p>{jobCard.customerName}</p>

                    </div>

                    <div className="col-md-4">

                        <strong>Paper Job Card Number</strong>

                        <p>{jobCard.paperJobCardNumber || "-"}</p>

                    </div>

                </div>

                <div className="row mb-4">

                    <div className="col-md-6">

                        <strong>Assigned Technicians</strong>

                        {assignedTechnicians.length > 0 ? (

                            <ul className="mt-2">

                                {assignedTechnicians.map(
                                    (technician) => (

                                        <li key={technician.technicianId}>

                                            {technician.technicianName}

                                            {technician.isLeadTechnician && (
                                                <span className="badge bg-primary ms-2">
                                                    Lead
                                                </span>
                                            )}

                                        </li>

                                    )
                                )}

                            </ul>

                        ) : (

                            <p>No technicians assigned.</p>

                        )}

                    </div>

                    <div className="col-md-6">

                        <strong>Captured By</strong>

                        <p>{jobCard.capturedByUserName || "-"}</p>

                    </div>

                </div>

                <div className="row mb-4">

                    <div className="col-md-6">

                        <strong>Paper Job Card Date</strong>

                        <p>
                            {jobCard.paperJobCardDate
                                ? new Date(
                                    jobCard.paperJobCardDate
                                ).toLocaleDateString()
                                : "-"}
                        </p>

                    </div>

                    <div className="col-md-6">

                        <strong>Date Captured</strong>

                        <p>
                            {jobCard.dateCaptured
                                ? new Date(
                                    jobCard.dateCaptured
                                ).toLocaleString()
                                : "-"}
                        </p>

                    </div>

                </div>

                <div className="row">

                    <div className="col-md-6">

                        <strong>Date Created</strong>

                        <p>
                            {jobCard.dateCreated
                                ? new Date(
                                    jobCard.dateCreated
                                ).toLocaleString()
                                : "-"}
                        </p>

                    </div>

                    <div className="col-md-6">

                        <strong>Date Completed</strong>

                        <p>
                            {jobCard.dateCompleted
                                ? new Date(
                                    jobCard.dateCompleted
                                ).toLocaleString()
                                : "-"}
                        </p>

                    </div>

                </div>

            </>

        );

    }

    //--------------------------------------------------
    // Digital Job Card Information
    // Existing functionality preserved
    //--------------------------------------------------

    return (

        <>

            <div className="row mb-4">

                <div className="col-md-4">

                    <strong>Status</strong>

                    <br />

                    <span
                        className={
                            jobCard.status === "Completed"
                                ? "badge bg-success"
                                : jobCard.status === "Open"
                                    ? "badge bg-warning text-dark"
                                    : "badge bg-info"
                        }
                    >
                        {jobCard.status}
                    </span>

                </div>

                <div className="col-md-4">

                    <strong>Customer</strong>

                    <p>{jobCard.customerName}</p>

                </div>

                <div className="col-md-4">

                    <strong>Assigned Technician</strong>

                    <p>{jobCard.assignedTechnician}</p>

                </div>

            </div>

            <div className="row">

                <div className="col-md-6">

                    <strong>Date Created</strong>

                    <p>
                        {new Date(jobCard.dateCreated).toLocaleString()}
                    </p>

                </div>

                <div className="col-md-6">

                    <strong>Date Completed</strong>

                    <p>
                        {jobCard.dateCompleted
                            ? new Date(
                                jobCard.dateCompleted
                            ).toLocaleString()
                            : "-"}
                    </p>

                </div>

            </div>

        </>

    );

}

export default JobCardInformation;
