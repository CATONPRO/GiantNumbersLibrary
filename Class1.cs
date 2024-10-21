using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GiantNumbersLibrary
{
    public class GiantNumber
    {
        private string currentNumber = "";
        public GiantNumber(string number)
        {
            Set(number);
        }
        public string Get()
        {
            return currentNumber;
        }
        public void Set(string number)
        {
            string pattern = @"[^-1234567890]";
            string result = Regex.Replace(number, pattern, "");
            ulong resultLength = GiantNumbersOperations.TextLength(result);
            if (resultLength == 0) throw new Exception("No number specified!");
            if (resultLength == ulong.MaxValue) throw new Exception("Too big a number!");
            currentNumber = result;
        }
    }
    public static class GiantNumbersOperations
    {
        public static GiantNumber Plus(GiantNumber number1, GiantNumber number2)
        {
            GiantNumber longerNum = new GiantNumber("0");
            GiantNumber shorterNum = new GiantNumber("0");

            if (TextLength(number1.Get()) > TextLength(number2.Get()))
            {
                longerNum = number1;
                shorterNum = number2;
            }
            else
            {
                longerNum = number2;
                shorterNum = number1;
            }

            Dictionary<ulong, string> longNum = new Dictionary<ulong, string>();
            Dictionary<ulong, string> shortNum = new Dictionary<ulong, string>();

            ulong longLength = TextLength(longerNum.Get()) - TextLength(shorterNum.Get()) -1;
            string zeros = "";
            for (ulong i = 0; i <= longLength; i++)
            {
                zeros = "0" + zeros;
            }
            shorterNum.Set(zeros + shorterNum.Get());

            // Console.WriteLine("Long num: " + longerNum.Get());
            // Console.WriteLine("Short num: " + shorterNum.Get());

            

            return longerNum;
        }

        public static ulong TextLength(string text)
        {
            ulong lenght = 0;
            foreach (char c in text)
            {
                if (lenght != ulong.MaxValue)
                {
                    lenght++;
                }
            }
            return lenght;
        }
    }
}