import { useEffect, useState } from "react";

import userService from "../services/userService";

function Users() {

    //--------------------------------------------------
    // State
    //--------------------------------------------------

    const [users, setUsers] = useState([]);

    const [loading, setLoading] = useState(true);

    const [error, setError] = useState("");

    //--------------------------------------------------
    // Load Users
    //--------------------------------------------------

    const loadUsers = async () => {

        try {

            setLoading(true);

            const data = await userService.getUsers();

            setUsers(data);

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
    // Load on Page Open
    //--------------------------------------------------

    useEffect(() => {

        loadUsers();

    }, []);

    //--------------------------------------------------
    // Loading
    //--------------------------------------------------

    if (loading) {

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

                <h2>

                    User Management

                </h2>

                <button
                    className="btn btn-primary"
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

                            {

                                users.length === 0 ?

                                    <tr>

                                        <td>
                                            colSpan="6"
                                            className="text-center"
                                        

                                            <td colSpan="6" className="text-center py-5 text-muted">

                                            No users have been added yet.

                                            </td>

                                        </td>

                                    </tr>

                                    :

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

                                                {

                                                    user.isActive ?

                                                        <span className="badge bg-success">

                                                            Active

                                                        </span>

                                                        :

                                                        <span className="badge bg-danger">

                                                            Inactive

                                                        </span>

                                                }

                                            </td>

                                            <td>

                                                <button
                                                    className="btn btn-warning btn-sm me-2"
                                                >
                                                    Edit
                                                </button>

                                                <button
                                                    className="btn btn-danger btn-sm"
                                                >
                                                    Delete
                                                </button>

                                            </td>

                                            

                                        </tr>

                                    ))

                            }

                        </tbody>

                    </table>

                </div>

            </div>

        </div>

    );

}

export default Users;