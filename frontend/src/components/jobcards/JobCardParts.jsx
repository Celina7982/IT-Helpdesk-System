import { useEffect, useState } from "react";
import jobCardService from "../../services/jobCardService";
import { jwtDecode } from "jwt-decode";

function JobCardParts({ jobCardId, status, onChangeParent }) {
  //--------------------------------------------------
  // State
  //--------------------------------------------------
  const [parts, setParts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [partName, setPartName] = useState("");
  const [quantity, setQuantity] = useState("");
  const [saving, setSaving] = useState(false);

  // Edit modal state
  const [editingPart, setEditingPart] = useState(null);
  const [editName, setEditName] = useState("");
  const [editQuantity, setEditQuantity] = useState("");
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
  const canModifyPart = (part) => {
    if (isAdmin) return true; // Admins can edit/delete regardless of status
    if (isCompleted) return false; // Technicians cannot edit/delete if completed

    const isOwner =
      part.addedByUserId && currentUserId && String(part.addedByUserId) === String(currentUserId);
    return isOwner; // Technicians can only edit/delete their OWN entries
  };

  // Helper check for Adding parts
  const canAddPart = isAdmin || !isCompleted;

  //--------------------------------------------------
  // Derived Totals
  //--------------------------------------------------
  const totalParts = parts.reduce((total, p) => total + Number(p.quantity || 0), 0);

  //--------------------------------------------------
  // Load Parts
  //--------------------------------------------------
  const loadParts = async () => {
    setLoading(true);
    try {
      const data = await jobCardService.getParts(jobCardId);
      const list = Array.isArray(data) ? data : data.items ?? data.parts ?? [];
      setParts(list);
      if (typeof onChangeParent === "function") onChangeParent();
    } catch (err) {
      console.error("Failed to load parts", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!jobCardId) return;
    loadParts();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [jobCardId]);

  //--------------------------------------------------
  // Add Part
  //--------------------------------------------------
  const addPart = async () => {
    if (!canAddPart) {
      alert("Cannot add parts to a completed Job Card.");
      return;
    }
    if (!partName.trim()) {
      alert("Please enter a part name.");
      return;
    }
    if (!quantity || parseInt(quantity, 10) <= 0) {
      alert("Quantity must be greater than zero.");
      return;
    }
    try {
      setSaving(true);
      await jobCardService.addPart(jobCardId, {
        partName: partName.trim(),
        quantity: parseInt(quantity, 10),
      });
      setPartName("");
      setQuantity("");
      await loadParts();
      alert("Part added successfully.");
    } catch (err) {
      console.error(err);
      alert("Unable to add part.");
    } finally {
      setSaving(false);
    }
  };

  //--------------------------------------------------
// Delete Part
//--------------------------------------------------
const deletePart = async (part) => {
  if (!canModifyPart(part)) {
    alert("You do not have permission to delete this part.");
    return;
  }
  if (!window.confirm("Delete this part?")) return;
  try {
    // Pass only part.partId
    await jobCardService.deletePart(part.partId);
    await loadParts();
    alert("Part deleted successfully.");
  } catch (err) {
    console.error(err);
    alert("Unable to delete part.");
  }
};
  //--------------------------------------------------
  // Open Edit Modal
  //--------------------------------------------------
  const openEdit = (part) => {
    if (!canModifyPart(part)) {
      alert("You do not have permission to edit this part.");
      return;
    }
    setEditingPart(part);
    setEditName(part.partName);
    setEditQuantity(String(part.quantity));
  };

  //--------------------------------------------------
  // Save Edit
  //--------------------------------------------------
  const saveEdit = async () => {
    if (!editingPart) return;
    if (!editName.trim()) {
      alert("Please enter a part name.");
      return;
    }
    if (!editQuantity || parseInt(editQuantity, 10) <= 0) {
      alert("Quantity must be greater than zero.");
      return;
    }

    try {
      setEditSaving(true);
      await jobCardService.updatePart(jobCardId, editingPart.partId, {
        partName: editName.trim(),
        quantity: parseInt(editQuantity, 10),
      });
      setEditingPart(null);
      await loadParts();
      alert("Part updated successfully.");
    } catch (err) {
      console.error(err);
      alert("Unable to update part.");
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
        <h5 className="mb-0">Parts Used</h5>
      </div>
      <div className="card-body">
        {loading ? (
          <div className="text-center py-4">
            <div className="spinner-border text-primary" />
          </div>
        ) : (
          <>
            {/* Parts Table */}
            <div className="table-responsive">
              <table className="table table-striped table-bordered align-middle">
                <thead className="table-light">
                  <tr>
                    <th style={{ width: "180px" }}>Date</th>
                    <th style={{ width: "200px" }}>User</th>
                    <th>Description</th>
                    <th style={{ width: "120px" }}>Quantity</th>
                    <th style={{ width: "160px" }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {parts.length === 0 ? (
                    <tr>
                      <td colSpan={5} className="text-center text-muted py-4">
                        No parts have been added.
                      </td>
                    </tr>
                  ) : (
                    parts.map((p) => {
                      const addedDate = p.dateAdded
                        ? new Date(p.dateAdded).toLocaleString()
                        : "N/A";
                      const userName = p.addedByName || p.addedBy || "System";
                      const canModify = canModifyPart(p);

                      return (
                        <tr key={p.partId}>
                          <td>{addedDate}</td>
                          <td>{userName}</td>
                          <td>{p.partName}</td>
                          <td>{p.quantity}</td>
                          <td>
                            <div className="d-flex gap-2">
                              <button
                                className="btn btn-sm btn-outline-primary"
                                onClick={() => openEdit(p)}
                                disabled={!canModify}
                                title={canModify ? "Edit part" : "You cannot edit this entry"}
                              >
                                Edit
                              </button>
                              <button
                                className="btn btn-sm btn-outline-danger"
                                onClick={() => deletePart(p)}
                                disabled={!canModify}
                                title={canModify ? "Delete part" : "You cannot delete this entry"}
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

            {/* Total Quantity */}
            <div className="alert alert-info mt-2">
              <strong>Total Quantity Used: </strong> {totalParts}
            </div>

            {/* Add Part Section */}
            {canAddPart ? (
              <>
                <hr />
                <h6 className="mb-3">Add Part</h6>
                <div className="row g-2">
                  <div className="col-md-7">
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Part Name"
                      value={partName}
                      onChange={(e) => setPartName(e.target.value)}
                      disabled={saving}
                    />
                  </div>
                  <div className="col-md-2">
                    <input
                      type="number"
                      min="1"
                      className="form-control"
                      placeholder="Qty"
                      value={quantity}
                      onChange={(e) => setQuantity(e.target.value)}
                      disabled={saving}
                    />
                  </div>
                  <div className="col-md-3 d-grid">
                    <button
                      className="btn btn-success"
                      onClick={addPart}
                      disabled={saving}
                    >
                      {saving ? "Saving..." : "Add Part"}
                    </button>
                  </div>
                </div>
              </>
            ) : (
              <div className="alert alert-warning mt-3 mb-0">
                This Job Card is completed. Technicians cannot add new parts.
              </div>
            )}
          </>
        )}
      </div>

      {/* Edit Modal */}
      {editingPart && (
        <div className="modal show d-block" tabIndex="-1" role="dialog" aria-modal="true">
          <div className="modal-dialog" role="document">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Edit Part Entry</h5>
                <button
                  type="button"
                  className="btn-close"
                  aria-label="Close"
                  onClick={() => setEditingPart(null)}
                  disabled={editSaving}
                />
              </div>
              <div className="modal-body">
                <div className="mb-3">
                  <label className="form-label">Part Name / Description</label>
                  <input
                    type="text"
                    className="form-control"
                    value={editName}
                    onChange={(e) => setEditName(e.target.value)}
                    disabled={editSaving}
                  />
                </div>
                <div className="mb-3">
                  <label className="form-label">Quantity</label>
                  <input
                    type="number"
                    min="1"
                    className="form-control"
                    value={editQuantity}
                    onChange={(e) => setEditQuantity(e.target.value)}
                    disabled={editSaving}
                  />
                </div>
              </div>
              <div className="modal-footer">
                <button
                  type="button"
                  className="btn btn-secondary"
                  onClick={() => setEditingPart(null)}
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

export default JobCardParts;