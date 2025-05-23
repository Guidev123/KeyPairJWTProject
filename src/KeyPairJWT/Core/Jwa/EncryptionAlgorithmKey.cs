﻿namespace KeyPairJWT.Core.Jwa;

public class EncryptionAlgorithmKey
{
    public const string Aes128KW = "A128KW";
    public const string Aes256KW = "A256KW";
    public const string RsaPKCS1 = "RSA1_5";
    public const string RsaOAEP = "RSA-OAEP";

    public EncryptionAlgorithmKey(string alg)
    {
        Alg = alg;
    }

    public string Alg { get; }


    public static implicit operator string(EncryptionAlgorithmKey value) => value.Alg;
    public static implicit operator EncryptionAlgorithmKey(string value) => new(value);
}
