using Coravel.Invocable;
using Quartz;
using SocialNetwork.Domain.Aggregates.UserAggregate;
using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Domain.Aggregates
{
    public class UsersFriends : Entity
    {
      public Guid UserId { get; set; }

      public Guid UserFriendId { get; set; }
    }
}
