using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Summary description for AESEncryption
/// </summary>
/// 

namespace Trump.Models
{
    public class AESEncryption
    {
        public static string DecryptStringAES(string cipherText, string key)
        {

            //var keybytes = Encoding.UTF8.GetBytes("8080808080808080");
            //var iv = Encoding.UTF8.GetBytes("8080808080808080");
            // var keybytes = Encoding.UTF8.GetBytes(key1);
            //var iv = Encoding.UTF8.GetBytes(key1);


            var keybytes = Encoding.UTF8.GetBytes(key);
            var iv = Encoding.UTF8.GetBytes(key);
            var encrypted = Convert.FromBase64String(cipherText);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            return string.Format(decriptedFromJavascript);
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                try
                {
                    // Create the streams used for decryption.
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }

            return plaintext;
        }

        private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            byte[] encrypted;
            // Create a RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }



        //------------\\
        public string Encode(string sData)
        {
            try
            {
                byte[] encData_byte = new byte[sData.Length+6];

                encData_byte = System.Text.Encoding.UTF8.GetBytes(sData);

                string encodedData = Convert.ToBase64String(encData_byte);

                return encodedData;

            }
            catch (Exception ex)
            {
                return "";
            }
        }


        public string Decode(string sData)
        {

            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();

            System.Text.Decoder utf8Decode = encoder.GetDecoder();

            byte[] todecode_byte = Convert.FromBase64String(sData);

            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);

            char[] decoded_char = new char[charCount];

            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);

            string result = new String(decoded_char);

            return result;

        }

        //--------------

        public String urlEncode(String Code)
        {
            try
            {
                string EncodedText;
                char[] charArray = new char[6];
                charArray = Code.ToCharArray();
                Array.Reverse(charArray);
                string reverseTxt = new string(charArray);
                EncodedText = reverseTxt.Substring(0, 4) + "-" + Convert.ToInt16(reverseTxt.Substring(reverseTxt.Length - 4, 4)).ToString("X") + "-" + reverseTxt.Substring(reverseTxt.Length - 4, 4) + "-" + Convert.ToInt16(reverseTxt.Substring(0, 4)).ToString("X");
                return EncodedText;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public String urlDecode(String Code)
        {
            try
            {
                char[] charArray = new char[6];
                string[] splitTxt = new string[3];
                splitTxt = Code.Split('-');
                charArray = (splitTxt[0].Substring(0, 3) + "" + splitTxt[2].Substring(splitTxt[2].Length - 3, 3)).ToCharArray();
                Array.Reverse(charArray);
                string DecodedText = new string(charArray);
                return DecodedText;
            }
            catch (Exception)
            {
                return null;
            }

        }


        //--------------



    }






}