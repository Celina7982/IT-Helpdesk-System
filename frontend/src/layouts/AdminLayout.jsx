import { Outlet, Link } from "react-router-dom";


function AdminLayout() {
                   const handleLogout = () => {
    localStorage.removeItem("token");
    window.location.href = "/";
};
    return (
        <div className="row">


            {/* Sidebar */}
            <div className="col-md-2 bg-dark text-white vh-100">


                <h3 className="mt-3 text-center">
                    IT Helpdesk
                </h3>


                <hr />


                <ul className="nav flex-column">


                    {/* Dashboard */}

                    <li className="nav-item mb-2">
                        <Link
                            to="/admin"
                            className="nav-link text-white"
                        >
                            🏠 Dashboard
                        </Link>
                    </li>


                    {/* Job Cards */}

                    <li className="nav-item mb-2">
                        <Link
                            to="/admin/jobcards"
                            className="nav-link text-white"
                        >
                            📋 Job Cards
                        </Link>
                    </li>


                    {/* All Tickets */}

                    <li className="nav-item mb-2">
                        <Link
                            to="/admin/tickets"
                            className="nav-link text-white"
                        >
                            🎫 All Tickets
                        </Link>
                    </li>


                    {/* My Tickets */}

                    <li className="nav-item mb-2">
                        <Link
                            to="/admin/mytickets"
                            className="nav-link text-white"
                        >
                            📝 My Tickets
                        </Link>
                    </li>


                    {/* Archived Tickets */}

                    <li className="nav-item mb-2">
                        <Link
                            to="/admin/archivedtickets"
                            className="nav-link text-white"
                        >
                            🗄️ Archived Tickets
                        </Link>
                    </li>


                    {/* Users */}

                    <li className="nav-item mb-2">
                        <Link
                            to="/admin/users"
                            className="nav-link text-white"
                        >
                            👥 Users
                        </Link>

                          <button
    type="button"
    className="btn btn-danger"
    onClick={handleLogout}
>
    Logout
</button>
                    </li>


                </ul>


            </div>
            {/* Main Content */}

            <div className="col-md-10">

                <div className="p-4">

                    <Outlet />

                </div>

            </div>


        </div>
    );
}


export default AdminLayout;