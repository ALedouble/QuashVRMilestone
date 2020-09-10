using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

public static class SecureData
{
    public static string Hash(string data)
    {
        byte[] textToByte = Encoding.UTF8.GetBytes(data);
        SHA256Managed mySHA256 = new SHA256Managed();

        byte[] hashValue = mySHA256.ComputeHash(textToByte);

        return GetHexStringFromHash(hashValue);
    }

    private static string GetHexStringFromHash(byte[] hash)
    {
        string hexString = String.Empty;

        foreach (byte b in hash)
        {
            hexString += b.ToString("x2");
        }

        return hexString;
    }

    //public static string EncryptDecrypt(string data, int key)
    //{
    //    StringBuilder input = new StringBuilder(data);
    //    StringBuilder output = new StringBuilder(data.Length);

    //    char character;

    //    for (int i = 0; i < data.Length; i++)
    //    {
    //        character = input[i];
    //        character = (char)(character ^ key);
    //        output.Append(character);
    //    }

    //    return output.ToString();
    //}

}
