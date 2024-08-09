using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Musafir.AmaduesAPI.Header
{

    /// <summary>
    /// WS-Security Header
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Security", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
    public class AmadeusSecurityHeader
    {
        #region "Constructors"

        public AmadeusSecurityHeader()
        {

        }
        public AmadeusSecurityHeader(IConfiguration configuration)
        {
            string username = configuration["AmadeusConfiguration:Username"] ?? string.Empty;
            string password = configuration["AmadeusConfiguration:Password"] ?? string.Empty;
            UsernameToken = new()
            {
                Id = "SecurityToken-" + Guid.NewGuid().ToString("D").ToLower(),
                Username = username,
                Created = XmlConvert.ToString(DateTime.Now.ToUniversalTime(), "yyyy-MM-ddTHH:mm:ssZ"),
                Nonce = new(16),
            };

            UsernameToken.Password = new PasswordInfo(password, UsernameToken.Nonce.GetBytes(), UsernameToken.Created);
        }

        #endregion

        #region "Public Properties"

        /// <summary>
        /// WSSecurity Header - UsernameToken
        /// </summary>
        public UsernameTokenInfo? UsernameToken
        {
            get;set;
        }
        
        

        #endregion

        #region "Namespace Declaration"

        private XmlSerializerNamespaces xmlns;

        /// <summary>
        /// Xmlns declarations.
        /// </summary>
        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns
        {
            get
            {
                if (xmlns == null)
                {
                    xmlns = new XmlSerializerNamespaces();
                    xmlns.Add("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
                    xmlns.Add("wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
                }
                return xmlns;
            }
            set { xmlns = value; }
        }

        #endregion
    }

    /// <summary>
    /// WS-Security Header - UsernameToken
    /// </summary>
    [Serializable]
    public class UsernameTokenInfo
    {
        #region "Private Members"

        private string? _id;

        #endregion

        #region "Public Properties"

        /// <summary>
        /// WS-Security Header - UsernameToken - Id
        /// </summary>
        [XmlAttribute(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
        public string? Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// WS-Security Header - UsernameToken - Username
        /// </summary>
        public string? Username
        {
            get;
            set;
        }

        /// <summary>
        /// WS-Security Header - UsernameToken - Password
        /// </summary>
        public PasswordInfo? Password
        {
            get;
            set;
        }

        /// <summary>
        /// WS-Security Header - UsernameToken - Nonce
        /// </summary>
        public NonceInfo? Nonce
        {
            get;
            set;
        }

        /// <summary>
        /// WS-Security Header - UsernameToken - Created
        /// </summary>
        [XmlElement(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
        public string? Created
        {
            get;
            set;
        }

        #endregion
    }

    /// <summary>
    /// WS-Security Header - UsernameToken - Password
    /// </summary>
    [Serializable]
    public class PasswordInfo
    {
        #region "Private Members"

        private string _type;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Constructor
        /// </summary>
        public PasswordInfo()
        {
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="nonceBytes">Bytes of Nonce</param>
        /// <param name="created">Created timestamp</param>
        public PasswordInfo(string password, byte[] nonceBytes, string created)
        {
            using (SHA1 shaPwd1 = SHA1.Create())
            {
                byte[] passwordBytes = shaPwd1.ComputeHash(Convert.FromBase64String(password));

                byte[] createdBytes = Encoding.UTF8.GetBytes(created);

                byte[] operand = new byte[nonceBytes.Length + createdBytes.Length + passwordBytes.Length];

                Array.Copy(nonceBytes, operand, nonceBytes.Length);
                Array.Copy(createdBytes, 0, operand, nonceBytes.Length, createdBytes.Length);
                Array.Copy(passwordBytes, 0, operand, nonceBytes.Length + createdBytes.Length, passwordBytes.Length);

                InnerText = Convert.ToBase64String(shaPwd1.ComputeHash(operand));
            }
        }

        #endregion

        #region "Public Properties"

        /// <summary>
        /// WS-Security Header - UsernameToken - Password - Type
        /// </summary>
        [XmlAttribute()]
        public string Type
        {
            get
            {
                return "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordDigest";
            }
            set
            {
                _type = value;
            }
        }

        /// <summary>
        /// WS-Security Header - UsernameToken - Password
        /// </summary>
        [XmlText()]
        public string? InnerText
        {
            get;
            set;
        }

        #endregion
    }

    /// <summary>
    /// WS-Security Header - UsernameToken - Nonce
    /// </summary>
    [Serializable]
    public class NonceInfo
    {
        #region "Private Members"

        private static readonly RandomNumberGenerator? Generator = new RNGCryptoServiceProvider();

        private byte[] _nonce;

        private string _encodingType;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Constructor
        /// </summary>
        public NonceInfo()
        {
        }

        /// <summary>
        /// Paramterized constructor
        /// </summary>
        /// <param name="size">Size</param>
        public NonceInfo(int size)
        {
            _nonce = new byte[size];
            Generator.GetBytes(_nonce);
        }

        #endregion

        #region "Public Properties"

        /// <summary>
        /// WS-Security Header - UsernameToken - Nonce - EncodingType
        /// </summary>
        [XmlAttribute()]
        public string EncodingType
        {
            get
            {
                return "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary";
            }
            set
            {
                _encodingType = value;
            }
        }

        /// <summary>
        /// WS-Security Header - UsernameToken - Nonce
        /// </summary>
        [XmlText()]
        public string InnerText
        {
            get
            {
                return Convert.ToBase64String(_nonce);
            }
            set
            {
                _nonce = Encoding.UTF8.GetBytes(value);
            }
        }

        #endregion

        #region "Public Methods"

        /// <summary>
        /// Get the Bytes of Nonce.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return _nonce;
        }

        #endregion
    }

}
