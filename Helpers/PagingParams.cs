namespace DatingApp.Helpers;

public class PagingParams
{
    private const int MaxPageSize = 50;
    private const int DefaultPageSize = 5;

    public int PageNumber { get; set; } = 1;

    private int _pageSize = DefaultPageSize;

    public int PageSize
    {

        get => _pageSize;
        set
        {
            if (value < 0 || value < DefaultPageSize )
            {
                _pageSize = DefaultPageSize;
            }
            else if (value > MaxPageSize)
            {
                _pageSize = MaxPageSize;
            }
            else
            {
                _pageSize = value;
            }
        }
    }
}
