﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Learning.Threads
{
    // This class contains a subset of data from the Northwind database
    // in the form of string arrays. The methods such as GetCustomers, GetOrders, and so on
    // transform the strings into object arrays that you can query in an object-oriented way.
    // Many of the code examples in the PLINQ How-to topics are designed to be pasted into 
    // the PLINQDataSample class and invoked from the Main method.
    public  class PLINQDataSample
    {

        public void Process()
        {
            ////Call methods here.
            TestDataSource();

            Console.WriteLine("Go to the next topic");

            var orders = GetOrders();
            var query = (from ord in orders.AsParallel()
                         orderby ord.CustomerID
                         select new
                         {
                             ord.CustomerID,
                             Details = ord.OrderID,
                             Date = ord.OrderDate,
                             Shipped = ord.ShippedDate
                         }).Take(10);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            query.ForAll(x => Console.WriteLine(x));
            

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private String MyOrder(Order ord)
        {
            Thread.Sleep((int)Math.Abs(ord.CustomerID.GetHashCode() / 10000000));

            return ord.CustomerID;
        }

        private bool MyWhere(Order ord)
        {
            Console.WriteLine(ord);

            return true;
        }

        static void TestDataSource()
        {
            Console.WriteLine("Customer count: {0}", GetCustomers().Count());
            Console.WriteLine("Product count: {0}", GetProducts().Count());
            Console.WriteLine("Order count: {0}", GetOrders().Count());
            Console.WriteLine("Order Details count: {0}", GetOrderDetails().Count());
        }

        #region DataClasses
        public class Order
        {
            private Lazy<OrderDetail[]> _orderDetails;
            public Order()
            {
                _orderDetails = new Lazy<OrderDetail[]>(() => GetOrderDetailsForOrder(OrderID));
            }
            public int OrderID { get; set; }
            public string CustomerID { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime ShippedDate { get; set; }
            public OrderDetail[] OrderDetails { get { return _orderDetails.Value; } }

            public override string ToString()
            {
                return $"{OrderID} - {CustomerID} - {OrderDate} - {ShippedDate}" ;
            }
        }

        public class Customer
        {
            private Lazy<Order[]> _orders;
            public Customer()
            {
                _orders = new Lazy<Order[]>(() => GetOrdersForCustomer(CustomerID));
            }
            public string CustomerID { get; set; }
            public string CustomerName { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string PostalCode { get; set; }
            public Order[] Orders
            {
                get
                {
                    return _orders.Value;
                }
            }
        }

        public class Product
        {
            public string ProductName { get; set; }
            public int ProductID { get; set; }
            public double UnitPrice { get; set; }
        }

        public class OrderDetail
        {
            public int OrderID { get; set; }
            public int ProductID { get; set; }
            public double UnitPrice { get; set; }
            public double Quantity { get; set; }
            public double Discount { get; set; }
        }
        #endregion

        public static IEnumerable<string> GetCustomersAsStrings()
        {
            return System.IO.File.ReadAllLines(@".\Threads\Data\plinqdata.csv")
                                            .SkipWhile((line) => line.StartsWith("CUSTOMERS") == false)
                                             .Skip(1)
                                            .TakeWhile((line) => line.StartsWith("END CUSTOMERS") == false);
        }

        public static IEnumerable<Customer> GetCustomers()
        {
            var customers = System.IO.File.ReadAllLines(@".\Threads\Data\plinqdata.csv")
                                             .SkipWhile((line) => line.StartsWith("CUSTOMERS") == false)
                                             .Skip(1)
                                             .TakeWhile((line) => line.StartsWith("END CUSTOMERS") == false);
            return (from line in customers
                    let fields = line.Split(',')
                    let custID = fields[0].Trim()
                    select new Customer()
                    {
                        CustomerID = custID,
                        CustomerName = fields[1].Trim(),
                        Address = fields[2].Trim(),
                        City = fields[3].Trim(),
                        PostalCode = fields[4].Trim()
                    });
        }

        public static Order[] GetOrdersForCustomer(string id)
        {
            // Assumes we copied the file correctly!
            var orders = System.IO.File.ReadAllLines(@".\Threads\Data\plinqdata.csv")
                                             .SkipWhile((line) => line.StartsWith("ORDERS") == false)
                                              .Skip(1)
                                            .TakeWhile((line) => line.StartsWith("END ORDERS") == false);
            var orderStrings = from line in orders
                               let fields = line.Split(',')
                               where fields[1].CompareTo(id) == 0
                               select new Order()
                               {
                                   OrderID = Convert.ToInt32(fields[0]),
                                   CustomerID = fields[1].Trim(),
                                   OrderDate = DateTime.Parse(fields[2]),
                                   ShippedDate = DateTime.Parse(fields[3])
                               };
            return orderStrings.ToArray();
        }

        //  "10248, VINET, 7/4/1996 12:00:00 AM, 7/16/1996 12:00:00 AM
        public static IEnumerable<Order> GetOrders()
        {
            // Assumes we copied the file correctly!
            var orders = System.IO.File.ReadAllLines(@".\Threads\Data\plinqdata.csv")
                                            .SkipWhile((line) => line.StartsWith("ORDERS") == false)
                                             .Skip(1)
                                            .TakeWhile((line) => line.StartsWith("END ORDERS") == false);
            return from line in orders
                   let fields = line.Split(',')

                   select new Order()
                   {
                       OrderID = Convert.ToInt32(fields[0]),
                       CustomerID = fields[1].Trim(),
                       OrderDate = DateTime.Parse(fields[2]),
                       ShippedDate = DateTime.Parse(fields[3])
                   };
        }

        public static IEnumerable<Product> GetProducts()
        {
            // Assumes we copied the file correctly!
            var products = System.IO.File.ReadAllLines(@".\Threads\Data\plinqdata.csv")
                                            .SkipWhile((line) => line.StartsWith("PRODUCTS") == false)
                                             .Skip(1)
                                            .TakeWhile((line) => line.StartsWith("END PRODUCTS") == false);
            return from line in products
                   let fields = line.Split(',')
                   select new Product()
                   {
                       ProductID = Convert.ToInt32(fields[0]),
                       ProductName = fields[1].Trim(),
                       UnitPrice = Convert.ToDouble(fields[2])
                   };
        }

        public static IEnumerable<OrderDetail> GetOrderDetails()
        {
            // Assumes we copied the file correctly!
            var orderDetails = System.IO.File.ReadAllLines(@".\Threads\Data\plinqdata.csv")
                                            .SkipWhile((line) => line.StartsWith("ORDER DETAILS") == false)
                                             .Skip(1)
                                            .TakeWhile((line) => line.StartsWith("END ORDER DETAILS") == false);

            return from line in orderDetails
                   let fields = line.Split(',')
                   select new OrderDetail()
                   {
                       OrderID = Convert.ToInt32(fields[0]),
                       ProductID = Convert.ToInt32(fields[1]),
                       UnitPrice = Convert.ToDouble(fields[2]),
                       Quantity = Convert.ToDouble(fields[3]),
                       Discount = Convert.ToDouble(fields[4])
                   };
        }

        public static OrderDetail[] GetOrderDetailsForOrder(int id)
        {
            // Assumes we copied the file correctly!
            var orderDetails = System.IO.File.ReadAllLines(@".\Threads\Data\plinqdata.csv")
                                            .SkipWhile((line) => line.StartsWith("ORDER DETAILS") == false)
                                             .Skip(1)
                                            .TakeWhile((line) => line.StartsWith("END ORDER DETAILS") == false);

            var orderDetailStrings = from line in orderDetails
                                     let fields = line.Split(',')
                                     let ordID = Convert.ToInt32(fields[0])
                                     where ordID == id
                                     select new OrderDetail()
                                     {
                                         OrderID = ordID,
                                         ProductID = Convert.ToInt32(fields[1]),
                                         UnitPrice = Convert.ToDouble(fields[2]),
                                         Quantity = Convert.ToDouble(fields[3]),
                                         Discount = Convert.ToDouble(fields[4])
                                     };

            return orderDetailStrings.ToArray();
        }
    }
}
