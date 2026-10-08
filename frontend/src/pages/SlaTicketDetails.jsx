import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

import slaTicketService from "../services/slaTicketService";

function SlaTicketDetails() {

    const { id } = useParams();

    const navigate = useNavigate();

    const [slaReport, setSlaReport] = useState(null);

    
    const [resolutionNotes, setResolutionNotes] = useState("");
    const [status, setStatus] = useState("Open");

    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);


    //--------------------------------------------------
    // Load SLA Report
    //--------------------------------------------------

    useEffect(() => {

        loadSlaReport();

    }, [id]);


    const loadSlaReport = async () => {

        try {

            setLoading(true);

            const data =
                await slaTicketService.getById(id);

            setSlaReport(data);

            
            setResolutionNotes(
                data.resolutionNotes || ""
            );

            setStatus(
                data.status || "Open"
            );

        }
        catch (error) {

            console.error(error);

            alert("Unable to load SLA Report.");

        }
        finally {

            setLoading(false);

        }

    };

    //--------------------------------------------------
// Download SLA Report PDF
//--------------------------------------------------

const handleDownloadPdf = async () => {

    try {

        const pdfBlob =
            await slaTicketService.downloadPdf(id);

        const url =
            window.URL.createObjectURL(pdfBlob);

        const link =
            document.createElement("a");

        link.href = url;

        link.download =
            `${slaReport.slaNumber}.pdf`;

        document.body.appendChild(link);

        link.click();

        document.body.removeChild(link);

        window.URL.revokeObjectURL(url);

    }
    catch (error) {

        console.error(
            "Failed to download SLA Report PDF:",
            error
        );

        alert(
            "Failed to download SLA Report PDF."
        );
    }
};

    //--------------------------------------------------
    // Save SLA Report
    //--------------------------------------------------

    const saveSlaReport = async () => {

        try {

            setSaving(true);

            const updated =
                await slaTicketService.update(
                    id,
                    {
                        resolutionNotes,
                        status
                    }
                );

            setSlaReport(updated);

            alert("SLA Report updated successfully.");

        }
        catch (error) {

            console.error(error);

            alert(
                error.response?.data?.message ||
                "Unable to update SLA Report."
            );

        }
        finally {

            setSaving(false);

        }

    };


    


    //--------------------------------------------------
    // Loading
    //--------------------------------------------------

    if (loading) {

        return (

            <div className="container mt-4">

                <div className="text-center">

                    <div className="spinner-border text-primary"></div>

                    <p className="mt-2">
                        Loading SLA Report...
                    </p>

                </div>

            </div>

        );

    }


    //--------------------------------------------------
    // SLA Report Not Found
    //--------------------------------------------------

    if (!slaReport) {

        return (

            <div className="container mt-4">

                <div className="alert alert-danger">

                    SLA Report could not be found.

                </div>

            </div>

        );

    }


    //--------------------------------------------------
    // Page
    //--------------------------------------------------

    return (

        <div className="container mt-4 mb-5">


            {/* ==========================================
                HEADER
            ========================================== */}

            <div className="d-flex justify-content-between align-items-center mb-4">

                <div>

                    <h2 className="fw-bold mb-1">
                        SLA Report
                    </h2>

                    <p className="text-muted mb-0">
                        {slaReport.slaNumber}
                    </p>

                </div>


                <button
                    className="btn btn-outline-secondary"
                    onClick={() => navigate(-1)}
                >
                    Back
                </button>

            </div>


            {/* ==========================================
                REPORT INFORMATION
            ========================================== */}

            <div className="card shadow-sm mb-4">

                <div className="card-header bg-dark text-white">
                    Report Information
                </div>

                <div className="card-body">

                    <div className="row">


                        <div className="col-md-4 mb-3">

                            <strong>SLA Number</strong>

                            <div>
                                {slaReport.slaNumber}
                            </div>

                        </div>


                        <div className="col-md-4 mb-3">

                            <strong>Ticket Number</strong>

                            <div>
                                #{slaReport.ticketId}
                            </div>

                        </div>


                        <div className="col-md-4 mb-3">

                            <strong>Status</strong>

                            <div>

                                <span
                                    className={
                                        slaReport.status === "Completed"
                                            ? "badge bg-success"
                                            : "badge bg-primary"
                                    }
                                >
                                    {slaReport.status}
                                </span>

                            </div>

                        </div>


                        <div className="col-md-4 mb-3">

                            <strong>Company</strong>

                            <div>
                                {slaReport.companyName || "-"}
                            </div>

                        </div>


                        <div className="col-md-4 mb-3">

                            <strong>Customer</strong>

                            <div>
                                {slaReport.customerName || "-"}
                            </div>

                        </div>


                        <div className="col-md-4 mb-3">

                            <strong>Customer Email</strong>

                            <div>
                                {slaReport.customerEmail || "-"}
                            </div>

                        </div>


                        <div className="col-md-4 mb-3">

                            <strong>Date Created</strong>

                            <div>
                                {slaReport.dateCreated
                                    ? new Date(
                                        slaReport.dateCreated
                                    ).toLocaleString()
                                    : "-"}
                            </div>

                        </div>


                        <div className="col-md-4 mb-3">

                            <strong>Date Completed</strong>

                            <div>
                                {slaReport.dateCompleted
                                    ? new Date(
                                        slaReport.dateCompleted
                                    ).toLocaleString()
                                    : "-"}
                            </div>

                        </div>


                        <div className="col-md-4 mb-3">

                            <strong>Technician</strong>

                            <div>

                                {slaReport.technician
                                    ? `${slaReport.technician.firstName} ${slaReport.technician.lastName}`
                                    : "-"}

                            </div>

                        </div>


                    </div>

                </div>

            </div>


            {/* ==========================================
                ISSUE
            ========================================== */}

            <div className="card shadow-sm mb-4">

                <div className="card-header bg-dark text-white">
                    Issue
                </div>

                <div className="card-body">

                    <p className="mb-0">

                        {slaReport.issue || "-"}

                    </p>

                </div>

            </div>


            

            {/* ==========================================
                RESOLUTION NOTES
            ========================================== */}

            <div className="card shadow-sm mb-4">

                <div className="card-header bg-dark text-white">
                    Resolution Notes
                </div>

                <div className="card-body">

                    <textarea
                        className="form-control"
                        rows="5"
                        value={resolutionNotes}
                        onChange={(e) =>
                            setResolutionNotes(e.target.value)
                        }
                        placeholder="Enter resolution notes..."
                    />

                </div>

            </div>


            {/* ==========================================
                STATUS
            ========================================== */}

            <div className="card shadow-sm mb-4">

                <div className="card-header bg-dark text-white">
                    Status
                </div>

                <div className="card-body">

                    <select
                        className="form-select"
                        value={status}
                        onChange={(e) =>
                            setStatus(e.target.value)
                        }
                    >

                        <option value="Open">
                            Open
                        </option>

                        <option value="In Progress">
                            In Progress
                        </option>

                        <option value="Completed">
                            Completed
                        </option>

                    </select>

                </div>

            </div>


            {/* ==========================================
    ACTIONS
========================================== */}

<div className="d-flex justify-content-end gap-2">

    <button
        type="button"
        className="btn btn-outline-primary"
        onClick={handleDownloadPdf}
    >
        Download PDF
    </button>

    <button
        type="button"
        className="btn btn-primary"
        onClick={saveSlaReport}
        disabled={saving}
    >
        {saving
            ? "Saving..."
            : "Save SLA Report"}
    </button>

</div>


        </div>

    );

}

export default SlaTicketDetails;