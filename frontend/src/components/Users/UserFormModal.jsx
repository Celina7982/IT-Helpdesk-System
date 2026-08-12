import { useEffect, useState } from "react";

function UserFormModal({
    show,
    onClose,
    onSave,
    editingUser
}) {

    //---------------------------------------
    // Form State
    //---------------------------------------

    const [formData, setFormData] = useState({
        firstName: "",
        lastName: "",
        email: "",
        password: "",
        role: "Client",
        isActive: true
    });

    //---------------------------------------
    // Load Existing User
    //---------------------------------------

    useEffect(() => {

        if (editingUser) {

            setFormData({
                firstName: editingUser.firstName,
                lastName: editingUser.lastName,
                email: editingUser.email,
                password: "",
                role: editingUser.role,
                isActive: editingUser.isActive
            });

        }
        else {

            setFormData({
                firstName: "",
                lastName: "",
                email: "",
                password: "",
                role: "Client",
                isActive: true
            });

        }

    }, [editingUser]);

    //---------------------------------------
    // Input Change
    //---------------------------------------

    const handleChange = (e) => {

        const { name, value, type, checked } = e.target;

        setFormData(prev => ({
            ...prev,
            [name]: type === "checkbox"
                ? checked
                : value
        }));

    };

    //---------------------------------------
    // Submit
    //---------------------------------------

    const handleSubmit = (e) => {

    e.preventDefault();

    onSave({

        ...formData,

        userId: editingUser?.userId

    });

};

    //---------------------------------------
    // Hide Modal
    //---------------------------------------

    if (!show) return null;

    //---------------------------------------
    // UI
    //---------------------------------------

    return (

        <div
            className="modal d-block"
            style={{ backgroundColor: "rgba(0,0,0,.5)" }}
        >

            <div className="modal-dialog modal-lg">

                <div className="modal-content">

                    <form onSubmit={handleSubmit}>

                        <div className="modal-header">

                            <h5>

                                {
                                    editingUser
                                        ? "Edit User"
                                        : "Add User"
                                }

                            </h5>

                            <button
                                type="button"
                                className="btn-close"
                                onClick={onClose}
                            />

                        </div>

                        <div className="modal-body">

                            <div className="row">

                                <div className="col-md-6 mb-3">

                                    <label>First Name</label>

                                    <input
                                        className="form-control"
                                        name="firstName"
                                        value={formData.firstName}
                                        onChange={handleChange}
                                        required
                                    />

                                </div>

                                <div className="col-md-6 mb-3">

                                    <label>Last Name</label>

                                    <input
                                        className="form-control"
                                        name="lastName"
                                        value={formData.lastName}
                                        onChange={handleChange}
                                        required
                                    />

                                </div>

                            </div>

                            <div className="mb-3">

                                <label>Email</label>

                                <input
                                    type="email"
                                    className="form-control"
                                    name="email"
                                    value={formData.email}
                                    onChange={handleChange}
                                    required
                                />

                            </div>

                            {

                                !editingUser &&

                                <div className="mb-3">

                                    <label>Password</label>

                                    <input
                                        type="password"
                                        className="form-control"
                                        name="password"
                                        value={formData.password}
                                        onChange={handleChange}
                                        required
                                    />

                                </div>

                            }

                            <div className="row">

                                <div className="col-md-6">

                                    <label>Role</label>

                                    <select
                                        className="form-select"
                                        name="role"
                                        value={formData.role}
                                        onChange={handleChange}
                                    >

                                        <option>Admin</option>
                                        <option>Technician</option>
                                        <option>Client</option>

                                    </select>

                                </div>

                                <div className="col-md-6">

                                    <label>Status</label>

                                    <div className="form-check mt-2">

                                        <input
                                            type="checkbox"
                                            className="form-check-input"
                                            name="isActive"
                                            checked={formData.isActive}
                                            onChange={handleChange}
                                        />

                                        <label className="form-check-label">

                                            Active

                                        </label>

                                    </div>

                                </div>

                            </div>

                        </div>

                        <div className="modal-footer">

                            <button
                                type="button"
                                className="btn btn-secondary"
                                onClick={onClose}
                            >

                                Cancel

                            </button>

                            <button
                                className="btn btn-primary"
                                type="submit"
                            >

                                Save User

                            </button>

                        </div>

                    </form>

                </div>

            </div>

        </div>

    );

}

export default UserFormModal;