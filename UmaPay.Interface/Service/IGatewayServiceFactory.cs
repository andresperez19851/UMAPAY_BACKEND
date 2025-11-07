namespace UmaPay.Interface.Service
{

    public interface IGatewayServiceFactory
    {
        IGatewayService GetGatewayService(string gatewayCode);
    }
}
