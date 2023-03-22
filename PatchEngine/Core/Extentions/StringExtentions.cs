namespace PatchEngine.Core.Extentions
{
    public static class StringExtentions
    {
        public static string RemoveIndex(this string path)
        {
            return path.Substring(0, path.IndexOf('['));
        }

        public static int ReturnIndexValue(this string path, int nth = 1)
        {
            if (nth <= 0)
            {
                throw new ArgumentException("The 'nth' parameter must be a positive integer.");
            }

            int occurrences = 0;
            int currentIndex = 0;

            while (currentIndex < path.Length)
            {
                int lastOpenBracket = path.IndexOf('[', currentIndex);
                int lastCloseBracket = path.IndexOf(']', currentIndex);

                if (lastOpenBracket != -1 && lastCloseBracket != -1 && lastCloseBracket > lastOpenBracket)
                {
                    occurrences++;
                    if (occurrences == nth)
                    {
                        string indexString = path.Substring(lastOpenBracket + 1, lastCloseBracket - lastOpenBracket - 1);
                        if (int.TryParse(indexString, out int index))
                        {
                            return index;
                        }
                        else
                        {
                            throw new ArgumentException("The input string does not contain a valid integer index.");
                        }
                    }
                    currentIndex = lastCloseBracket + 1;
                }
                else
                {
                    break;
                }
            }

            return -1;
        }
        public static string RemoveIndexFromString(this string input)
        {
            // Find the position of the last opening square bracket '['
            int lastOpenBracket = input.LastIndexOf('[');

            // Check if the last opening square bracket '[' was found
            if (lastOpenBracket != -1)
            {
                // Return the string without the index part
                return input.Substring(0, lastOpenBracket);
            }

            // If the input string does not contain an index, return the input string unchanged
            return input;
        }
        public static bool isArrayIndexSegmet(this string segment)
        {
            return segment.Contains("[") && segment.Contains("]");
        }
        public static string GetPropertyName(this string path)
        {
            string[] parts = path.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            string lastPart = parts.LastOrDefault();
            return lastPart.Contains(":") ? lastPart.Split(':')[1] : lastPart;
        }
        public static string GetLastArrayIndexSegment(this string path)
        {
            string[] parts = path.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            string lastPart = parts.LastOrDefault();

            return lastPart.isArrayIndexSegmet() ? lastPart.RemoveIndex() : lastPart;
        }
    }

}
