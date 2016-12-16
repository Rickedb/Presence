using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Presence.Core.Security
{
    /// <summary>
    ///-----------------------------------------------------------------
    ///   Namespace:      DCX.ITLC.USA.Core.Security
    ///   Class:          Encryption
    ///   Description:    Provê métodos para manipulação de informações 
    ///                   criptografadas no Sistema.
    ///   Author:         Leandro Piqueira                 Date: 04/11/2014
    ///   Notes:          
    ///-----------------------------------------------------------------
    ///   Revision History:
    ///   Name:            Date: //       Description: 
    ///-----------------------------------------------------------------
    /// </summary>
    public static class Encryption
    {
        

        /// <summary>
        /// Chave de Criptografia utilizada pelo Sistema
        /// </summary>
        public static String SYSTEM_KEY = "debishsdgai";

        public static string Encrypt(string Texto)
        {
            try
            {
                TripleDESCryptoServiceProvider TripleDES = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
                TripleDES.Key = MD5.ComputeHash(ASCIIEncoding.UTF8.GetBytes(SYSTEM_KEY));
                TripleDES.Mode = CipherMode.ECB;

                byte[] buffer = ASCIIEncoding.ASCII.GetBytes(Texto);
                return Convert.ToBase64String(TripleDES.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Rotina que descriptografa uma determinada informação baseada na chave fornecida
        /// </summary>
        /// <param name="text">Texto a ser descriptografado</param>
        /// <param name="key">Chave utilizada para criptografar o texto</param>
        /// <returns>Texto descriptografado</returns>
        public static String decrypt(String text, String key)
        {
            TripleDESCryptoServiceProvider TripleDES = new TripleDESCryptoServiceProvider();
            try
            {
                if (text == string.Empty)
                    return string.Empty;

                TripleDES.Key = new MD5CryptoServiceProvider().ComputeHash(ASCIIEncoding.UTF8.GetBytes(key));
                TripleDES.Mode = CipherMode.ECB;

                byte[] buffer = Convert.FromBase64String(text);
                return ASCIIEncoding.ASCII.GetString(TripleDES.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu uma falha ao descriptografar dados do sistema. " + Environment.NewLine + ex.ToString());
            }
        }
    }
}
