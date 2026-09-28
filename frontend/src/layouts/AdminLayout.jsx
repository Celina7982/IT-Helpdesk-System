import { Outlet, Link } from "react-router-dom";

function AdminLayout() {
    const handleLogout = () => {
        localStorage.removeItem("token");
        window.location.href = "/";
    };

    return (
        <div className="container-fluid">
            <div className="row g-0 min-vh-100">
                {/* Sidebar */}
                <div className="col-md-2 bg-dark text-white d-flex flex-column p-3">
                    <h3 className="mt-3 text-center">
                        IT Helpdesk
                    </h3>

                    <hr className="text-secondary" />

                    <ul className="nav flex-column mb-auto">
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
                        </li>
                    </ul>

                    {/* Logout Button pinned to bottom of sidebar */}
                    <div className="mt-auto pt-3 border-top border-secondary">
                        <button
                            type="button"
                            className="btn btn-danger w-100"
                            onClick={handleLogout}
                        >
                            Logout
                        </button>
                    </div>
                </div>

                {/* Main Content */}
                <div className="col-md-10 bg-light">
                    <div className="p-4">
                        <Outlet />
                    </div>
                </div>
            </div>
        </div>
    );
}

export default AdminLayout;