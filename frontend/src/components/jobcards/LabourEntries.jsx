import { useState } from "react";

function LabourEntries({

    labourEntries = [],

    onAdd,

    role,

    status

}) {

    //--------------------------------------------------
    // State
    //--------------------------------------------------

    const [hoursWorked, setHoursWorked] = useState("");

    const [workPerformed, setWorkPerformed] = useState("");

    const [saving, setSaving] =useState(false);

    //--------------------------------------------------
    // Technician Read Only
    //--------------------------------------------------

    const technicianReadOnly =

        role === "Technician" &&
        status === "Completed";

    //--------------------------------------------------
    // Total Labour Hours
    //--------------------------------------------------

    const totalHours = labourEntries.reduce(

        (total, entry) =>

            total + Number(entry.hoursWorked || 0),

        0

    );

    //--------------------------------------------------
    // Add Labour Entry
    //--------------------------------------------------

    const addEntry = async () => {

        //--------------------------------------------------
        // Validation
        //--------------------------------------------------

        if (!hoursWorked || !workPerformed.trim()) {

            alert("Please complete all fields.");

            return;

        }

        try {

            setSaving(true);

            await onAdd({

                hoursWorked: parseFloat(hoursWorked),

                workPerformed,

                dateWorked: new Date().toISOString()

            });

            //--------------------------------------------------
            // Clear Form
            //--------------------------------------------------

            setHoursWorked("");

            setWorkPerformed("");

        }
        finally {

            setSaving(false);

        }

    };

    //--------------------------------------------------
    // Return
    //--------------------------------------------------

    return (

        <div className="card shadow mt-4">

            <div className="card-header bg-secondary text-white">

                <h5 className="mb-0">

                    Technician Labour Entries

                </h5>

            </div>

            <div className="card-body">

                {/*==========================================
                    Labour Table
                ==========================================*/}

                <table className="table table-striped table-bordered">

                    <thead className="table-light">

                        <tr>

                            <th style={{ width: "150px" }}>
                                Date
                            </th>

                            <th style={{ width: "120px" }}>
                                Hours
                            </th>

                            <th>
                                Work Performed
                            </th>

                        </tr>

                    </thead>

                    <tbody>

                        {

                            labourEntries.length === 0 ?

                                <tr>

                                    <td
                                        colSpan="3"
                                        className="text-center text-muted py-4"
                                    >

                                        No labour entries have been added.

                                    </td>

                                </tr>

                                :

                                labourEntries.map((entry, index) => (

                                    <tr key={index}>

                                        <td>

                                            {

                                                new Date(
                                                    entry.dateWorked
                                                ).toLocaleDateString()

                                            }

                                        </td>

                                        <td>

                                            {entry.hoursWorked}

                                        </td>

                                        <td>

                                            {entry.workPerformed}

                                        </td>

                                    </tr>

                                ))

                        }

                    </tbody>

                </table>

                {/*==========================================
                    Total Hours
                ==========================================*/}

                <div className="alert alert-info">

                    <strong>

                        Total Labour Hours:

                    </strong>

                    {" "}

                    {totalHours.toFixed(2)} hrs

                </div>

                {/*==========================================
                    Add Labour Form
                ==========================================*/}

                {

                    !technicianReadOnly && (

                        <>

                            <hr />

                            <h6 className="mb-3">

                                Add New Labour Entry

                            </h6>

                            <div className="row g-2">

                                <div className="col-md-2">

                                    <input
                                        type="number"
                                        className="form-control"
                                        min="0.25"
                                        step="0.25"
                                        placeholder="Hours"
                                        value={hoursWorked}
                                        onChange={(e) =>
                                            setHoursWorked(e.target.value)
                                        }
                                    />

                                </div>

                                <div className="col-md-8">

                                    <input
                                        className="form-control"
                                        placeholder="Describe the work completed..."
                                        value={workPerformed}
                                        onChange={(e) =>
                                            setWorkPerformed(e.target.value)
                                        }
                                    />

                                </div>

                                <div className="col-md-2 d-grid">

                                    <button
                                        className="btn btn-success"
                                        onClick={addEntry}
                                        disabled={saving}
                                    >

                                        {

                                            saving

                                                ? "Saving..."

                                                : "Add Entry"

                                        }

                                    </button>

                                </div>

                            </div>

                        </>

                    )

                }

                {/*==========================================
                    Completed Message
                ==========================================*/}

                {

                    technicianReadOnly && (

                        <div className="alert alert-warning mt-3">

                            <strong>

                                Job Card Completed

                            </strong>

                            <br />

                            Labour entries can no longer be added.

                        </div>

                    )

                }

            </div>

        </div>

    );

}

export default LabourEntries;