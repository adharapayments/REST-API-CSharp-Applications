using System;
using System.Collections.Generic;
using System.IO;

class Example14
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

    public static void Main14(string[] args)
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

        // HISTORICAL PRICE POLLING

        // get tinterfaces
        List<ArthikaHFT.tinterfaceTick> tinterfaceTickList = wrapper.getInterface();

        Console.WriteLine("Starting Candle list 1");
        List<String> tinterfacelist = null;
        if (tinterfaceTickList != null && tinterfaceTickList.Count > 1)
        {
            tinterfacelist = new List<string>();
            tinterfacelist.Add(tinterfaceTickList[1].name);
        }
        List<ArthikaHFT.candleTick> candleTickList1 = wrapper.getHistoricalPrice(new List<string> { "EUR/USD", "EUR/GBP", "EUR/JPY", "GBP/JPY", "GBP/USD", "USD/JPY" }, tinterfacelist, ArthikaHFT.CANDLE_GRANULARITY_10MINUTES, ArthikaHFT.SIDE_ASK, 5);
        foreach (ArthikaHFT.candleTick tick in candleTickList1)
        {
            Console.WriteLine("Security: " + tick.security + " tinterface: " + tick.tinterface + " TimeStamp: " + tick.timestamp + " Side: " + tick.side + " Open: " + tick.open + " High: " + tick.high + " Low: " + tick.low + " Close: " + tick.close + " Ticks: " + tick.ticks);
        }
        Console.WriteLine("Candle list 1 Finished");

        Console.WriteLine("Starting Candle list 2");
        List<ArthikaHFT.candleTick> candleTickList2 = wrapper.getHistoricalPrice(new List<string> { "EUR/USD" }, null, ArthikaHFT.CANDLE_GRANULARITY_30MINUTES, ArthikaHFT.SIDE_BID, 3);
        foreach (ArthikaHFT.candleTick tick in candleTickList2)
        {
            Console.WriteLine("Security: " + tick.security + " tinterface: " + tick.tinterface + " TimeStamp: " + tick.timestamp + " Side: " + tick.side + " Open: " + tick.open + " High: " + tick.high + " Low: " + tick.low + " Close: " + tick.close + " Ticks: " + tick.ticks);
        }
        Console.WriteLine("Candle list 2 Finished");

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
