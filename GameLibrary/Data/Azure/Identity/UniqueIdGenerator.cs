using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data.Azure.Identity
{
    /// <summary>
    /// Used to generate a unique sequential unicode id that omits special characters. Uses an IOptimisticSyncStore to 
    /// handle multi-node concurrency.
    /// </summary>
    public class UniqueIdGenerator
    {
        private readonly int RangeSize;
        private readonly int MaxRetries;
        private readonly object padlock;
        private readonly IOptimisticSyncStore OptimisticSyncStore;

        public long CurrentId { get; private set; }
        public long MaxId { get; private set; }

        public UniqueIdGenerator(IOptimisticSyncStore optimisticSyncStore, int rangeSize = 256, int maxRetries = 10)
        {
            OptimisticSyncStore = optimisticSyncStore;
            RangeSize = rangeSize;
            MaxRetries = maxRetries;

            UpdateFromSyncStore();

            
        }

        public string GetNextId()
        {
            lock (padlock)
            {
                while (!IsValid(CurrentId))
                {
                    if (CurrentId++ > MaxId)
                    {
                        UpdateFromSyncStore();
                    }
                }
                long retId = CurrentId;
                CurrentId++;

                return GetIdFromInt(retId);
            }
        }

        public static long GetIntFromId(string id)
        {
            long i = 0;
            long j = id.Length - 1;

            for (int x = 0; x < id.Length; x++)
            {
                i += (long)((long)id[x] * Math.Pow(ushort.MaxValue, j));
                j--;
            }

            return i;
        }

        public static string GetIdFromInt(long id)
        {
            int numOfChars = GetNumberOfCharacters(id);
 
            char[] characters = new char[numOfChars];

            long tempNumber = id;
            int j = 0;

            for (int i = numOfChars; i > 0; i--)
            {              
                int codepoint = (int)Math.Floor(tempNumber / Math.Pow(ushort.MaxValue, i - 1));
                characters[j] = char.ConvertFromUtf32(codepoint).FirstOrDefault();
                tempNumber -= (long)(codepoint * (long)(Math.Pow(ushort.MaxValue, i - 1)));
                j++;
            }

            return new string(characters);        
        }

        private void UpdateFromSyncStore()
        {
            int retryCount = 0;
            long currentId;
            // maxRetries + 1 because the first run isn't a 're'try.
            while (retryCount < MaxRetries + 1)
            {
                string data = OptimisticSyncStore.GetData();
                if (!Int64.TryParse(data, out currentId))
                {
                    throw new Exception(string.Format(
                      "Data '{0}' in storage was corrupt and " +
                      "could not be parsed as an Int64", data));
                }
                MaxId = currentId + RangeSize - 1;
                if (OptimisticSyncStore.TryOptimisticWrite(
                  (MaxId + 1).ToString()))
                {
                    CurrentId = currentId;
                    return;
                }
                retryCount++;
                // update failed, go back around the loop
            }
            throw new Exception(string.Format(
              "Failed to update the OptimisticSyncStore after {0} attempts",
              retryCount));
        }

        private static int GetNumberOfCharacters(long id)
        {
            int numOfChars = 1;

            while (id > ushort.MaxValue)
            {
                id /= ushort.MaxValue;
                numOfChars++;
            }

            return numOfChars;
        }

        private bool IsValid(long number)
        {
            int numOfChars = GetNumberOfCharacters(number);
            int j = 0;
            for (int i = numOfChars; i > 0; i--)
            {
                ushort codepoint = (ushort)Math.Floor(number / Math.Pow(ushort.MaxValue, i - 1));
                if (!IsValid(codepoint))
                {
                    return false;
                }

                number -= (codepoint * (int)(Math.Pow(ushort.MaxValue, i - 1)));
                j++;
            }


            return true;
        }

        private bool IsValid(ushort number)
        {
            if ((number >= 0xD800 && number <= 0xDFFF) ||
                (number >= 0x0000 && number <= 0x001F) ||
                (number >= 0x007F && number <= 0x009F) ||
                number == 0x002F ||
                number == 0x005C ||
                number == 0x0023 ||
                number == 0x0081 ||
                number == 0xE000 ||
                number == 0x003F)
            {
                return false;
            }

            return true;

        }
    }
}
