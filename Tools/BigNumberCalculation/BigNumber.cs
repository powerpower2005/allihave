using System.Threading;
using UnityEngine;


public static string GetNumberString(double value)
{
string res;
if (value < 1000)
{
    res = value.ToString("F0");
}
else
{
    var str = value.ToString("F0");
    //1
    //21
    //321
    //4321  4.32a
    //54321 
    //654321 
    //7654321 7a654
    //87654321 87a654
    //987654321 987a654
    //1987654321 1b987a
                
    //123b
    //12b
    //1b 12a
    var a = Mathf.Max(str.Length / 3 - 1, 0);
    var aa = str.Length % 3;
    if (aa == 0) aa = 3;

    //Debug.Log($"[{str}] {a},{aa}");
    var prefix = str.Substring(0, aa);
    var suffix = str.Substring(aa, 3-aa);
    var numSuffix = suffix.Length==0?0:int.Parse(suffix);
    if (numSuffix == 0&&a>0)
        res =
            $"{prefix}{Constant.numberAlpha[a-1]}";
    else if (a > 0)
        res =
            $"{prefix}. {suffix}{Constant.numberAlpha[a]}";
            //$"{prefix}{Constant.numberAlpha[a]} {suffix}{Constant.numberAlpha[b]}";
    else
        res = $"{prefix}{Constant.numberAlpha[a]} {suffix}";


    // Debug.Log(str + " -> " + res + "a:"+a+"  b:"+b);
}

return res;
}

public static string ToNumberString(this double value)
{
return GetNumberString(value);
}


public static class Constant
{
public const float UnitDefaultColliderRadius = 0.02f;
public const float UIBottomRate = 0.45f;
public const float UIBottomFixHeight = 80;

public const int EquipmentMixNeedCount = 5;
public const float TouchScreenSensitive = 0.01f;
public static readonly int ShaderPropertyIdMaskAmount = Shader.PropertyToID("_MaskAmount");

          
            
public static readonly string[] numberAlpha =
{
    "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U",
    "V", "W", "X", "Y", "Z",
    "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ",
    "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ",
    "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ",
    "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ",
    "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ",
    "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ",
    "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ",
    "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ",
    "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ",
    "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ",
    "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ",
    "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ"
};
}
