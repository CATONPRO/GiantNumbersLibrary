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
            foreach (char c in CurrentNumber) lenght++;
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
        public static string Sum(string longerNum, string shorterNum, double currentNum = 0)
        {
            Dictionary<double, char> numMap1 = Utils.StringToCharList(longerNum);
            Dictionary<double, char> numMap2 = Utils.StringToCharList(Utils.AddZeros(shorterNum, Utils.TextLength(longerNum)));

            string result = "";
            byte transfer = 0;
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
            if (IsBigger(shorterNum, longerNum)) return "-1";
    
            Dictionary<double, char> numMap1 = Utils.StringToCharList(longerNum);
            Dictionary<double, char> numMap2 = Utils.StringToCharList(Utils.AddZeros(shorterNum, Utils.TextLength(longerNum)));

            string result = "";
            byte transfer = 0;
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
            
            if (transfer == 1) result = "1" + result;
            while (result[0] == '0' && Utils.TextLength(result) > 1) result = Utils.TakeSymbols(result, 1, Utils.TextLength(result));
            return result;
        }
        /// <summary>
        /// Умножение чисел
        /// </summary>
        /// <param name="number">Число</param>
        /// <param name="multiplier">Множитель</param>
        public static string Multiply(string num1, string num2)
        {
            string number = Utils.GetLongestNum(num1, num2);
            string multiplier = Utils.GetShortestNum(num1, num2);
            string res = "0";
            string tmp = "";
            string increment = "";
            int iterationCounter;
            Dictionary<double, char> multiplier_dict;
            if (number == num1) multiplier_dict = Utils.StringToCharList(num2);
            else multiplier_dict = Utils.StringToCharList(num1);
            // i определяет индекс цифры в множителе в виде Dictionary<double, char>, но из-за того что 
            // в начале главного цикла первым делом происходит вычитание единицы из i, то i составляет длину
            // multiplier_dict, а не максимальный индекс в нем
            double i = Utils.ArrayLength(multiplier_dict);
            
            // из-за нюансов метода Sum первый аргумент должен быть длиннее второго,
            // что позволяет сделать опциональной проверку длин аргументов,
            // поэтому первая итерация цикла вне его основной конструкции
            // т. к. изначально длина res = 1 а длина tmp не может быть < 1
            // что позволяет убрать повторяющуюся проверку в главном цикле
            i--;
            tmp = number;
            increment = "1";
            iterationCounter = 0;

            for (double I = 1; I != i; I++)
            {
                tmp += "0";
                increment += "0";
            }

            while (iterationCounter.ToString() != multiplier_dict[i].ToString())
            {
                iterationCounter++;
                res = Sum(Utils.GetLongestNum(tmp, res), Utils.GetShortestNum(tmp, res));
            }

            while (i != 1)       
            {
                i--;
                tmp = number;
                increment = "1";
                iterationCounter = 0;

                for (double I = 1; I != i; I++)
                {
                    tmp += "0";
                    increment += "0";
                }

                while (iterationCounter.ToString() != multiplier_dict[i].ToString())
                {
                    iterationCounter++;
                    res = Sum(res, tmp);
                }
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
            string res = num;

            while (pow != "1")
            {
                pow = Subtract(pow, "1");
                res = Multiply_Light(res, num);
            }
            return res;
        }
        /// <summary>
        /// Умножение чисел с упором на большое количество небольших операций
        /// </summary>
        /// <param name="num">Число</param>
        /// <param name="multiplier">Множитель</param>
        public static string Multiply_Light(string num, string multiplier)
        {
            string iterationNum_Multiply = "1";
            string res_Multiply = num;

            while (iterationNum_Multiply != multiplier)       
            {
                iterationNum_Multiply = Sum(iterationNum_Multiply, "1");
                res_Multiply = Sum(res_Multiply, num);
            }
            return res_Multiply;
        }
        /// <summary>
        /// Получение квадратного корня числа
        /// </summary>
        /// <param name="num">Число</param>
        public static string Sqrt(string num)
        {
            string res = "3";
            
            while (Multiply(res, res) != num) res = Average(res, Divide(num, res));

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
            string res = "1";
            num = Mod(num, modulus);

            while (pow != "0")
            {
                if (!IsEven(pow)) res = Mod(Multiply(res, num), modulus);

                num = Mod(Multiply(num, num), modulus);
                if (!IsEven(pow)) pow = Subtract(pow, "1");
                pow = Divide(pow, "2");
            }

            return res;
        }

        /// <summary>
        /// Четное ли число?
        /// </summary>
        /// <param name="num">Число</param>
        public static bool IsEven(string num)
        {
            char lastDigit = Utils.StringToCharList(num).Last().Value;
            return lastDigit == '2' || lastDigit == '4' || lastDigit == '6' || lastDigit == '8' || lastDigit == '0';
        }
        /// <summary>
        /// Деление 
        /// </summary>
        /// <param name="dividend">Делимое</param>
        /// <param name="divisor">Делитель</param>
        // деление грубо отбрасывает нецелые числа, уходит в бесконечны цикл
        public static string Divide(string dividend, string divisor)
        {
            if (divisor == "0") throw new DivideByZeroException("Деление на ноль!");
    
            string repeats = "0";
            string subtrahend = divisor;
            string increment_rep = "1";

            // первая итерация вынесена по той причине что и в Myltiply
            while (Utils.TextLength(subtrahend) + 1 < Utils.TextLength(dividend) && dividend != "1"
            && dividend != "2" && dividend != "3" && dividend != "4" && dividend != "5" && dividend != "6"
            && dividend != "7" && dividend != "9")
            {
                subtrahend += "0";
                increment_rep += "0";
            }

            if (IsBiggerOrEqual(dividend, subtrahend))
            {
                repeats = Sum(increment_rep, repeats);
                dividend = Subtract(dividend, subtrahend);
            }

            while (IsBiggerOrEqual(dividend, subtrahend))
            {
                repeats = Sum(repeats, increment_rep);
                dividend = Subtract(dividend, subtrahend);
            }

            while (IsBiggerOrEqual(dividend, divisor))
            { 
                increment_rep = "1";
                subtrahend = divisor;

                // такая проверка быстрее чем вызов расчета длины dividend, ведь dividend может оказаться длиной в сотни символов и
                // Utils.TextLength(dividend) вернет очень большое и бесполезное значение 
                while (Utils.TextLength(subtrahend) + 1 < Utils.TextLength(dividend) && dividend != "1"
                && dividend != "2" && dividend != "3" && dividend != "4" && dividend != "5" && dividend != "6"
                && dividend != "7" && dividend != "9") 
                {
                    subtrahend += "0";
                    increment_rep += "0";
                }

                while (IsBiggerOrEqual(dividend, subtrahend))
                {
                    repeats = Sum(repeats, increment_rep);
                    dividend = Subtract(dividend, subtrahend);
                }
            }
            return repeats;
        }

        /// <summary>
        /// нахождение остатка от деления
        /// </summary>
        /// <param name="dividend"> число </param>
        /// <param name="divisor"> модуль </param>
        public static string Mod(string dividend, string divisor)
        {
            string subtrahend = "";
            
            while(IsBiggerOrEqual(dividend, divisor))   
            {     
                subtrahend = divisor;
                while (Utils.TextLength(dividend) - 1 > Utils.TextLength(subtrahend)) subtrahend += "0";

                while (IsBiggerOrEqual(dividend, subtrahend)) dividend = Subtract(dividend, subtrahend);
            }

            return dividend;
        }

        /// <summary> 
        /// больше ли одно число другого
        /// </summary>
        public static bool IsBigger (string num1, string num2)
        {    
            Dictionary<double, char> number1 = Utils.StringToCharList(Utils.GetShortestNum(num1, num2));
            Dictionary<double, char> number2 = Utils.StringToCharList(Utils.GetLongestNum(num1, num2));

            if (Utils.TextLength(num1) > Utils.TextLength(num2)) return true;
            if (Utils.TextLength(num1) < Utils.TextLength(num2)) return false;
                
            for (double i = Utils.TextLength(num1); i != 0; i--)
            {
                if (int.Parse(number1[i].ToString()) > int.Parse(number2[i].ToString())) return true;
                if (int.Parse(number1[i].ToString()) < int.Parse(number2[i].ToString())) return false;
            }

            return false;
        }

        /// <summary> 
        /// больше или равно одно другому
        /// </summary>
        public static bool IsBiggerOrEqual (string num1, string num2)
        {    
            Dictionary<double, char> number1 = Utils.StringToCharList(Utils.GetShortestNum(num1, num2));
            Dictionary<double, char> number2 = Utils.StringToCharList(Utils.GetLongestNum(num1, num2));

            if (Utils.TextLength(num1) > Utils.TextLength(num2)) return true;
            if (Utils.TextLength(num1) < Utils.TextLength(num2)) return false;
                
            for (double i = Utils.TextLength(num1); i != 0; i--)
            {
                if (int.Parse(number1[i].ToString()) > int.Parse(number2[i].ToString())) return true;
                if (int.Parse(number1[i].ToString()) < int.Parse(number2[i].ToString())) return false;
            }

            return true;
        }
    }
    static class Utils
    {
        public static string TakeSymbols (string source, double fromWhatDigit = 0, double toWhatDigit = 1)
        {
            Dictionary<double, char> str = StringToCharList(source);
            string res = "";
            for (double i = fromWhatDigit; i < toWhatDigit; i++) res = str[i] + res;
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
            if (TextLength(number1) > TextLength(number2)) return number1;
            return number2;
        }
        public static string GetShortestNum(string number1, string number2)
        {
            string result = number1;
            string longestNum = number2;
            
            if (TextLength(number1) > TextLength(number2))
            {
                result = number2;
                longestNum = number1;
            }
            while (TextLength(longestNum) > TextLength(result)) result = "0" + result;
            return result;
        }
        public static double TextLength(string text)
        {
            double lenght = 0;
            foreach (char c in text) lenght++;
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
