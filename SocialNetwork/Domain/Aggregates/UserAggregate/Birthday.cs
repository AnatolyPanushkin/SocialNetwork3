using SocialNetwork.Domain.Common;
using SocialNetwork.Domain.ValueObjects;

namespace SocialNetwork.Domain.Aggregates.UserAggregate;

public class Birthday : ValueObject
{
    public DateOnly BirthDate { get; }

    private Birthday()
    {
        
    }
    public Birthday(string date)
    {
        DateOnly newDate;
        try
        {  
            newDate = DateOnly.Parse(date);
        }
        catch (Exception e)
        {
            throw new IncorrectDateFormat();
        }
        
        var currentDate = ToDateOnly(DateTime.Now);
        
        if (newDate > currentDate)
        {
            throw new ArgumentException("Incorrect Birthdate");
        }

        if (newDate.Year < currentDate.Year - 100)
        {
            throw new ArgumentException("Incorrect Birthdate");
        }

        BirthDate = newDate;
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return BirthDate;
    }

    public static DateOnly ToDateOnly(DateTime dateTime)
    {
        return new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
    }
}