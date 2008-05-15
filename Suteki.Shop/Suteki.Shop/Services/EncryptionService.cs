using System;
using System.Security.Cryptography;
using System.Text;
using Suteki.Shop.Validation;

namespace Suteki.Shop.Services
{
    public class EncryptionService : IEncryptionService
    {
        string privateKey;
        readonly string publicKey;

        /// <summary>
        /// Only supply the publicKey for the TFB Web Server
        /// </summary>
        /// <param name="publicKey"></param>
        public EncryptionService(string publicKey) : this(publicKey, null)
        { }

        /// <summary>
        /// Supply both public and private keys for the EDF Application
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="privateKey"></param>
        public EncryptionService(string publicKey, string privateKey)
        {
            if (publicKey == null) throw new ArgumentNullException("publicKey");

            this.publicKey = publicKey;
            this.privateKey = privateKey;
        }

        /// <summary>
        /// turn a string into a base64 encoded encrypted byte array
        /// </summary>
        /// <param name="stringToEncrypt"></param>
        /// <returns></returns>
        public string Encrypt(string stringToEncrypt)
        {
            // .net uses UTF8 encoding internally
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] dataToEncrypt = encoding.GetBytes(stringToEncrypt);

            // use the RSA async enctyption algorithm
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            // import the public key
            try
            {
                rsa.ImportCspBlob(Convert.FromBase64String(publicKey));
            }
            catch
            {
                throw new ValidationException("Invalid Public Key");
            }

            // return a base64 encoded byte array
            return Convert.ToBase64String(rsa.Encrypt(dataToEncrypt, false));
        }

        /// <summary>
        /// turn a base 64 encoded byte array into a decoded string
        /// </summary>
        /// <param name="encryptedString"></param>
        /// <returns></returns>
        public string Decrypt(string encryptedString)
        {
            // we need the private key for this
            if (privateKey == null)
                throw new ApplicationException("Can't decrypt. Private key unavailable");

            // get the byte array
            byte[] encryptedData = Convert.FromBase64String(encryptedString);

            // use RSA again
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            // import the private key
            try
            {
                rsa.ImportCspBlob(Convert.FromBase64String(privateKey));
            }
            catch
            {
                throw new ValidationException("Invalid Private Key");
            }

            // decrypt the byte array.
            byte[] decryptedBytes;
            try
            {
                decryptedBytes = rsa.Decrypt(encryptedData, false);
            }
            catch (CryptographicException exception)
            {
                throw new ValidationException("Couldn't decrypt data", exception);
            }

            // turn the bytes into a string
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetString(decryptedBytes);
        }

        public void EncryptCard(Card card)
        {
            card.SetEncryptedNumber(Encrypt(card.Number));
            card.SetEncryptedSecurityCode(Encrypt(card.SecurityCode));
        }

        public void DecryptCard(Card card)
        {
            card.Number = Decrypt(card.Number);
            card.SecurityCode = Decrypt(card.SecurityCode);
        }

        public string PrivateKey
        {
            get
            {
                return this.privateKey;
            }
            set
            {
                privateKey = value;
            }
        }
    }
}
