import axios from "axios";

const api = axios.create({
    baseURL: "https://localhost:7112/api",
    headers: {
        "Content-Type": "application/json"
    }
});

// Automatically attach the JWT token to every request
api.interceptors.request.use((config) => {

    const token = localStorage.getItem("token");

    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }

    return config;

});

export default api;