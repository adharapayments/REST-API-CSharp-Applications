using System;
using System.Collections.Generic;
using System.IO;

class Example4
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

    public static void Main4(string[] args)
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

        // POSITION POLLING

        // get accounts
        List<ArthikaHFT.accountTick> accountTickList = wrapper.getAccount();

        Console.WriteLine("Starting Polling1");
        ArthikaHFT.positionTick positionTickList1 = wrapper.getPosition(null, new List<string> { "EUR/USD", "GBP/JPY", "GBP/USD" }, null);
        Console.WriteLine("StrategyPL: " + positionTickList1.accountingTick.strategyPL + " TotalEquity: " + positionTickList1.accountingTick.totalequity + " UsedMargin: " + positionTickList1.accountingTick.usedmargin + " FreeMargin: " + positionTickList1.accountingTick.freemargin);
        foreach (ArthikaHFT.assetPositionTick tick in positionTickList1.assetPositionTickList)
        {
            Console.WriteLine("Asset: " + tick.asset + " Account: " + tick.account + " Exposure: " + tick.exposure + " TotalRisk: " + tick.totalrisk);
        }
        foreach (ArthikaHFT.securityPositionTick tick in positionTickList1.securityPositionTickList)
        {
            Console.WriteLine("Security: " + tick.security + " Account: " + tick.account + " Equity: " + tick.equity + " Exposure: " + tick.exposure + " Price: " + tick.price.ToString("F" + tick.pips) + " Pips: " + tick.pips);
        }
        Console.WriteLine("Polling1 Finished");

        Console.WriteLine("Starting Polling2");
        List<string> accountlist = null;
        if (accountTickList != null && accountTickList.Count > 1)
        {
            accountlist = new List<string>();
            accountlist.Add(accountTickList[0].name);
            accountlist.Add(accountTickList[1].name);
        }
        ArthikaHFT.positionTick positionTickList2 = wrapper.getPosition(new List<string> { "EUR", "GBP", "JPY", "USD" }, null, accountlist);
        Console.WriteLine("StrategyPL: " + positionTickList2.accountingTick.strategyPL + " TotalEquity: " + positionTickList2.accountingTick.totalequity + " UsedMargin: " + positionTickList2.accountingTick.usedmargin + " FreeMargin: " + positionTickList2.accountingTick.freemargin);
        foreach (ArthikaHFT.assetPositionTick tick in positionTickList2.assetPositionTickList)
        {
            Console.WriteLine("Asset: " + tick.asset + " Account: " + tick.account + " Exposure: " + tick.exposure + " TotalRisk: " + tick.totalrisk);
        }
        foreach (ArthikaHFT.securityPositionTick tick in positionTickList2.securityPositionTickList)
        {
            Console.WriteLine("Security: " + tick.security + " Account: " + tick.account + " Equity: " + tick.equity + " Exposure: " + tick.exposure + " Price: " + tick.price.ToString("F" + tick.pips) + " Pips: " + tick.pips);
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
