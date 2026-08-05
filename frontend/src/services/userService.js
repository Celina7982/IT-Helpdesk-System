import api from "./api";

const userService = {

    //----------------------------------------------------
    // Get All Users
    //----------------------------------------------------

    getUsers: async () => {

        const response = await api.get("/User", {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        });

        return response.data;
    },

    //----------------------------------------------------
    // Get Technicians
    //----------------------------------------------------

    getTechnicians: async () => {

        const response = await api.get("/User/technicians", {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        });

        return response.data;
    },

    //----------------------------------------------------
    // Create User
    //----------------------------------------------------

    createUser: async (user) => {

        const response = await api.post("/User", user, {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        });

        return response.data;
    },

    //----------------------------------------------------
// Update User
//----------------------------------------------------

updateUser: async (id, user) => {

    await api.put(`/User/${id}`, user, {
        headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`
        }
    });

},

    //----------------------------------------------------
    // Delete User
    //----------------------------------------------------

    deleteUser: async (id) => {

        await api.delete(`/User/${id}`, {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`
            }
        });

    }

};

export default userService;