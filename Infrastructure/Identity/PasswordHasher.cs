using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Infrastructure.Identity;

public class PasswordHasher(RandomNumberGenerator rng)
{
    private const int _iterCount = 100000;
    private const KeyDerivationPrf _keyDerivationPrf = KeyDerivationPrf.HMACSHA512;

    private readonly RandomNumberGenerator _rng = rng;

    public string HashPassword(string password)
    {
        const int saltSize = 16;
        const int numBytesRequested = 32;

        byte[] array = new byte[saltSize];
        _rng.GetBytes(array);
        byte[] array2 = KeyDerivation.Pbkdf2(password, array, _keyDerivationPrf, _iterCount, numBytesRequested);
        byte[] array3 = new byte[13 + array.Length + array2.Length];
        array3[0] = 1;
        WriteNetworkByteOrder(array3, 1, (uint)_keyDerivationPrf);
        WriteNetworkByteOrder(array3, 5, (uint)_iterCount);
        WriteNetworkByteOrder(array3, 9, (uint)saltSize);
        Buffer.BlockCopy(array, 0, array3, 13, array.Length);
        Buffer.BlockCopy(array2, 0, array3, 13 + saltSize, array2.Length);

        return Convert.ToBase64String(array3);
    }

    public PasswordVerificationResult VerifyHashedPassword(string providedPassword, string hashedPassword)
    {
        byte[] array = Convert.FromBase64String(hashedPassword);
        if (array.Length == 0)
        {
            return PasswordVerificationResult.Failed;
        }

        if (VerifyHashedPasswordV3(array, providedPassword, out var iterCount, out var prf))
        {
            if (iterCount < _iterCount)
            {
                return PasswordVerificationResult.SuccessRehashNeeded;
            }

            if (prf == KeyDerivationPrf.HMACSHA1 || prf == KeyDerivationPrf.HMACSHA256)
            {
                return PasswordVerificationResult.SuccessRehashNeeded;
            }

            return PasswordVerificationResult.Success;
        }

        return PasswordVerificationResult.Failed;
    }

    private static bool VerifyHashedPasswordV3(byte[] hashedPassword, string password, out int iterCount, out KeyDerivationPrf prf)
    {
        iterCount = 0;
        prf = KeyDerivationPrf.HMACSHA1;
        try
        {
            prf = (KeyDerivationPrf)ReadNetworkByteOrder(hashedPassword, 1);
            iterCount = (int)ReadNetworkByteOrder(hashedPassword, 5);
            int num = (int)ReadNetworkByteOrder(hashedPassword, 9);
            if (num < 16)
            {
                return false;
            }

            byte[] array = new byte[num];
            Buffer.BlockCopy(hashedPassword, 13, array, 0, array.Length);
            int num2 = hashedPassword.Length - 13 - array.Length;
            if (num2 < 16)
            {
                return false;
            }

            byte[] array2 = new byte[num2];
            Buffer.BlockCopy(hashedPassword, 13 + array.Length, array2, 0, array2.Length);
            return CryptographicOperations.FixedTimeEquals(KeyDerivation.Pbkdf2(password, array, prf, iterCount, num2), array2);
        }
        catch
        {
            return false;
        }
    }

    private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
    {
        buffer[offset] = (byte)(value >> 24);
        buffer[offset + 1] = (byte)(value >> 16);
        buffer[offset + 2] = (byte)(value >> 8);
        buffer[offset + 3] = (byte)value;
    }

    private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
    {
        return (uint)((buffer[offset] << 24) | (buffer[offset + 1] << 16) | (buffer[offset + 2] << 8) | buffer[offset + 3]);
    }
}

public enum PasswordVerificationResult
{
    Failed,
    Success,
    SuccessRehashNeeded
}