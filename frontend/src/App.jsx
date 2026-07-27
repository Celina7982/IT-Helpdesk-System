import { Routes, Route } from "react-router-dom";

import Login from "./pages/Login";

import AdminLayout from "./layouts/AdminLayout";
import TechnicianLayout from "./layouts/TechnicianLayout";

import AdminDashboard from "./pages/AdminDashboard";
import TechnicianDashboard from "./pages/TechnicianDashboard";
import ClientDashboard from "./pages/ClientDashboard";

import JobCards from "./pages/JobCards";
import JobCardDetails from "./pages/JobCardDetails";
import Users from "./pages/Users";

function App() {
    return (
        <Routes>

            {/*=========================
                LOGIN
            =========================*/}
            <Route
                path="/"
                element={<Login />}
            />

            {/*=========================
                ADMIN
            =========================*/}
            <Route
                path="/admin"
                element={<AdminLayout />}
            >
                <Route
                    index
                    element={<AdminDashboard />}
                />

                <Route
                    path="jobcards"
                    element={<JobCards />}
                />

                <Route
                    path="jobcards/:id"
                    element={<JobCardDetails />}
                />
                    <Route
                        path="users"
                        element={<Users />}
                    />

                <Route
                    path="users"
                    element={<Users />}
                />

            </Route>

            {/*=========================
                TECHNICIAN
            =========================*/}
            <Route
                path="/technician"
                element={<TechnicianLayout />}
            >
                <Route
                    index
                    element={<TechnicianDashboard />}
                />

                <Route
                    path="jobcards"
                    element={<JobCards />}
                />

                <Route
                    path="jobcards/:id"
                    element={<JobCardDetails />}
                />
            </Route>

            {/*=========================
                CLIENT
            =========================*/}
            <Route
                path="/client"
                element={<ClientDashboard />}
            />

        </Routes>
    );
}

export default App;