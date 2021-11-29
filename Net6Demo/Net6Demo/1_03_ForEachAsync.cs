namespace Net6Demo
{
    internal class ForEachAsync
    {
        public static async Task Test()
        {
            var repos = new[]
            {
                "nemesissoft/Nemesis.Essentials",
                "nemesissoft/Nemesis.TextParsers",
            };

            using HttpClient client = new() { BaseAddress = new("https://api.github.com") };
            client.DefaultRequestHeaders.UserAgent.Add(new("DotNet", "6"));

            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = 3
            };

            await Parallel.ForEachAsync(repos, parallelOptions, async (repo, token) =>
            {
                var elements = await client.GetFromJsonAsync<GithubElement[]>($"repos/{repo}/contents", token) ?? Enumerable.Empty<GithubElement>();

                var text = new StringBuilder();

                text.AppendLine($"Repo {repo} elements:");
                foreach (var (name, downloadUrl) in elements)
                    text.AppendLine($"   {name,45} @ ({Truncate(downloadUrl)})");
                text.AppendLine();

                Console.WriteLine(text);
            });

            Console.ReadLine();

            static string Truncate(string? value, int maxChars = 30) =>
                (value is null || value.Length <= maxChars)
                    ? value ?? "<empty>"
                    : $"...{value[^maxChars..]}";
        }

        private readonly record struct GithubElement(string Name, [property: JsonPropertyName("download_url")] string DownloadUrl);
    }
}
