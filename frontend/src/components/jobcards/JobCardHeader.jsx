function JobCardHeader({ jobCard }) {

    return (

        <div className="card-header bg-dark text-white">

            <div className="d-flex justify-content-between align-items-center">

                <div>

                    <h2 className="mb-0">

                        IT Helpdesk System

                    </h2>

                    <small>

                        Professional Service Job Card

                    </small>

                </div>

                <div className="text-end">

                    <h4 className="mb-0 text-warning">

                        {jobCard.jobNumber}

                    </h4>

                    <small>

                        Ticket #{jobCard.ticketId}

                    </small>

                </div>

            </div>

        </div>

    );

}

export default JobCardHeader;