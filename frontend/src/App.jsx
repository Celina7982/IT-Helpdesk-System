import CreateTicket from "./pages/CreateTicket";
import MyTickets from "./pages/MyTickets";
import { Routes, Route } from "react-router-dom";
import TechnicianDashboard from "./pages/TechnicianDashboard";
import Login from "./pages/Login";
import ClientDashboard from "./pages/ClientDashboard";
import TicketDetails from "./pages/TicketDetails";

function App() {
    return (
        <Routes>

       <Route path="/" element={<Login />} />
<Route path="/client" element={<ClientDashboard />} />
<Route path="/create-ticket" element={<CreateTicket />} />
<Route path="/my-tickets" element={<MyTickets />} />
<Route path="/technician" element={<TechnicianDashboard />} />
<Route path="/ticket-details" element={<TicketDetails />} />
        </Routes>
    );
}

export default App;