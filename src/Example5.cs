using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

class ArthikaHFTListenerImp5 : ArthikaHFTListener
{

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

class Example5
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

    public static void Main5(string[] args)
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

        // ORDER STREAMING

        // get tinterfaces
        List<ArthikaHFT.tinterfaceTick> tinterfaceTickList = wrapper.getInterface();

        string tinterface1;
        if (tinterfaceTickList.Count > 1)
        {
            tinterface1 = tinterfaceTickList[1].name;
        }
        else
        {
            tinterface1 = tinterfaceTickList[0].name;
        }
        ArthikaHFT.orderRequest order1 = new ArthikaHFT.orderRequest();
        order1.security = "EUR/USD";
        order1.tinterface = tinterface1;
        order1.quantity = 500000;
        order1.side = ArthikaHFT.SIDE_BUY;
        order1.type = ArthikaHFT.TYPE_LIMIT;
        order1.timeinforce = ArthikaHFT.VALIDITY_DAY;
        order1.price = 1.10548;

        string tinterface2 = tinterfaceTickList[0].name;
        ArthikaHFT.orderRequest order2 = new ArthikaHFT.orderRequest();
        order2.security = "GBP/USD";
        order2.tinterface = tinterface2;
        order2.quantity = 600000;
        order2.side = ArthikaHFT.SIDE_BUY;
        order2.type = ArthikaHFT.TYPE_LIMIT;
        order2.timeinforce = ArthikaHFT.VALIDITY_DAY;
        order2.price = 1.47389;

        // Open order streaming
        string id1 = wrapper.getOrderBegin(null, null, null, interval, new ArthikaHFTListenerImp5());
        Thread.Sleep(2000);

        // Create two orders
        List<ArthikaHFT.orderRequest> orderList = wrapper.setOrder(new List<ArthikaHFT.orderRequest> { order1, order2 });
        foreach (ArthikaHFT.orderRequest orderresponse in orderList)
        {
            Console.WriteLine("Id: " + orderresponse.tempid + " Security: " + orderresponse.security + " Side: " + orderresponse.side + " Quantity: " + orderresponse.quantity + " Price: " + orderresponse.price + " Type: " + orderresponse.type);
        }
        Thread.Sleep(5000);

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
