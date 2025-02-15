using System.Text.RegularExpressions;

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
        public static string Sum(string longerNum, string shorterNum)
        {
            Dictionary<double, char> numMap1 = Utils.StringToCharList(longerNum);
            Dictionary<double, char> numMap2 = Utils.StringToCharList(Utils.AddZeros_Beginning(shorterNum, Utils.TextLength(longerNum)));

            double currentNum = 1;
            string result = "";
            byte transfer = 0;
            int resultDigit = 0;
            double numMap1Length = Utils.ArrayLength(numMap1);
            double numMap2Length = Utils.ArrayLength(numMap2);

            while (currentNum != numMap2Length)
            {
                resultDigit = int.Parse(numMap1[currentNum].ToString()) + int.Parse(numMap2[currentNum].ToString()) + transfer;
                currentNum++;
                if (resultDigit >= 10)
                {
                    resultDigit -= 10;
                    transfer = 1;
                }
                else transfer = 0;
                result = resultDigit.ToString() + result;
            }

            while (currentNum != numMap1Length)
            {
                result += numMap1[currentNum];
                currentNum++;
            }

            if (transfer == 1) return "1" + result;

            return result;
        }
        /// <summary>
        /// Вычитание чисел
        /// </summary>
        /// <param name="number1">Вычитаемое</param>
        /// <param name="number2">Вычитаемое</param>
         public static string Subtract(string longerNum, string shorterNum)
         {
            // можно раскоментить и тогда будет чуть медленнее,
            // но оно будет понимать, что одно число больше другого
            //  if (IsBigger(shorterNum, longerNum)) return "-1";

            Dictionary<double, char> numMap1 = Utils.StringToCharList(longerNum);
            Dictionary<double, char> numMap2 = Utils.StringToCharList(Utils.AddZeros_Beginning(shorterNum, Utils.TextLength(longerNum)));

            string result = "";
            int transfer = 0;
            int resultDigit = 0;
            double currentNum = 1;

            try
            {
                while (true)
                {
                    resultDigit = int.Parse(numMap1[currentNum].ToString()) - int.Parse(numMap2[currentNum].ToString()) - transfer;
                    currentNum++;
                    if (resultDigit < 0)
                    {
                        resultDigit += 10;
                        transfer = 1;
                    }
                    else transfer = 0;
                    result = resultDigit.ToString() + result;
                }

            } catch
            {
                if (transfer == 1) result = "1" + result;
                try
                {
                    while (result[0] == '0') result = ("q" + result).Replace("q0", "");
                }
                catch
                {
                    return "0";
                }
            
                return result;
            }
        }
        /// <summary>
        /// Умножение чисел
        /// </summary>
        /// <param name="number">Число</param>
        /// <param name="multiplier">Множитель</param>
        public static string Multiply(string num1, string num2)
        {
            string res = "";
            double num1Length = Utils.TextLength(num1);
            double num2Length = Utils.TextLength(num2);

            if (num2Length <= 8 || num1Length <= 8 || Math.Abs(num1Length - num2Length) != 0)
            {
                return MultiplyClassic(num1, num2);
            }

            double n = Math.Max(num1Length, num2Length);
            double m = Math.Floor(Math.Min(num1Length, num2Length) / 2);
            string a = Utils.TakeSymbols(num1, 0, Math.Ceiling(num1Length / 2));
            string b = Utils.TakeSymbols(num1, Math.Ceiling(num1Length / 2), num1Length);
            string c = Utils.TakeSymbols(num2, 0, Math.Ceiling(num2Length / 2));
            string d = Utils.TakeSymbols(num2, Math.Ceiling(num2Length / 2), num2Length);

            Console.WriteLine($"{a} | {b} \n{c} | {d}");
            string ac = Multiply(a, c);
            string bd = Multiply(b, d);
            string tmp1 = Sum(a, b);
            string tmp2 = Sum(c, d);
            string ab_cd = Subtract
                           (
                            Multiply
                            (
                                Utils.GetLongestNum(tmp1, tmp2),
                                Utils.GetShortestNum(tmp1, tmp2)
                            ),
                            Sum
                            (
                                Utils.GetLongestNum(ac, bd),
                                Utils.GetShortestNum(ac, bd)
                            )
                           );

            res = Sum
                    (
                       Utils.AddZeros_Ending
                       (
                           ac,
                           2 * m
                       ),
                       Sum
                       (
                           Utils.AddZeros_Ending
                           (
                               ab_cd,
                               m
                           ),
                           bd
                       )
                    );
            Console.WriteLine($"num1 = {num1} | num2 = {num2}\nz0 = {a} * {c} = {ac}\nz2 = {b} * {d} = {bd}\nz1 = ({a} + {b}) * ({c} + {d}) - {bd} - {ac} = {ab_cd}\nm = {m}\nres = {res}\n-----------------------------");

            return res;
        }

        public static string MultiplyClassic(string num1, string num2)
        {
            if (num1 == "0" || num2 == "0") return "0";

            string number = Utils.GetLongestNum(num1, num2); 
            string multiplier = Utils.GetShortestNum(num1, num2); 
            string res = "0";
            string tmp = number;
            string decrement = "1";
            double multiplerLength = Utils.TextLength(multiplier);

            for (double i = 1; i < multiplerLength; i++)
            {
                tmp += "0";
                decrement += "0";
            }

            multiplier = Subtract(multiplier, decrement);
            res = Sum(tmp, res);

            while (multiplier != "0")
            {
                tmp = number;
                decrement = "1";
                multiplerLength = Utils.TextLength(multiplier);

                for (double i = 1; i < multiplerLength; i++)
                {
                    tmp += "0";
                    decrement += "0";
                }

                multiplier = Subtract(multiplier, decrement);
                res = Sum(res, tmp);
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
            if (pow == "0") return "1";
            if (pow == "1") return num;

            string res = num;

            while (pow != "1")
            {
                pow = Subtract(pow, "1");
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
            string res = "";
            string oldRes = "";
            double numFirstTwoDigitsToInt = Convert.ToInt32(num[0].ToString() + num[1].ToString());
            double approximateResLength = Utils.TextLength(num) / 2;

            if (approximateResLength % 1 == 0)
            {
                if (numFirstTwoDigitsToInt <= 12) res = "3";
                else if (numFirstTwoDigitsToInt <= 20) res = "4";
                else if (numFirstTwoDigitsToInt <= 30) res = "5";
                else if (numFirstTwoDigitsToInt <= 42) res = "6";
                else if (numFirstTwoDigitsToInt <= 56) res = "7";
                else if (numFirstTwoDigitsToInt <= 72) res = "8";
                else res = "9";
            }
            else
            {
                if (numFirstTwoDigitsToInt <= 11) res = "11";
                else if (numFirstTwoDigitsToInt <= 15) res = "12";
                else if (numFirstTwoDigitsToInt <= 18) res = "13";
                else if (numFirstTwoDigitsToInt <= 21) res = "14";
                else if (numFirstTwoDigitsToInt <= 24) res = "15";
                else if (numFirstTwoDigitsToInt <= 27) res = "16";
                else if (numFirstTwoDigitsToInt <= 31) res = "17";
                else if (numFirstTwoDigitsToInt <= 34) res = "18";
                else if (numFirstTwoDigitsToInt <= 38) res = "19";
                else if (numFirstTwoDigitsToInt <= 42) res = "20";
                else if (numFirstTwoDigitsToInt <= 46) res = "21";
                else if (numFirstTwoDigitsToInt <= 51) res = "22";
                else if (numFirstTwoDigitsToInt <= 55) res = "23";
                else if (numFirstTwoDigitsToInt <= 60) res = "24";
                else if (numFirstTwoDigitsToInt <= 65) res = "25";
                else if (numFirstTwoDigitsToInt <= 70) res = "26";
                else if (numFirstTwoDigitsToInt <= 76) res = "27";
                else if (numFirstTwoDigitsToInt <= 81) res = "28";
                else if (numFirstTwoDigitsToInt <= 87) res = "29";
                else if (numFirstTwoDigitsToInt <= 93) res = "30";
                else res = "31";
                approximateResLength--;
            }
            
            for (double i = 1; i < approximateResLength; i++) res += "0";

            while (oldRes != res)
            {
                Console.WriteLine(res);
                oldRes = res;
                res = Average(res, Divide(num, res));
            }

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
            if (modulus == "0") throw new DivideByZeroException("модуль не может быть нулём");
            if (modulus == "1") return "0";

            string res = "1";

            num = Mod(num, modulus);

            while (pow != "0")
            {
                if (!IsEven(pow)) res = Mod(Multiply(res, num), modulus);
                pow = Divide(pow, "2");
                num = Mod(Multiply(num, num), modulus);
            }
            
            return res;
        }

        /// <summary>
        /// Четное ли число?
        /// </summary>
        /// <param name="num">Число</param>
        public static bool IsEven(string num)
        {
            char lastDigit = Utils.StringToCharList(num)[1];

            return (lastDigit == '2' || lastDigit == '4' || lastDigit == '6' || lastDigit == '8' || lastDigit == '0');
        }
        /// <summary>
        /// Деление 
        /// </summary>
        /// <param name="dividend">Делимое</param>
        /// <param name="divisor">Делитель</param>
        public static string Divide(string dividend, string divisor)
        {
            if (divisor == "0") throw new DivideByZeroException("Деление на ноль!");
    
            string repeats = "0";
            string subtrahend = divisor;
            string increment_rep = "1";
            double subtrahendLengthPlusOne = Utils.TextLength(subtrahend) + 1;
            double dividendLength = Utils.TextLength(dividend);

            // первая итерация вынесена по той причине что и в Myltiply
            while (subtrahendLengthPlusOne < dividendLength)
            {
                subtrahendLengthPlusOne++;
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
                dividendLength = Utils.TextLength(dividend);
                subtrahendLengthPlusOne = Utils.TextLength(subtrahend) + 1;

                while (subtrahendLengthPlusOne < dividendLength)
                {
                    subtrahendLengthPlusOne++;
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
            double dividendLengthMinusOne, subtrahendLength;

            while(IsBiggerOrEqual(dividend, divisor))   
            {     
                subtrahend = divisor;
                dividendLengthMinusOne = Utils.TextLength(dividend) - 1;
                subtrahendLength = Utils.TextLength(subtrahend);

                while (dividendLengthMinusOne > subtrahendLength)
                {
                    subtrahendLength++;
                    subtrahend += "0";
                }

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
            
            double num1Length = Utils.TextLength(num1);
            double num2Length = Utils.TextLength(num2);

            if (num1Length > num2Length) return true;
            if (num1Length < num2Length) return false;

            for (double i = num1Length; i != 0; i--)
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

            double num1Length = Utils.TextLength(num1);
            double num2Length = Utils.TextLength(num2);

            if (num1Length > num2Length) return true;
            if (num1Length < num2Length) return false;
                
            for (double i = num1Length; i != 0; i--)
            {
                if (int.Parse(number1[i].ToString()) > int.Parse(number2[i].ToString())) return true;
                if (int.Parse(number1[i].ToString()) < int.Parse(number2[i].ToString())) return false;
            }

            return true;
        }
    }
    static class Utils
    {
        public static string AddZeros_Beginning (string num, double limit)
        {
            double numLength = TextLength(num);

            while (numLength < limit)
            {
                num = "0" + num;
                numLength++;
            }   

            return num;
        }
        public static string AddZeros_Ending (string num, double howMuch)
        {
            double tmp = 0;

            while (tmp < howMuch)
            {
                num += "0";
                tmp++;
            }

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
            if (TextLength(number1) > TextLength(number2)) return number2;
            return number1;
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

        public static string TakeSymbols (string src, double fromWhatDigit, double toWhatDigit)
        {
            Dictionary<double, char> src_dict = StringToCorrectCharList(src);

            string res = "";

            while (fromWhatDigit != toWhatDigit)
            {
                res += src_dict[fromWhatDigit];
                fromWhatDigit++;
            }

            return res;
        }

        public static Dictionary<double, char> StringToCorrectCharList (string str)
        {
            double lenght = 0;
            Dictionary<double, char> map = ArrayAddEmpty(lenght - 1);
            foreach (char Char in str)
            {
                map[lenght] = Char;
                lenght++;
            }

            return map;
        }
    }
}
