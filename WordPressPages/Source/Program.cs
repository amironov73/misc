// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

#region Using directives

using WordPressPCL;
using WordPressPCL.Models.Exceptions;
using WordPressPCL.Utility;

#endregion

if (args.Length != 1)
{
    Console.WriteLine ("Usage: WordPressPages <url>");

    return 1;
}

var url =  args[0];
var allPages = new Dictionary<int, PageInfo>();
GetAllPages ();
Console.WriteLine();
Console.WriteLine ($"ALL PAGES COUNT: {allPages.Count}");
Console.WriteLine();

foreach (var pair in allPages)
{
    CountPageLevel (pair.Value);
    // Console.WriteLine (pair.Value);
}

var levelZero = allPages.Values
    .Where (p => p.Level == 0)
    .ToList();

foreach (var page in levelZero)
{
    DumpPageInfo (page);
}

return 0;

// получение всех страниц
void GetAllPages()
{
    var pageNumber = 1;
    var client = new WordPressClient (url);

    while (true)
    {
        var builder = new PagesQueryBuilder
        {
            PerPage = 30,
            Page = pageNumber
        };

        try
        {
            var pages = client.Pages.QueryAsync(builder)
                .GetAwaiter().GetResult();
            if (pages is null)
            {
                Console.WriteLine("DONE");

                return;
            }

            foreach (var page in pages)
            {
                allPages.Add
                    (
                        page.Id,
                        new PageInfo
                        {
                            Id = page.Id,
                            Title = page.Title.Rendered,
                            Link = page.Link,
                            ParentId = page.Parent,
                            Created = page.Date,
                            Modified = page.Modified
                        }
                    );
            }

            Console.Write($"{pageNumber} ");
            pageNumber++;
            Thread.Sleep(500);
        }
        catch (WPException)
        {
            return;
        }
    }
}

// вычисление уровня страницы
void CountPageLevel
    (
        PageInfo page
    )
{
    var level = 0;
    var current = page;
    while (true)
    {
        var parentId = current.ParentId;
        if (parentId == 0)
        {
            break;
        }

        if (!allPages.TryGetValue(parentId, out current))
        {
            break;
        }

        level++;
    }

    page.Level = level;
}

// вывод данных о странице
void DumpPageInfo
    (
        PageInfo page
    )
{
    var id = page.Id;
    var title = new string (' ', page.Level * 4) + page.Title;
    var created = page.Created.ToShortDateString();
    var modified = page.Modified.ToShortDateString();
    Console.WriteLine ($"{id}\t{title}\t{created}\t{modified}\t{page.Link}");

    var children = allPages.Values
        .Where (x => x.ParentId == page.Id)
        .ToList();

    foreach (var child in children)
    {
        DumpPageInfo (child);
    }
}

/// <summary>
/// Информация о странице.
/// </summary>
internal sealed class PageInfo
{
    #nullable disable

    public int Id { get; init; }

    public string Title { get; init; }

    public string Link { get; init; }

    public int ParentId { get; init; }

    public int Level { get; set; }

    public DateTime Created { get; init; }

    public DateTime Modified { get; init; }

    #nullable restore

    public override string ToString() => $"{Id};{ParentId};{Level};{Title};{Link}";
}
