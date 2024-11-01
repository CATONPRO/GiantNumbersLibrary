using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GiantNumbersLibrary
{
    /// <summary>
    /// Класс для больших чисел с упором на максимальный размер (Пока поддержка только чисел без плавающей запятой и/или минуса).
    /// Авторы: ItzKITb (https://github.com/CATONPRO), voxelll_ (https://github.com/voxelll1)
    /// </summary>
    public class GiantNumber
    {
        public string CurrentNumber = "";
        /// <summary>
        /// Создание числа
        /// </summary>
        /// <param name="number">Число в формате string (regex: [^1234567890])</param>
        public GiantNumber(string number)
        {
            Set(number);
        }
        /// <summary>
        /// Получение содержимого числа в формате string
        /// </summary>
        public string Get()
        {
            return CurrentNumber;
        }
        /// <summary>
        /// Установка содержимого числа в формате string
        /// </summary>
        /// <param name="number">Число в формате string (regex: [^1234567890])</param>
        public void Set(string number)
        {
            string pattern = @"[^1234567890]";
            string result = Regex.Replace(number, pattern, "");
            double resultLength = Utils.TextLength(result);
            if (resultLength == 0) result = "0";
            if (resultLength == double.MaxValue) throw new Exception("Слишком большое число!");
            CurrentNumber = result;
        }
        /// <summary>
        /// Получение длинны числа
        /// </summary>
        public double GetLength()
        {
            double lenght = 0;
            foreach (char c in CurrentNumber) if (lenght != double.MaxValue) lenght++;
            return lenght;
        }
    }
    public static class GiantNumbersOperations
    {
        /// <summary>
        /// Сложение чисел
        /// </summary>
        /// <param name="number1">Слогаемое</param>
        /// <param name="number2">Слогаемое</param>
        public static string Sum(string longerNum, string shorterNum)
        {
            Dictionary<double, char> numMap1 = Utils.StringToCharList(longerNum);
            Dictionary<double, char> numMap2 = Utils.StringToCharList(Utils.AddZeros(shorterNum, Utils.TextLength(longerNum)));

            string result = "";
            byte transfer = 0;
            double currentNum = 0;
            int resultDigit = 0;
            
            foreach (char Char in numMap1.Values)
            {
                resultDigit = int.Parse(Char.ToString()) + int.Parse(numMap2[currentNum].ToString()) + transfer;
                currentNum++;
                if (resultDigit >= 10)
                {
                    resultDigit -= 10;
                    transfer = 1;
                }
                else transfer = 0;
                if (currentNum != 1) result = resultDigit.ToString() + result;
            }

            if (transfer == 1) return "1" + result;

            return result;
        }
        /// <summary>
        /// Вычитание чисел
        /// </summary>
        /// <param name="number1">Вычитаемое</param>
        /// <param name="number2">Вычитаемое</param>
        public static string Subtract(string longerNum, string shorterNum, double currentNum = 0)
        {
            Dictionary<double, char> numMap1 = Utils.StringToCharList(longerNum);
            Dictionary<double, char> numMap2 = Utils.StringToCharList(Utils.AddZeros(shorterNum, Utils.TextLength(longerNum)));

            string result = "";
            int transfer = 0;
            int resultDigit = 0;

            foreach (char Char in numMap1.Values)
            {
                resultDigit = int.Parse(numMap1[currentNum].ToString()) - int.Parse(numMap2[currentNum].ToString()) - transfer;
                currentNum++;
                if (currentNum != 1)
                {
                    if (resultDigit < 0)
                    {
                        resultDigit += 10;
                        transfer = 1;
                    }
                    else transfer = 0;
                    result = resultDigit.ToString() + result;
                }
            }
            while(result[0] == '0' && Utils.TextLength(result) > 1)
                result = Utils.TakeSymbols(result, 1, Utils.TextLength(result));
            return result;
        }
        /// <summary>
        /// Учножение чисел
        /// </summary>
        /// <param name="num">Число</param>
        /// <param name="multiplier">Множитель</param>
        public static string Multiply(string num, string multiplier)
        {
            string iterationNum = "1";
            string res = num;

            while (iterationNum != multiplier)       
            {
                iterationNum = Sum(iterationNum, "1");
                res = Sum(res, num);
            }
            return res;
        }
        /// <summary>
        /// Возведение числа в степень
        /// </summary>
        /// <param name="num">Число</param>
        /// <param name="pow">Степень</param>
        public static string Pow(string num, string pow)
        {
            string iterationNum = "1";
            string res = num;

            while (iterationNum != pow)
            {
                iterationNum = Sum(iterationNum, "1");
                res = Multiply(res, num);
            }
            return res;
        }
        /// <summary>
        /// Получение квадратного корня числа
        /// </summary>
        /// <param name="num">Число</param>
        public static string Sqrt(string num)
        {
            string guess = "";
            string previousGuess;
            if (Mod(num, "2") == "0") guess = Utils.TakeSymbols(num, Utils.TextLength(num) / 2, Utils.TextLength(num));
            else guess = Utils.TakeSymbols(num, (Utils.TextLength(num) + 1) / 2, Utils.TextLength(num));
            
            do 
            {
                previousGuess = guess;
                guess = Average(guess, Divide(num, guess));
            } while (Subtract(guess, previousGuess) != "0");

            return guess;
        }
        /// <summary>
        /// специально оптимизированный метод Substract для mod с целью ускорения работы 
        /// </summary>
        public static string Subtract_Mod(string longerNum, string shorterNum)
        {
            Dictionary<double, char> numMap1 = Utils.StringToCharList(longerNum);
            Dictionary<double, char> numMap2 = Utils.StringToCharList(shorterNum);

            string res = "";
            double currentNum = 0;
            
            while (numMap2[currentNum] == 0) 
                res = numMap1[currentNum] + res;
                currentNum++;
            
            res = Subtract(Utils.TakeSymbols(longerNum, 0, currentNum), Utils.TakeSymbols(shorterNum, 0, currentNum)) + res;
            if (res[0] == '0')
                return Utils.TakeSymbols(res, 1, Utils.TextLength(res));
            return res;
        }
        /// <summary>
        /// Получение среднего арифметического числа
        /// </summary>
        /// <param name="a">Число 1</param>
        /// <param name="b">Число 2</param>
        public static string Average(string a, string b)
        {
            return Divide(Sum(Utils.GetLongestNum(a, b), Utils.GetShortestNum(a, b)), "2");
        }
        /// <summary>
        /// Возведение числа в степень и получение остатка от деления
        /// </summary>
        /// <param name="num">Число</param>
        /// <param name="pow">Степень</param>
        /// <param name="modulus">Делитель</param>
        public static string ModPow(string num, string pow, string modulus)
        {
            return Mod(Pow(num, pow), modulus);
        }
        /// <summary>
        /// Получение остатка от деления
        /// </summary>
        /// <param name="dividend">Делимое</param>
        /// <param name="divisor">Делитель</param>
        public static string Mod(string dividend, string divisor)
        {
            if (divisor == "0") throw new DivideByZeroException("Деление на ноль!");
            string subtrahend = "";
            string originSubtrahend = "";

            while (GiantNumbersComparing(divisor, "<", dividend))
                subtrahend = divisor;
                do
                {
                    subtrahend = subtrahend + "0";
                } while (Utils.TextLength(subtrahend) != Utils.TextLength(dividend) - 1);

                originSubtrahend = subtrahend;
                do
                {
                    subtrahend = Sum(subtrahend, originSubtrahend);
                } while (GiantNumbersComparing(dividend, ">", subtrahend));
                    
                dividend = Subtract_Mod(dividend, Subtract(subtrahend, originSubtrahend));

            return dividend;  
        }
        /// <summary>
        /// Четное ли число?
        /// </summary>
        /// <param name="num">Число</param>
        public static bool IsOdd(string num)
        {
            Dictionary<double, char> charList = Utils.StringToCharList(num);
            char lastDigit = charList.Last().Value;
            return lastDigit == '1' || lastDigit == '3' || lastDigit == '5' || lastDigit == '7' || lastDigit == '9';
        }
        /// <summary>
        /// Деление
        /// </summary>
        /// <param name="dividend">Делимое</param>
        /// <param name="divisor">Делитель</param>
        public static string Divide(string dividend, string divisor)
        {
            if (divisor == "0") throw new DivideByZeroException("Деление на ноль!");

            bool IsReady = false;
            string repeats = "1";
            string tmp = dividend;

            while(!IsReady)
            {
                tmp = Subtract(tmp, divisor);
                if (Utils.Compare(tmp, divisor) < 0) IsReady = true;
                else repeats = Sum(repeats, "1");
            }

            return repeats;
        }
        /// <summary> 
        /// определение наибольшего числа
        /// </summary>
        public static bool GiantNumbersComparing (string num1, string operationType, string num2)
        {
            Dictionary<double, char> number1 = Utils.StringToCharList(Utils.GetLongestNum(num1, num2));
            Dictionary<double, char> number2 = Utils.StringToCharList(Utils.GetShortestNum(num1, num2));

            if (Utils.TextLength(num1) > Utils.TextLength(num2)) return true;
            if (Utils.TextLength(num1) < Utils.TextLength(num2)) return false;
            
            double i = 0;
            switch (operationType)
            {
                case ">": 
                
                for (i = 0; i < Utils.TextLength(Utils.GetLongestNum(num1, num2)); i++)
                    if (int.Parse(number1[i].ToString()) < int.Parse(number2[i].ToString())) return false;
                    if (int.Parse(number1[i].ToString()) > int.Parse(number2[i].ToString())) return true;
                return false;

                case "<": 
                if (Utils.TextLength(num1) > Utils.TextLength(num2)) return false;
                if (Utils.TextLength(num1) < Utils.TextLength(num2)) return true;

                for (i = 0; i < Utils.TextLength(Utils.GetLongestNum(num1, num2)); i++)
                    if (int.Parse(number1[i].ToString()) < int.Parse(number2[i].ToString())) return true;
                    if (int.Parse(number1[i].ToString()) > int.Parse(number2[i].ToString())) return false;
                return false;

                default:
                return false;
            }
        }
    }
    static class Utils
    {
        public static string TakeSymbols (string source, double fromWhatDigit = 1, double toWhatDigit = 2)
        {
            Dictionary<double, char> str = StringToCharList(source);
            string res = "";
            for (double i = fromWhatDigit; i < toWhatDigit; i++)
                res = str[i] + res;
            return res;
        }
        public static string AddZeros (string num, double limit)
        {
            while (TextLength(num) < limit) num = "0" + num;
            return num;
        }
        public static int Compare(string number1, string number2)
        {
            if (TextLength(number1) > TextLength(number2))      return 1;
            else if (TextLength(number1) < TextLength(number2)) return -1;
            return string.Compare(number1, number2, StringComparison.Ordinal);
        }
        public static Dictionary<double, char> StringToCharList(string str)
        {
            double lenght = TextLength(str);
            Dictionary<double, char> map = ArrayAddEmpty(lenght - 1);
            foreach (char Char in str)
            {
                map[lenght] = Char;
                lenght--;
            }
            return map;
        }
        public static string GetLongestNum(string number1, string number2)
        {
            if (TextLength(number1) >= TextLength(number2)) return number1;
            else return number2;
            return number2;
        }
        public static string GetShortestNum(string number1, string number2)
        {
            string result = number1;
            string longestNum = number2;
            
            if (TextLength(number1) >= TextLength(number2))
            {
                result = number2;
                longestNum = number1;
            }
            while (TextLength(longestNum) > TextLength(result)) result = ("0" + result);
            return result;
        }
        public static double TextLength(string text)
        {
            double lenght = 0;
            foreach (char c in text)lenght++;
            return lenght;
        }
        public static double ArrayLength(Dictionary<double, char> dict)
        {
            double lenght = 0;
            foreach (var c in dict) lenght++;
            return lenght;
        }
        public static Dictionary<double, char> ArrayAddEmpty(double newSize)
        {
            Dictionary<double, char> emptyArray = new Dictionary<double, char>();
            for (double num = 0; num <= newSize; num++) emptyArray.Add(num, '0');
            return emptyArray;
        }
    }
}
