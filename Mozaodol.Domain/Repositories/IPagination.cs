namespace Mozaodol.Domain.Repositories;

    public interface IPagination
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }

