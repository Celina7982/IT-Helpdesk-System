function JobCardWorkDetails({

    jobCard,
    setJobCard,
    onSave,
    onComplete,
    role

})

{

    //--------------------------------------------------
    // Permissions
    //--------------------------------------------------

    const technicianReadOnly =

        role === "Technician" &&
        jobCard.status === "Completed";

    //--------------------------------------------------
    // Handle Changes
    //--------------------------------------------------

    const handleChange = (e) => {

        setJobCard({

            ...jobCard,

            [e.target.name]: e.target.value

        });

    };

    return (

        <div className="card mt-4">

            <div className="card-header bg-primary text-white">

                Work Details

            </div>

            <div className="card-body">

                {/* Status */}

                <div className="mb-3">

                    <label className="form-label">

                        Status

                    </label>

                    <select
                        className="form-select"
                        name="status"
                        value={jobCard.status}
                        onChange={handleChange}
                        disabled={technicianReadOnly}
                    >

                        <option value="Open">

                            Open

                        </option>

                        <option value="In Progress">

                            In Progress

                        </option>

                        

                    </select>

                </div>

                {/* Fault Reported */}

                <div className="mb-3">

                    <label className="form-label">

                        Fault Reported

                    </label>

                    <textarea
                        className="form-control"
                        rows="3"
                        value={jobCard.faultReported}
                        disabled
                    />

                </div>

                {/* Fault Found */}

                <div className="mb-3">

                    <label className="form-label">

                        Fault Found

                    </label>

                    <textarea
                        className="form-control"
                        rows="4"
                        name="faultFound"
                        value={jobCard.faultFound || ""}
                        onChange={handleChange}
                        disabled={technicianReadOnly}
                    />

                </div>

                {/* Work Performed */}

                <div className="mb-3">

                    <label className="form-label">

                        Work Performed

                    </label>

                    <textarea
                        className="form-control"
                        rows="5"
                        name="workPerformed"
                        value={jobCard.workPerformed || ""}
                        onChange={handleChange}
                        disabled={technicianReadOnly}
                    />

                </div>

                {/* Completion Notes */}

                <div className="mb-3">

                    <label className="form-label">

                        Completion Notes

                    </label>

                    <textarea
                        className="form-control"
                        rows="4"
                        name="completionNotes"
                        value={jobCard.completionNotes || ""}
                        onChange={handleChange}
                        disabled={technicianReadOnly}
                    />

                </div>

                {/* Customer Signature */}

                <div className="mb-4">

                    <label className="form-label">

                        Customer Signature

                    </label>

                    <input
                        className="form-control"
                        name="customerSignature"
                        value={jobCard.customerSignature || ""}
                        onChange={handleChange}
                        disabled={technicianReadOnly}
                    />

                </div>

                {/* Save Button */}

                {(!technicianReadOnly) && (

                    <button
                        className="btn btn-success"
                        onClick={onSave}
                    >

                        💾 Save Changes

                    </button>

                )}

                {
                    jobCard.status !== "Completed" && (

                        <button
                            className="btn btn-primary ms-2"
                            onClick={() => onComplete()}
                        >

                            ✔ Complete Job Card

                        </button>

                    )
                }

                {/* Read-only Message */}

                {technicianReadOnly && (

                    <div className="alert alert-warning mt-3 mb-0">

                        <strong>Job Card Completed</strong>

                        <br />

                        This Job Card is read-only because it has been completed.

                    </div>

                )}

            </div>

        </div>

    );

}

export default JobCardWorkDetails;