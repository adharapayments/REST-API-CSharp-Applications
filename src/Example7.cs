using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

class Example7
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

    public static void Main7(string[] args)
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

        // ORDER CREATION

        // get tinterfaces
        List<ArthikaHFT.tinterfaceTick> tinterfaceTickList = wrapper.getInterface();

        string tinterface;
        if (tinterfaceTickList.Count > 1)
        {
            tinterface = tinterfaceTickList[1].name;
        }
        else
        {
            tinterface = tinterfaceTickList[0].name;
        }

        ArthikaHFT.orderRequest order1 = new ArthikaHFT.orderRequest();
        order1.security = "EUR/USD";
        order1.tinterface = tinterface;
        order1.quantity = 500000;
        order1.side = ArthikaHFT.SIDE_BUY;
        order1.type = ArthikaHFT.TYPE_MARKET;

        Console.WriteLine("Starting Polling1");
        List<ArthikaHFT.orderTick> orderTickList1 = wrapper.getOrder(new List<string> { "EUR/USD" }, new List<string> { tinterface }, null);
        foreach (ArthikaHFT.orderTick tick in orderTickList1)
        {
            Console.WriteLine("TempId: " + tick.tempid + " OrderId: " + tick.orderid + " Security: " + tick.security + " Account: " + tick.account + " Quantity: " + tick.quantity + " Type: " + tick.type + " Side: " + tick.side + " Status: " + tick.status);
        }
        Console.WriteLine("Polling1 Finished");
        Thread.Sleep(2000);

        Console.WriteLine("Sending order");
        List<ArthikaHFT.orderRequest> orderList = wrapper.setOrder(new List<ArthikaHFT.orderRequest> { order1 });
        foreach (ArthikaHFT.orderRequest orderresponse in orderList)
        {
            Console.WriteLine("Id: " + orderresponse.tempid + " Security: " + orderresponse.security + " Side: " + orderresponse.side + " Quantity: " + orderresponse.quantity + " Price: " + orderresponse.price + " Type: " + orderresponse.type);
        }
        Console.WriteLine("Order sended");
        Thread.Sleep(2000);

        Console.WriteLine("Starting Polling2");
        List<ArthikaHFT.orderTick> orderTickList2 = wrapper.getOrder(new List<string> { "EUR/USD" }, new List<string> { tinterface }, null);
        foreach (ArthikaHFT.orderTick tick in orderTickList2)
        {
            Console.WriteLine("TempId: " + tick.tempid + " OrderId: " + tick.orderid + " Security: " + tick.security + " Account: " + tick.account + " Quantity: " + tick.quantity + " Type: " + tick.type + " Side: " + tick.side + " Status: " + tick.status);
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
