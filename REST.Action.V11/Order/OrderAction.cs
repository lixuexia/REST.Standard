using System;
using System.Collections.Generic;

namespace REST.Action.V11.Order
{
    public class OrderAction : REST.Base.ActionBase
    {
        /// <summary>
        /// 订单列表服务
        /// </summary>
        /// <returns></returns>
        public Model.V11.Output.OrderList OrderList()
        {
            Model.V11.Output.OrderList output = new Model.V11.Output.OrderList();
            Model.V11.Input.OrderList input = this.InputSDKTypeObject as Model.V11.Input.OrderList;

            //条件判断
            if (!string.IsNullOrEmpty(input.Condition1))
            {
                //自定查询逻辑
            }
            if (input.PageSize <= 0)
            {
                input.PageSize = 20;
            }
            output.Orders = new List<Model.V11.Output.OrderItem>();
            output.PageIndex = input.PageIndex;
            output.PageSize = input.PageSize;
            output.RecordCount = 100;
            output.PageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(output.RecordCount) / Convert.ToDouble(output.PageSize)));

            output.Orders.Add(new Model.V11.Output.OrderItem()
            {
                Amount = 299,
                CustomerName = "测试客户1",
                OrderCode = "120113012"
            });
            output.Orders.Add(new Model.V11.Output.OrderItem()
            {
                Amount = 180,
                CustomerName = "测试客户2",
                OrderCode = "150177018"
            });
            return output;
        }
    }
}