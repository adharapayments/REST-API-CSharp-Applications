using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

class ArthikaHFTListenerImp11 : ArthikaHFTListener
{

    public Dictionary<int, String> fixidDictionary = new Dictionary<int, String>();

    void ArthikaHFTListener.timestampEvent(string timestamp)
    {
        //Console.WriteLine("Response timestamp: " + timestamp + " Contents:");
    }

    void ArthikaHFTListener.heartbeatEvent()
    {
        Console.WriteLine("Heartbeat!");
    }

    void ArthikaHFTListener.messageEvent(string message)
    {
        Console.WriteLine("Message from server: " + message);
    }

    void ArthikaHFTListener.priceEvent(List<ArthikaHFT.priceTick> priceTickList)
    {
        foreach (ArthikaHFT.priceTick tick in priceTickList)
        {
            Console.WriteLine("Security: " + tick.security + " Price: " + tick.price.ToString("F" + tick.pips) + " Side: " + tick.side + " TI: " + tick.tinterface + " Liquidity: " + tick.liquidity);
        }
    }

    void ArthikaHFTListener.accountingEvent(ArthikaHFT.accountingTick accountingTick)
    {
        Console.WriteLine("StrategyPL: " + accountingTick.strategyPL + " TotalEquity: " + accountingTick.totalequity + " UsedMargin: " + accountingTick.usedmargin + " FreeMargin: " + accountingTick.freemargin);
    }

    void ArthikaHFTListener.assetPositionEvent(List<ArthikaHFT.assetPositionTick> assetPositionTickList)
    {
        foreach (ArthikaHFT.assetPositionTick tick in assetPositionTickList)
        {
            Console.WriteLine("Asset: " + tick.asset + " Account: " + tick.account + " Exposure: " + tick.exposure + " TotalRisk: " + tick.totalrisk);
        }
    }

    void ArthikaHFTListener.securityPositionEvent(List<ArthikaHFT.securityPositionTick> securityPositionTickList)
    {
        foreach (ArthikaHFT.securityPositionTick tick in securityPositionTickList)
        {
            Console.WriteLine("Security: " + tick.security + " Account: " + tick.account + " Equity: " + tick.equity + " Exposure: " + tick.exposure + " Price: " + tick.price + " Pips: " + tick.pips);
        }
    }

    void ArthikaHFTListener.positionHeartbeatEvent(ArthikaHFT.positionHeartbeat positionHeartbeatList)
    {
        Console.Write("Asset: ");
        for (int i = 0; i < positionHeartbeatList.asset.Count; i++)
        {
            Console.Write(positionHeartbeatList.asset.ElementAt(i));
            if (i < positionHeartbeatList.asset.Count - 1)
            {
                Console.Write(",");
            }
        }
        Console.Write(" Security: ");
        for (int i = 0; i < positionHeartbeatList.security.Count; i++)
        {
            Console.Write(positionHeartbeatList.security.ElementAt(i));
            if (i < positionHeartbeatList.security.Count - 1)
            {
                Console.Write(", ");
            }
        }
        Console.Write(" Account: ");
        for (int i = 0; i < positionHeartbeatList.account.Count; i++)
        {
            Console.Write(positionHeartbeatList.account.ElementAt(i));
            if (i < positionHeartbeatList.account.Count - 1)
            {
                Console.Write(",");
            }
        }
        Console.WriteLine();
    }

    void ArthikaHFTListener.orderEvent(List<ArthikaHFT.orderTick> orderTickList)
    {
        foreach (ArthikaHFT.orderTick tick in orderTickList)
        {
            Console.WriteLine("TempId: " + tick.tempid + " OrderId: " + tick.orderid + " Security: " + tick.security + " Account: " + tick.account + " Quantity: " + tick.quantity + " Type: " + tick.type + " Side: " + tick.side + " Status: " + tick.status);
            fixidDictionary[tick.tempid] = tick.fixid;
        }
    }

    void ArthikaHFTListener.orderHeartbeatEvent(ArthikaHFT.orderHeartbeat orderHeartbeat)
    {
        Console.Write("Security: ");
        for (int i = 0; i < orderHeartbeat.security.Count; i++)
        {
            Console.Write(orderHeartbeat.security.ElementAt(i));
            if (i < orderHeartbeat.security.Count - 1)
            {
                Console.Write(", ");
            }
        }
        Console.Write(" Interface: ");
        for (int i = 0; i < orderHeartbeat.tinterface.Count; i++)
        {
            Console.Write(orderHeartbeat.tinterface.ElementAt(i));
            if (i < orderHeartbeat.tinterface.Count - 1)
            {
                Console.Write(",");
            }
        }
        Console.WriteLine();
    }
}

class Example11
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

    public static void Main11(string[] args)
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

        // MODIFY PENDING ORDER WITH ORDER STREAMING

        // get tinterfaces
        List<ArthikaHFT.tinterfaceTick> tinterfaceTickList = wrapper.getInterface();
        string tinterface1 = tinterfaceTickList[0].name;

        // Open order streaming
        ArthikaHFTListenerImp10 listener = new ArthikaHFTListenerImp10();
        string id1 = wrapper.getOrderBegin(null, null, null, interval, listener);
        Thread.Sleep(5000);

        // get current price
        double price = 0.0;
        List<ArthikaHFT.priceTick> priceTickList1 = wrapper.getPrice(new List<string> { "EUR/USD" }, new List<string> { tinterface1 }, ArthikaHFT.GRANULARITY_TOB, 1);
        foreach (ArthikaHFT.priceTick tick in priceTickList1)
        {
            price = tick.price;
        }

        // Create pending order. If buy, order price must be lower than current price
        ArthikaHFT.orderRequest order1 = new ArthikaHFT.orderRequest();
        order1.security = "EUR/USD";
        order1.tinterface = tinterface1;
        order1.quantity = 500000;
        order1.side = ArthikaHFT.SIDE_BUY;
        order1.type = ArthikaHFT.TYPE_LIMIT;
        order1.timeinforce = ArthikaHFT.VALIDITY_DAY;
        order1.price = price - 0.01;
        List<ArthikaHFT.orderRequest> orderList = wrapper.setOrder(new List<ArthikaHFT.orderRequest> { order1 });
        int tempid = -1;
        foreach (ArthikaHFT.orderRequest orderresponse in orderList)
        {
            Console.WriteLine("Id: " + orderresponse.tempid + " Security: " + orderresponse.security + " Side: " + orderresponse.side + " Quantity: " + orderresponse.quantity + " Price: " + orderresponse.price + " Type: " + orderresponse.type);
            tempid = orderresponse.tempid;
        }
        Thread.Sleep(5000);

        // Modify pending order
        Console.WriteLine("Modify order");
        ArthikaHFT.modOrder ordermod = new ArthikaHFT.modOrder();
        ordermod.fixid = listener.fixidDictionary[tempid];
        ordermod.price = price - 0.015;
        ordermod.quantity = 200000;
        List<ArthikaHFT.modifyTick> modifyList = wrapper.modifyOrder(new List<ArthikaHFT.modOrder> { ordermod });
        foreach (ArthikaHFT.modifyTick modifyresponse in modifyList)
        {
            Console.WriteLine("FixId: " + modifyresponse.fixid + " Result: " + modifyresponse.result);
        }
        Console.WriteLine("Order modified");
        Thread.Sleep(2000);

        // Close order streaming
        wrapper.getOrderEnd(id1);

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
