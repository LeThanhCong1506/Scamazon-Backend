using System;
using System.Collections.Generic;

namespace MV.DomainLayer.Entities;

public partial class VAdminDashboardStat
{
    public long? TotalCustomers { get; set; }

    public long? NewCustomers7days { get; set; }

    public long? TotalProducts { get; set; }

    public long? LowStockProducts { get; set; }

    public long? PendingOrders { get; set; }

    public long? ConfirmedOrders { get; set; }

    public long? ShippingOrders { get; set; }

    public long? DeliveredOrders { get; set; }

    public long? OrdersToday { get; set; }

    public decimal? RevenueToday { get; set; }

    public decimal? Revenue7days { get; set; }

    public decimal? Revenue30days { get; set; }

    public long? ActiveChats { get; set; }
}
