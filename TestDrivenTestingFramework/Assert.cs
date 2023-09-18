using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDrivenTestingFramework
{
    internal static class Assert
    {
        public static void ShouldThrow(Action operation)
        {
            bool exceptionWasThrown = false;
            try
            {
                operation();
            }
            catch
            {
                exceptionWasThrown = true;
            }

            if (!exceptionWasThrown)
            {
                throw new Exception("No exception was thrown!");
            }
        }

        public static void Equal(string expected, string actual)
        {
            if (expected != actual)
                throw new Exception($"Expected '{expected}' but got '{actual}'");
        }

        public static void Equal(int expected, int actual)
        {
            if (expected != actual)
                throw new Exception($"Expected '{expected}' but got '{actual}'");
        }
    }
}
