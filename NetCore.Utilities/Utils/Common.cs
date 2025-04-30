using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;

namespace NetCore.Utilities.Utils
{
    public static class Common
    {
        /// <summary>
        /// Data Protection 지정하기
        /// </summary>
        /// <param name="services">등록할 서비스</param>
        /// <param name="keyPath">키 경로</param>
        /// <param name="applicationName">어플리케이션 이름</param>
        /// <param name="cryptoType">암호화 유형</param>
        public static void SetDataProtection(IServiceCollection services,string keyPath,string applicationName,Enum cryptoType)
        {
            var builder = services.AddDataProtection()
                            .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
                            .SetDefaultKeyLifetime(TimeSpan.FromDays(7))
                            .SetApplicationName(applicationName);//솔루션 이름으로 가져옴

            switch (cryptoType)
            {
                case Enums.CryptoTpye.Unmanaged:
                    // AES
                    //Advanced Encryption Standard
                    //Two-way : 암호화, 복호화
                    builder.UseCryptographicAlgorithms(
                        new AuthenticatedEncryptorConfiguration()
                        {
                            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                            //SHA
                            //Secure Hash Algorithm
                            //숫자가 클수록 보안이 좋음
                            //One-way : 암호화
                            ValidationAlgorithm = ValidationAlgorithm.HMACSHA512
                        }
                        );
                    break;
                case Enums.CryptoTpye.Managed:
                    builder.UseCustomCryptographicAlgorithms(
                        new ManagedAuthenticatedEncryptorConfiguration()
                        {

                            EncryptionAlgorithmType = typeof(System.Security.Cryptography.Aes),
                            EncryptionAlgorithmKeySize = 256,
                            ValidationAlgorithmType = typeof(HMACSHA512)
                        }
                        );
                    break;

                case Enums.CryptoTpye.CngCbc:
                    //Windows CNG alogrithm using CBC-mode encryption

                    //CNG algorithm
                    //=>Cryptography API : Next Generation
                    //CBC-mode
                    //=>Cipher Block Chaining

                    builder.UseCustomCryptographicAlgorithms(
                        new CngCbcAuthenticatedEncryptorConfiguration()
                        {
                            EncryptionAlgorithm = "AES",
                            EncryptionAlgorithmProvider = null,

                            EncryptionAlgorithmKeySize = 256,

                            HashAlgorithm = "SHA512",
                            HashAlgorithmProvider = null
                        });
                    break;

                case Enums.CryptoTpye.CngGcm:
                    //Windows CNG algorithm using Galois/Counter Mode encryption
                    //GCM => Galois/Counter Mode

                    builder.UseCustomCryptographicAlgorithms(
                        new CngGcmAuthenticatedEncryptorConfiguration()
                        {
                            EncryptionAlgorithm = "AES",
                            EncryptionAlgorithmProvider = null,

                            EncryptionAlgorithmKeySize = 256
                        });
                    break;

                default:
                    break;
            }
        }
    }
}
