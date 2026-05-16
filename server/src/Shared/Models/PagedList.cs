using Microsoft.EntityFrameworkCore;

namespace Server.Shared.Models;
public class PageMeta
{
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}

public class PaginatedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public PageMeta PageMeta { get; }

    public PaginatedList(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize)
    {
        count = count == 0 ? items.Count() : count;
        var totalPages = (int)Math.Ceiling(count / (double)pageSize);
        PageMeta = new PageMeta
        {
            PageNumber = pageNumber,
            TotalPages = totalPages,
            TotalCount = count,
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < totalPages
        };
        Items = items;
    }
    public PaginatedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        count = count == 0 ? items.Count() : count;

        var totalPages = (int)Math.Ceiling(count / (double)pageSize);
        PageMeta = new PageMeta
        {
            PageNumber = pageNumber,
            TotalPages = totalPages,
            TotalCount = count,
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < totalPages
        };
        Items = (IReadOnlyCollection<T>?)items!;
    }


    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, string? sortColumn = null, string? sortDirection = null)
    {
        //ASC & DESC
        if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortDirection))
        {
            if (string.Equals(sortDirection, "ASC", StringComparison.OrdinalIgnoreCase))
            {
                source = source.OrderBy(x => EF.Property<object>(x!, sortColumn));
            }
            else if (string.Equals(sortDirection, "DESC", StringComparison.OrdinalIgnoreCase))
            {
                source = source.OrderByDescending(x => EF.Property<object>(x!, sortColumn));
            }
        }
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}