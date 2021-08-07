using UnityEngine;
using System;
using System.Collections.Generic;
[System.Serializable]
public class LargeNumber
{
    public double sig = 0;//significant figure
    public int exp = 0;//exponent

    public LargeNumber()
    {
        sig = 0;
        exp = 0;
    }

    public LargeNumber(LargeNumber a)
    {
        sig = a.sig;
        exp = a.exp;
    }
    public LargeNumber(int _significantFigure, int _exponent)
    {
        sig = (double)_significantFigure;
        exp = _exponent;
    }

    public LargeNumber(float _significantFigure, int _exponent)
    {
        sig = (double)_significantFigure;
        exp = _exponent;
    }

    public LargeNumber(double _significantFigure, int _exponent)
    {
        sig = _significantFigure;
        exp = _exponent;
    }

    public static readonly LargeNumber zero = new LargeNumber(0, 0);
    public static readonly LargeNumber one = new LargeNumber(1, 0);
    public static readonly LargeNumber maxValue = new LargeNumber(double.MaxValue, int.MaxValue); 
    public static readonly LargeNumber minValue = new LargeNumber(float.MinValue, int.MinValue);

    #region Basic Math
    public static LargeNumber operator +(LargeNumber a, LargeNumber b)
    {  //c is always bigger
        LargeNumber c = new LargeNumber(a.exp >= b.exp ? a : b);
        LargeNumber d = new LargeNumber(a.exp < b.exp ? a : b);

        if (c.exp!=d.exp)
        {
            int dif = c.exp - d.exp;
            d = Math.Abs(dif) > 200 ? LargeNumber.zero : new LargeNumber(d.sig/(Math.Pow(10,dif)),d.exp+dif);
            
        }
        return new  LargeNumber(c.sig + d.sig, c.exp).Evaluate();
    }
    public static LargeNumber operator -(LargeNumber a, LargeNumber b)=> a+-b;
    public static LargeNumber operator *(LargeNumber a, LargeNumber b)
        => new LargeNumber(a.sig * b.sig, a.exp + b.exp).Evaluate();
    public static LargeNumber operator /(LargeNumber a, LargeNumber b)
        => new LargeNumber(a.sig / b.sig, a.exp - b.exp).Evaluate();
    public static LargeNumber operator -(LargeNumber a) => new LargeNumber(-a.sig,a.exp);

    public static bool operator <(LargeNumber a, LargeNumber b) =>
        a.exp < b.exp ? true : a.exp > b.exp ? false : a.sig < b.sig ? true : false;
    public static bool operator >(LargeNumber a, LargeNumber b) =>
        a.exp > b.exp ? true : a.exp < b.exp ? false : a.sig > b.sig ? true : false;
    public static bool operator <=(LargeNumber a, LargeNumber b) =>
        a.exp < b.exp ? true : a.exp > b.exp ? false : a.sig <= b.sig ? true : false;    
    public static bool operator >=(LargeNumber a, LargeNumber b) =>
        a.exp > b.exp ? true : a.exp < b.exp ? false : a.sig >= b.sig ? true : false;

    #endregion



    public static implicit operator LargeNumber(int a)=> new LargeNumber(a, 0).Evaluate();
    public static implicit operator LargeNumber(float a) => new LargeNumber(a, 0).Evaluate();
    public static implicit operator LargeNumber(double a) => new LargeNumber(a, 0).Evaluate();
}

public static class LargeNumberExtension
{
    public static int displayMethod = 0;
    public static string displayStringString;
    static string[][] digits = new string[][] {
        //zeroes
        new string[] {"thousand", "m", "b", "tr", "quadr",      "quint",    "sext",     "sept",     "oct", "non"},
        //ones
        new string[] { "/", "un/", "duo/", "tre/s", "quattuor/", "quinqua/", "se/sx", "septe/mn", "octo/", "nove/mn" },
        //tens
        new string[] { "/", "n/deci", "ms/viginti", "ns/triginta", "ns/quadraginta", "ns/quinquaginta", "n/sexaginta", "n/septuaginta", "mx/octoginta", "/nonaginta" },
        //hundreds
        new string[] { "/", "nx/centi", "n/ducenti", "ns/trecenti", "ns/quadringenti", "ns/quingenti", "n/sescenti", "n/septingenti", "mx/octingenti", "/nongenti" },
        //thousands
        new string[] { "/", "nx/mili", "n/duomili", "ns/tresmili", "ns/quadramili", "ns/quinquamili", "n/sexmili", "n/septuamili", "mx/octomili", "/nonamili" },
    };
    static string[] smallAffix = new string[] { "thousand", "m", "b", "tr", "quadr", "quint", "sext", "sept", "oct", "non", "dec" };
    public static float ToFloat (this LargeNumber number)
    {
        //Debug.Log("number to convert: " + number.sig + "E" + number.exp);
        if(number.exp<38)
        {
            return (float)number.sig * (Mathf.Pow(10,number.exp));
        }
        else
        {
            Debug.Log("ToFloat float was too big");
            return float.NaN;
        }
    }

    public static double ToDouble (this LargeNumber number)
    {
        //Debug.Log("number to convert: " + number.sig + "E" + number.exp);
        if (number.exp < 300)
        {
            return number.sig * (Math.Pow(10, number.exp));
        }
        else
        {
            Debug.Log("ToDouble float was too big");
            return double.NaN;
        }
    }

    public static LargeNumber Evaluate(this LargeNumber num)
    {
        if (Math.Abs(num.sig) >= 10 || (Math.Abs(num.sig) <= 1 && Math.Abs(num.sig)>0))
        {
            int dif = Mathf.FloorToInt((float)Math.Log10(Math.Abs(num.sig)));
            //Debug.Log("Before " + num.DisplayString() + " diff = " + dif);
            num.sig /= Math.Pow(10, dif);
            num.exp += dif;
            //Debug.Log("After " + num.DisplayString());
        }
        else if(num.sig==0)
        {
            num.exp = 0;
        }

        return num;
    }

    public static void ChangeDisplayMethod(int id = 0)
    {
        if (id <= 0)
        {
            displayMethod++;
            if (displayMethod >= 5)
                displayMethod = 0;
        }
        else if (id < 5)
        {
            displayMethod = id;
        }
        else
        {
            Debug.Log("displaymethod change not found");
        }

        switch (displayMethod)
        {
            case 0:// regular
                displayStringString = "Regular Double";
                break;
            case 1: // scientific
                displayStringString = "Scientific";
                break;
            case 2: // engineering
                displayStringString = "Engineering";
                break;
            case 3: // engineering with short scale
                displayStringString = "Engineering With Short Scale";
                break;
            case 4:// engineering but with symbols
                displayStringString = "Engineering + metric prefix";
                break;
            default:
                displayStringString = "none/notfound";
                break;
        }
    }
    public static string DisplayString(this LargeNumber a)
    {
        string stringNum,macroPrefixes;
        int digitDif = 0;
        LargeNumber number = new LargeNumber(a.sig,a.exp);
        switch (displayMethod)
        {
            case 0:// regular
                stringNum = number.ToDouble().ToString("0.000");
                break;
            case 1: // scientific
                stringNum = number.sig.ToString("0.000") + "E" + number.exp.ToString();
                break;
            case 2: // engineering
                digitDif = number.exp % 3;
                number.exp -= digitDif;
                number.sig *= Mathf.Pow(10, digitDif);

                stringNum = number.sig.ToString("0.000") + (number.exp != 0?"E" + number.exp.ToString():"");
                break;
            case 3: // engineering with short scale

                digitDif = number.exp % 3;
                number.exp -= digitDif;
                number.sig *= Mathf.Pow(10, digitDif);
                //                 3 6 9 12
                macroPrefixes = "  K M B T Q QiSxSp";
                stringNum = number.sig.ToString("0.000") + macroPrefixes[number.exp*2 / 3] + macroPrefixes[number.exp * 2 / 3 + 1];
                break;
            case 4:// engineering but with symbols
                digitDif = number.exp % 3;
                number.exp -= digitDif;
                number.sig *= Mathf.Pow(10, digitDif);

                macroPrefixes = "  k M G T P E Z Y";

                stringNum = number.sig.ToString("0.000") + macroPrefixes[number.exp * 2 / 3] +macroPrefixes[number.exp * 2 / 3 + 1];
                break;
            default:
                Debug.Log("Display method " + displayMethod + " not found");
                stringNum = number.ToFloat().ToString();
                break;
        }
        return stringNum;
    }

    public static LargeNumber Clamp(this LargeNumber num, LargeNumber min, LargeNumber max)
    {
        if (num >= min && num <= max)
            return num;
        else if (num > max)
            return max;
        else
            return min;
    }

    static readonly List<string> onesSymbols = new List<string>() { "K", "M", "B", "T", "Qu", "Qi", "Sx", "Sp", "Oc", "No" };
    static readonly List<string> tensSymbols = new List<string>() { "Dc", "Vg","Tg","Qg" ,"Qq","Sg","St" ,"Ot"};


    static readonly List<string> postTensuffix = new List<string>() { "","Un","Duo","Tre","Quattuor","Quin","Sex","Septen","Octo","Novem"};
    static readonly List<string> postCentsuffix = new List<string>() { "" ,"Un","Du"};

    public static string ToName(this LargeNumber a)
    {
        List<string> onesAffix = new List<string>() { "m", "b", "tr", "quadr", "quint", "sext", "sept", "oct", "non", "dec" };
        //add expection for affix 0
        List<string> tensAffix = new List<string>() { "", "Vi", "Tri", "Quadra", "Quinqua", "Sexa", "Septua", "Octo", "Nona" };
        List<string> hundredsAffix = new List<string>() { "Cent", };

        string numberName = GetLargeName(a.exp);
        return numberName;
    }

    public static string GetLargeName(int og,string overrideString="")
    {
        string result = "";

        if (og <= 2)
            return result;
        string num = ((og - og % 3 - 3) / 3).ToString();
        if (overrideString.CompareTo("")!=0)
            num = overrideString;
        //Debug.Log($"og: {og} = {num}");

        if(og<33)
        {
            string[] smallAffix = new string[] { "thousand", "m", "b", "tr", "quadr", "quint", "sext", "sept", "oct", "non", "dec" };
            result = smallAffix[int.Parse(num)] + (og > 3 ? "illion" : "");
        }
        else
        {
            int count = 0;
            List<string> baseString = new List<string>();
            string[] currentDigit;
            //get the bases
            for (int i = num.Length-1; i >= 0; i--, count++)
            {                
                if(int.Parse(num[i].ToString())!=0)
                {
                    currentDigit = GetLargeDigits(count);
                    baseString.Add(currentDigit[int.Parse(num[i].ToString())]);
                }
            }
            // join the segments
            string joint = "";
            for (int i = 0; i < baseString.Count; i++)
            {
                var temp = baseString[i].Split('/');
                bool isOnes = temp[1].Length > temp[0].Length && (temp[0].CompareTo("se") != 0);
                joint = isOnes ? temp[1] : temp[0];
                if (!isOnes)
                    joint += FirstCopyCompare(isOnes ? temp[0] : temp[1], baseString[i + 1].Split('/')[0]);
                result += joint;
            }
            //if ends in a change to i
            if (result[result.Length - 1] == 'a')
               result = result.Substring(0,result.Length-1) + "i";
            //add llion
            result += "llion";
        }

        return result;
    }
    private static string FirstCopyCompare(string a, string b)
    {
        for (int i = 0; i < a.Length; i++)
        {
            for (int j = 0; j < b.Length; j++)
            {
                if (a[i] == b[j])
                    return a[i].ToString();
            }            
        }
        return "";
    }

    private static string[] GetLargeDigits(int place)
    {

        if(place<4)
        {
            return digits[place+1];
        }
        else
        {
            //Debug.Log("Sig: " +place);
            string[] temp = new string[digits[4].Length];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = digits[4][i];
                temp[i] = temp[i].Replace("m", smallAffix[place - 2]);
                //Debug.Log($"i:{i},len {temp.Length},smallafix {smallAffix[place-2]}, temp[i] {temp[i]}");
            }

            return temp;
        }
    }
}
