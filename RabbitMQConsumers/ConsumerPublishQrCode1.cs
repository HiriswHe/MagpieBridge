using daoextend.baseservice;
using MTDB3._0To4._0.Domain._4._0;
using RabbitMQ.Client.Events;
using RabbitMQRecieverInterface;
using System;
using Utils;

namespace RabbitMQConsumers
{
    public class ConsumerPublishQrCode1 : IRabbitMQRecieverService
    {
        CURDAllBaseService<WorkOrderProductsQrcode1PO, WorkOrderProductsQrcode1PO> service = new CURDAllBaseService<WorkOrderProductsQrcode1PO, WorkOrderProductsQrcode1PO>();
        public void HandleMessage(object ch, BasicDeliverEventArgs ea, string msg)
        {
            Console.WriteLine("ConsumerPublishQrCode1:" + msg);
            if (string.IsNullOrEmpty(msg)) return;
            WorkOrderProductsQrcode1PO workOrderProductsQrcode1PO = JsonUtil.GetInstanceFronJson<WorkOrderProductsQrcode1PO>(msg);
            service.InsertOrExistNot(workOrderProductsQrcode1PO);
        }
    }
}
