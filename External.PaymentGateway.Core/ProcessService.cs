using External.PaymentGateway.Entities;
using External.PaymentGateway.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace External.PaymentGateway
{
    public class ProcessService
    {
        private readonly IQueryRepository<Account> accountQueryRepository;

        private readonly IPersistenceRepository<PaymentOrder> paymentPersistence;
        private readonly IPersistenceRepository<Account> accountPersistence;
        private readonly IWebHookCallbackService webHookCallbackService;
        private readonly IConsumer<PaymentOrder> consumer;

        public ProcessService(
            IQueryRepository<Account> accountQueryRepository,
            IPersistenceRepository<PaymentOrder> paymentPersistence,
            IPersistenceRepository<Account> accountPersistence,
            IWebHookCallbackService webHookCallbackService,
            IConsumer<PaymentOrder> consumer
            )
        {
            this.accountQueryRepository = accountQueryRepository;
            this.paymentPersistence = paymentPersistence;
            this.accountPersistence = accountPersistence;
            this.webHookCallbackService = webHookCallbackService;
            this.consumer = consumer;
        }


        public void Consume()
        {
            this.consumer.Start("payments", this.ProcessPayment);
        }

        private void ProcessPayment(PaymentOrder payment)
        {
            var account = accountQueryRepository.SingleOrDefault(it => it.Id == payment.AccountId);

            if (account == null)
            {
                throw new InvalidOperationException("Account Not found");
            }

            bool executed = false;

            string message = "";

            //BLoqueio Valor 10 não é permitido.
            //Motivo (Exemplificar uma negação)
            if (payment.Amount != 10)
            {
                account.Balance += payment.Amount;

                payment.Processed = true;

                this.paymentPersistence.Update(payment);

                this.accountPersistence.Update(account);

                executed = true;

                message = "Processed";

            }
            else
            {
                executed = false;
                message = "Amount = 10 (can't be processed)";
            }



            if (!string.IsNullOrWhiteSpace(payment.WebHookEndpoint))
            {
                webHookCallbackService.Trigger(payment.WebHookEndpoint, new { 
                    CorrelationId = payment.CorrelationId ,
                    Executed = executed,
                    Message = message
                });
            }
        }
    }
}
