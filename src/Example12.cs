using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

class AdharaHFTListenerImp12 : AdharaHFTListener
{

    void AdharaHFTListener.timestampEvent(string timestamp)
    {
        //Console.WriteLine("Response timestamp: " + timestamp + " Contents:");
    }

    void AdharaHFTListener.heartbeatEvent()
    {
        Console.WriteLine("Heartbeat!");
    }

    void AdharaHFTListener.messageEvent(string message)
    {
        Console.WriteLine("Message from server: " + message);
    }

    void AdharaHFTListener.priceEvent(List<AdharaHFT.priceTick> priceTickList)
    {
        foreach (AdharaHFT.priceTick tick in priceTickList)
        {
            Console.WriteLine("Security: " + tick.security + " Price: " + tick.price.ToString("F" + tick.pips) + " Side: " + tick.side + " TI: " + tick.tinterface + " Liquidity: " + tick.liquidity);
            if (tick.side.Equals("ask"))
            {
                if (tick.tinterface.Equals(Example12.tinterface2))
                {
                    Example12.bestcanask = tick.price;
                    Example12.bestcanaskliquidity = tick.liquidity;
                }
                if (tick.tinterface.Equals(Example12.tinterface1))
                {
                    Example12.bestbaxask = tick.price;
                    Example12.bestbaxaskliquidity = tick.liquidity;
                }

            }
            if (tick.side.Equals("bid"))
            {
                if (tick.tinterface.Equals(Example12.tinterface2))
                {
                    Example12.bestcanbid = tick.price;
                    Example12.bestcanbidliquidity = tick.liquidity;
                }
                if (tick.tinterface.Equals(Example12.tinterface1))
                {
                    Example12.bestbaxbid = tick.price;
                    Example12.bestbaxbidliquidity = tick.liquidity;
                }
            }
        }
        Example12.checkPrices();
    }

    void AdharaHFTListener.accountingEvent(AdharaHFT.accountingTick accountingTick)
    {
        Console.WriteLine("StrategyPL: " + accountingTick.strategyPL + " TotalEquity: " + accountingTick.totalequity + " UsedMargin: " + accountingTick.usedmargin + " FreeMargin: " + accountingTick.freemargin);
    }

    void AdharaHFTListener.assetPositionEvent(List<AdharaHFT.assetPositionTick> assetPositionTickList)
    {
        foreach (AdharaHFT.assetPositionTick tick in assetPositionTickList)
        {
            Console.WriteLine("Asset: " + tick.asset + " Account: " + tick.account + " Exposure: " + tick.exposure + " TotalRisk: " + tick.totalrisk);
        }
    }

    void AdharaHFTListener.securityPositionEvent(List<AdharaHFT.securityPositionTick> securityPositionTickList)
    {
        foreach (AdharaHFT.securityPositionTick tick in securityPositionTickList)
        {
            Console.WriteLine("Security: " + tick.security + " Account: " + tick.account + " Equity: " + tick.equity + " Exposure: " + tick.exposure + " Price: " + tick.price + " Pips: " + tick.pips);
        }
    }

    void AdharaHFTListener.positionHeartbeatEvent(AdharaHFT.positionHeartbeat positionHeartbeatList)
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

    void AdharaHFTListener.orderEvent(List<AdharaHFT.orderTick> orderTickList)
    {
        foreach (AdharaHFT.orderTick tick in orderTickList)
        {
            Console.WriteLine("TempId: " + tick.tempid + " OrderId: " + tick.orderid + " Security: " + tick.security + " Account: " + tick.account + " Quantity: " + tick.quantity + " Type: " + tick.type + " Side: " + tick.side + " Status: " + tick.status);
        }
    }

    void AdharaHFTListener.orderHeartbeatEvent(AdharaHFT.orderHeartbeat orderHeartbeat)
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

class Example12
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

    public static double bestcanask = 0.0;
    public static int bestcanaskliquidity = 0;
    public static string bestcanaskti = "";
    public static double bestcanbid = 0.0;
    public static int bestcanbidliquidity = 0;
    public static string bestcanbidti = "";

    public static double bestbaxask = 0.0;
    public static int bestbaxaskliquidity = 0;
    public static string bestbaxaskti = "";
    public static double bestbaxbid = 0.0;
    public static int bestbaxbidliquidity = 0;
    public static string bestbaxbidti = "";

    public static string tinterface1 = "";
    public static string tinterface2 = "";

    public static void Main12(string[] args)
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

        // STRATEGY

        // get tinterfaces
        List<AdharaHFT.tinterfaceTick> tinterfaceTickList = wrapper.getInterface();
        tinterface1 = tinterfaceTickList[0].name;
        if (tinterfaceTickList.Count > 1)
        {
            tinterface2 = tinterfaceTickList[1].name;
        }
        else
        {
            tinterface2 = tinterfaceTickList[0].name;
        }
        List<string> tinterfacelist = null;
        if (tinterfaceTickList != null && tinterfaceTickList.Count > 1)
        {
            tinterfacelist = new List<string>();
            tinterfacelist.Add(tinterface1);
            tinterfacelist.Add(tinterface2);
        }

        // Open price streaming
        string id1 = wrapper.getPriceBegin(new List<string> { "EUR/USD" }, tinterfacelist, AdharaHFT.GRANULARITY_TOB, 1, interval, new AdharaHFTListenerImp12());
        Thread.Sleep(20000);

        // Close price streaming
        wrapper.getPriceEnd(id1);

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

    public static void checkPrices()
    {
        if ((bestcanask < bestbaxbid && bestcanask > 0.0) || (bestbaxask < bestcanbid && bestbaxask > 0.0))
        {
            AdharaHFT.orderRequest orderask = new AdharaHFT.orderRequest();
            AdharaHFT.orderRequest orderbid = new AdharaHFT.orderRequest();
            int quantity = 0;
            if (bestcanask < bestbaxbid && bestcanask > 0.0)
            {
                quantity = bestcanaskliquidity;
                if (bestcanaskliquidity > bestbaxbidliquidity)
                {
                    quantity = bestbaxbidliquidity;
                }
                orderask.tinterface = tinterface2;
                orderbid.tinterface = tinterface1;
                orderask.price = bestcanask;
                orderbid.price = bestbaxbid;
            }
            if (bestbaxask < bestcanbid && bestbaxask > 0.0)
            {
                quantity = bestbaxaskliquidity;
                if (bestbaxaskliquidity > bestcanbidliquidity)
                {
                    quantity = bestcanbidliquidity;
                }
                orderask.tinterface = tinterface1;
                orderbid.tinterface = tinterface2;
                orderask.price = bestbaxask;
                orderbid.price = bestcanbid;
            }

            orderask.security = "EUR/USD";
            orderask.quantity = quantity;
            orderask.side = AdharaHFT.SIDE_BUY;
            orderask.type = AdharaHFT.TYPE_LIMIT;
            orderask.timeinforce = AdharaHFT.VALIDITY_DAY;

            orderbid.security = "EUR/USD";
            orderbid.quantity = quantity;
            orderbid.side = AdharaHFT.SIDE_SELL;
            orderbid.type = AdharaHFT.TYPE_LIMIT;
            orderbid.timeinforce = AdharaHFT.VALIDITY_DAY;

            try
            {
                List<AdharaHFT.orderRequest> orderList1 = wrapper.setOrder(new List<AdharaHFT.orderRequest> { orderask, orderbid });
                foreach (AdharaHFT.orderRequest orderresponse in orderList1)
                {
                    Console.WriteLine("Id: " + orderresponse.tempid + " Security: " + orderresponse.security + " Side: " + orderresponse.side + " Quantity: " + orderresponse.quantity + " Price: " + orderresponse.price + " Type: " + orderresponse.type);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

