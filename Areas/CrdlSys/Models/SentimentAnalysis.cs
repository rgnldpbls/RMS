namespace CrdlSys.Models
{
    public class SentimentAnalysis
    {
        private HashSet<string> positiveWords;
        private HashSet<string> negativeWords;

        public SentimentAnalysis(string positiveFilePath, string negativeFilePath)
        {
            positiveWords = LoadWords(positiveFilePath);
            negativeWords = LoadWords(negativeFilePath);
        }

        private HashSet<string> LoadWords(string filePath)
        {
            var words = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in File.ReadLines(filePath))
            {
                words.Add(line.Trim());
            }

            return words;
        }

        public (int score, List<string> positives, List<string> negatives) AnalyzeSentiment(string inputText)
        {
            var score = 0;
            var positives = new HashSet<string>();
            var negatives = new HashSet<string>();

            var words = inputText.Split(new[] { ' ', '.', '!', '?', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                var lowerWord = word.ToLower();
                if (positiveWords.Contains(lowerWord))
                {
                    score++;
                    positives.Add(word);
                }
                else if (negativeWords.Contains(lowerWord))
                {
                    score--;
                    negatives.Add(word);
                }
            }

            return (score, new List<string>(positives), new List<string>(negatives));
        }
    }
}
