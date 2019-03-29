using System;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Zenfolio7.Zenfolio;

namespace Zenfolio7.DataAccess
{
    /// <summary>
    /// Transparent helper inherited from generated ZfApi web service proxy class.
    /// Provides login sequence and manages authentication token.
    /// </summary>
    public class ZenfolioClient : ZfApi
    {
        private string _token;
        private string _loginName;
        private string _authority;

        public ZenfolioClient()
        {
        }

        /// <summary>
        /// Gets login name of current user
        /// </summary>
        public string LoginName
        {
            get { return _loginName; }
        }

        /// <summary>
        /// Gets current user token.
        /// </summary>
        public string Token
        {
            get { return _token; }
        }

        /// <summary>
        /// Gets Zenfolio authority i.e. scheme/host/port
        /// </summary>
        public string Authority
        {
            get
            {
                if (_authority == null)
                    _authority = new Uri(this.Url).GetLeftPart(UriPartial.Authority);
                return _authority;
            }
        }

        /// <summary>
        /// Computes salted data hash
        /// </summary>
        /// <param name="data">Data to hash</param>
        /// <param name="salt">Salt</param>
        /// <returns>Computed SHA-256 hash of salt+data pair</returns>
        private static byte[] HashData(byte[] salt, byte[] data)
        {
            byte[] buffer = new byte[data.Length + salt.Length];
            salt.CopyTo(buffer, 0);
            data.CopyTo(buffer, salt.Length);
            return new SHA256Managed().ComputeHash(buffer);
        }

        /// <summary>
        /// Logs into Zenfolio API
        /// </summary>
        /// <param name="loginName">User's login name</param>
        /// <param name="password">User's password</param>
        /// <returns>True if login was successful, false otherwise.</returns>
        public bool Login(string loginName, string password)
        {
            if (string.IsNullOrEmpty(loginName))
            {
                throw new Exception("Username invalid");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("Password invalid");
            }
            try
            {
                // Get API challenge
                AuthChallenge ch = this.GetChallenge(loginName);

                // Extract and hash password bytes
                byte[] passwordHash = HashData(ch.PasswordSalt,
                                               Encoding.UTF8.GetBytes(password));

                // Compute secret proof
                byte[] proof = HashData(ch.Challenge, passwordHash);

                // Authenticate
                _token = this.Authenticate(ch.Challenge, proof);
                if (_token != null)
                {
                    _loginName = loginName;
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                // Swallow all exceptions and return false
            }
            return false;
        }

        /// <summary>
        /// Logs into Zenfolio API
        /// </summary>
        /// <param name="loginName">User's login name</param>
        /// <param name="password">User's password</param>
        /// <returns>True if login was successful, false otherwise.</returns>
        public bool LoginPlain(string loginName, string password)
        {
            try
            {
                _token = this.AuthenticatePlain(loginName, password);
                if (_token != null)
                {
                    _loginName = loginName;
                    return true;
                }
            }
            catch
            {
                // Swallow all exceptions and return false
            }
            return false;
        }

        /// <summary>
        /// Creates a WebRequest instance for the specified URI.
        /// Overriden to provide authentication header.
        /// </summary>
        /// <param name="uri">The URI to use when creating the WebRequest.</param>
        /// <returns>The WebRequest instance.</returns>
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest wr = base.GetWebRequest(uri);
            if (_token != null)
                wr.Headers.Add("X-Zenfolio-Token", _token);
            return wr;
        }
    }
}
