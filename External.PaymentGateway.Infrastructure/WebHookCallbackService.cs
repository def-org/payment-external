using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;

namespace External.PaymentGateway
{
    public class WebHookCallbackService : IWebHookCallbackService
    {
        public void Trigger(Uri endpoint, object notification) => Trigger(endpoint.ToString(), notification);

        public void Trigger(string endpoint, object notification)
        {
            HttpClient client = new HttpClient();

            string serializedContent = System.Text.Json.JsonSerializer.Serialize(notification, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

            StringContent stringContent = new StringContent(serializedContent);

            client.PostAsync(endpoint, stringContent).GetAwaiter().GetResult();
        }
    }
}
