//--------------------------------------------------
// React Imports
//--------------------------------------------------

import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

//--------------------------------------------------
// Services
//--------------------------------------------------

import jobCardService from "../services/jobCardService";


//=================
//JWT decode import
//=================
import { jwtDecode } from "jwt-decode";


import ConfirmationModal from "../components/common/ConfirmationModal";

//==================================================
// Job Cards Page
//==================================================
//
// Purpose
// -------
// Displays all Job Cards available to the currently
// logged in user.
//
// Administrators
//      Can view ALL Job Cards.
//
// Technicians
//      Can only view Job Cards assigned to them.
//
// Features
// --------
// • Dashboard statistics
// • Search
// • Status filtering
// • Clickable dashboard cards
// • Responsive Bootstrap layout
// • View Job Card button
//
//==================================================

function JobCards() {

    //--------------------------------------------------
    // Navigation
    //--------------------------------------------------
    //
    // Used when the user clicks "View"
    // to open the Job Card Details page.
    //
    //--------------------------------------------------

    const navigate = useNavigate();

    //--------------------------------------------------
    // State
    //--------------------------------------------------

    // Complete list returned by the API.
    const [jobCards, setJobCards] = useState([]);

    // Shows loading spinner while data loads.
    const [loading, setLoading] = useState(true);

    // Search textbox.
    const [searchTerm, setSearchTerm] = useState("");

    // Current selected status.
    const [statusFilter, setStatusFilter] = useState("All");

    // Selected dashboard card.
    // Used for highlighting the active card.
    const [selectedCard, setSelectedCard] = useState("All");


    //--------------------------------------------------
// Complete Job Card Modal
//--------------------------------------------------

const [showCompleteModal, setShowCompleteModal] = useState(false);

const [selectedJobCardId, setSelectedJobCardId] = useState(null);
    //--------------------------------------------------
    // Load Job Cards
    //--------------------------------------------------
    //
    // Executes once when the page loads.
    //
    //--------------------------------------------------

    useEffect(() => {

        loadJobCards();

    }, []);

    //--------------------------------------------------
    // Retrieve Job Cards
    //--------------------------------------------------
    //
    // Calls the backend API and retrieves the Job Cards
    // the logged in user is allowed to see.
    //
    //--------------------------------------------------

    const loadJobCards = async () => {

        try {

            const data = await jobCardService.getAll();

            setJobCards(data);

        }
        catch (error) {

            console.error(error);

            alert("Unable to load Job Cards.");

        }
        finally {

            setLoading(false);

        }

    };

    //--------------------------------------------------
    // Status Badge Colours
    //--------------------------------------------------
    //
    // Returns the Bootstrap badge colour based on
    // the Job Card status.
    //
    //--------------------------------------------------

    const getStatusBadge = (status) => {

        switch (status) {

            case "Open":
                return "bg-primary";

            case "In Progress":
                return "bg-warning text-dark";

            case "Completed":
                return "bg-success";

            case "Cancelled":
                return "bg-danger";

            default:
                return "bg-secondary";

        }

    };

    //--------------------------------------------------
    // Dashboard Statistics
    //--------------------------------------------------
    //
    // Calculates the totals displayed in the dashboard.
    //
    //--------------------------------------------------

    const totalJobs =
        jobCards.length;

    const openJobs =
        jobCards.filter(job => job.status === "Open").length;

    const inProgressJobs =
        jobCards.filter(job => job.status === "In Progress").length;

    const completedJobs =
        jobCards.filter(job => job.status === "Completed").length;

    const cancelledJobs =
        jobCards.filter(job => job.status === "Cancelled").length;

    //--------------------------------------------------
    // Search + Filter
    //--------------------------------------------------
    //
    // Filters the Job Cards based on:
    //
    // • Search text
    // • Selected status
    //
    //--------------------------------------------------

    const filteredJobCards = jobCards.filter(job => {

        //--------------------------------------------------
        // Search
        //--------------------------------------------------

       //--------------------------------------------------
// Safe Search
//
// Some Job Cards may have null values.
// Convert null values into empty strings before
// searching.
//--------------------------------------------------
const search = searchTerm.trim().toLowerCase();

const jobNumber =
    (job.jobNumber ?? "").toLowerCase();

const customer =
    (job.customerName ?? "").toLowerCase();

const company =
    (job.companyName ?? "").toLowerCase();

const subject =
    (job.subject ?? "").toLowerCase();

const matchesSearch =

    jobNumber.includes(search) ||

    customer.includes(search) ||

    company.includes(search) ||

    subject.includes(search);
        //--------------------------------------------------
        // Status
        //--------------------------------------------------

        const matchesStatus =

            statusFilter === "All" ||

            job.status === statusFilter;

        //--------------------------------------------------
        // Display rows that satisfy BOTH filters.
        //--------------------------------------------------

        return matchesSearch && matchesStatus;

    });

    //--------------------------------------------------
    // Dashboard Card Click
    //--------------------------------------------------
    //
    // Clicking a dashboard card automatically
    // filters the Job Card table.
    //
    //--------------------------------------------------

    const selectDashboardCard = (status) => {

        setSelectedCard(status);

        setStatusFilter(status);

    };

//--------------------------------------------------
// Get Current User Role
//--------------------------------------------------

//--------------------------------------------------
// Current User Role
//--------------------------------------------------

const role = (() => {

    try {

        const token = localStorage.getItem("token");

        if (!token)
            return "";

        const decoded = jwtDecode(token);

        return (
            decoded.role ||
            decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
            ""
        );

    }
    catch {

        return "";

    }

})();

//--------------------------------------------------
// Open Job Card
//--------------------------------------------------

const openJobCard = (jobCardId) => {

    if (role === "Admin") {

        navigate(`/admin/jobcards/${jobCardId}`);

    }
    else {

        navigate(`/technician/jobcards/${jobCardId}`);

    }

};

//--------------------------------------------------
// Edit Job Card
//--------------------------------------------------

const editJobCard = (jobCardId) => {

    if (role === "Admin") {

        navigate(`/admin/jobcards/${jobCardId}`);

    }
    else {

        navigate(`/technician/jobcards/${jobCardId}`);

    }

};
//--------------------------------------------------
// Complete Job Card
//--------------------------------------------------
//
// Marks the selected Job Card as completed,
// then reloads the table.
//
//--------------------------------------------------

//--------------------------------------------------
// Open Complete Modal
//--------------------------------------------------

const completeJobCard = (jobCardId) => {

    setSelectedJobCardId(jobCardId);

    setShowCompleteModal(true);

};

//--------------------------------------------------
// Confirm Completion
//--------------------------------------------------

const confirmCompleteJobCard = async () => {

    try {

        await jobCardService.completeJobCard(selectedJobCardId);

        setShowCompleteModal(false);

        setSelectedJobCardId(null);

        await loadJobCards();

        alert("Job Card completed successfully.");

    }
    catch (error) {

    console.error(error);

    setShowCompleteModal(false);

    setSelectedJobCardId(null);

    alert("Unable to complete Job Card.");

}

};


//--------------------------------------------------
// Print Job Card
//--------------------------------------------------

const printJobCard = (jobCardId) => {

    alert("Print functionality coming soon.");

};;
    //--------------------------------------------------
    // Return JSX
    //--------------------------------------------------
    return (
                <div className="container mt-4">

            {/*==================================================
                Page Header
            ==================================================*/}

            <div className="mb-4">

                <h2 className="fw-bold mb-1">
                    Job Cards
                </h2>

                <p className="text-muted">
                    View and manage all Job Cards assigned to you.
                </p>

            </div>

            {/*==================================================
                Dashboard Statistics
            ==================================================*/}

            <div className="row mb-4">

                {/* Total */}

                <div className="col-lg col-md-4 col-sm-6 mb-3">

                    <div
                        className={`card shadow-sm h-100 border-2 ${selectedCard === "All"
                                ? "border-primary"
                                : ""
                            }`}
                        style={{ cursor: "pointer" }}
                        onClick={() => selectDashboardCard("All")}
                    >

                        <div className="card-body text-center">

                            <h6 className="text-muted">
                                Total Job Cards
                            </h6>

                            <h2 className="fw-bold text-primary">
                                {totalJobs}
                            </h2>

                        </div>

                    </div>

                </div>

                {/* Open */}

                <div className="col-lg col-md-4 col-sm-6 mb-3">

                    <div
                        className={`card shadow-sm h-100 border-2 ${selectedCard === "Open"
                                ? "border-primary"
                                : ""
                            }`}
                        style={{ cursor: "pointer" }}
                        onClick={() => selectDashboardCard("Open")}
                    >

                        <div className="card-body text-center">

                            <h6 className="text-muted">

                                Open

                            </h6>

                            <h2 className="text-primary">

                                {openJobs}

                            </h2>

                        </div>

                    </div>

                </div>

                {/* In Progress */}

                <div className="col-lg col-md-4 col-sm-6 mb-3">

                    <div
                        className={`card shadow-sm h-100 border-2 ${selectedCard === "In Progress"
                                ? "border-warning"
                                : ""
                            }`}
                        style={{ cursor: "pointer" }}
                        onClick={() => selectDashboardCard("In Progress")}
                    >

                        <div className="card-body text-center">

                            <h6 className="text-muted">

                                In Progress

                            </h6>

                            <h2 className="text-warning">

                                {inProgressJobs}

                            </h2>

                        </div>

                    </div>

                </div>

                {/* Completed */}

                <div className="col-lg col-md-4 col-sm-6 mb-3">

                    <div
                        className={`card shadow-sm h-100 border-2 ${selectedCard === "Completed"
                                ? "border-success"
                                : ""
                            }`}
                        style={{ cursor: "pointer" }}
                        onClick={() => selectDashboardCard("Completed")}
                    >

                        <div className="card-body text-center">

                            <h6 className="text-muted">

                                Completed

                            </h6>

                            <h2 className="text-success">

                                {completedJobs}

                            </h2>

                        </div>

                    </div>

                </div>

                {/* Cancelled */}

                <div className="col-lg col-md-4 col-sm-6 mb-3">

                    <div
                        className={`card shadow-sm h-100 border-2 ${selectedCard === "Cancelled"
                                ? "border-danger"
                                : ""
                            }`}
                        style={{ cursor: "pointer" }}
                        onClick={() => selectDashboardCard("Cancelled")}
                    >

                        <div className="card-body text-center">

                            <h6 className="text-muted">

                                Cancelled

                            </h6>

                            <h2 className="text-danger">

                                {cancelledJobs}

                            </h2>

                        </div>

                    </div>

                </div>

            </div>

            {/*==================================================
                Search + Filter
            ==================================================*/}

            <div className="row mb-4">

                <div className="col-md-8 mb-2">

                    <input
                        type="text"
                        className="form-control"
                        placeholder="Search Job Number, Customer, Company or Subject..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                    />

                </div>

                <div className="col-md-4">

                    <select
                        className="form-select"
                        value={statusFilter}
                        onChange={(e) => {

                            setStatusFilter(e.target.value);

                            setSelectedCard(e.target.value);

                        }}
                    >

                        <option value="All">

                            All Statuses

                        </option>

                        <option value="Open">

                            Open

                        </option>

                        <option value="In Progress">

                            In Progress

                        </option>

                        <option value="Completed">

                            Completed

                        </option>

                        <option value="Cancelled">

                            Cancelled

                        </option>

                    </select>

                </div>

            </div>

            {/*==================================================
                Loading Spinner
            ==================================================*/}

            {

                loading ?

                    <div className="text-center py-5">

                        <div className="spinner-border text-primary"></div>

                        <p className="mt-3">

                            Loading Job Cards...

                        </p>

                    </div>

                    :

                    <div className="card shadow-sm">

                        <div className="table-responsive">

                            <table className="table table-hover align-middle mb-0">

                                <thead className="table-dark">

                                    <tr>

                                        <th>Job Number</th>

                                        <th>Customer</th>

                                        <th>Company</th>

                                        <th>Subject</th>

                                        <th>Technician</th>

                                        <th>Status</th>

                                        <th>Created</th>

                                       <th style={{ width: "360px" }}>
                                            Actions
                                        </th> 

                                    </tr>

                                </thead>

                                
                                <tbody>

                                    {filteredJobCards.length === 0 ? (

                                        <tr>

                                            <td
                                                colSpan="8"
                                                className="text-center py-5"
                                            >
                                                No Job Cards Found.
                                            </td>

                                        </tr>

                                    ) : (

                                        filteredJobCards.map(job => (

                                            <tr key={job.jobCardId}>

                                                <td>
                                                    <strong>{job.jobNumber}</strong>
                                                </td>

                                                <td>
                                                    {job.customerName}
                                                </td>

                                                <td>
                                                    {job.companyName}
                                                </td>

                                                <td>
                                                    {job.subject}
                                                </td>

                                                <td>
                                                    {job.assignedTechnicianName}
                                                </td>

                                                <td>

                                                    <span
                                                        className={`badge ${getStatusBadge(job.status)}`}
                                                    >
                                                        {job.status}
                                                    </span>

                                                </td>

                                                <td>
                                                    {new Date(job.dateCreated).toLocaleDateString()}
                                                </td>

                                                <td>

                                                    {/* View */}

                                                    <button
                                                        className="btn btn-outline-primary btn-sm me-2"
                                                        onClick={() => openJobCard(job.jobCardId)}
                                                    >
                                                        View
                                                    </button>

                                                    {/* Admin only */}

                                                    {role === "Admin" && job.status !== "Completed" && (

                                                        <button
                                                            className="btn btn-outline-warning btn-sm me-2"
                                                            onClick={() => editJobCard(job.jobCardId)}
                                                        >
                                                            Edit
                                                        </button>

                                                    )}

                                                    {/* Only when In Progress */}

                                                    {job.status === "In Progress" && (

                                                        <button
                                                            className="btn btn-outline-success btn-sm me-2"
                                                            onClick={() => completeJobCard(job.jobCardId)}
                                                        >
                                                            Complete
                                                        </button>

                                                    )}

                                                    {/* Everyone */}

                                                    <button
                                                        className="btn btn-outline-secondary btn-sm"
                                                        onClick={() => printJobCard(job.jobCardId)}
                                                    >
                                                        Print
                                                    </button>

                                                </td>

                                            </tr>

                                        ))

                                    )}

                                </tbody>
                        

                            </table>

                        </div>

                    </div>

            }
                <div>

                </div>
                {/*Complete Job Card Confirmation */}
                        <ConfirmationModal
                            show={showCompleteModal}
                            title="Complete Job Card"
                            message="Are you sure you want to mark this Job Card as completed? Once completed, technicians will no longer be able to edit it."
                            confirmText="Complete"
                            cancelText="Cancel"
                            confirmVariant="success"
                            onConfirm={confirmCompleteJobCard}
                            onCancel={() => {

                                setShowCompleteModal(false);

                                setSelectedJobCardId(null);

                            }}
                        />

        </div>

    );

}

export default JobCards;