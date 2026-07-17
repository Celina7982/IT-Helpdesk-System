import { Routes, Route } from "react-router-dom";

import Login from "./pages/Login";
import AdminDashboard from "./pages/AdminDashboard";
import AdminLayout from "./layouts/AdminLayout";
import TechnicianDashboard from "./pages/TechnicianDashboard";

function App() {
    return (
        <Routes>

            <Route
                path="/"
                element={<Login />}
            />

            <Route
                path="/admin"
                element={<AdminLayout />}
            >
                <Route
                    index
                    element={<AdminDashboard />}
                />
            </Route>

            <Route
                path="/technician"
                element={<TechnicianDashboard />}
            />

        </Routes>
    );
}

export default App;