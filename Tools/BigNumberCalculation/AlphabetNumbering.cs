using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;

namespace TRPUtil.AlphabetNubmer
{
	public static class Conversion
	{

		/// <summary>
        /// for문으로 Dictionary 원소 추가할 예정
        /// </summary>

		static Dictionary<string, int> alphabetNumberDict = new Dictionary<string, int>()
		{
			{"", 0 },
			{"A", 1 },{"B", 2 },{"C", 3 },{"D", 4 },{"E", 5 },{"F", 6 },{"G", 7 },{"H", 8 },{"I", 9 },{"J", 10 },{"K", 11 },{"L", 12 },{"M", 13 },{"N", 14 },{"O", 15 },{"P", 16 },{"Q", 17 },{"R", 18 },{"S", 19 },{"T", 20 },{"U", 21 },{"V", 22 },{"W", 23 },{"X", 24 },{"Y", 25 },{"Z", 26 },
			{"AA", 27 },{"AB", 28 },{"AC", 29 },{"AD", 30 },{"AE", 31 },{"AF", 32 },{"AG", 33 },{"AH", 34 },{"AI", 35 },{"AJ", 36 },{"AK", 37 },{"AL", 38 },{"AM", 39 },{"AN", 40 },{"AO", 41 },{"AP", 42 },{"AQ", 43 },{"AR", 44 },{"AS", 45 },{"AT", 46 },{"AU", 47 },{"AV", 48 },{"AW", 49 },{"AX", 50 },{"AY", 51 },{"AZ", 52 },
			{"BA", 53 },{"BB", 54 },{"BC", 55 },{"BD", 56 },{"BE", 57 },{"BF", 58 },{"BG", 59 },{"BH", 60 },{"BI", 61 },{"BJ", 62 },{"BK", 63 },{"BL", 64 },{"BM", 65 },{"BN", 66 },{"BO", 67 },{"BP", 68 },{"BQ", 69 },{"BR", 70 },{"BS", 71 },{"BT", 72 },{"BU", 73 },{"BV", 74 },{"BW", 75 },{"BX", 76 },{"BY", 77 },{"BZ", 78 },
			{"CA", 79 },{"CB", 80 },{"CC", 81 },{"CD", 82 },{"CE", 83 },{"CF", 84 },{"CG", 85 },{"CH", 86 },{"CI", 87 },{"CJ", 88 },{"CK", 89 },{"CL", 90 },{"CM", 91 },{"CN", 92 },{"CO", 93 },{"CP", 94 },{"CQ", 95 },{"CR", 96 },{"CS", 97 },{"CT", 98 },{"CU", 99 },{"CV", 100 },{"CW", 101 },{"CX", 102 },{"CY", 103 },{"CZ", 104 }
		};

		/// <summary>
		/// 숫자를 문자로 변환합니다. 예) 1000 => 1A로 변환
		/// </summary>
		/// <param name="number">변환할 숫자</param>
		/// <param name="count">재귀를 위한 디폴트 매개변수</param>
		/// <returns></returns>
		public static string BigIntegerToAlphabetNumber(BigInteger number, int count = 0)
		{
			for (count = 0; count < alphabetNumberDict.Count; count++)
			{
				if (number >= 1 && number < 1000)
				{
					return (number.ToString() + alphabetNumberDict.Keys.ElementAt(count));
				}

				else number /= 1000;
			}

			return "ERROR";
		}

		/// <summary>
		/// 문자를 숫자로 변환합니다. 예) 1A => 1000으로 변환
		/// </summary>
		/// <param name="number">변환할 문자</param>
		/// <returns></returns>
		public static BigInteger AlphabetNumberToBigInteger(string number)
		{
			int value = int.Parse(Regex.Replace(number, @"\D", ""));
			string key = Regex.Replace(number, @"\d", "");

			BigInteger powUnit = BigInteger.Pow(1000, alphabetNumberDict[key]);

			return value * powUnit;
		}
	}

	public class AlphabetNumber
	{
		public BigInteger bigInteger;


		//해당하는 거듭제곱에 대한 숫자에 대응하는 문자를 만듬. -> 26진법이라고 생각하면된다.
		public List<char> alpha = new List<char> { 'Z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y' };

		/// <summary>
		/// x에 대응하는 문자를 만듬.
		/// </summary>
		public string Gen(int x)
		{
			string s = "";
			int num = x;
			while (num != 0)
			{
				int div = num-- % 26;
				s = alpha[div] + s;
				num /= 26;
			}
			return s;
		}


		#region Creator
		public AlphabetNumber() { }
		public AlphabetNumber(BigInteger bigInteger) { this.bigInteger = bigInteger; }
		public AlphabetNumber(string stringNumber) { bigInteger = Conversion.AlphabetNumberToBigInteger(stringNumber); }
		#endregion

		#region Add operator
		public static AlphabetNumber operator +(AlphabetNumber alphabetNumber1, AlphabetNumber alphabetNumber2)
		{
			BigInteger returnNumber = BigInteger.Add(alphabetNumber1.bigInteger, alphabetNumber2.bigInteger);
			return new AlphabetNumber(returnNumber);
		}
		public static AlphabetNumber operator +(AlphabetNumber alphabetNumber, BigInteger bigInteger)
		{
			BigInteger returnNumber = BigInteger.Add(alphabetNumber.bigInteger, bigInteger);
			return new AlphabetNumber(returnNumber);
		}
		public static AlphabetNumber operator +(AlphabetNumber alphabetNumber, string stringNumber)
		{
			BigInteger conversion = Conversion.AlphabetNumberToBigInteger(stringNumber);
			BigInteger returnNumber = BigInteger.Add(alphabetNumber.bigInteger, conversion);

			return new AlphabetNumber(returnNumber);
		}
		#endregion

		#region Subtrac operator
		public static AlphabetNumber operator -(AlphabetNumber alphabetNumber1, AlphabetNumber alphabetNumber2)
		{
			BigInteger returnNumber = BigInteger.Subtract(alphabetNumber1.bigInteger, alphabetNumber2.bigInteger);
			return new AlphabetNumber(returnNumber);
		}
		public static AlphabetNumber operator -(AlphabetNumber alphabetNumber, BigInteger bigInteger)
		{
			BigInteger returnNumber = BigInteger.Subtract(alphabetNumber.bigInteger, bigInteger);
			return new AlphabetNumber(returnNumber);
		}
		public static AlphabetNumber operator -(AlphabetNumber alphabetNumber, string stringNumber)
		{
			BigInteger conversion = Conversion.AlphabetNumberToBigInteger(stringNumber);
			BigInteger returnNumber = BigInteger.Subtract(alphabetNumber.bigInteger, conversion);

			return new AlphabetNumber(returnNumber);
		}
		#endregion

		#region Multiply operator
		public static AlphabetNumber operator *(AlphabetNumber alphabetNumber1, AlphabetNumber alphabetNumber2)
		{
			BigInteger returnNumber = BigInteger.Multiply(alphabetNumber1.bigInteger, alphabetNumber2.bigInteger);
			return new AlphabetNumber(returnNumber);
		}
		public static AlphabetNumber operator *(AlphabetNumber alphabetNumber, BigInteger bigInteger)
		{
			BigInteger returnNumber = BigInteger.Multiply(alphabetNumber.bigInteger, bigInteger);
			return new AlphabetNumber(returnNumber);
		}
		public static AlphabetNumber operator *(AlphabetNumber alphabetNumber, string stringNumber)
		{
			BigInteger conversion = Conversion.AlphabetNumberToBigInteger(stringNumber);
			BigInteger returnNumber = BigInteger.Multiply(alphabetNumber.bigInteger, conversion);

			return new AlphabetNumber(returnNumber);
		}
		#endregion

		#region Divide operator
		public static AlphabetNumber operator /(AlphabetNumber alphabetNumber1, AlphabetNumber alphabetNumber2)
		{
			BigInteger returnNumber = BigInteger.Divide(alphabetNumber1.bigInteger, alphabetNumber2.bigInteger);
			return new AlphabetNumber(returnNumber);
		}
		public static AlphabetNumber operator /(AlphabetNumber alphabetNumber, BigInteger bigInteger)
		{
			BigInteger returnNumber = BigInteger.Divide(alphabetNumber.bigInteger, bigInteger);
			return new AlphabetNumber(returnNumber);
		}
		public static AlphabetNumber operator /(AlphabetNumber alphabetNumber, string stringNumber)
		{
			BigInteger conversion = Conversion.AlphabetNumberToBigInteger(stringNumber);
			BigInteger returnNumber = BigInteger.Divide(alphabetNumber.bigInteger, conversion);

			return new AlphabetNumber(returnNumber);
		}
		#endregion
	}
}