# REST API C# Applications
This repository contains complete Application Examples using our C# REST API wrapper

* Example0: GET ACCOUNTS AND TINTERFACES
* Example1: PRICE STREAMING
* Example2: PRICE POLLING
* Example3: POSITION STREAMING
* Example4: POSITION POLLING
* Example5: ORDER STREAMING
* Example6: ORDER POLLING
* Example7: ORDER CREATION
* Example8: CANCEL PENDING ORDER WITH ORDER POLLING
* Example9: MODIFY PENDING ORDER WITH ORDER POLLING
* Example10: CANCEL PENDING ORDER WITH ORDER STREAMING
* Example11: MODIFY PENDING ORDER WITH ORDER STREAMING
* Example12: STRATEGY
* Example13: MULTIPLE ORDER CREATION
* Example14: GET HISTORICAL PRICE

### Pre-requisites:
Will users need previous registration, account, strategy set-up...? After all, these examples require a pre-existing strategy
Visual Studio

### How to:

**1. Clone this repository to the location of your choice** 

The repository contains the wrapper and all the examples listed above together with the classes needed. 

**2. Modify config.properties file with your settings** 

```
domain=http://demo.arthikatrading.com
user=demo
password=demo
```

**3. Modify the following lines in the Java program you would like to run.** 

From here on we will assume it is Example1.cs.
```
string id1 = wrapper.getPriceBegin(new List<string> { "GBP/USD" }, null, ArthikaHFT.GRANULARITY_TOB, 1, interval, new ArthikaHFTListenerImp1());
```

In case you want to disable ssl protocol, change the following line:
```
private static bool ssl = true;
```

**4. Generate executable file.**


**5. Run executable file.**
```javascript
$ REST-API-CSharp-Examples

Choose option: 
-1: EXIT
 0: GET ACCOUNTS AND TINTERFACES
 1: PRICE STREAMING
 2: PRICE POLLING
 3: POSITION STREAMING
 4: POSITION POLLING
 5: ORDER STREAMING
 6: ORDER POLLING
 7: ORDER CREATION
 8: CANCEL PENDING ORDER WITH ORDER POLLING
 9: MODIFY PENDING ORDER WITH ORDER POLLING
10: CANCEL PENDING ORDER WITH ORDER STREAMING
11: MODIFY PENDING ORDER WITH ORDER STREAMING
12: STRATEGY
13: MULTIPLE ORDER CREATION
14: GET HISTORICAL PRICE
```

or

```javascript
$ REST-API-CSharp-Examples 1
PriceStreaming
Response timestamp: 1445961714.977141 Contents:
Security: GBP/USD Price: 1.5312 Side: ask Liquidity: 10000000
Security: GBP/USD Price: 1.5312 Side: bid Liquidity: 10000000
Security: GBP/USD Price: 1.5312 Side: ask Liquidity: 10000000
Security: GBP/USD Price: 1.53129 Side: bid Liquidity: 2000000
Response timestamp: 1445961715.334755 Contents:
Security: GBP/USD Price: 1.5312 Side: ask Liquidity: 10000000
Security: GBP/USD Price: 1.5312 Side: bid Liquidity: 10000000
Security: GBP/USD Price: 1.5312 Side: ask Liquidity: 10000000
Security: GBP/USD Price: 1.53134 Side: bid Liquidity: 500000
```
