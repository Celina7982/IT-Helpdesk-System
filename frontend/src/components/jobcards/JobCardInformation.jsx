function JobCardInformation({ jobCard }) {

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

                        {
                            jobCard.dateCompleted
                                ? new Date(jobCard.dateCompleted).toLocaleString()
                                : "-"
                        }

                    </p>

                </div>

            </div>

        </>

    );

}

export default JobCardInformation;