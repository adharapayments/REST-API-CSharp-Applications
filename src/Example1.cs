using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

class AdharaHFTListenerImp1 : AdharaHFTListener
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
        }
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

class Example1
{
    private static AdharaHFT wrapper;
    private static bool ssl = false;
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

    public static void Main1(string[] args)
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

        // PRICE STREAMING

        // get tinterfaces
        List<AdharaHFT.tinterfaceTick> tinterfaceTickList = wrapper.getInterface();

        // Open first price streaming for one security in all tinterfaces
        string id1 = wrapper.getPriceBegin(new List<string> { "GBP/USD" }, null, AdharaHFT.GRANULARITY_TOB, 1, interval, new AdharaHFTListenerImp1());
        Thread.Sleep(5000);

        // Open second price streaming for two securities in the two first tinterfaces
        List<String> tinterfacelist = null;
        if (tinterfaceTickList != null && tinterfaceTickList.Count > 1)
        {
            tinterfacelist = new List<string>();
            tinterfacelist.Add(tinterfaceTickList.ElementAt(0).name);
            tinterfacelist.Add(tinterfaceTickList.ElementAt(1).name);
        }
        string id2 = wrapper.getPriceBegin(new List<string> { "EUR/USD", "GBP/JPY" }, tinterfacelist, AdharaHFT.GRANULARITY_FAB, 2, interval, new AdharaHFTListenerImp1());
        Thread.Sleep(5000);

        // Close second price streaming
        wrapper.getPriceEnd(id2);
        Thread.Sleep(5000);

        // Close first price streaming
        wrapper.getPriceEnd(id1);
        Thread.Sleep(5000);

        // Open third price streaming for six securities in the first tinterface
        if (tinterfaceTickList != null && tinterfaceTickList.Count > 0)
        {
            tinterfacelist = new List<string>();
            tinterfacelist.Add(tinterfaceTickList.ElementAt(0).name);
        }
        string id3 = wrapper.getPriceBegin(new List<string> { "EUR/USD", "EUR/GBP", "EUR/JPY", "GBP/JPY", "GBP/USD", "USD/JPY" }, tinterfacelist, AdharaHFT.GRANULARITY_TOB, 1, interval, new AdharaHFTListenerImp1());
        Thread.Sleep(5000);

        // Close third price streaming
        wrapper.getPriceEnd(id3);

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
