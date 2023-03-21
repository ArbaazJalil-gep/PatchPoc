namespace PatchEngine.Core
{
    public static class StringExtentions
    {
        public static string RemoveIndex(this string path)
        {
            return path.Substring(0, path.LastIndexOf('['));
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
    }
}
