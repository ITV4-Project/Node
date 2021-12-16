using System.Text;
using System.Security.Cryptography;

namespace NodeWebApi.lib {
    public class ECDsaKey {

        private readonly ECDsa key;

        public ECDsaKey() {
            key = ECDsa.Create(ECCurve.NamedCurves.nistP256); 
        }
        public ECDsaKey(ECParameters parameters) {
            key = ECDsa.Create(parameters);
        }
        public ECDsaKey(String exisitingKey, bool isPrivateKey) {
            if (isPrivateKey) {
                ECParameters parameters = new ECParameters();
                parameters.Curve = ECCurve.NamedCurves.nistP256;
                parameters.D = Convert.FromHexString(exisitingKey);

                key = ECDsa.Create(parameters);
            } else {
                ECParameters parameters = new ECParameters();
                parameters.Curve = ECCurve.NamedCurves.nistP256;

                byte[] publicKeyBytes = Convert.FromHexString(exisitingKey);
                parameters.Q = new ECPoint {
                    X = publicKeyBytes.Skip(1).Take(16).ToArray(),
                    Y = publicKeyBytes.Skip(17).ToArray()
                };

                key = ECDsa.Create(parameters);
            }
        }

        public ECDsaKey(byte[] exisitingKey, bool isPrivateKey)
        {
            if (isPrivateKey)
            {
                ECParameters parameters = new ECParameters();
                parameters.Curve = ECCurve.NamedCurves.nistP256;
                parameters.D = exisitingKey;

                key = ECDsa.Create(parameters);
            }
            else
            {
                ECParameters parameters = new ECParameters();
                parameters.Curve = ECCurve.NamedCurves.nistP256;

                parameters.Q = new ECPoint
                {
                    X = exisitingKey.Skip(1).Take(16).ToArray(),
                    Y = exisitingKey.Skip(17).ToArray()
                };

                key = ECDsa.Create(parameters);
            }
        }

        public string GetPrivateKey() {
            ECParameters p = key.ExportParameters(true);
            var privateKey = p.D;
            return Convert.ToHexString(privateKey);
        }

        public string GetPublicKey() {
            ECParameters p = key.ExportParameters(true);
            byte[] prefix = { 0x04 };
            byte[] x = p.Q.X;
            byte[] y = p.Q.Y;
            byte[] publicKey = prefix.Concat(x).Concat(y).ToArray();
            return Convert.ToHexString(publicKey);
        }

        public byte[] Sign(byte[] data) {
            return key.SignData(data, HashAlgorithmName.SHA256);
        }

        public string Sign(String data) {
            byte[] dataBytes = Encoding.Unicode.GetBytes(data);
            byte[] signature = key.SignData(dataBytes, HashAlgorithmName.SHA256);
            return Convert.ToBase64String(signature);
        }   

        public bool Verify(byte[] data, byte[] signature) {
            return key.VerifyData(data, signature, HashAlgorithmName.SHA256);
        }

        public bool Verify(String data, string signature) {
            byte[] dataBytes = Encoding.Unicode.GetBytes(data);
            byte[] signatureByteArray = Convert.FromBase64String(signature);
            return Verify(dataBytes, signatureByteArray); 
        }  
    }
}