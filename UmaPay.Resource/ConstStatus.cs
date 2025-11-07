namespace UmaPay.Resource
{
    public static class ConstStatus
    {
        public const int Initiated = 1;
        public const int Processing = 2;
        public const int Completed = 3;
        public const int Failed = 4;
        public const int Cancelled = 5;
        public const int CompletedInSap = 6;
        public const int FailedInSap = 7;
        public const int GatewaySucess = 8;
        public const int GatewayFailure = 9;
        public const int GatewayPending = 10;
        public const int GatewayReview = 11;
        public const int Review = 12;

        public static string GetStatusName(int statusId)
        {
            return statusId switch
            {
                Initiated => "Iniciada",
                Processing => "Procesando",
                Completed => "Completada",
                Failed => "Fallida",
                Cancelled => "Cancelada",
                CompletedInSap => "Completada en SAP",
                FailedInSap => "Fallida en SAP",
                GatewaySucess => "Consulta URL Sucess",
                GatewayFailure => "Consulta URL Failure",
                GatewayPending => "Consulta URL Pending",
                GatewayReview => "Consulta URL Review",
                _ => "Desconocido"
            };
        }

        public static string GetStatusDescription(int statusId)
        {
            return statusId switch
            {
                Initiated => "La transacción ha sido iniciada pero no completada.",
                Processing => "La transacción está siendo procesada por la pasarela de pago.",
                Completed => "La transacción se ha completado exitosamente.",
                Failed => "La transacción ha fallado.",
                Cancelled => "La transacción ha sido cancelada por el usuario o el sistema.",
                CompletedInSap => "La transacción se ha completado exitosamente en SAP.",
                FailedInSap => "La transacción ha fallado en SAP.",
                GatewaySucess => "Se ha realizado la consulta del URL con el estado Sucess.",
                GatewayFailure => "Se ha realizado la consulta del URL con el estado Failure.",
                GatewayPending => "Se ha realizado la consulta del URL con el estado Pending.",
                GatewayReview => "Se ha realizado la consulta del URL con el estado Review.",
                _ => "Estado desconocido."
            };
        }

        public static int GetStatusFromFlag(string flag)
        {
            return flag.ToLower() switch
            {
                "initiated" => Initiated,
                "processing" => Processing,
                "completed" => Completed,
                "failed" => Failed,
                "cancelled" => Cancelled,
                "completedinsap" => CompletedInSap,
                "failedinsap" => FailedInSap,
                "gatewaysucess" => GatewaySucess,
                "gatewayfailure" => GatewayFailure,
                "gatewaypending" => GatewayPending,
                "gatewayreview" => GatewayReview,
                "review" => Review,
                _ => 0 // o algún valor por defecto que indique un estado inválido
            };
        }
    }
}