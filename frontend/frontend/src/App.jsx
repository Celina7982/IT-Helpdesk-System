import { Routes, Route } from "react-router-dom";

import Login from "./pages/Login";
import AdminDashboard from "./pages/AdminDashboard";
import AdminLayout from "./layouts/AdminLayout";

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

        </Routes>
    );
}

export default App;