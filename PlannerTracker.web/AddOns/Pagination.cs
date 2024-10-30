namespace PlannerTracker.web.AddOns
{
    public class Pagination<T> : List<T>
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int TotalData { get; set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public Pagination() { }

        public Pagination(List<T> pageData, int totalData, int pageIdx, int pageSize)
        {
            PageIndex = pageIdx;
            TotalPages = (int)Math.Ceiling(totalData / (double)pageSize);
            TotalData = totalData;
            AddRange(pageData);
        }

        public static Pagination<T> Create(List<T> sourceData, int pageIdx, int pageSize)
        {
            List<T> pageData = sourceData.Skip((pageIdx - 1) * pageSize).Take(pageSize).ToList();

            return new Pagination<T>(pageData, sourceData.Count, pageIdx, pageSize);
        }
    }
}
