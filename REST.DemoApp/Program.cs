using System;

namespace REST.DemoApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            //调用接口，获取数据
            REST.Model.V11.Output.OrderList orderlist =
            new ActionHandler()
            {
                MethodName = "OrderList",
                Version = "v1.1",
                Model = new REST.Model.V11.Input.OrderList()
                {
                    Condition1 = "",
                    Condition2 = "",
                    PageIndex = 1,
                    PageSize = 5
                }
            }.GetModel<REST.Model.V11.Output.OrderList>();
            //显示数据
            orderlist.Orders.ForEach(item =>
            {
                Console.WriteLine(item.CustomerName);
            });
            Console.Read();
        }
    }
}