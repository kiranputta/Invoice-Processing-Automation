using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace IPA
{
    static class  Cls_Crypto
    {
        private static bool useHashing;

        public static string Encrypt(string StrToEncrypt, string sEncryptionKey)
        {
            useHashing = true;
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(StrToEncrypt);

                //string sEncryptionKey = "removeusb";

                if (useHashing)
                {

                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(sEncryptionKey));
                    hashmd5.Clear();
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(sEncryptionKey);


                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch (Exception)
            {
                return "";
            }
            finally
            {
                hashmd5.Clear();
                tdes.Clear();
            }
        }

        public static string Decrypt(string StrToDecrypt, string sEncryptionKey)
        {
            useHashing = true;
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(StrToDecrypt);

                //string sEncryptionKey = "removeusb";

                if (useHashing)
                {

                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(sEncryptionKey));
                    hashmd5.Clear();
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(sEncryptionKey);


                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);


                return UTF8Encoding.UTF8.GetString(resultArray);
            }

            catch (Exception)
            {
                return "";
            }
            finally
            {
                hashmd5.Clear();
                tdes.Clear();
            }
        }

        #region "Old Code"

        public static string Decrypt1(string stringToDecrypt, string sEncryptionKey)
        {
            //Decrypts strings using the parsed Private Key:
            byte[] key = { };
            //byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
            byte[] IV = { 45, 23, 98, 23, 18, 89, 73, 29 };
            byte[] inputByteArray = new byte[stringToDecrypt.Length];
            key = Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(stringToDecrypt.Replace(" ", "+"));
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            Encoding encoding = Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }

        public static string Encrypt1(string stringToEncrypt, string sEncryptionKey)
        {
            //Encrypts strings using the parsed Private Key:
            byte[] key = { };
            //byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
            byte[] IV = { 45, 23, 98, 23, 18, 89, 73, 29 };
            byte[] inputByteArray;
            key = Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        public static string generateRandomPrivateKey()
        {
            //Generates an 8-bit Private Key:
            int[] key = new int[8];
            Random rnd = new Random();
            key[0] = rnd.Next(1, 9);
            key[1] = rnd.Next(0, 9);
            key[2] = rnd.Next(0, 9);
            key[3] = rnd.Next(0, 9);
            key[4] = rnd.Next(0, 9);
            key[5] = rnd.Next(0, 9);
            key[6] = rnd.Next(0, 9);
            key[7] = rnd.Next(0, 9);
            string pvtKey = String.Empty;
            foreach (int i in key)
            {
                pvtKey += i.ToString();
            }
            return pvtKey;
        }
        public static string generate16bitRandomPrivateKey()
        {
            //Generates an 8-bit Private Key:
            int[] key = new int[16];
            Random rnd = new Random();
            key[0] = rnd.Next(1, 9);
            key[1] = rnd.Next(0, 9);
            key[2] = rnd.Next(0, 9);
            key[3] = rnd.Next(0, 9);
            key[4] = rnd.Next(0, 9);
            key[5] = rnd.Next(0, 9);
            key[6] = rnd.Next(0, 9);
            key[7] = rnd.Next(0, 9);
            key[8] = rnd.Next(0, 9);
            key[9] = rnd.Next(0, 9);
            key[10] = rnd.Next(0, 9);
            key[11]= rnd.Next(0, 9);
            key[12] = rnd.Next(0, 9);
            key[13] = rnd.Next(0, 9);
            key[14] = rnd.Next(0, 9);
            key[15] = rnd.Next(0, 9);
            string pvtKey = String.Empty;
            foreach (int i in key)
            {
                pvtKey += i.ToString();
            }
            return pvtKey;
        }

        #endregion
    }
}
