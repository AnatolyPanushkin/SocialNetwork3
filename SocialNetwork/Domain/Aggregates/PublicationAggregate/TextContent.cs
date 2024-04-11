using SocialNetwork.Domain.Common;
using SocialNetwork.Domain.ValueObjects;

namespace SocialNetwork.Domain.Aggregates.PublicationAggregate;

public class TextContent: ValueObject
{
    public String Content { get; }

    public TextContent(string content)
    {
        if (content == "") throw new EmptyContentField();
        if (content.Length > 500) throw new ContentLenghtIsTooLong();
        Content = content;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Content;
    }
}