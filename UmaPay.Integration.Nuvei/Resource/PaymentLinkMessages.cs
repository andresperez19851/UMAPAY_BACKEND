namespace UmaPay.Integration.Nuvei
{
    public static class PaymentLinkMessages
    {
        public const string GeneratingPaymentLink = "Generando enlace de pago para la transacción {0}";
        public const string FailedToGeneratePaymentLink = "Error al generar el enlace de pago para la transacción {0}. Código de estado: {1}";
        public const string PaymentLinkError = "Ocurrió un error al generar el enlace de pago";
        public const string SuccessfullyGeneratedPaymentLink = "Enlace de pago generado exitosamente para la transacción {0}";
        public const string PaymentLinkGenerationFailed = "La generación del enlace de pago falló para la transacción {0}. Detalles: {1}";
        public const string PaymentLinkErrorStatus = "Fallo en la generación del enlace de pago: {0}";
        public const string ErrorGeneratingPaymentLink = "Ocurrió un error al generar el enlace de pago para la transacción {0}";
        public const string CustomerEmailRequired = "El correo electrónico del cliente es obligatorio";
        public const string CustomerFirstNameRequired = "El nombre del cliente es obligatorio";
        public const string CustomerLastNameRequired = "El apellido del cliente es obligatorio";
        public const string SuccessUrlRequired = "La URL de éxito es obligatoria";
        public const string FailureUrlRequired = "La URL de fallo es obligatoria";
        public const string PendingUrlRequired = "La URL de pendiente es obligatoria";
        public const string ReviewUrlRequired = "La URL de revisión es obligatoria";
        public const string DescriptionRequired = "La descripción es obligatoria";

        public const string ConfigurationMissing = "La configuración {0} está ausente";
        public const string ArgumentNullMessage = "El argumento {0} no puede ser nulo";
        public const string DeserializationFailed = "Error al deserializar la respuesta";

    }
}
