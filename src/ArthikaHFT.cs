using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Web.Script.Serialization;

// Note: To enable JSON (JavaScriptSerializer) add following reference: System.Web.Extensions

public interface ArthikaHFTListener
{
    void timestampEvent(string timestamp);
    void heartbeatEvent();
    void messageEvent(string message);
    void priceEvent(List<ArthikaHFT.priceTick> priceTickList);
    void accountingEvent(ArthikaHFT.accountingTick accountingTick);
    void assetPositionEvent(List<ArthikaHFT.assetPositionTick> assetPositionTickList);
    void securityPositionEvent(List<ArthikaHFT.securityPositionTick> securityPositionTickList);
    void positionHeartbeatEvent(ArthikaHFT.positionHeartbeat positionHeartbeat);
    void orderEvent(List<ArthikaHFT.orderTick> orderTickList);
    void orderHeartbeatEvent(ArthikaHFT.orderHeartbeat orderHeartbeat);
}

public class ArthikaHFT
{

    private bool ssl;
    private string domain;
    private string url_stream;
    private string url_polling;
    private string url_challenge;
    private string url_token;
    private string user;
    private string password;
    private string authentication_port;
    private string request_port;
    private string ssl_cert;
    private string challenge;
    private string token = null;
    private X509Certificate certificate1 = null;
    private static int requestId = 0;
    private static object Lock = new object();

    private Dictionary<Thread, StreamReader> streamReaderMap;

    public static string SIDE_BUY = "buy";
    public static string SIDE_SELL = "sell";
    public static string SIDE_ASK = "ask";
    public static string SIDE_BID = "bid";
    public static string TYPE_MARKET = "market";
    public static string TYPE_LIMIT = "limit";
    public static string VALIDITY_DAY = "day";
    public static string VALIDITY_FILLORKILL = "fill or kill";
    public static string VALIDITY_INMEDIATEORCANCEL = "inmediate or cancel";
    public static string VALIDITY_GOODTILLCANCEL = "good till cancel";
    public static string GRANULARITY_TOB = "tob";
    public static string GRANULARITY_FAB = "fab";

    public static string ORDERTYPE_PENDING = "pending";
    public static string ORDERTYPE_INDETERMINATED = "indetermined";
    public static string ORDERTYPE_EXECUTED = "executed";
    public static string ORDERTYPE_CANCELED = "canceled";
    public static string ORDERTYPE_REJECTED = "rejected";

    public static string CANDLE_GRANULARITY_1SECOND = "S1";
    public static string CANDLE_GRANULARITY_5SECONDS = "S5";
    public static string CANDLE_GRANULARITY_10SECONDS = "S10";
    public static string CANDLE_GRANULARITY_30SECONDS = "S30";
    public static string CANDLE_GRANULARITY_1MINUTE = "M1";
    public static string CANDLE_GRANULARITY_5MINUTES = "M5";
    public static string CANDLE_GRANULARITY_10MINUTES = "M10";
    public static string CANDLE_GRANULARITY_30MINUTES = "M30";
    public static string CANDLE_GRANULARITY_1HOUR = "H1";
    public static string CANDLE_GRANULARITY_2HOURS = "H2";
    public static string CANDLE_GRANULARITY_6HOURS = "H6";

    public class hftRequest
    {
        public getAuthorizationChallengeRequest getAuthorizationChallenge { get; set; }
        public getAuthorizationTokenRequest getAuthorizationToken { get; set; }
        public getAccountRequest getAccount { get; set; }
        public getInterfaceRequest getInterface { get; set; }
        public getPriceRequest getPrice { get; set; }
        public getPositionRequest getPosition { get; set; }
        public getOrderRequest getOrder { get; set; }
        public setOrderRequest setOrder { get; set; }
        public cancelOrderRequest cancelOrder { get; set; }
        public modifyOrderRequest modifyOrder { get; set; }
        public getHistoricalPriceRequest getHistoricalPrice { get; set; }
    }

    public class hftResponse
    {
        public getAuthorizationChallengeResponse getAuthorizationChallengeResponse { get; set; }
        public getAuthorizationTokenResponse getAuthorizationTokenResponse { get; set; }
        public getAccountResponse getAccountResponse { get; set; }
        public getInterfaceResponse getInterfaceResponse { get; set; }
        public getPriceResponse getPriceResponse { get; set; }
        public getPositionResponse getPositionResponse { get; set; }
        public getOrderResponse getOrderResponse { get; set; }
        public setOrderResponse setOrderResponse { get; set; }
        public cancelOrderResponse cancelOrderResponse { get; set; }
        public modifyOrderResponse modifyOrderResponse { get; set; }
        public getHistoricalPriceResponse getHistoricalPriceResponse { get; set; }
    }

    public class getAuthorizationChallengeRequest
    {
        public string user { get; set; }

        public getAuthorizationChallengeRequest(string user)
        {
            this.user = user;
        }
    }

    public class getAuthorizationChallengeResponse
    {
        public string challenge { get; set; }
        public string timestamp { get; set; }
    }

    public class getAuthorizationTokenRequest
    {
        public string user { get; set; }
        public string challengeresp { get; set; }

        public getAuthorizationTokenRequest(string user, string challengeresp)
        {
            this.user = user;
            this.challengeresp = challengeresp;
        }
    }

    public class getAuthorizationTokenResponse
    {
        public string token { get; set; }
        public string timestamp { get; set; }
    }

    public class getAccountRequest
    {
        public string user { get; set; }
        public string token { get; set; }

        public getAccountRequest(string user, string token)
        {
            this.user = user;
            this.token = token;
        }
    }

    public class getAccountResponse
    {
        public List<accountTick> account { get; set; }
        public string timestamp { get; set; }
    }

    public class getInterfaceRequest
    {
        public string user { get; set; }
        public string token { get; set; }

        public getInterfaceRequest(string user, string token)
        {
            this.user = user;
            this.token = token;
        }
    }

    public class getInterfaceResponse
    {
        public List<tinterfaceTick> tinterface { get; set; }
        public string timestamp { get; set; }
    }

    public class getPriceRequest
    {
        public string        user { get; set; }
        public string        token { get; set; }
        public List<string>  security { get; set; }
        public List<string>  tinterface { get; set; }
        public string granularity { get; set; }
        public int levels { get; set; }
        public int interval { get; set; }

        public getPriceRequest(string user, string token, List<string> security, List<string> tinterface, string granularity, int levels, int interval)
        {
            this.user = user;
            this.token = token;
            this.security = security;
            this.tinterface = tinterface;
            this.granularity = granularity;
            this.levels = levels;
            this.interval = interval;
        }
    }

    public class getPriceResponse
    {
        public int              result { get; set; }
        public string           message { get; set; }
        public List<priceTick>  tick { get; set; }
        public priceHeartbeat   heartbeat { get; set; }
        public string           timestamp { get; set; }
    }

    public class getPositionRequest
    {
        public string user { get; set; }
        public string token { get; set; }
        public List<string> asset { get; set; }
        public List<string> security { get; set; }
        public List<string> account { get; set; }
        public int interval { get; set; }

        public getPositionRequest(string user, string token, List<string> asset, List<string> security, List<string> account, int interval)
        {
            this.user = user;
            this.token = token;
            this.asset = asset;
            this.security = security;
            this.account = account;
            this.interval = interval;
        }
    }

    public class getPositionResponse
    {
        public int result { get; set; }
        public string message { get; set; }
        public List<assetPositionTick> assetposition { get; set; }
        public List<securityPositionTick> securityposition { get; set; }
        public accountingTick accounting { get; set; }
        public positionHeartbeat heartbeat { get; set; }
        public string timestamp { get; set; }
    }

    public class getOrderRequest
    {
        public string user { get; set; }
        public string token { get; set; }
        public List<string> security { get; set; }
        public List<string> tinterface { get; set; }
        public List<string> type { get; set; }
        public int interval { get; set; }

        public getOrderRequest(string user, string token, List<string> security, List<string> tinterface, List<string> type, int interval)
        {
            this.user = user;
            this.token = token;
            this.security = security;
            this.tinterface = tinterface;
            this.type = type;
            this.interval = interval;
        }
    }

    public class getOrderResponse
    {
        public int result { get; set; }
        public string message { get; set; }
        public List<orderTick> order { get; set; }
        public orderHeartbeat heartbeat { get; set; }
        public string timestamp { get; set; }
    }

    public class setOrderRequest
    {
        public string user { get; set; }
        public string token { get; set; }
        public List<orderRequest> order { get; set; }

        public setOrderRequest(string user, string token, List<orderRequest> order)
        {
            this.user = user;
            this.token = token;
            this.order = order;
        }
    }

    public class setOrderResponse
    {
        public int result { get; set; }
        public string message { get; set; }
        public List<orderRequest> order { get; set; }
        public string timestamp { get; set; }
    }

    public class cancelOrderRequest
    {
        public string user { get; set; }
        public string token { get; set; }
        public List<string> fixid { get; set; }

        public cancelOrderRequest(string user, string token, List<string> fixid)
        {
            this.user = user;
            this.token = token;
            this.fixid = fixid;
        }
    }

    public class cancelOrderResponse
    {
        public List<cancelTick> order { get; set; }
        public string message { get; set; }
        public string timestamp { get; set; }
    }

    public class modifyOrderRequest
    {
        public string user { get; set; }
        public string token { get; set; }
        public List<modOrder> order { get; set; }

        public modifyOrderRequest(string user, string token, List<modOrder> order)
        {
            this.user = user;
            this.token = token;
            this.order = order;
        }
    }

    public class modifyOrderResponse
    {
        public List<modifyTick> order { get; set; }
        public string message { get; set; }
        public string timestamp { get; set; }
    }

    public class getHistoricalPriceRequest
    {
        public string user { get; set; }
        public string token { get; set; }
        public List<string> security { get; set; }
        public List<string> tinterface { get; set; }
        public string granularity { get; set; }
        public string side { get; set; }
        public int number { get; set; }

        public getHistoricalPriceRequest(string user, string token, List<string> security, List<string> tinterface, string granularity, string side, int number)
        {
            this.user = user;
            this.token = token;
            this.security = security;
            this.tinterface = tinterface;
            this.granularity = granularity;
            this.side = side;
            this.number = number;
        }
    }

    public class getHistoricalPriceResponse
    {
        public int result { get; set; }
        public string message { get; set; }
        public List<candleTick> candle { get; set; }
        public string timestamp { get; set; }
    }

    public class accountTick
    {
        public string name { get; set; }
        public string description { get; set; }
        public string style { get; set; }
        public int leverage { get; set; }
        public string rollover { get; set; }
        public string settlement { get; set; }
    }

    public class tinterfaceTick
    {
        public string name { get; set; }
        public string description { get; set; }
        public string account { get; set; }
        public string commissions { get; set; }
    }

    public class priceTick
    {
        public string security { get; set; }
        public string tinterface { get; set; }
        public double price { get; set; }
        public int pips { get; set; }
        public int liquidity { get; set; }
        public string side { get; set; }
    }

    public class priceHeartbeat
    {
        public List<string> security { get; set; }
        public List<string> tinterface { get; set; }
    }

    public class assetPositionTick
    {
        public string account { get; set; }
        public string asset { get; set; }
        public double exposure { get; set; }
        public double totalrisk { get; set; }
        public double pl { get; set; }
    }

    public class securityPositionTick
    {
        public string account { get; set; }
        public string security { get; set; }
        public double exposure { get; set; }
        public string side { get; set; }
        public double price { get; set; }
        public int pips { get; set; }
        public double equity { get; set; }
        public double freemargin { get; set; }
        public double pl { get; set; }
    }

    public class accountingTick
    {
        public double strategyPL { get; set; }
        public double totalequity { get; set; }
        public double usedmargin { get; set; }
        public double freemargin { get; set; }
        public string m2mcurrency { get; set; }
    }

    public class positionHeartbeat
    {
        public List<string> asset { get; set; }
        public List<string> security { get; set; }
        public List<string> account { get; set; }
    }

    public class orderTick
    {
        public int tempid { get; set; }
        public string orderid { get; set; }
        public string fixid { get; set; }
        public string account { get; set; }
        public string tinterface { get; set; }
        public string security { get; set; }
        public int pips { get; set; }
        public int quantity { get; set; }
        public string side { get; set; }
        public string type { get; set; }
        public double limitprice { get; set; }
        public int maxshowquantity { get; set; }
        public string timeinforce { get; set; }
        public int seconds { get; set; }
        public int milliseconds { get; set; }
        public string expiration { get; set; }
        public double finishedprice { get; set; }
        public int finishedquantity { get; set; }
        public string commcurrency { get; set; }
        public double commission { get; set; }
        public double priceatstart { get; set; }
        public int userparam { get; set; }
        public string status { get; set; }
        public string reason { get; set; }
    }

    public class orderHeartbeat
    {
        public List<string> security { get; set; }
        public List<string> tinterface { get; set; }
    }

    public class orderRequest
    {
        public string security { get; set; }
        public string tinterface { get; set; }
        public int quantity { get; set; }
        public string side { get; set; }
        public string type { get; set; }
        public string timeinforce { get; set; }
        public double price { get; set; }
        public int expiration { get; set; }
        public int userparam { get; set; }
        public int tempid { get; set; }
        public string result { get; set; }
    }

    public class positionTick
    {
        public List<assetPositionTick> assetPositionTickList { get; set; }
        public List<securityPositionTick> securityPositionTickList { get; set; }
        public accountingTick accountingTick { get; set; }
    }

    public class cancelTick
    {
        public string fixid { get; set; }
        public string result { get; set; }
    }

    public class modOrder
    {
        public string fixid { get; set; }
        public double price { get; set; }
        public int quantity { get; set; }
    }

    public class modifyTick
    {
        public string fixid { get; set; }
        public string result { get; set; }
    }

    public class candleTick
    {
        public string security { get; set; }
        public string tinterface { get; set; }
        public int timestamp { get; set; }
        public string side { get; set; }
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }
        public int ticks { get; set; }
    }

    
    public ArthikaHFT(string domain, string url_stream, string url_polling, string url_challenge, string url_token, string user, string password, string authentication_port, string request_port, bool ssl, string ssl_cert)
    {
        this.domain = domain;
		this.url_stream = url_stream;
		this.url_polling = url_polling;
		this.url_challenge = url_challenge;
		this.url_token = url_token;
		this.user = user;
		this.password = password;
		this.authentication_port = authentication_port;
		this.request_port = request_port;
		this.ssl = ssl;
		this.ssl_cert = ssl_cert;
        streamReaderMap = new Dictionary<Thread, StreamReader>();
        ServicePointManager.DefaultConnectionLimit = 100;
    }

    public bool doAuthentication()
    {
        HttpWebRequest httpWebRequest;
        JavaScriptSerializer serializer;
        HttpWebResponse httpResponse;

        // download certificate
        certificate1 = null;
        if (ssl)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(ssl_cert, "ssl.cert");
            }
            certificate1 = new X509Certificate("ssl.cert");
        }

        // get challenge
        httpWebRequest = (HttpWebRequest)WebRequest.Create(domain + ":" + authentication_port + url_challenge);
        serializer = new JavaScriptSerializer();
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";
        if (ssl)
        {
            httpWebRequest.ClientCertificates.Add(certificate1);
        }
        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
            hftRequest request = new hftRequest();
            request.getAuthorizationChallenge = new getAuthorizationChallengeRequest(user);
            streamWriter.WriteLine(serializer.Serialize(request));
            //Console.WriteLine(serializer.Serialize(request));
        }
        try
        {
            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected response status: " + ex.Message);
            return false;
        }
        using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            try
            {
                challenge = (string)handleResponse(streamReader, false, null);
            }
            catch (SocketException ex) { Console.WriteLine(ex.Message); }
            catch (IOException ioex) { Console.WriteLine(ioex.Message); }
        }
        
        // create challenge response
        string res = challenge;
        char[] passwordArray = password.ToCharArray();
        foreach (byte passwordLetter in passwordArray)
        {
            int value = Convert.ToInt32(passwordLetter);
            string hexOutput = String.Format("{0:X}", value);
            res = res + hexOutput;
        }
        int NumberChars = res.Length;
        byte[] bytes = new byte[NumberChars / 2];
        for (int i = 0; i < NumberChars; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(res.Substring(i, 2), 16);
        }
        SHA1 sha = new SHA1CryptoServiceProvider();
        byte[] tokenArray = sha.ComputeHash(bytes);
        string challengeresp = BitConverter.ToString(tokenArray);
        challengeresp = challengeresp.Replace("-", "");

        // get token with challenge response
        httpWebRequest = (HttpWebRequest)WebRequest.Create(domain + ":" + authentication_port + url_token);
        serializer = new JavaScriptSerializer();
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";
        if (ssl)
        {
            httpWebRequest.ClientCertificates.Add(certificate1);
        }
        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
            hftRequest request = new hftRequest();
            request.getAuthorizationToken = new getAuthorizationTokenRequest(user, challengeresp);
            streamWriter.WriteLine(serializer.Serialize(request));
            //Console.WriteLine(serializer.Serialize(request));
        }
        try
        {
            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        }
        catch (Exception ex){
            Console.WriteLine("Unexpected response status: " + ex.Message);
            return false;
        }
        using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            try
            {
                token = (string)handleResponse(streamReader, false, null);
                return true;
            }
            catch (SocketException ex) { Console.WriteLine(ex.Message); return false; }
            catch (IOException ioex) { Console.WriteLine(ioex.Message); return false; }
        }
        
    }

    public List<accountTick> getAccount()
    {
        hftRequest hftrequest = new hftRequest();
        hftrequest.getAccount = new getAccountRequest(user, token);
        Object res = sendRequest(hftrequest, "/getAccount", false, null);
        return (List<accountTick>) res;
    }

    public List<tinterfaceTick> getInterface()
    {
        hftRequest hftrequest = new hftRequest();
        hftrequest.getInterface = new getInterfaceRequest(user, token);
        Object res = sendRequest(hftrequest, "/getInterface", false, null);
        return (List<tinterfaceTick>) res;
    }

    public List<priceTick> getPrice(List<string> securities, List<string> tinterfaces, string granularity, int levels)
    {
		hftRequest hftrequest = new hftRequest();
		hftrequest.getPrice = new getPriceRequest(user, token, securities, tinterfaces, granularity, levels, 0);
		Object res = sendRequest(hftrequest, "/getPrice", false, null);
		return (List<priceTick>) res;
	}

    public string getPriceBegin(List<string> securities, List<string> tinterfaces, string granularity, int levels, int interval, ArthikaHFTListener listener)
    {
		hftRequest hftrequest = new hftRequest();
		hftrequest.getPrice = new getPriceRequest(user, token, securities, tinterfaces, granularity, levels, interval);
		Object res = sendRequest(hftrequest, "/getPrice", true, listener);
        return (string) res;
	}

    public bool getPriceEnd(string threadname){
        return finishStreaming(threadname);
	}

    public positionTick getPosition(List<string> assets, List<string> securities, List<string> accounts)
    {
		hftRequest hftrequest = new hftRequest();
		hftrequest.getPosition = new getPositionRequest(user, token, assets, securities, accounts, 0); 
		Object res = sendRequest(hftrequest, "/getPosition", false, null);
		return (positionTick) res;
	}

    public string getPositionBegin(List<string> assets, List<string> securities, List<string> accounts, int interval, ArthikaHFTListener listener)
    {
        hftRequest hftrequest = new hftRequest();
        hftrequest.getPosition = new getPositionRequest(user, token, assets, securities, accounts, interval);
        Object res = sendRequest(hftrequest, "/getPosition", true, listener);
        return (string)res;
    }

    public bool getPositionEnd(string threadname)
    {
        return finishStreaming(threadname);
    }

    public List<orderTick> getOrder(List<string> securities, List<string> tinterfaces, List<string> types)
    {
        hftRequest hftrequest = new hftRequest();
        hftrequest.getOrder = new getOrderRequest(user, token, securities, tinterfaces, types, 0);
        Object res = sendRequest(hftrequest, "/getOrder", false, null);
        return (List<orderTick>)res;
    }

    public string getOrderBegin(List<string> securities, List<string> tinterfaces, List<string> types, int interval, ArthikaHFTListener listener)
    {
        hftRequest hftrequest = new hftRequest();
        hftrequest.getOrder = new getOrderRequest(user, token, securities, tinterfaces, types, interval);
        Object res = sendRequest(hftrequest, "/getOrder", true, listener);
        return (string)res;
    }

    public bool getOrderEnd(string threadname)
    {
        return finishStreaming(threadname);
    }

    public List<orderRequest> setOrder(List<orderRequest> orders)
    {
		hftRequest hftrequest = new hftRequest();
		hftrequest.setOrder = new setOrderRequest(user, token, orders);
        Object res = sendRequest(hftrequest, "/setOrder", false, null);
		return (List<orderRequest>) res;
	}

    public List<cancelTick> cancelOrder(List<string> orders)
    {
        hftRequest hftrequest = new hftRequest();
        hftrequest.cancelOrder = new cancelOrderRequest(user, token, orders);
        Object res = sendRequest(hftrequest, "/cancelOrder", false, null);
        return (List<cancelTick>) res;
    }

    public List<modifyTick> modifyOrder(List<modOrder> orders)
    {
        hftRequest hftrequest = new hftRequest();
        hftrequest.modifyOrder = new modifyOrderRequest(user, token, orders);
        Object res = sendRequest(hftrequest, "/modifyOrder", false, null);
        return (List<modifyTick>)res; ;
    }

    public List<candleTick> getHistoricalPrice(List<string> securities, List<string> tinterfaces, string granularity, string side, int number)
    {
        hftRequest hftrequest = new hftRequest();
        hftrequest.getHistoricalPrice = new getHistoricalPriceRequest(user, token, securities, tinterfaces, granularity, side, number);
        Object res = sendRequest(hftrequest, "/getHistoricalPrice", false, null);
        return (List<candleTick>) res;
    }

    private Object sendRequest(hftRequest hftrequest, string urlpath, bool stream, ArthikaHFTListener listener)
    {
        if (token == null)
        {
            return null;
        }

        int currentRequestId;
        lock (Lock)
        {
            currentRequestId = requestId;
            requestId++;
        }

        HttpWebRequest httpWebRequest;
        if (stream)
        {
            httpWebRequest = (HttpWebRequest)WebRequest.Create(domain + ":" + request_port + url_stream + urlpath);
        }
        else
        {
            httpWebRequest = (HttpWebRequest)WebRequest.Create(domain + ":" + request_port + url_polling + urlpath);
        }
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        serializer.RegisterConverters(new JavaScriptConverter[] { new NullPropertiesConverter() });
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";
        if (ssl)
        {
            httpWebRequest.ClientCertificates.Add(certificate1);
        }

   
        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
            streamWriter.WriteLine(serializer.Serialize(hftrequest));
            Console.WriteLine(serializer.Serialize(hftrequest));
        }
        
        HttpWebResponse httpResponse;
        try
        {
            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected response status: " + ex.Message);
            return null;
        }
        StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
        if (stream)
        {
            Thread t = new Thread(() => handleResponse(streamReader, true, listener));
            t.Name = currentRequestId.ToString();
            streamReaderMap.Add(t, streamReader);
            t.Start();
            Console.WriteLine("Running " + t.Name);
            return t.Name;
        }
        else
        {
            Object res = handleResponse(streamReader, false, null);
            streamReader.Close();
            return res;
        }
    }

    private Object handleResponse(StreamReader streamReader, bool stream, ArthikaHFTListener listener)
    {
        try
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            hftResponse response;
            String line;

            while ((line = streamReader.ReadLine()) != null)
            {
                try{
                    response = serializer.Deserialize<hftResponse>(line);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error reading: " + line);
                    return null;
                }

                if (response.getAuthorizationChallengeResponse != null)
                {
                    return response.getAuthorizationChallengeResponse.challenge;
                }

                if (response.getAuthorizationTokenResponse != null)
                {
                    return response.getAuthorizationTokenResponse.token;
                }

                if (response.getAccountResponse != null)
                {
                    List<accountTick> accountTickList = new List<accountTick>();
                    if (response.getAccountResponse.account != null)
                    {
                        foreach (accountTick tick in response.getAccountResponse.account)
                        {
                            accountTickList.Add(tick);
                        }
                    }
                    return accountTickList;
                }
                if (response.getInterfaceResponse != null)
                {
                    List<tinterfaceTick> tinterfaceTickList = new List<tinterfaceTick>();
                    if (response.getInterfaceResponse.tinterface != null)
                    {
                        foreach (tinterfaceTick tick in response.getInterfaceResponse.tinterface)
                        {
                            tinterfaceTickList.Add(tick);
                        }
                    }
                    return tinterfaceTickList;
                }

                if (response.getPriceResponse != null)
                {
                    if (stream)
                    {
                        if (response.getPriceResponse.timestamp != null)
                        {
                            listener.timestampEvent(response.getPriceResponse.timestamp);
                        }
                        if (response.getPriceResponse.tick != null)
                        {
                            listener.priceEvent(response.getPriceResponse.tick);
                        }
                        if (response.getPriceResponse.heartbeat != null)
                        {
                            listener.heartbeatEvent();
                        }
                        if (response.getPriceResponse.message != null)
                        {
                            listener.messageEvent(response.getPriceResponse.message);
                        }
                    }
                    else
                    {
                        List<priceTick> priceTickList = new List<priceTick>();
                        if (response.getPriceResponse.tick != null)
                        {
                            foreach (priceTick tick in response.getPriceResponse.tick)
                            {
                                priceTickList.Add(tick);
                            }
                        }
                        return priceTickList;
                    }
                }

                if (response.getPositionResponse!=null){
                    if(stream){
                        if (response.getPositionResponse.timestamp != null){
                            listener.timestampEvent(response.getPositionResponse.timestamp);
                        }
                        if (response.getPositionResponse.accounting != null)
                        {
                            listener.accountingEvent(response.getPositionResponse.accounting);
                        }
						if (response.getPositionResponse.assetposition!= null){
                            listener.assetPositionEvent(response.getPositionResponse.assetposition);
                        }
                        if (response.getPositionResponse.securityposition!= null){
                            listener.securityPositionEvent(response.getPositionResponse.securityposition);
                        }
						if (response.getPositionResponse.heartbeat!= null){
                            listener.positionHeartbeatEvent(response.getPositionResponse.heartbeat);
                        }
						if (response.getPositionResponse.message != null){
                            listener.messageEvent(response.getPositionResponse.message);
                        }
                    }
                    else{
                        positionTick positiontick = new positionTick();
                        List<assetPositionTick> assetPositionTickList = new List<assetPositionTick>();
                        List<securityPositionTick> securityPositionTickList = new List<securityPositionTick>();
                        if (response.getPositionResponse.accounting != null)
                        {
                            positiontick.accountingTick = response.getPositionResponse.accounting;
                        }
                        if (response.getPositionResponse.assetposition != null)
                        {
                            foreach (assetPositionTick tick in response.getPositionResponse.assetposition){
                                assetPositionTickList.Add(tick);
                            }
                        }
                        if (response.getPositionResponse.securityposition != null)
                        {
                            foreach (securityPositionTick tick in response.getPositionResponse.securityposition){
                                securityPositionTickList.Add(tick);
                            }
                        }
                        positiontick.assetPositionTickList = assetPositionTickList;
                        positiontick.securityPositionTickList = securityPositionTickList;
                        return positiontick;
                    }
                }

                if (response.getOrderResponse!=null){
                    if(stream){
                        if (response.getOrderResponse.timestamp != null){
                            listener.timestampEvent(response.getOrderResponse.timestamp);
                        }
                        if (response.getOrderResponse.order!= null){
                            listener.orderEvent(response.getOrderResponse.order);
                        }
                        if (response.getOrderResponse.heartbeat!= null){
                            listener.orderHeartbeatEvent(response.getOrderResponse.heartbeat);
                        }
                        if (response.getOrderResponse.message != null){
                            listener.messageEvent(response.getOrderResponse.message);
                        }
                    }
                    else{
                        List<orderTick> orderTickList = new List<orderTick>();
                        if (response.getOrderResponse.order != null){
                            foreach (orderTick tick in response.getOrderResponse.order){
                                orderTickList.Add(tick);
                            }
                        }
                        return orderTickList;
                    }
                }

                if (response.setOrderResponse != null)
                {
                    List<orderRequest> orderList = new List<orderRequest>();
                    if (response.setOrderResponse.timestamp != null)
                    {
                        //listener.timestampEvent(response.setOrderResponse.timestamp);
                    }
                    if (response.setOrderResponse.order != null)
                    {
                        foreach (orderRequest tick in response.setOrderResponse.order)
                        {
                            orderList.Add(tick);
                        }
                    }
                    if (response.setOrderResponse.message != null)
                    {
                        //listener.messageEvent(response.setOrderResponse.message);
                    }
                    return orderList;
                }

                if (response.cancelOrderResponse != null)
                {
                    List<cancelTick> cancelTickList = new List<cancelTick>();
                    if (response.cancelOrderResponse.timestamp != null)
                    {
                        //listener.timestampEvent(response.cancelOrderResponse.timestamp);
                    }
                    if (response.cancelOrderResponse.order != null)
                    {
                        foreach (cancelTick tick in response.cancelOrderResponse.order)
                        {
                            cancelTickList.Add(tick);
                        }
                    }
                    if (response.cancelOrderResponse.message != null)
                    {
                        //listener.messageEvent(response.cancelOrderResponse.message);
                    }
                    return cancelTickList;
                }

                if (response.modifyOrderResponse != null)
                {
                    List<modifyTick> modifyTickList = new List<modifyTick>();
                    if (response.modifyOrderResponse.timestamp != null)
                    {
                        //listener.timestampEvent(response.modifyOrderResponse.timestamp);
                    }
                    if (response.modifyOrderResponse.order != null)
                    {
                        foreach (modifyTick tick in response.modifyOrderResponse.order)
                        {
                            modifyTickList.Add(tick);
                        }
                    }
                    if (response.modifyOrderResponse.message != null)
                    {
                        //listener.messageEvent(response.modifyOrderResponse.message);
                    }
                    return modifyTickList;
                }

                if (response.getHistoricalPriceResponse != null)
                {
                    List<candleTick> candleTickList = new List<candleTick>();
                    if (response.getHistoricalPriceResponse.candle != null){
                        foreach (candleTick tick in response.getHistoricalPriceResponse.candle){
                            candleTickList.Add(tick);
                        }
                    }
                    if (response.getHistoricalPriceResponse.message != null){
                        //listener.messageEvent(response.getHistoricalPriceResponse.message);
                    }
                    return candleTickList;
                }

            }
            return null;
        }
        catch (WebException)
        {
            //Console.WriteLine(ex.Message);
            return null;
        }
        catch (SocketException)
        {
            //Console.WriteLine(ex.Message);
            return null;
        }
        catch (IOException)
        {
            //Console.WriteLine(ioex.Message);
            return null;
        }
    }

    private bool finishStreaming(string threadname)
    {
		Console.WriteLine("Ending " + threadname);
		Thread thread = null;
        StreamReader streamReader = null;
        bool found = false;
        foreach(KeyValuePair<Thread, StreamReader> entry in streamReaderMap)
        {
            thread = entry.Key;
            if (thread.Name.Equals(threadname))
            {
                found = true;
                streamReader = entry.Value;
                break;
            }
        }
        if (!found)
        {
            return false;
        }

        streamReader.Close();
        thread.Abort();
        streamReaderMap.Remove(thread);
		return true;
	}

    private class NullPropertiesConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var jsonExample = new Dictionary<string, object>();
            foreach (var prop in obj.GetType().GetProperties())
            {
                //check if decorated with ScriptIgnore attribute
                bool ignoreProp = prop.IsDefined(typeof(ScriptIgnoreAttribute), true);

                var value = prop.GetValue(obj, BindingFlags.Public, null, null, null);
                if (value != null && !ignoreProp)
                    jsonExample.Add(prop.Name, value);
            }

            return jsonExample;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return GetType().Assembly.GetTypes(); }
        }
    }

}
