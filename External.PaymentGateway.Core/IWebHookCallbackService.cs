using System;

namespace External.PaymentGateway
{
    public interface IWebHookCallbackService
    {
        void Trigger(string endpoint, object notification);
        void Trigger(Uri endpoint, object notification);
    }
}