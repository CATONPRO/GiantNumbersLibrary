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
        /// <param name="number1">Слагаемое</param>
        /// <param name="number2">Слагаемое</param>
        public static string Sum(string longerNum, string shorterNum)
        {
            Dictionary<double, char> numMap1 = Utils.StringToCharList(longerNum);
            Dictionary<double, char> numMap2 = Utils.StringToCharList(Utils.AddZeros_Beginning(shorterNum, Utils.TextLength(longerNum)));

            string result = "";
            int transfer = 0;
            double currentNum = 0;
            int resultDigit = 0;

            foreach (char Char in numMap1.Values)
            {
                resultDigit = int.Parse(numMap1[currentNum].ToString()) + int.Parse(numMap2[currentNum].ToString()) + transfer;
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

            }
            catch
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
        public static string Multiply (string num1, string num2)
        {
            string longerNum = Utils.GetLongestNum(num1, num2);
            string shorterNum;
            if (longerNum == num1) shorterNum = num2;
            else shorterNum = num1;
            double shorterNumLength = Utils.TextLength(shorterNum);
            double longerNumLength = Utils.TextLength(longerNum);
            if (shorterNumLength < 33) return MultiplyClassic(longerNum, shorterNum);
            double smallestPowerOfTwo = 1;
            while (smallestPowerOfTwo < shorterNumLength) smallestPowerOfTwo = smallestPowerOfTwo * 2;
            smallestPowerOfTwo = smallestPowerOfTwo / 2; 
            string shorterNum_Left = Utils.TakeSymbols(shorterNum, 0, smallestPowerOfTwo);
            string shorterNum_Right = Utils.TakeSymbols_ToTheEnd(shorterNum, smallestPowerOfTwo);
            string longerNum_Left = Utils.TakeSymbols(longerNum, 0, smallestPowerOfTwo);
            string longerNum_Right = Utils.TakeSymbols_ToTheEnd(longerNum, smallestPowerOfTwo);

            return Sum(Utils.AddZeros_Ending(Multiply_PowsOfTwo(longerNum_Left, shorterNum_Left), longerNumLength + shorterNumLength - smallestPowerOfTwo - smallestPowerOfTwo), MultiplyClassic(longerNum_Right, shorterNum_Right));
        }
        /// <summary>
        /// Карацубовский способ умножения, он быстрее, но работает только со степенями двоек
        /// </summary>
        /// <param name="number">Число</param>
        /// <param name="multiplier">Множитель</param>
        public static string Multiply_PowsOfTwo(string num1, string num2)
        {
            double num1Length = Utils.TextLength(num1);
            double num2Length = Utils.TextLength(num2);

            if (num2Length < 33 || num1Length < 33)
            {
                return MultiplyClassic(num1, num2);
            }

            double n = Math.Max(num1Length, num2Length);
            double m = Math.Floor(Math.Min(num1Length, num2Length) / 2);
            string a = Utils.TakeSymbols(num1, 0, Math.Ceiling(num1Length / 2));
            string b = Utils.TakeSymbols_ToTheEnd(num1, Math.Ceiling(num1Length / 2));
            string c = Utils.TakeSymbols(num2, 0, Math.Ceiling(num2Length / 2));
            string d = Utils.TakeSymbols_ToTheEnd(num2, Math.Ceiling(num2Length / 2));

            // если мы будем умножать например 123 на 043 будет некруто
            try
            {
                while (b[0] == '0')
                {
                    b = Utils.TakeSymbols_ToTheEnd(b, 1);
                    a = a + '0';
                }
            } catch
            {
                if (b == "") b = "0";
            }

            try
            {
                while (d[0] == '0')
                {
                    d = Utils.TakeSymbols_ToTheEnd(d, 1);
                    c = c + '0';
                }
            } catch
            {
                if (d == "") d = "0";
            }

            // z0
            string ac = Multiply_PowsOfTwo(a, c);
            // z2
            string bd = Multiply_PowsOfTwo(b, d);
            // вспомогательные переменные для tmp1 и tmp2
            double aLength = Utils.TextLength(a);
            double bLength = Utils.TextLength(b);
            double cLength = Utils.TextLength(c);
            double dLength = Utils.TextLength(d);
            // tmp1 и tmp2 используются для временного хранения, ведь если мы зотим сравнить две суммы, чтобы засунуть
            // их в сумму побольше, их придется сравнивать из за нюанса Sum. (Сначала бОльшее число, потом меньшее)
            string tmp1 = Sum(a, d);
            string tmp2 = Sum(c, b);
            // вспомогательные для ab_cd (z1)
            double tmp1Length = Utils.TextLength(tmp1);
            double tmp2Length = Utils.TextLength(tmp2);
            double acLength = Utils.TextLength(ac);
            double bdLength = Utils.TextLength(bd);
            // z1
            string ab_cd = Subtract( Multiply_PowsOfTwo( Utils.GetLongestNum(tmp1, tmp2, tmp1Length, tmp2Length), Utils.GetShortestNum(tmp1, tmp2, tmp1Length, tmp2Length) ), Sum( Utils.GetLongestNum(ac, bd, acLength, bdLength), Utils.GetShortestNum(ac, bd, acLength, bdLength)));
            return Sum(Utils.AddZeros_Ending(ac, 2 * m), Sum(Utils.AddZeros_Ending(ab_cd, m), bd));
        }

        /// <summary>
        /// Классическое умножение
        /// </summary>
        /// <param name="number">Число</param>
        /// <param name="multiplier">Множитель</param>
        public static string MultiplyClassic(string num1, string num2)
        {
            if (num1 == "0" || num2 == "0") return "0";

            string number = Utils.GetLongestNum(num1, num2);
            string multiplier = "";
            if (number == num1) multiplier = num2;
            if (number == num2) multiplier = num1;

            string res = "0";
            string tmp = number;
            string decrement = "1";
            double multiplerLength = Utils.TextLength(multiplier);

            for (double I = 1; I < multiplerLength; I++)
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
        /// Факториал
        /// </summary>
        /// <param name="num">Число</param>
        public static string Factorial(double num)
        {
            string res1 = "1";
            string res2 = "1";

            while (num != Math.Floor(num / 2))
            {
                res1 = Multiply(res1, num.ToString());
                num--;
            }

            while (num != 0)
            {
                res2 = Multiply(res2, num.ToString());
                num--;
            }

            return Multiply(res1, res2);
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

            string maxPowOfTwo = "1";
            while (IsBigger(pow, maxPowOfTwo)) maxPowOfTwo = Multiply(maxPowOfTwo, "2");
            maxPowOfTwo = Divide(maxPowOfTwo, "2");

            string remainder = Subtract(pow, maxPowOfTwo);
            string res = num;
            
            while (maxPowOfTwo != "1")
            {
                maxPowOfTwo = Divide(maxPowOfTwo, "2");
                res = Multiply(res, res);   
            }

            while (remainder != "0")
            {
                res = Multiply(res, num);
                remainder = Subtract(remainder, "1");
            }

            return res;
        }

        /// <summary>
        /// Получение квадратного корня числа
        /// </summary>
        /// <param name="num">Число</param>
        public static string Sqrt(string num)
        {
            string res = "1";
            string oldRes = "";
            int numFirstTwoDigitsToInt = 0;

            try
            {
                numFirstTwoDigitsToInt = Convert.ToInt32(num[0].ToString() + num[1].ToString());
            }
            catch
            {
                if (num == "1" || num == "2") return "1";
                if (num == "3" || num == "4" || num == "5" || num == "6") return "2";
                if (num == "7" || num == "8" || num == "9") return "3";
            }
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

            string infiniteLoopPrevention = "";

            while (oldRes != res && infiniteLoopPrevention != res)
            {
                infiniteLoopPrevention = oldRes;
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
            double aLength = Utils.TextLength(a);
            double bLength = Utils.TextLength(b);
            return Divide(Sum(Utils.GetLongestNum(a, b, aLength, bLength), Utils.GetShortestNum(a, b, aLength, bLength)), "2");
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
        /// Деление с числами после запятой
        /// </summary>
        /// <param name="dividend">Делимое</param>
        /// <param name="divisor">Делитель</param>
        public static string Divide_Decimal (string dividend, string divisor, double accuracy)
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

            string res = repeats + ".";

            for (double i = accuracy; i != 0; i--)
            {
                repeats = "0";
                dividend += "0";

                if (IsBiggerOrEqual(dividend, divisor))
                {
                    repeats = Sum("1", repeats);
                    dividend = Subtract(dividend, divisor);
                }

                while (IsBiggerOrEqual(dividend, divisor))
                {
                    repeats = Sum(repeats, "1");
                    dividend = Subtract(dividend, divisor);
                }

                res += repeats;
            }

            return res;
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

            while (IsBiggerOrEqual(dividend, divisor))
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
        public static bool IsBigger(string num1, string num2)
        {
            double num1Length = Utils.TextLength(num1);
            double num2Length = Utils.TextLength(num2);

            Dictionary<double, char> number1 = Utils.StringToCharList(Utils.GetShortestNum(num1, num2, num1Length, num2Length));
            Dictionary<double, char> number2 = Utils.StringToCharList(Utils.GetLongestNum(num1, num2, num1Length, num2Length));

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
        public static bool IsBiggerOrEqual(string num1, string num2)
        {
            double num1Length = Utils.TextLength(num1);
            double num2Length = Utils.TextLength(num2);

            Dictionary<double, char> number1 = Utils.StringToCharList(Utils.GetShortestNum(num1, num2, num1Length, num2Length));
            Dictionary<double, char> number2 = Utils.StringToCharList(Utils.GetLongestNum(num1, num2, num1Length, num2Length));

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
        public static string AddZeros_Beginning(string num, double limit)
        {
            double numLength = TextLength(num);

            while (numLength < limit)
            {
                num = "0" + num;
                numLength++;
            }

            return num;
        }
        public static string AddZeros_Ending(string num, double howMuch)
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
            if (TextLength(number1) > TextLength(number2)) return 1;
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
        public static string GetLongestNum(string number1, string number2, double? num1Length = null, double? num2Length = null)
        {
            if (num1Length == null) num1Length = TextLength(number1);
            if (num2Length == null) num2Length = TextLength(number2);
            if (num1Length > num2Length) return number1;
            return number2;
        }
        public static string GetShortestNum(string number1, string number2, double? num1Length = null, double? num2Length = null)
        {
            if (num1Length == null) num1Length = TextLength(number1);
            if (num2Length == null) num2Length = TextLength(number2);
            if (num1Length > num2Length) return number2;
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

        public static string TakeSymbols(string src, double fromWhatDigit, double toWhatDigit)
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

        public static string TakeSymbols_ToTheEnd(string src, double fromWhatDigit)
        {
            Dictionary<double, char> src_dict = StringToCorrectCharList(src);

            string res = "";

            try
            {
                while (true)
                {
                    res += src_dict[fromWhatDigit];
                    fromWhatDigit++;
                }
            }
            catch
            {
                return res;
            }
        }

        public static Dictionary<double, char> StringToCorrectCharList(string str)
        {
            double lenght = 0;
            Dictionary<double, char> map = new Dictionary<double, char>();
            foreach (char Char in str)
            {
                map[lenght] = Char;
                lenght++;
            }

            return map;
        }
    }
}
