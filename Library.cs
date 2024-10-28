using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GiantNumbersLibrary
{
    /// <summary>
    /// Класс для больших чисел (Пока поддержка только чисел без плавающей запятой и без минуса).
    /// Авторы: ItzKITb (https://github.com/CATONPRO), voxelll_ (https://github.com/voxelll1)
    /// </summary>
    public class GiantNumber
    {
        private string CurrentNumber = "";
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
        public static GiantNumber Sum(GiantNumber number1, GiantNumber number2)
        {
            GiantNumber longerNum = Utils.GetLongestNum(number1, number2);
            GiantNumber shorterNum = Utils.GetShortestNum(number1, number2);
            Dictionary<double, char> numMap1 = Utils.StringToCharList(longerNum.Get());
            Dictionary<double, char> numMap2 = Utils.StringToCharList(shorterNum.Get());

            string result = "";
            int transfer = 0;
            double currentNum = 0;
            foreach (char Char in numMap1.Values)
            {
                int resultDigit = int.Parse(Char.ToString()) + int.Parse(numMap2[currentNum].ToString()) + transfer;
                currentNum++;
                if (resultDigit >= 10)
                {
                    resultDigit -= 10;
                    transfer = 1;
                }
                else transfer = 0;
                if (currentNum != 1) result = resultDigit.ToString() + result;
            }

            if (transfer == 1) result = "1" + result;

            GiantNumber resultNumber = new GiantNumber(result);
            return resultNumber;
        }
        /// <summary>
        /// Вычитание чисел
        /// </summary>
        /// <param name="number1">Вычитаемое</param>
        /// <param name="number2">Вычитаемое</param>
        public static GiantNumber Subtract(GiantNumber number1, GiantNumber number2)
        {
            GiantNumber longerNum = Utils.GetLongestNum(number1, number2);
            GiantNumber shorterNum = Utils.GetShortestNum(number1, number2);
            Dictionary<double, char> numMap1 = Utils.StringToCharList(longerNum.Get());
            Dictionary<double, char> numMap2 = Utils.StringToCharList(shorterNum.Get());

            string result = "";
            int transfer = 0;
            double currentNum = 0;

            foreach (char Char in numMap1.Values)
            {
                int resultDigit = int.Parse(Char.ToString()) - int.Parse(numMap2[currentNum].ToString()) - transfer;
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
            bool NumIsReady = false;
            while (!NumIsReady)
            {
                if (("\n" + result).Contains("\n0")) result = result.Substring(1, result.Length - 1);
                else                                 NumIsReady = true;
            }

            if (result == "") result = "0";
            GiantNumber resultNumber = new GiantNumber(result);
            return resultNumber;
        }
        /// <summary>
        /// Учножение чисел
        /// </summary>
        /// <param name="num">Число</param>
        /// <param name="multiplier">Множитель</param>
        public static GiantNumber Multiply(GiantNumber num, GiantNumber multiplier)
        {
            GiantNumber operationNum = new GiantNumber("0");
            GiantNumber increment = new GiantNumber("1");
            GiantNumber res = new GiantNumber("0");

            while (operationNum.Get() != multiplier.Get())
            {
                operationNum = Sum(operationNum, increment);
                res = Sum(res, num);
            }
            return res;
        }
        /// <summary>
        /// Возведение числа в степень
        /// </summary>
        /// <param name="num">Число</param>
        /// <param name="pow">Степень</param>
        public static GiantNumber Pow(GiantNumber num, GiantNumber pow)
        {
            if (pow.Get() == "0") return new GiantNumber("1");
            if (pow.Get() == "1") return new GiantNumber(num.Get());

            int numberOfTasks = Environment.ProcessorCount;
            GiantNumber[] results = new GiantNumber[numberOfTasks];
            int exponent = int.Parse(pow.Get());
            int chunkSize = exponent / numberOfTasks;

            Parallel.For(0, numberOfTasks, i =>
            {
                int start = i * chunkSize;
                int end = (i == numberOfTasks - 1) ? exponent : (i + 1) * chunkSize;

                GiantNumber partialResult = new GiantNumber("1");
                for (int j = start; j < end; j++)
                {
                    partialResult = Multiply(partialResult, num);
                }
                results[i] = partialResult;
            });

            GiantNumber finalResult = new GiantNumber("1");
            foreach (var partial in results)
            {
                finalResult = Multiply(finalResult, partial);
            }

            return finalResult;
        }
        /// <summary>
        /// Получение квадратного корня числа
        /// </summary>
        /// <param name="num">Число</param>
        public static GiantNumber Sqrt(GiantNumber num)
        {
            if (num.Get() == "0") return new GiantNumber("0");
            if (num.Get() == "1") return new GiantNumber("1");
            GiantNumber guess = new GiantNumber(num.Get().Substring(0, num.Get().Length / 2));
            GiantNumber previousGuess;
            do
            {
                previousGuess = guess;
                GiantNumber temp = Divide(num, guess);
                guess = Average(guess, temp);
            } while (Subtract(guess, previousGuess).Get() != "0");

            return guess;
        }
        /// <summary>
        /// Получение среднего арифметического числа
        /// </summary>
        /// <param name="a">Число 1</param>
        /// <param name="b">Число 1</param>
        public static GiantNumber Average(GiantNumber a, GiantNumber b)
        {
            GiantNumber sum = Sum(a, b);
            return Divide(sum, new GiantNumber("2"));
        }
        /// <summary>
        /// Возведение числа в степень и получение остатка от деления
        /// </summary>
        /// <param name="baseNum">Число</param>
        /// <param name="exponent">Степень</param>
        /// <param name="modulus">Делимое</param>
        public static GiantNumber ModPow(GiantNumber baseNum, GiantNumber exponent, GiantNumber modulus)
        {
            if (modulus.Get() == "0") throw new DivideByZeroException("Модуль не может быть нулем!");
            GiantNumber result = new GiantNumber("1");
            GiantNumber baseMod = Mod(baseNum, modulus);
            while (exponent.Get() != "0")
            {
                if (IsOdd(exponent)) result = Mod(Multiply(result, baseMod), modulus);
                baseMod = Mod(Multiply(baseMod, baseMod), modulus);
                exponent = Divide(exponent, new GiantNumber("2"));
            }
            return result;
        }
        /// <summary>
        /// Получение остатка от деления
        /// </summary>
        /// <param name="dividend">Делимое</param>
        /// <param name="divisor">Делитель</param>
        public static GiantNumber Mod(GiantNumber dividend, GiantNumber divisor)
        {
            if (divisor.Get() == "0") throw new DivideByZeroException("Деление на ноль!");
            GiantNumber quotient = Divide(dividend, divisor);
            GiantNumber product = Multiply(quotient, divisor);
            return Subtract(dividend, product);
        }
        /// <summary>
        /// Четное ли число?
        /// </summary>
        /// <param name="num">Число</param>
        public static bool IsOdd(GiantNumber num)
        {
            Dictionary<double, char> charList = Utils.StringToCharList(num.Get());
            char lastDigit = charList.Last().Value;
            return lastDigit == '1' || lastDigit == '3' || lastDigit == '5' || lastDigit == '7' || lastDigit == '9';
        }
        /// <summary>
        /// Деление
        /// </summary>
        /// <param name="dividend">Делимое</param>
        /// <param name="divisor">Делитель</param>
        public static GiantNumber Divide(GiantNumber dividend, GiantNumber divisor)
        {
            if (divisor.Get() == "0")                 throw new DivideByZeroException("Деление на ноль!");
            if (dividend.Get() == "0")                return new GiantNumber("0");
            if (Utils.Compare(dividend, divisor) < 0) return new GiantNumber("0");

            bool IsReady = false;
            GiantNumber repeats = new GiantNumber("1");
            GiantNumber tmp = new GiantNumber(dividend.Get());
            var charList = Utils.StringToCharList(dividend.Get());

            while(!IsReady)
            {
                tmp = Subtract(tmp, divisor);
                if (Utils.Compare(tmp, divisor) < 0) IsReady = true;
                else repeats = Sum(repeats, new GiantNumber("1"));
            }

            return repeats;
        }
    }
    static class Utils
    {
        public static int Compare(GiantNumber number1, GiantNumber number2)
        {
            if (number1.GetLength() > number2.GetLength())      return 1;
            else if (number1.GetLength() < number2.GetLength()) return -1;
            return string.Compare(number1.Get(), number2.Get(), StringComparison.Ordinal);
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
        public static GiantNumber GetLongestNum(GiantNumber number1, GiantNumber number2)
        {
            GiantNumber result = new GiantNumber("0");
            if (number1.Get().Length == number2.Get().Length || number1.Get().Length > number2.Get().Length) result.Set(number1.Get());
            else result.Set(number2.Get());
            return result;
        }
        public static GiantNumber GetShortestNum(GiantNumber number1, GiantNumber number2)
        {
            GiantNumber result = new GiantNumber(number1.Get());
            GiantNumber longestNum = new GiantNumber(number2.Get());
            bool IsNeedAddZeros = true;
            if (TextLength(number1.Get()) >= TextLength(number2.Get()))
            {
                result.Set(number2.Get());
                longestNum.Set(number1.Get());
            }
            if (TextLength(number1.Get()) == TextLength(number2.Get())) IsNeedAddZeros = TextLength(number1.Get()) != TextLength(number2.Get());
            if (IsNeedAddZeros) while (TextLength(longestNum.Get()) > TextLength(result.Get())) result.Set("0" + result.Get());
            return result;
        }
        public static double TextLength(string text)
        {
            double lenght = 0;
            foreach (char c in text) if (lenght != double.MaxValue) lenght++;
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