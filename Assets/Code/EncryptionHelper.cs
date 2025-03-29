using System;
using System.Security.Cryptography;
using System.Text;

public static class EncryptionHelper
{
    /* private static readonly string encryptionKey = "MySuperSecureKey!"; 

     public static string Encrypt(string plainText)
     {
         byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);
         using (Aes aes = Aes.Create())
         {
             aes.Key = keyBytes;
             aes.Mode = CipherMode.ECB;
             aes.Padding = PaddingMode.PKCS7;

             using (ICryptoTransform encryptor = aes.CreateEncryptor())
             {
                 byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
                 byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                 return Convert.ToBase64String(encryptedBytes);
             }
         }
     }

     public static string Decrypt(string encryptedText)
     {
         byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);
         using (Aes aes = Aes.Create())
         {
             aes.Key = keyBytes;
             aes.Mode = CipherMode.ECB;
             aes.Padding = PaddingMode.PKCS7;

             using (ICryptoTransform decryptor = aes.CreateDecryptor())
             {
                 byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                 byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                 return Encoding.UTF8.GetString(decryptedBytes);
             }
         }
     }*/
    private static readonly string encryptionKey = "MySuperSecureKey123"; // ✅ Must be 16, 24, or 32 bytes long

    public static string Encrypt(string plainText)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);
        Array.Resize(ref keyBytes, 32); // ✅ Force key to be 32 bytes (AES-256)

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            using (ICryptoTransform encryptor = aes.CreateEncryptor())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                return Convert.ToBase64String(encryptedBytes);
            }
        }
    }

    public static string Decrypt(string encryptedText)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);
        Array.Resize(ref keyBytes, 32); // ✅ Force key to be 32 bytes

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            using (ICryptoTransform decryptor = aes.CreateDecryptor())
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }

}
