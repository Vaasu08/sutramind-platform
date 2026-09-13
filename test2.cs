using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SutraMind.Infrastructure.Persistence;

class Program {
    static void Main() {
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ""SutraMind"", ""local.db"");
        var key = ""dev-local-demo-key-change-me"";
        using var context = LocalDatabase.CreateContext(dbPath, key);
        var queries = context.Queries.ToList();
        Console.WriteLine($""Total queries in DB: {queries.Count}"");
        foreach (var q in queries) {
            Console.WriteLine($""Query: ID={q.Id}, StudyId={q.StudyId}, TargetId={q.TargetId}, Msg={q.Message}, Status={q.Status}"");
        }
    }
}
