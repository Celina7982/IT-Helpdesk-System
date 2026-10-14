
import { useState } from "react";
import jobCardService from "../../services/jobCardService";

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
    
    ticketId: null,
    assignedTechnicianId: null
  });

  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState("");

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
  // Submit Paper Job Card
  //----------------------------------------------------

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (isSubmitting) return;

    setIsSubmitting(true);
    setError("");

    try {
      const payload = {
        ...formData,
        ticketId: null,
        assignedTechnicianId: null
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
        assignedTechnicianId: null
      });
    } catch (err) {
      console.error("Paper Job Card creation failed:", err);

      setError(
        err.response?.data?.message ||
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
      <div className="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
        <div className="modal-content">

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
            <div className="modal-body">

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

            <div className="modal-footer">
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
