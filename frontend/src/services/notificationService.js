import api from "./api";

const notificationService = {

    //--------------------------------------------------
    // Get paginated notifications
    //--------------------------------------------------

    getMyNotifications: async (
        pageNumber = 1,
        pageSize = 10
    ) => {

        const response = await api.get(
            "/notifications",
            {
                params: {
                    pageNumber,
                    pageSize
                }
            }
        );

        return response.data;
    },


    //--------------------------------------------------
    // Get unread notifications
    //--------------------------------------------------

   getUnreadNotificationCount: async () => {

    const response =
        await api.get("/notifications/unread/count");

    return response.data;
},


    //--------------------------------------------------
    // Mark notification as read
    //--------------------------------------------------

    markAsRead: async (notificationId) => {

        const response =
            await api.put(
                `/notifications/${notificationId}/read`
            );

        return response.data;
    },


    //--------------------------------------------------
    // Delete notification
    //--------------------------------------------------

    deleteNotification: async (notificationId) => {

        const response =
            await api.delete(
                `/notifications/${notificationId}`
            );

        return response.data;
    }

};

export default notificationService;