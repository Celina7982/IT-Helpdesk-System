import { useEffect, useState } from "react";
import jobCardService from "../../services/jobCardService";
import { jwtDecode } from "jwt-decode";

function LabourEntries({ jobCardId, status, onChangeParent }) {
  //--------------------------------------------------
  // State
  //--------------------------------------------------
  const [labourEntries, setLabourEntries] = useState([]);
  const [loading, setLoading] = useState(true);
  const [hoursWorked, setHoursWorked] = useState("");
  const [workPerformed, setWorkPerformed] = useState("");
  const [saving, setSaving] = useState(false);

  // Edit modal state
  const [editingEntry, setEditingEntry] = useState(null);
  const [editHours, setEditHours] = useState("");
  const [editWork, setEditWork] = useState("");
  const [editSaving, setEditSaving] = useState(false);

  //--------------------------------------------------
  // Auth / Permissions Extraction
  //--------------------------------------------------
  const token = localStorage.getItem("token");
  let currentUserId = null;
  let role = "";

  try {
    if (token) {
      const decoded = jwtDecode(token);
      currentUserId =
        decoded.sub ||
        decoded.nameid ||
        decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ||
        null;

      role =
        decoded.role ||
        decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
        "";

      if (currentUserId && typeof currentUserId === "string" && /^\d+$/.test(currentUserId)) {
        currentUserId = parseInt(currentUserId, 10);
      }
    }
  } catch {
    currentUserId = null;
    role = "";
  }

  const isAdmin = role === "Admin";
  const isCompleted = status === "Completed";

  // Helper check for Edit / Delete actions
  const canModifyEntry = (entry) => {
    if (isAdmin) return true; // Admins can edit/delete regardless of status
    if (isCompleted) return false; // Technicians cannot edit/delete if completed

    // DTO maps primary technician identifier directly to technicianId
    const isOwner =
      entry?.technicianId && currentUserId && String(entry.technicianId) === String(currentUserId);
    return isOwner; // Technicians can only edit/delete their OWN entries
  };

  // Helper check for Adding entries
  const canAddEntry = isAdmin || !isCompleted;

  // Helper extractor for Primary Key
  const getEntryId = (entry) => entry?.labourId || entry?.id;

  //--------------------------------------------------
  // Derived Totals
  //--------------------------------------------------
  const totalHours = labourEntries.reduce(
    (total, entry) => total + Number(entry.hoursWorked || 0),
    0
  );

  //--------------------------------------------------
  // Load Labour Entries
  //--------------------------------------------------
  const loadLabourEntries = async () => {
    setLoading(true);
    try {
      const data = await jobCardService.getLabourEntries(jobCardId);
      const list = Array.isArray(data) ? data : data.items ?? data.labourEntries ?? [];
      setLabourEntries(list);
      if (typeof onChangeParent === "function") onChangeParent();
    } catch (err) {
      console.error("Failed to load labour entries", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!jobCardId) return;
    loadLabourEntries();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [jobCardId]);

  //--------------------------------------------------
  // Add Entry
  //--------------------------------------------------
  const addEntry = async () => {
    if (!canAddEntry) {
      alert("Cannot add labour entries to a completed Job Card.");
      return;
    }
    if (!hoursWorked || parseFloat(hoursWorked) <= 0) {
      alert("Please enter valid hours worked.");
      return;
    }
    if (!workPerformed.trim()) {
      alert("Please enter a description of the work performed.");
      return;
    }

    try {
      setSaving(true);
      await jobCardService.addLabourEntry(jobCardId, {
        hoursWorked: parseFloat(hoursWorked),
        workPerformed: workPerformed.trim(),
        dateWorked: new Date().toISOString()
      });
      setHoursWorked("");
      setWorkPerformed("");
      await loadLabourEntries();
      alert("Labour entry added successfully.");
    } catch (err) {
      console.error(err);
      alert("Unable to add labour entry.");
    } finally {
      setSaving(false);
    }
  };

  //--------------------------------------------------
  // Delete Entry
  //--------------------------------------------------
  const deleteEntry = async (entry) => {
    if (!canModifyEntry(entry)) {
      alert("You do not have permission to delete this entry.");
      return;
    }

    const entryId = getEntryId(entry);
    if (!entryId) {
      console.error("Labour entry ID is undefined:", entry);
      alert("Unable to identify entry ID for deletion.");
      return;
    }

    if (!window.confirm("Delete this labour entry?")) return;

    try {
      await jobCardService.deleteLabourEntry(entryId);
      await loadLabourEntries();
      alert("Labour entry deleted successfully.");
    } catch (err) {
      console.error(err);
      alert("Unable to delete entry.");
    }
  };

  //--------------------------------------------------
  // Open Edit Modal
  //--------------------------------------------------
  const openEdit = (entry) => {
    if (!canModifyEntry(entry)) {
      alert("You do not have permission to edit this entry.");
      return;
    }
    setEditingEntry(entry);
    setEditHours(String(entry.hoursWorked));
    setEditWork(entry.workPerformed || "");
  };

  //--------------------------------------------------
  // Save Edit
  //--------------------------------------------------
  const saveEdit = async () => {
    if (!editingEntry) return;
    if (!editHours || parseFloat(editHours) <= 0) {
      alert("Please enter valid hours worked.");
      return;
    }
    if (!editWork.trim()) {
      alert("Please enter a description of the work performed.");
      return;
    }

    const entryId = getEntryId(editingEntry);
    if (!entryId) {
      console.error("Labour entry ID is undefined:", editingEntry);
      alert("Unable to identify entry ID for updating.");
      return;
    }

    try {
      setEditSaving(true);
      await jobCardService.updateLabourEntry(jobCardId, entryId, {
        hoursWorked: parseFloat(editHours),
        workPerformed: editWork.trim()
      });
      setEditingEntry(null);
      await loadLabourEntries();
      alert("Labour entry updated successfully.");
    } catch (err) {
      console.error(err);
      alert("Unable to update entry.");
    } finally {
      setEditSaving(false);
    }
  };

  //--------------------------------------------------
  // Render JSX
  //--------------------------------------------------
  return (
    <div className="card shadow mt-4">
      <div className="card-header bg-secondary text-white">
        <h5 className="mb-0">Technician Labour Entries</h5>
      </div>
      <div className="card-body">
        {loading ? (
          <div className="text-center py-4">
            <div className="spinner-border text-primary" />
          </div>
        ) : (
          <>
            {/* Table Matching Parts Layout */}
            <div className="table-responsive">
              <table className="table table-striped table-bordered align-middle">
                <thead className="table-light">
                  <tr>
                    <th style={{ width: "180px" }}>Date</th>
                    <th style={{ width: "200px" }}>User</th>
                    <th>Work Performed</th>
                    <th style={{ width: "120px" }}>Hours</th>
                    <th style={{ width: "160px" }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {labourEntries.length === 0 ? (
                    <tr>
                      <td colSpan={5} className="text-center text-muted py-4">
                        No labour entries have been added.
                      </td>
                    </tr>
                  ) : (
                    labourEntries.map((entry, index) => {
                      const entryDate = entry.dateWorked
                        ? new Date(entry.dateWorked).toLocaleString()
                        : "N/A";
                      const userName = entry.technicianName || "Technician";
                      const canModify = canModifyEntry(entry);
                      const keyId = getEntryId(entry) || index;

                      return (
                        <tr key={keyId}>
                          <td>{entryDate}</td>
                          <td>{userName}</td>
                          <td>{entry.workPerformed}</td>
                          <td>{entry.hoursWorked}</td>
                          <td>
                            <div className="d-flex gap-2">
                              <button
                                className="btn btn-sm btn-outline-primary"
                                onClick={() => openEdit(entry)}
                                disabled={!canModify}
                                title={
                                  canModify ? "Edit entry" : "You cannot edit this entry"
                                }
                              >
                                Edit
                              </button>
                              <button
                                className="btn btn-sm btn-outline-danger"
                                onClick={() => deleteEntry(entry)}
                                disabled={!canModify}
                                title={
                                  canModify ? "Delete entry" : "You cannot delete this entry"
                                }
                              >
                                Delete
                              </button>
                            </div>
                          </td>
                        </tr>
                      );
                    })
                  )}
                </tbody>
              </table>
            </div>

            {/* Total Hours Banner */}
            <div className="alert alert-info mt-2">
              <strong>Total Labour Hours: </strong> {totalHours.toFixed(2)} hrs
            </div>

            {/* Add Entry Form */}
            {canAddEntry ? (
              <>
                <hr />
                <h6 className="mb-3">Add New Labour Entry</h6>
                <div className="row g-2">
                  <div className="col-md-2">
                    <input
                      type="number"
                      className="form-control"
                      min="0.25"
                      step="0.25"
                      placeholder="Hours"
                      value={hoursWorked}
                      onChange={(e) => setHoursWorked(e.target.value)}
                      disabled={saving}
                    />
                  </div>
                  <div className="col-md-7">
                    <input
                      className="form-control"
                      placeholder="Describe the work completed..."
                      value={workPerformed}
                      onChange={(e) => setWorkPerformed(e.target.value)}
                      disabled={saving}
                    />
                  </div>
                  <div className="col-md-3 d-grid">
                    <button
                      className="btn btn-success"
                      onClick={addEntry}
                      disabled={saving}
                    >
                      {saving ? "Saving..." : "Add Entry"}
                    </button>
                  </div>
                </div>
              </>
            ) : (
              <div className="alert alert-warning mt-3 mb-0">
                <strong>Job Card Completed.</strong> Labour entries can no longer be added.
              </div>
            )}
          </>
        )}
      </div>

      {/* Edit Modal */}
      {editingEntry && (
        <div className="modal show d-block" tabIndex="-1" role="dialog" aria-modal="true">
          <div className="modal-dialog" role="document">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Edit Labour Entry</h5>
                <button
                  type="button"
                  className="btn-close"
                  aria-label="Close"
                  onClick={() => setEditingEntry(null)}
                  disabled={editSaving}
                />
              </div>
              <div className="modal-body">
                <div className="mb-3">
                  <label className="form-label">Hours Worked</label>
                  <input
                    type="number"
                    step="0.25"
                    min="0.25"
                    className="form-control"
                    value={editHours}
                    onChange={(e) => setEditHours(e.target.value)}
                    disabled={editSaving}
                  />
                </div>
                <div className="mb-3">
                  <label className="form-label">Work Performed</label>
                  <textarea
                    rows={3}
                    className="form-control"
                    value={editWork}
                    onChange={(e) => setEditWork(e.target.value)}
                    disabled={editSaving}
                  />
                </div>
              </div>
              <div className="modal-footer">
                <button
                  type="button"
                  className="btn btn-secondary"
                  onClick={() => setEditingEntry(null)}
                  disabled={editSaving}
                >
                  Cancel
                </button>
                <button
                  type="button"
                  className="btn btn-primary"
                  onClick={saveEdit}
                  disabled={editSaving}
                >
                  {editSaving ? "Saving..." : "Save changes"}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default LabourEntries;