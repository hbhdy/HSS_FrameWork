using System;
using System.Globalization;

public static class AlphabetUnitChange
{
    private const string Zero = "0";
    private const string Infinity = "Infinity";

    // double형 e+308까지의 자릿수를 표현 하기 위함
    private static readonly string[] AlphabetUnits = new string[]
    {
            "",
            "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O", "P","Q","R","S","T","U","V","W","X","Y","Z",
            "AA","AB","AC","AD","AE","AF","AG","AH","AI","AJ","AK","AL","AM","AN","AO","AP","AQ","AR","AS","AT","AU","AV","AW","AX","AY","AZ",
            "BA","BB","BC","BD","BE","BF","BG","BH","BI","BJ","BK","BL","BM","BN","BO","BP","BQ","BR","BS","BT","BU","BV","BW","BX","BY","BZ",
            "CA","CB","CC","CD","CE","CF","CG","CH","CI","CJ","CK","CL","CM","CN","CO","CP","CQ","CR","CS","CT","CU","CV","CW","CX",
    };

    // double형 데이터를 알파벳 단위로 표현
    public static string ToAlphabetString(this double number, bool isDecimalPoint = true)
    {
        if (number <= 0)
            return Zero;
        else if (double.IsInfinity(number))
            return Infinity;
        else if (number < 10)
            return number.ToString(isDecimalPoint ? "n2" : "n0");
        else if (number < 100)
            return number.ToString(isDecimalPoint ? "n1" : "n0");

        // 지수 표현식 사용
        string[] partsSplit = number.ToString("E").Split('+');

        if (partsSplit.Length < 2)
            return Zero;

        if (!int.TryParse(partsSplit[1], out int exponent))  // 지수가 존재하는지
            return Zero;

        int quotient = exponent / 3;
        int remainder = exponent % 3;

        double value = double.Parse(partsSplit[0].Replace("E", "")) * Math.Pow(10, remainder);

        if (quotient >= AlphabetUnits.Length)
            return Infinity;

        return $"{value}{AlphabetUnits[quotient]}";
    }

    // 문자열로 입력된 데이터를 double형으로 표현
    public static double AlphabetToDouble(this string stringNum)
    {
        double result;
        const NumberStyles style = NumberStyles.Any;
        var invariantCulture = CultureInfo.InvariantCulture;

        var s = stringNum.Replace(',', '.').ToString(invariantCulture);
        if (double.TryParse(s, style, invariantCulture, out result))
            return result;

        int length = s.Length - 1;
        int lastNumberIndex = -1;

        for (int i = length; i >= 0; --i)
        {
            if (char.IsNumber(s, i) == true)
            {
                lastNumberIndex = i;
                break;
            }
        }

        if (lastNumberIndex < 0)
            throw new ArgumentException("Input string does not contain valid number.");

        string number = s[..(lastNumberIndex + 1)].ToString(invariantCulture);
        string unit = s[(lastNumberIndex + 1)..];

        // 대소문자 구분 X
        int index = Array.FindIndex(AlphabetUnits, p => p.Equals(unit, StringComparison.OrdinalIgnoreCase));

        if (index == -1)
            throw new ArgumentException($"Invalid unit '{unit}' in input string.");

        s = $"{number}E+{index * 3}";
        if (double.TryParse(s, style, invariantCulture, out result))
            return result;

        throw new InvalidOperationException("Failed to convert string to double.");
    }
}
