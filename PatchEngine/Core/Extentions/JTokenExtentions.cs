using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace PatchEngine.Core.Extentions
{
    public static class JTokenExtentions
    {
        public static JToken GetParent(this JToken token, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            var p = path.Split(".").ToList();

            if (p.Last().Contains(":"))
            {
                var l = p.Last().Split(':')[1];
                var f = p.Last().Split(':')[0];
                p[p.Count - 1] = f;
                //p.Add(l);
            }
            else if (p.Last().isArrayIndexSegmet())
            {
                p[p.Count - 1] = p.Last().RemoveIndexFromString(); //removing [index] from last segment
            }
            else
            {
                if (p.Count > 1)
                {
                    var secondlast = p[p.Count - 2];
                    if (secondlast.Contains(":")) p.RemoveAt(p.Count - 1);
                    else
                        p.RemoveAt(p.Count - 1);
                }
                else
                {
                    return token.SelectTokenByPath("/"); // return root object
                }
            }
            string tokenPath = string.Join(".", p.ToArray());
            if (tokenPath == null || tokenPath == "") tokenPath = "/";
            return token.SelectTokenByPath(tokenPath);
        }
        public static JToken SelectTokenByPath(this JToken root, string path)
        {
            if (root == null || string.IsNullOrEmpty(path))
            {
                return null;
            }
            if (path == "/") return root;
            JToken currentToken = root;
            var parts = path.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var propertyName = part.Split(':')[0];
                var arrayMatch = Regex.Match(propertyName, @"\[\d+\]");

                if (arrayMatch.Success)
                {
                    propertyName = propertyName.Substring(0, arrayMatch.Index);
                }

                if (currentToken.Type == JTokenType.Object)
                {
                    currentToken = currentToken[propertyName];
                }

                while (arrayMatch.Success)
                {
                    if (currentToken != null && currentToken.Type == JTokenType.Array)
                    {
                        int index = int.Parse(arrayMatch.Value.Substring(1, arrayMatch.Value.Length - 2));
                        currentToken = currentToken[index];
                        arrayMatch = arrayMatch.NextMatch();
                    }
                    else
                    {
                        break;
                    }
                }

                if (currentToken != null && currentToken.Type == JTokenType.Array && part.Contains(":"))
                {
                    string id = part.Split(':')[1];
                    currentToken = currentToken.FirstOrDefault(x => x.Type == JTokenType.Object && x["Id"]?.ToString() == id);
                }

                if (currentToken == null)
                {
                    return null;
                }
            }
            return currentToken;
        }
    }
}
