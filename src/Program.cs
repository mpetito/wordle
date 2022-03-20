using Wordle;

var answers = new SessionAnalyzer(
    //new("BBBBY",
    //    "YYYBY",
    //    "BGGGG",
    //    "BGGGG",
    //    "BGGGG"),

    //new("BBBBB",
    //    "YBYBB",
    //    "BGGGG",
    //    "BGGGG",
    //    "BGGGG")

    new ("YBBBB", "BBYGB", "GBGGY", "GGGGB"),
    new ("BBBYB", "GGBGB", "GGBGG"),
    new ("GBBBB", "GBYBB"),
    new ("BYBBB", "YYBYB", "YBBYY")
).GetPossibleAnswers();

foreach (var answer in answers)
{
    Console.WriteLine(answer);
}

