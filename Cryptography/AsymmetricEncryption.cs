using System.Numerics;

namespace Cryptography;

public class AsymmetricEncryptionTesting
{
    [Fact]
    public void Test1()
    {
        int numberToEncrypt = 121;
        AsymmetricEncryption e = new();
        Console.WriteLine($"private key {e.PrivateKey}");
        Console.WriteLine($"public key {e.PublicKey}");
        Console.WriteLine($"message to encrypt: {numberToEncrypt}");
        var encryptedMessage = e.Encrypt(numberToEncrypt);
        Console.WriteLine($"encrytped message: {encryptedMessage}");
        var decryptedMessage = e.Decrypt(encryptedMessage);
        Console.WriteLine($"decrypted message: {decryptedMessage}");
        Assert.Equal(numberToEncrypt, decryptedMessage);
    }
    [Fact]
    public void Test2()
    {
        int numberToEncrypt = 121;
        AsymmetricEncryption e = new();
        Console.WriteLine($"private key {e.PrivateKey}");
        Console.WriteLine($"public key {e.PublicKey}");
        Console.WriteLine($"message to encrypt: {numberToEncrypt}");
        var encryptedMessage = e.Decrypt(numberToEncrypt);
        Console.WriteLine($"encrytped message: {encryptedMessage}");
        var decryptedMessage = e.Encrypt(encryptedMessage);
        Console.WriteLine($"decrypted message: {decryptedMessage}");
        Assert.Equal(numberToEncrypt, decryptedMessage);
    }

    public class AsymmetricEncryption
    {

        private int _p;
        private int _q;
        private int _n;
        private int _t;
        private int _publicKey;
        private int _privateKey;

        public AsymmetricEncryption()
        {
            _p = RandomPrimeGenerator.GenerateRandomPrime(2, 50);
            Console.WriteLine($"P = {_p}");
            _q = RandomPrimeGenerator.GenerateRandomPrime(50, 100);
            Console.WriteLine($"Q = {_q}");
            _n = _p * _q;
            _t = (_p - 1) * (_q - 1);
            Console.WriteLine($"T = {_t}");
            _publicKey = GeneratePublicKey();
            _privateKey = GeneratePrivateKey(PublicKey);
        }

        public int PublicKey
        {
            get
            {
                return _publicKey;
            }
        }
        public int PrivateKey
        {
            get
            {
                return _privateKey;
            }
        }

        private int GeneratePublicKey() // must be prime, less than totient and not a factor of totient
        {
            Console.Write("creating public key...\n");
            while (true)
            {
                var publicKeyCandidate = RandomPrimeGenerator.GenerateRandomPrime(0, 42);
                if (publicKeyCandidate < _t && (_t % publicKeyCandidate != 0))
                {
                    Console.Write($"public key is {publicKeyCandidate}\n");
                    return publicKeyCandidate;
                }
            }
            throw new Exception("Failed to generate a valid public key.");
        }
        private int GeneratePrivateKey(int PublicKey)
        {

            Console.Write("creating private key...\n");

            int privateKeyCandidate = 1; // we could also make this random? but let's just iterate until we find the first number that works
            while (privateKeyCandidate < _t)
            {
                if (privateKeyCandidate != _publicKey && privateKeyCandidate * _publicKey % _t == 1)
                {
                    Console.WriteLine($"private key is: {privateKeyCandidate}");
                    return privateKeyCandidate;
                }
                privateKeyCandidate++;
            }
            throw new Exception("Failed to generate a valid private key.\n");
        }

        public BigInteger Encrypt(int message)
        {
            Console.WriteLine("encrypting");
            BigInteger encryptedValue = BigInteger.ModPow(message, _publicKey, _n);
            return encryptedValue;
        }
        public int Decrypt(BigInteger cipher)
        {
            Console.WriteLine("decrypting");
            BigInteger decryptedValue = BigInteger.ModPow(cipher, _privateKey, _n);
            return (int)decryptedValue;
        }
    }


    class RandomPrimeGenerator
    {
        private static Random random = new Random();


        public static bool IsPrime(int number)
        {
            if (number <= 1)
                return false;
            if (number <= 3)
                return true;

            if (number % 2 == 0 || number % 3 == 0)
                return false;

            for (int i = 5; i * i <= number; i += 6)
            {
                if (number % i == 0 || number % (i + 2) == 0)
                    return false;
            }

            return true;
        }

        public static int GenerateRandomPrime(int minValue, int maxValue)
        {
            while (true)
            {
                int candidate = random.Next(minValue, maxValue + 1);

                if (IsPrime(candidate))
                    return candidate;
            }
        }
    }


}


