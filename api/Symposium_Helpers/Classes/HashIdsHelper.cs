using HashidsNet;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Classes
{
    public class HashIdsHelper
    {
        private ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Encode a number to s string using HashIds class
        /// </summary>
        /// <param name="encodeNumber"></param>
        /// <returns></returns>
        public string EncodeIds(int encodeNumber)
        {
            string result = "";
            try
            {
                //1. First key for HashIds
                string hashKey1 = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "hashEncryptedKey1");

                //2. Second key for HashIds
                string hashKey2 = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "hashEncryptedKey2");

                //3. Number of max length for parameters to be coded
                long arrayLength = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "hashArrayLength");

                //4. Position on parameter array where the values we want to encrypt will be placed
                long numberPosition = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "hashNumberPosition");

                //5. Insance for Hashids
                Hashids hashids = new Hashids(hashKey1, 0, hashKey2);

                //6. Instance for random integers
                Random rnd = new Random();

                //7. Paramter array to be encrepted. Lenth is from web config.
                int[] parameters = new int[arrayLength];

                //8. If position of the parameter equal to number of position that our number will be encrepted then we place it on this position else we generate random numbers
                for (int i = 0; i < arrayLength; i++)
                {
                    if (i == numberPosition)
                        parameters[i] = encodeNumber;
                    else
                        parameters[i] = rnd.Next(1500);
                }

                //9. Encrepted results
                result = hashids.Encode(parameters);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            return result;
        }

        /// <summary>
        /// Returns a number frm a encrepted string using Hashids class
        /// </summary>
        /// <param name="codedValue"></param>
        /// <returns></returns>
        public int DecodeIds(string codedValue)
        {
            int result = 0;
            try
            {
                //1. First key for HashIds
                string hashKey1 = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "hashEncryptedKey1");

                //2. Second key for HashIds
                string hashKey2 = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "hashEncryptedKey2");

                //3. Position on parameter array where the values we want to encrypt will be placed
                long numberPosition = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "hashNumberPosition");

                //4. Insance for Hashids
                Hashids hashids = new Hashids(hashKey1, 0, hashKey2);

                //5. Array of integer with decrypted data
                int[] hashResults = hashids.Decode(codedValue);

                //6. Returns specific position (get it from web config)
                result = hashResults[numberPosition - 1];
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            return result;
        }

    }
}
