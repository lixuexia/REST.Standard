using System.Collections.Generic;
using System.ComponentModel;

namespace REST.Model.V11.Output
{
    [Description("订单列表结果")]
    public class OrderList
    { 
        [Description("订单列表")]
        public List<OrderItem> Orders { get; set; }

        [Description("当前页码")]
        public int PageIndex { get; set; }

        [Description("页大小")]
        public int PageSize { get; set; }

        [Description("记录总数")]
        public long RecordCount { get; set; }

        [Description("总页数")]
        public int PageCount { get; set; }
    }
    [Description("单个订单信息")]
    public class OrderItem
    {
        [Description("订单编码")]
        public string OrderCode { get; set; }

        [Description("客户姓名")]
        public string CustomerName { get; set; }

        [Description("订单总价")]
        public decimal Amount { get; set; }
    }
}