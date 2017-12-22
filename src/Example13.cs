using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

class Example13
{
    private static ArthikaHFT wrapper;
    private static bool ssl = true;
    private static string domain;
    private static string url_stream;
    private static string url_polling;
    private static string url_challenge;
    private static string url_token;
    private static string user;
    private static string password;
    private static string authentication_port;
    private static string request_port;
    private static string ssl_cert;
    private static int interval;

    public static void Main13(string[] args)
    {

        // get properties from file
        getProperties();

        wrapper = new ArthikaHFT(domain, url_stream, url_polling, url_challenge, url_token, user, password, authentication_port, request_port, ssl, ssl_cert);

        bool auth = wrapper.doAuthentication();
        if (!auth)
        {
            Console.WriteLine("Authentication failed");
            Console.Read();
            return;
        }

        // MULTIPLE ORDER CREATION

        // get tinterfaces
        List<ArthikaHFT.tinterfaceTick> tinterfaceTickList = wrapper.getInterface();
        string tinterface1 = tinterfaceTickList[0].name;
        string tinterface2;
        if (tinterfaceTickList.Count > 1)
        {
            tinterface2 = tinterfaceTickList[1].name;
        }
        else
        {
            tinterface2 = tinterfaceTickList[0].name;
        }

        ArthikaHFT.orderRequest order1 = new ArthikaHFT.orderRequest();
        order1.security = "EUR/USD";
        order1.tinterface = tinterface2;
        order1.quantity = 1000000;
        order1.side = ArthikaHFT.SIDE_BUY;
        order1.type = ArthikaHFT.TYPE_MARKET;
        order1.timeinforce = ArthikaHFT.VALIDITY_DAY;

        ArthikaHFT.orderRequest order2 = new ArthikaHFT.orderRequest();
        order2.security = "EUR/USD";
        order2.tinterface = tinterface1;
        order2.quantity = 1000000;
        order2.side = ArthikaHFT.SIDE_SELL;
        order2.type = ArthikaHFT.TYPE_MARKET;
        order2.timeinforce = ArthikaHFT.VALIDITY_DAY;

        ArthikaHFT.orderRequest order3 = new ArthikaHFT.orderRequest();
        order3.security = "EUR/USD";
        order3.tinterface = tinterface2;
        order3.quantity = 2000000;
        order3.side = ArthikaHFT.SIDE_BUY;
        order3.type = ArthikaHFT.TYPE_MARKET;
        order3.timeinforce = ArthikaHFT.VALIDITY_DAY;

        ArthikaHFT.orderRequest order4 = new ArthikaHFT.orderRequest();
        order4.security = "EUR/USD";
        order4.tinterface = tinterface1;
        order4.quantity = 2000000;
        order4.side = ArthikaHFT.SIDE_SELL;
        order4.type = ArthikaHFT.TYPE_MARKET;
        order4.timeinforce = ArthikaHFT.VALIDITY_DAY;

        ArthikaHFT.orderRequest order5 = new ArthikaHFT.orderRequest();
        order5.security = "EUR/USD";
        order5.tinterface = tinterface2;
        order5.quantity = 1000000;
        order5.side = ArthikaHFT.SIDE_BUY;
        order5.type = ArthikaHFT.TYPE_MARKET;
        order5.timeinforce = ArthikaHFT.VALIDITY_DAY;

        ArthikaHFT.orderRequest order6 = new ArthikaHFT.orderRequest();
        order6.security = "EUR/USD";
        order6.tinterface = tinterface1;
        order6.quantity = 1000000;
        order6.side = ArthikaHFT.SIDE_SELL;
        order6.type = ArthikaHFT.TYPE_MARKET;
        order6.timeinforce = ArthikaHFT.VALIDITY_DAY;

        Console.WriteLine("Starting Polling1");
        List<ArthikaHFT.orderTick> orderTickList1 = wrapper.getOrder(new List<string> { "EUR/USD" }, null, null);
        foreach (ArthikaHFT.orderTick tick in orderTickList1)
        {
            Console.WriteLine("TempId: " + tick.tempid + " OrderId: " + tick.orderid + " Security: " + tick.security + " Account: " + tick.account + " Quantity: " + tick.quantity + " Type: " + tick.type + " Side: " + tick.side + " Status: " + tick.status);
        }
        Console.WriteLine("Polling1 Finished");
        Thread.Sleep(5000);

        Console.WriteLine("Sending order");
        List<ArthikaHFT.orderRequest> orderList1 = wrapper.setOrder(new List<ArthikaHFT.orderRequest> { order1, order2, order3, order4, order5, order6, order1, order2, order3, order4, order5, order6, order1, order2, order3, order4, order5, order6 });
        foreach (ArthikaHFT.orderRequest orderresponse in orderList1)
        {
            Console.WriteLine("Id: " + orderresponse.tempid + " Security: " + orderresponse.security + " Side: " + orderresponse.side + " Quantity: " + orderresponse.quantity + " Price: " + orderresponse.price + " Type: " + orderresponse.type);
        }
        List<ArthikaHFT.orderRequest> orderList2 = wrapper.setOrder(new List<ArthikaHFT.orderRequest> { order1, order2, order3, order4, order5, order6, order1, order2, order3, order4, order5, order6, order1, order2, order3, order4, order5, order6 });
        foreach (ArthikaHFT.orderRequest orderresponse in orderList2)
        {
            Console.WriteLine("Id: " + orderresponse.tempid + " Security: " + orderresponse.security + " Side: " + orderresponse.side + " Quantity: " + orderresponse.quantity + " Price: " + orderresponse.price + " Type: " + orderresponse.type);
        }
        List<ArthikaHFT.orderRequest> orderList3 = wrapper.setOrder(new List<ArthikaHFT.orderRequest> { order1, order2, order3, order4, order5, order6, order1, order2, order3, order4, order5, order6, order1, order2, order3, order4, order5, order6 });
        foreach (ArthikaHFT.orderRequest orderresponse in orderList3)
        {
            Console.WriteLine("Id: " + orderresponse.tempid + " Security: " + orderresponse.security + " Side: " + orderresponse.side + " Quantity: " + orderresponse.quantity + " Price: " + orderresponse.price + " Type: " + orderresponse.type);
        }
        Console.WriteLine("Order sended");
        Thread.Sleep(5000);

        Console.WriteLine("Starting Polling2");
        List<ArthikaHFT.orderTick> orderTickList2 = wrapper.getOrder(new List<string> { "EUR/USD" }, null, null);
        foreach (ArthikaHFT.orderTick tick in orderTickList2)
        {
            Console.WriteLine("TempId: " + tick.tempid + " OrderId: " + tick.orderid + " Security: " + tick.security + " Account: " + tick.account + " Quantity: " + tick.finishedquantity + " Type: " + tick.type + " Side: " + tick.side + " Status: " + tick.status);
        }
        Console.WriteLine("Polling2 Finished");

        Console.WriteLine("Press Enter to exit");
        Console.Read();
    }

    private static void getProperties()
    {
        try
        {
            foreach (var row in File.ReadAllLines("config.properties"))
            {
                //Console.WriteLine(row);
                if ("url-stream".Equals(row.Split('=')[0]))
                {
                    url_stream = row.Split('=')[1];
                }
                if ("url-polling".Equals(row.Split('=')[0]))
                {
                    url_polling = row.Split('=')[1];
                }
                if ("url-challenge".Equals(row.Split('=')[0]))
                {
                    url_challenge = row.Split('=')[1];
                }
                if ("url-token".Equals(row.Split('=')[0]))
                {
                    url_token = row.Split('=')[1];
                }
                if ("user".Equals(row.Split('=')[0]))
                {
                    user = row.Split('=')[1];
                }
                if ("password".Equals(row.Split('=')[0]))
                {
                    password = row.Split('=')[1];
                }
                if ("interval".Equals(row.Split('=')[0]))
                {
                    interval = Int32.Parse(row.Split('=')[1]);
                }
                if (ssl)
                {
                    if ("ssl-domain".Equals(row.Split('=')[0]))
                    {
                        domain = row.Split('=')[1];
                    }
                    if ("ssl-authentication-port".Equals(row.Split('=')[0]))
                    {
                        authentication_port = row.Split('=')[1];
                    }
                    if ("ssl-request-port".Equals(row.Split('=')[0]))
                    {
                        request_port = row.Split('=')[1];
                    }
                    if ("ssl-cert".Equals(row.Split('=')[0]))
                    {
                        ssl_cert = row.Split('=')[1];
                    }
                }
                else
                {
                    if ("domain".Equals(row.Split('=')[0]))
                    {
                        domain = row.Split('=')[1];
                    }
                    if ("authentication-port".Equals(row.Split('=')[0]))
                    {
                        authentication_port = row.Split('=')[1];
                    }
                    if ("request-port".Equals(row.Split('=')[0]))
                    {
                        request_port = row.Split('=')[1];
                    }
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
