import api from "./api";

const userService = {

    getTechnicians: async () => {

        const response = await api.get(
            "/User/technicians",
            {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`
                }
            }
        );

        return response.data;
    }

};

export default userService;