
import { useEffect, useState } from "react";

import userService from "../services/userService";
import UserFormModal from "../components/users/UserFormModal";
import ResetPasswordModal from "../components/Users/ResetPasswordModal";

function Users() {

    //--------------------------------------------------
    // State variables
    //--------------------------------------------------

    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [showResetModal, setShowResetModal] = useState(false);
    const [selectedUser, setSelectedUser] = useState(null);

    //--------------------------------------------------
    // Pagination & Search State
    //--------------------------------------------------

    const [currentPage, setCurrentPage] = useState(1);
    const [pageSize] = useState(5);
    const [totalCount, setTotalCount] = useState(0);

    const [searchTerm, setSearchTerm] = useState("");
    const [debouncedSearchTerm, setDebouncedSearchTerm] = useState("");

    //--------------------------------------------------
    // Modal State
    //--------------------------------------------------

    const [showModal, setShowModal] = useState(false);
    const [editingUser, setEditingUser] = useState(null);

    //--------------------------------------------------
    // Debounce Search Input
    //--------------------------------------------------

    useEffect(() => {

        const handler = setTimeout(() => {

            setDebouncedSearchTerm(searchTerm);
            setCurrentPage(1);

        }, 400);

        return () => {
            clearTimeout(handler);
        };

    }, [searchTerm]);

    //--------------------------------------------------
    // Load Users
    // Server-Side Pagination & Search
    //--------------------------------------------------

    const loadUsers = async () => {

        try {

            setLoading(true);
            setError("");

            const data = await userService.getUsers(
                currentPage,
                pageSize,
                debouncedSearchTerm
            );

            setUsers(data.items || []);
            setTotalCount(data.totalCount || 0);

        }
        catch (err) {

            console.error(err);

            setError("Unable to load users.");

        }
        finally {

            setLoading(false);

        }

    };

    //--------------------------------------------------
    // Load Users When Page or Search Changes
    //--------------------------------------------------

    useEffect(() => {

        loadUsers();

    }, [currentPage, debouncedSearchTerm]);

    //--------------------------------------------------
    // Total Pages
    //--------------------------------------------------

    const totalPages = Math.ceil(totalCount / pageSize);

    //--------------------------------------------------
    // Save User
    //--------------------------------------------------

    const handleSaveUser = async (user) => {

        try {

            if (editingUser) {

                // Editing existing user
                // Stay on current page

                await userService.updateUser(
                    editingUser.userId,
                    user
                );

                setShowModal(false);
                setEditingUser(null);

                await loadUsers();

            }
            else {

                // Creating new user
                // Return to page 1

                await userService.createUser(user);

                setShowModal(false);
                setEditingUser(null);
                setCurrentPage(1);

            }

        }
        catch (err) {

            console.error(err);

            alert(
                JSON.stringify(
                    err.response?.data,
                    null,
                    2
                )
            );

        }

    };

    //--------------------------------------------------
    // Delete User
    // Smart Pagination Boundary Check
    //--------------------------------------------------

    const handleDeleteUser = async (id) => {

        const confirmDelete = window.confirm(
            "Are you sure you want to delete this user?"
        );

        if (!confirmDelete) {
            return;
        }

        try {

            await userService.deleteUser(id);

            //--------------------------------------------------
            // If this was the last item on the current page
            // and we're past page 1, move back one page.
            //--------------------------------------------------

            if (users.length === 1 && currentPage > 1) {

                setCurrentPage(currentPage - 1);

            }
            else {

                await loadUsers();

            }

        }
        catch (err) {

            console.error(err);

            alert(
                err.response?.data?.message ||
                "Unable to delete user."
            );

        }

    };

    //--------------------------------------------------
    // Activate / Deactivate User
    // Maintains Current Page
    //--------------------------------------------------

    const handleToggleStatus = async (user) => {

        try {

            await userService.updateUser(
                user.userId,
                {
                    ...user,
                    isActive: !user.isActive
                }
            );

            await loadUsers();

        }
        catch (err) {

            console.error(err);

            alert("Unable to update user status.");

        }

    };

    //--------------------------------------------------
    // Reset Password
    //--------------------------------------------------

    const handleResetPassword = async (newPassword) => {

        try {

            await userService.resetPassword(
                selectedUser.userId,
                newPassword
            );

            alert("Password reset successfully.");

            setShowResetModal(false);
            setSelectedUser(null);

        }
        catch (err) {

            console.error(err);

            alert(
                err.response?.data?.message ||
                "Unable to reset password."
            );

        }

    };

    //--------------------------------------------------
    // Loading
    //--------------------------------------------------

    if (loading && users.length === 0) {

        return (
            <div className="text-center mt-5">

                <div className="spinner-border text-primary"></div>

                <p className="mt-3">
                    Loading users...
                </p>

            </div>
        );

    }

    //--------------------------------------------------
    // Error
    //--------------------------------------------------

    if (error) {

        return (
            <div className="alert alert-danger">
                {error}
            </div>
        );

    }

    //--------------------------------------------------
    // Page
    //--------------------------------------------------

    return (

        <div className="container mt-4">

            <div className="d-flex justify-content-between align-items-center mb-4">

                <div>

                    <h2>User Management</h2>

                    <input
                        type="text"
                        className="form-control mt-2"
                        placeholder="Search users..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        style={{ width: "300px" }}
                    />

                </div>

                <button
                    className="btn btn-primary"
                    onClick={() => {

                        setEditingUser(null);
                        setShowModal(true);

                    }}
                >
                    + Add User
                </button>

            </div>

            <div className="card shadow">

                <div className="card-body">

                    <table className="table table-hover">

                        <thead className="table-dark">

                            <tr>

                                <th>ID</th>
                                <th>Name</th>
                                <th>Email</th>
                                <th>Role</th>
                                <th>Status</th>

                                <th style={{ width: "170px" }}>
                                    Actions
                                </th>

                            </tr>

                        </thead>

                        <tbody>

                            {users.length === 0 ? (

                                <tr>

                                    <td
                                        colSpan="6"
                                        className="text-center py-5 text-muted"
                                    >
                                        No users found.
                                    </td>

                                </tr>

                            ) : (

                                users.map(user => (

                                    <tr key={user.userId}>

                                        <td>
                                            {user.userId}
                                        </td>

                                        <td>
                                            {user.firstName} {user.lastName}
                                        </td>

                                        <td>
                                            {user.email}
                                        </td>

                                        <td>

                                            <span className="badge bg-primary">
                                                {user.role}
                                            </span>

                                        </td>

                                        <td>

                                            {user.isActive ? (

                                                <span className="badge bg-success">
                                                    Active
                                                </span>

                                            ) : (

                                                <span className="badge bg-danger">
                                                    Inactive
                                                </span>

                                            )}

                                        </td>

                                        <td>

                                            <button
                                                className="btn btn-warning btn-sm me-2"
                                                onClick={() => {

                                                    setEditingUser(user);
                                                    setShowModal(true);

                                                }}
                                            >
                                                Edit
                                            </button>

                                            <button
                                                className={
                                                    user.isActive
                                                        ? "btn btn-secondary btn-sm me-2"
                                                        : "btn btn-success btn-sm me-2"
                                                }
                                                onClick={() =>
                                                    handleToggleStatus(user)
                                                }
                                            >
                                                {user.isActive
                                                    ? "Deactivate"
                                                    : "Activate"}
                                            </button>

                                            <button
                                                className="btn btn-info btn-sm me-2"
                                                onClick={() => {

                                                    setSelectedUser(user);
                                                    setShowResetModal(true);

                                                }}
                                            >
                                                Reset Password
                                            </button>

                                            <button
                                                className="btn btn-danger btn-sm"
                                                onClick={() =>
                                                    handleDeleteUser(
                                                        user.userId
                                                    )
                                                }
                                            >
                                                Delete
                                            </button>

                                        </td>

                                    </tr>

                                ))

                            )}

                        </tbody>

                    </table>

                </div>

                <div className="d-flex justify-content-between align-items-center mt-3 p-3">

                    <button
                        className="btn btn-outline-primary"
                        disabled={currentPage === 1}
                        onClick={() =>
                            setCurrentPage(currentPage - 1)
                        }
                    >
                        Previous
                    </button>

                    <span>
                        Page {currentPage} of {totalPages || 1}
                    </span>

                    <button
                        className="btn btn-outline-primary"
                        disabled={
                            currentPage >= totalPages ||
                            totalPages === 0
                        }
                        onClick={() =>
                            setCurrentPage(currentPage + 1)
                        }
                    >
                        Next
                    </button>

                </div>

            </div>

            <UserFormModal
                show={showModal}
                editingUser={editingUser}
                onClose={() => {

                    setShowModal(false);
                    setEditingUser(null);

                }}
                onSave={handleSaveUser}
            />

            <ResetPasswordModal
                show={showResetModal}
                onClose={() => {

                    setShowResetModal(false);
                    setSelectedUser(null);

                }}
                onSave={handleResetPassword}
            />

        </div>

    );

}

export default Users;
