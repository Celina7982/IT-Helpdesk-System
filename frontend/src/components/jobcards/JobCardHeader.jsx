
function JobCardHeader({ jobCard }) {

    // Identify Paper Job Cards
    const isPaperJobCard = jobCard.jobCardType === 1;

    return (

        <div className="card-header bg-dark text-white">

            <div className="d-flex justify-content-between align-items-center">

                <div>

                    <h2 className="mb-0">
                        IT Helpdesk System
                    </h2>

                    <small>
                        {isPaperJobCard
                            ? "Paper Service Job Card"
                            : "Professional Service Job Card"}
                    </small>

                </div>

                <div className="text-end">

                    <h4 className="mb-0 text-warning">

                        {isPaperJobCard
                            ? jobCard.paperJobCardNumber || jobCard.jobNumber
                            : jobCard.jobNumber}

                    </h4>

                    <small>

                        {isPaperJobCard
                            ? "Paper Job Card Number"
                            : `Ticket #${jobCard.ticketId}`}

                    </small>

                </div>

            </div>

        </div>

    );

}

export default JobCardHeader;
