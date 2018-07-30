using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

class Example8
{
    private static AdharaHFT wrapper;
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

    public static void Main8(string[] args)
    {

        // get properties from file
        getProperties();

        wrapper = new AdharaHFT(domain, url_stream, url_polling, url_challenge, url_token, user, password, authentication_port, request_port, ssl, ssl_cert);

        bool auth = wrapper.doAuthentication();
        if (!auth)
        {
            Console.WriteLine("Authentication failed");
            Console.Read();
            return;
        }

        // CANCEL PENDING ORDER WITH ORDER POLLING

        // get tinterfaces
        List<AdharaHFT.tinterfaceTick> tinterfaceTickList = wrapper.getInterface();
        string tinterface1 = tinterfaceTickList[0].name;

        Console.WriteLine("Starting Polling1");
        List<AdharaHFT.orderTick> orderTickList1 = wrapper.getOrder(null, null, new List<string> { AdharaHFT.ORDERTYPE_PENDING, AdharaHFT.ORDERTYPE_CANCELED });
        foreach (AdharaHFT.orderTick tick in orderTickList1)
        {
            Console.WriteLine("TempId: " + tick.tempid + " OrderId: " + tick.orderid + " Security: " + tick.security + " Account: " + tick.account + " Quantity: " + tick.quantity + " Type: " + tick.type + " Side: " + tick.side + " Status: " + tick.status + " Price: " + tick.limitprice);
        }
        Console.WriteLine("Polling1 Finished");
        Thread.Sleep(2000);

        // get current price
        double price = 0.0;
        List<AdharaHFT.priceTick> priceTickList1 = wrapper.getPrice(new List<string> { "EUR/USD" }, new List<string> { tinterface1 }, AdharaHFT.GRANULARITY_TOB, 1);
        foreach (AdharaHFT.priceTick tick in priceTickList1)
        {
            price = tick.price;
        }

        // Create pending order. If buy, order price must be lower than current price
        AdharaHFT.orderRequest order1 = new AdharaHFT.orderRequest();
        order1.security = "EUR/USD";
        order1.tinterface = tinterface1;
        order1.quantity = 500000;
        order1.side = AdharaHFT.SIDE_BUY;
        order1.type = AdharaHFT.TYPE_LIMIT;
        order1.timeinforce = AdharaHFT.VALIDITY_DAY;
        order1.price = price - 0.01;

        Console.WriteLine("Sending order");
        int tempid = -1;
        string fixid = "";
        List<AdharaHFT.orderRequest> orderList = wrapper.setOrder(new List<AdharaHFT.orderRequest> { order1 });
        foreach (AdharaHFT.orderRequest orderresponse in orderList)
        {
            Console.WriteLine("Id: " + orderresponse.tempid + " Security: " + orderresponse.security + " Side: " + orderresponse.side + " Quantity: " + orderresponse.quantity + " Price: " + orderresponse.price + " Type: " + orderresponse.type);
            tempid = orderresponse.tempid;
        }
        Console.WriteLine("Order sended order");
        Thread.Sleep(2000);

        Console.WriteLine("Starting Polling2");
        List<AdharaHFT.orderTick> orderTickList2 = wrapper.getOrder(null, null, new List<string> { AdharaHFT.ORDERTYPE_PENDING, AdharaHFT.ORDERTYPE_CANCELED });
        foreach (AdharaHFT.orderTick tick in orderTickList2)
        {
            Console.WriteLine("TempId: " + tick.tempid + " OrderId: " + tick.orderid + " Security: " + tick.security + " Account: " + tick.account + " Quantity: " + tick.quantity + " Type: " + tick.type + " Side: " + tick.side + " Status: " + tick.status + " Price: " + tick.limitprice);
            if (tempid == tick.tempid)
            {
                fixid = tick.fixid;
            }
        }
        Console.WriteLine("Polling2 Finished");
        Thread.Sleep(2000);

        Console.WriteLine("Cancel order");
        List<AdharaHFT.cancelTick> cancelList = wrapper.cancelOrder(new List<string> { fixid });
        foreach (AdharaHFT.cancelTick cancelresponse in cancelList)
        {
            Console.WriteLine("FixId: " + cancelresponse.fixid + " Result: " + cancelresponse.result);
        }
        Console.WriteLine("Order canceled");
        Thread.Sleep(2000);

        Console.WriteLine("Starting Polling3");
        List<AdharaHFT.orderTick> orderTickList3 = wrapper.getOrder(null, null, new List<string> { AdharaHFT.ORDERTYPE_PENDING, AdharaHFT.ORDERTYPE_CANCELED });
        foreach (AdharaHFT.orderTick tick in orderTickList3)
        {
            Console.WriteLine("TempId: " + tick.tempid + " OrderId: " + tick.orderid + " Security: " + tick.security + " Account: " + tick.account + " Quantity: " + tick.quantity + " Type: " + tick.type + " Side: " + tick.side + " Status: " + tick.status + " Price: " + tick.limitprice);
        }
        Console.WriteLine("Polling3 Finished");

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
