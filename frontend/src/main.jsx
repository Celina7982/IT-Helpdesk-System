// React imports
import React from "react";
import ReactDOM from "react-dom/client";

// Bootstrap CSS
import "bootstrap/dist/css/bootstrap.min.css";

// React Router
import { BrowserRouter } from "react-router-dom";

// Our App
import App from "./App";

// Custom CSS
import "./index.css";

// Start the application
ReactDOM.createRoot(document.getElementById("root")).render(
  <React.StrictMode>
    <BrowserRouter>
      <App />
    </BrowserRouter>
  </React.StrictMode>
);