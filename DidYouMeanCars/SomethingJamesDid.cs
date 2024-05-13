using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidYouMeanCars
{
    internal class SomethingJamesDid 
    {
        //TODO implement this
        public bool IsBarcodeICanUnderstand(string barcode)
        {
            return true;
        }
        public string ToBarCodeString()
        {
            throw new NotImplementedException();
        }
        public static bool TryMatch(string? possibleBarCode, out object match)
        {
            throw new NotImplementedException();
        }
        //public static IReadOnlyList<(Confidence, object)> GetPossibleMatches(string? possibleBarCode)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
