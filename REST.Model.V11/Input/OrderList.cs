using System.ComponentModel;

namespace REST.Model.V11.Input
{
    [Description("订单列表输入")]
    public class OrderList
    {
        [Description("页码")]
        public int PageIndex { get; set; }

        [Description("页大小")]
        public int PageSize { get; set; }

        [Description("查询条件1")]
        public string Condition1 { get; set; }

        [Description("查询条件2")]
        public string Condition2 { get; set; }
    }
}