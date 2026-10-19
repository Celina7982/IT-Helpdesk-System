
import { useEffect, useState } from "react";
import jobCardService from "../../services/jobCardService";
import userService from "../../services/userService";

function CreatePaperJobCardModal({
  show,
  onClose,
  onJobCardCreated
}) {
  
const [formData, setFormData] = useState({
    paperJobCardNumber: "",
    paperJobCardDate: "",
    customerName: "",
    faultReported: "",
    faultFound: "",
    workPerformed: "",
    completionNotes: "",

    ticketId: null,

    // Lead Technician
    assignedTechnicianId: "",

    // All assigned technicians, including the lead
    assignedTechnicianIds: []
});


  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState("");

const [assignableUsers, setAssignableUsers] = useState([]);
const [loadingUsers, setLoadingUsers] = useState(false);


useEffect(() => {
    if (!show) return;

    const loadAssignableUsers = async () => {
        try {
            setLoadingUsers(true);
            setError("");

            const users = await userService.getAssignableUsers();

            setAssignableUsers(users);
        } catch (err) {
            console.error("Failed to load assignable users:", err);
            setError("Unable to load technicians.");
        } finally {
            setLoadingUsers(false);
        }
    };

    loadAssignableUsers();
}, [show]);

  //----------------------------------------------------
  // Handle Input Changes
  //----------------------------------------------------

  const handleChange = (e) => {
    const { name, value } = e.target;

    setFormData((prev) => ({
      ...prev,
      [name]: value
    }));
  };

  
  //----------------------------------------------------
  // Handle Multiple Technician Selection
  //----------------------------------------------------

  const handleTechnicianSelection = (technicianId) => {
    const id = Number(technicianId);

    setFormData((prev) => {
      const alreadySelected =
        prev.assignedTechnicianIds.includes(id);

      return {
        ...prev,
        assignedTechnicianIds: alreadySelected
          ? prev.assignedTechnicianIds.filter(
              (selectedId) => selectedId !== id
            )
          : [...prev.assignedTechnicianIds, id]
      };
    });
  };


  //----------------------------------------------------
  // Submit Paper Job Card
  //----------------------------------------------------

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (isSubmitting) return;

    setIsSubmitting(true);
    setError("");

    try {

const leadTechnicianId = Number(
    formData.assignedTechnicianId
);

if (!leadTechnicianId) {
    throw new Error("Please select a Lead Technician.");
}

const assignedTechnicianIds = [
    ...new Set([
        leadTechnicianId,
        ...formData.assignedTechnicianIds.map(Number)
    ])
];

const payload = {
    ...formData,

    ticketId: null,

    // Lead Technician
    assignedTechnicianId: leadTechnicianId,

    // All assigned technicians, including the Lead
    assignedTechnicianIds
};


      const result =
        await jobCardService.createPaperJobCard(payload);

      alert(
        `Paper Job Card ${result.jobNumber} created successfully.`
      );

      onJobCardCreated?.(result);
      onClose();

     
setFormData({
    paperJobCardNumber: "",
    paperJobCardDate: "",
    customerName: "",
    faultReported: "",
    faultFound: "",
    workPerformed: "",
    completionNotes: "",
    ticketId: null,
    assignedTechnicianId: "",
    assignedTechnicianIds: []
});

    } catch (err) {
      console.error("Paper Job Card creation failed:", err);

     setError(
    err.response?.data?.message ||
    err.message ||
    "Unable to create paper job card."
);
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!show) return null;

  return (
    <div
      className="modal d-block"
      tabIndex="-1"
      style={{ backgroundColor: "rgba(0,0,0,0.5)" }}
      role="dialog"
      aria-modal="true"
      aria-labelledby="createPaperJobCardTitle"
    >
      
<div
    className="modal-dialog modal-lg modal-dialog-centered"
    style={{
        maxHeight: "95vh",
        marginTop: "2.5vh",
        marginBottom: "2.5vh"
    }}
>
    <div
        className="modal-content"
        style={{
            maxHeight: "95vh",
            display: "flex",
            flexDirection: "column",
            overflow: "hidden"
        }}
    >


          <div className="modal-header bg-dark text-white">
            <h5
              className="modal-title"
              id="createPaperJobCardTitle"
            >
              Register Paper Job Card
            </h5>

            <button
              type="button"
              className="btn-close btn-close-white"
              onClick={onClose}
              disabled={isSubmitting}
              aria-label="Close"
            />
          </div>

          <form onSubmit={handleSubmit}>
            
<div
    className="modal-body"
    style={{
        overflowY: "auto",
        minHeight: 0,
        maxHeight: "calc(95vh - 145px)"
    }}
>


              {error && (
                <div className="alert alert-danger">
                  {error}
                </div>
              )}

              <div className="row g-3">

                <div className="col-md-6">
                  <label className="form-label">
                    Paper Job Card Number *
                  </label>
                  <input
                    type="text"
                    className="form-control"
                    name="paperJobCardNumber"
                    value={formData.paperJobCardNumber}
                    onChange={handleChange}
                    required
                    maxLength={50}
                    placeholder="e.g. PJC-001"
                  />
                </div>

                <div className="col-md-6">
                  <label className="form-label">
                    Paper Job Card Date *
                  </label>
                  <input
                    type="date"
                    className="form-control"
                    name="paperJobCardDate"
                    value={formData.paperJobCardDate}
                    onChange={handleChange}
                    required
                  />
                </div>

                <div className="col-12">
                  <label className="form-label">
                    Customer Name *
                  </label>
                  <input
                    type="text"
                    className="form-control"
                    name="customerName"
                    value={formData.customerName}
                    onChange={handleChange}
                    required
                    maxLength={200}
                  />
                </div>

                
{/*----------------------------------------------
    Lead Technician
----------------------------------------------*/}

<div className="col-12">
    <label className="form-label">
        Lead Technician *
    </label>

    <select
        className="form-select"
        name="assignedTechnicianId"
        value={formData.assignedTechnicianId}
        onChange={handleChange}
        required
        disabled={loadingUsers}
    >
        <option value="">
            {loadingUsers
                ? "Loading technicians..."
                : "Select Lead Technician"}
        </option>

        {assignableUsers.map((user) => (
            <option
                key={user.userId}
                value={user.userId}
            >
                {user.fullName}
            </option>
        ))}
    </select>

    <div className="form-text">
        The Lead Technician can manage work completed
        and parts for all technicians assigned to this
        Paper Job Card.
    </div>
</div>


{/*----------------------------------------------
    Additional Assigned Technicians
----------------------------------------------*/}

<div className="col-12">
    <label className="form-label">
        Additional Assigned Technicians
    </label>

    <div className="border rounded p-3">
        {loadingUsers ? (
            <div className="text-muted">
                Loading technicians...
            </div>
        ) : assignableUsers.length === 0 ? (
            <div className="text-muted">
                No technicians available.
            </div>
        ) : (
            assignableUsers
                .filter(
                    (user) =>
                        Number(user.userId) !==
                        Number(formData.assignedTechnicianId)
                )
                .map((user) => (
                    <div
                        className="form-check mb-2"
                        key={user.userId}
                    >
                        <input
                            className="form-check-input"
                            type="checkbox"
                            id={`paper-technician-${user.userId}`}
                            checked={formData.assignedTechnicianIds.includes(
                                Number(user.userId)
                            )}
                            onChange={() =>
                                handleTechnicianSelection(user.userId)
                            }
                        />

                        <label
                            className="form-check-label"
                            htmlFor={`paper-technician-${user.userId}`}
                        >
                            {user.fullName}
                        </label>
                    </div>
                ))
        )}
    </div>

    <div className="form-text">
        Select any additional technicians assigned to
        this Paper Job Card. The Lead Technician is
        included automatically.
    </div>
</div>



                <div className="col-12">
                  <label className="form-label">
                    Fault Reported *
                  </label>
                  <textarea
                    className="form-control"
                    name="faultReported"
                    rows={3}
                    value={formData.faultReported}
                    onChange={handleChange}
                    required
                    maxLength={2000}
                  />
                </div>

                <div className="col-12">
                  <label className="form-label">
                    Fault Found
                  </label>
                  <textarea
                    className="form-control"
                    name="faultFound"
                    rows={2}
                    value={formData.faultFound}
                    onChange={handleChange}
                    maxLength={2000}
                  />
                </div>

                <div className="col-12">
                  <label className="form-label">
                    Work Performed
                  </label>
                  <textarea
                    className="form-control"
                    name="workPerformed"
                    rows={3}
                    value={formData.workPerformed}
                    onChange={handleChange}
                    maxLength={4000}
                  />
                </div>

                
              </div>
            </div>

           
<div
    className="modal-footer"
    style={{
        flexShrink: 0,
        backgroundColor: "#fff"
    }}
>

              <button
                type="button"
                className="btn btn-secondary"
                onClick={onClose}
                disabled={isSubmitting}
              >
                Cancel
              </button>

              <button
                type="submit"
                className="btn btn-primary"
                disabled={isSubmitting}
              >
                {isSubmitting
                  ? "Registering..."
                  : "Register Paper Job Card"}
              </button>
            </div>
          </form>

        </div>
      </div>
    </div>
  );
}

export default CreatePaperJobCardModal;
