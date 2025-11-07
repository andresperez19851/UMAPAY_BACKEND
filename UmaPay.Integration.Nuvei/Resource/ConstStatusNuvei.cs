namespace UmaPay.Integration.Nuvei
{
    public static class Status
    {
        public const int WaitingForPayment = 0;
        public const int RequiresVerification = 1;
        public const int PartiallyPaid = 2;
        public const int Paid = 3;
        public const int InDispute = 4;
        public const int Overpaid = 5;
        public const int Fraud = 6;
        public const int Reversed = 7;
        public const int Chargeback = 8;
        public const int RejectedByProcessor = 9;
        public const int SystemError = 10;
        public const int FraudDetectedByNuvei = 11;
        public const int NuveiBlacklist = 12;
        public const int ToleranceTime = 13;
        public const int ExpiredByNuvei = 14;
        public const int ExpiredByCarrier = 15;
        public const int RejectedByNuvei = 16;
        public const int AbandonedByNuvei = 17;
        public const int AbandonedByCustomer = 18;
        public const int InvalidAuthorizationCode = 19;
        public const int ExpiredAuthorizationCode = 20;
        public const int NuveiFraudPendingReversal = 21;
        public const int InvalidAuthCodePendingReversal = 22;
        public const int ExpiredAuthCodePendingReversal = 23;
        public const int NuveiFraudReversalRequested = 24;
        public const int InvalidAuthCodeReversalRequested = 25;
        public const int ExpiredAuthCodeReversalRequested = 26;
        public const int MerchantPendingReversal = 27;
        public const int MerchantReversalRequested = 28;
        public const int Voided = 29;
        public const int SettledTransaction = 30;
        public const int WaitingForOTP = 31;
        public const int OTPValidatedCorrectly = 32;
        public const int OTPNotValidated = 33;
        public const int PartialReversal = 34;
        public const int ThreeDSMethodRequested = 35;
        public const int ThreeDSChallengeRequested = 36;
        public const int RejectedByThreeDS = 37;
        public const int CPFValidationFailed = 47;
        public const int AuthenticatedByThreeDS = 48;

        public static string GetStatusName(int statusCode)
        {
            return statusCode switch
            {
                WaitingForPayment => "Esperando para ser Pagada",
                RequiresVerification => "Se requiere verificación",
                PartiallyPaid => "Pagada Parcialmente",
                Paid => "Pagada",
                InDispute => "En Disputa",
                Overpaid => "Sobrepagada",
                Fraud => "Fraude",
                Reversed => "Reverso",
                Chargeback => "Contracargo",
                RejectedByProcessor => "Rechazada por el procesador",
                SystemError => "Error en el sistema",
                FraudDetectedByNuvei => "Fraude detectado por Nuvei",
                NuveiBlacklist => "Blacklist de Nuvei",
                ToleranceTime => "Tiempo de tolerancia",
                ExpiredByNuvei => "Expirada por Nuvei",
                ExpiredByCarrier => "Expirado por el carrier",
                RejectedByNuvei => "Rechazado por Nuvei",
                AbandonedByNuvei => "Abandonada por Nuvei",
                AbandonedByCustomer => "Abandonada por el cliente",
                InvalidAuthorizationCode => "Código de autorización inválido",
                ExpiredAuthorizationCode => "Código de autorización expirado",
                NuveiFraudPendingReversal => "Fraude Nuvei - Reverso pendiente",
                InvalidAuthCodePendingReversal => "AuthCode Inválido - Reverso pendiente",
                ExpiredAuthCodePendingReversal => "AuthCode Expirado - Reverso pendiente",
                NuveiFraudReversalRequested => "Fraude Nuvei - Reverso solicitado",
                InvalidAuthCodeReversalRequested => "AuthCode Inválido - Reverso solicitado",
                ExpiredAuthCodeReversalRequested => "AuthCode Expirado - Reverso solicitado",
                MerchantPendingReversal => "Comercio - Reverso pendiente",
                MerchantReversalRequested => "Comercio - Reverso solicitado",
                Voided => "Anulada",
                SettledTransaction => "Transacción asentada (solo para Ecuador)",
                WaitingForOTP => "Esperando OTP",
                OTPValidatedCorrectly => "OTP validado correctamente",
                OTPNotValidated => "OTP no validado",
                PartialReversal => "Reverso parcial",
                ThreeDSMethodRequested => "Método 3DS solicitado, esperando para continuar",
                ThreeDSChallengeRequested => "Desafío 3DS solicitado, esperando el CRES",
                RejectedByThreeDS => "Rechazada por 3DS",
                CPFValidationFailed => "Validación de CPF fallida",
                AuthenticatedByThreeDS => "Autenticado por 3DS",
                _ => "Estado desconocido"
            };
        }
    }
}