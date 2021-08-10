namespace MainMusicStore.Utility
{
    public static class ProjectConstant
    {
        public const string ResultNotFound = "Data Not Found";
        public const string ResultSuccess = "Delete Operation Successfully";
        //----------------------------------------------------------//
        public const string ProcCoverTypeGetAll = "usp_GetCoverTypes";
        public const string ProcCoverTypeGet = "usp_GetCoverType";
        public const string ProcCoverTypeDelete = "usp_DeleteCoverType";
        public const string ProcCoverTypeCreate = "usp_CreateCoverType";
        public const string ProcCoverTypeUpdate = "usp_UpdateCoverType";
        //----------------------------------------------------------//

        public const string RoleUserIndi = "Individual Customer";
        public const string RoleUserComp = "Company Customer";
        public const string RoleAdmin = "Admin";
        public const string RoleEmployee = "Employee";
        //----------------------------------------------------------//

        public const string ShoppingCart = "ShoppingCart";

        //PaymentStatus----------------------------------------------------------//

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusRejected = "Rejected";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "Delayed";

        //OrderStatus----------------------------------------------------------//

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefund = "Refund";

        //---------------------------------------------------------------------//
    }
}
