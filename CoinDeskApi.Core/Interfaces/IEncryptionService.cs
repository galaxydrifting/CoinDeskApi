namespace CoinDeskApi.Core.Interfaces
{
    public interface IEncryptionService
    {
        string EncryptAES(string plainText, string key);
        string DecryptAES(string cipherText, string key);
        (string publicKey, string privateKey) GenerateRSAKeyPair();
        string EncryptRSA(string plainText, string publicKey);
        string DecryptRSA(string cipherText, string privateKey);
    }
}
