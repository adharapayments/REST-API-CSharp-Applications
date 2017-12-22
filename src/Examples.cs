using System;
using System.Linq;

class Examples
{

    static void Main(string[] args)
    {
        int sel = -1;
        if (args != null && args.Count() > 0)
        {
            if (!Int32.TryParse(args[0], out sel))
            {
                sel = -1;
            }
        }
        switch (sel)
        {
            case 0:
                Console.WriteLine("Example 0: GET ACCOUNTS AND TINTERFACES");
                Example0.Main0(args);
                break;
            case 1:
                Console.WriteLine("Example 1: PRICE STREAMING");
                Example1.Main1(args);
                break;
            case 2:
                Console.WriteLine("Example 2: PRICE POLLING");
                Example2.Main2(args);
                break;
            case 3:
                Console.WriteLine("Example 3: POSITION STREAMING");
                Example3.Main3(args);
                break;
            case 4:
                Console.WriteLine("Example 4: POSITION POLLING");
                Example4.Main4(args);
                break;
            case 5:
                Console.WriteLine("Example 5: ORDER STREAMING");
                Example5.Main5(args);
                break;
            case 6:
                Console.WriteLine("Example 6: ORDER POLLING");
                Example6.Main6(args);
                break;
            case 7:
                Console.WriteLine("Example 7: ORDER CREATION");
                Example7.Main7(args);
                break;
            case 8:
                Console.WriteLine("Example 8: CANCEL PENDING ORDER WITH ORDER POLLING");
                Example8.Main8(args);
                break;
            case 9:
                Console.WriteLine("Example 9: MODIFY PENDING ORDER WITH ORDER POLLING");
                Example9.Main9(args);
                break;
            case 10:
                Console.WriteLine("Example 10: CANCEL PENDING ORDER WITH ORDER STREAMING");
                Example10.Main10(args);
                break;
            case 11:
                Console.WriteLine("Example 11: MODIFY PENDING ORDER WITH ORDER STREAMING");
                Example11.Main11(args);
                break;
            case 12:
                Console.WriteLine("Example 12: STRATEGY");
                Example12.Main12(args);
                break;
            case 13:
                Console.WriteLine("Example 13: MULTIPLE ORDER CREATION");
                Example13.Main13(args);
                break;
            case 14:
                Console.WriteLine("Example 14: GET HISTORICAL PRICE");
                Example14.Main14(args);
                break;
            default:
                Console.WriteLine("Choose option: ");
                Console.WriteLine("-1: EXIT");
                Console.WriteLine(" 0: GET ACCOUNTS AND TINTERFACES");
                Console.WriteLine(" 1: PRICE STREAMING");
                Console.WriteLine(" 2: PRICE POLLING");
                Console.WriteLine(" 3: POSITION STREAMING");
                Console.WriteLine(" 4: POSITION POLLING");
                Console.WriteLine(" 5: ORDER STREAMING");
                Console.WriteLine(" 6: ORDER POLLING");
                Console.WriteLine(" 7: ORDER CREATION");
                Console.WriteLine(" 8: CANCEL PENDING ORDER WITH ORDER POLLING");
                Console.WriteLine(" 9: MODIFY PENDING ORDER WITH ORDER POLLING");
                Console.WriteLine("10: CANCEL PENDING ORDER WITH ORDER STREAMING");
                Console.WriteLine("11: MODIFY PENDING ORDER WITH ORDER STREAMING");
                Console.WriteLine("12: STRATEGY");
                Console.WriteLine("13: MULTIPLE ORDER CREATION");
                Console.WriteLine("14: GET HISTORICAL PRICE");
                string input = Console.ReadLine();
                if (input.Equals("-1"))
                {
                    Console.WriteLine("Exit");
                    return;
                }
                string[] newargs = new string[1];
                newargs[0] = input;
                Main(newargs);
                break;
        }
    }

}
