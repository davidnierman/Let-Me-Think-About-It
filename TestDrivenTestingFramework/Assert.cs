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

        public static void True(bool value)
        {
            if (!value) throw new Exception("Expected true");
        }

        public static void False(bool value)
        {
            if (value) throw new Exception("Expected false");
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

        public static void Equal<T>(T expected, T actual) where T : Enum
        {
            if (!expected.Equals(actual))
            {
                throw new Exception($"Expected '{expected}' but got '{actual}'");
            }
        }

        public static void Collection<T>(IEnumerable<T> collection, params Action<T>[] actions)
        {
            List<Exception> errors = new List<Exception>();
            foreach (var item in collection)
            {
                foreach (var action in actions)
                {
                    try
                    {
                        action(item);
                    }
                    catch (Exception e)
                    {
                        errors.Add(e);
                    }
                }
            }

            if (errors.Count > 0)
            {
                throw new AggregateException(errors);
            }
        }
    }
}
