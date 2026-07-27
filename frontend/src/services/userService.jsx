import api from "./api";

const userService = {

    //--------------------------------------------------
    // Get All Users
    //--------------------------------------------------

    getUsers: async () => {

        const response = await api.get("/User");

        return response.data;

    },

    //--------------------------------------------------
    // Get User
    //--------------------------------------------------

    getUser: async (id) => {

        const response = await api.get(`/User/${id}`);

        return response.data;

    },

    //--------------------------------------------------
    // Create User
    //--------------------------------------------------

    createUser: async (user) => {

        const response = await api.post("/User", user);

        return response.data;

    },

    //--------------------------------------------------
    // Update User
    //--------------------------------------------------

    updateUser: async (id, user) => {

        await api.put(`/User/${id}`, user);

    },

    //--------------------------------------------------
    // Delete User
    //--------------------------------------------------

    deleteUser: async (id) => {

        await api.delete(`/User/${id}`);

    },

    //--------------------------------------------------
    // Get Technicians
    //--------------------------------------------------

    getTechnicians: async () => {

        const response = await api.get("/User/technicians");

        return response.data;

    }

};

export default userService;