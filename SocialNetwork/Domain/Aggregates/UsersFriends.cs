using SocialNetwork.Domain.Aggregates.UserAggregate;

namespace SocialNetwork.Domain.Aggregates
{
    public class UsersFriends
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public Guid UsersFriendId { get; set; }
        public virtual User UsersFriend { get; set; }
    }
}
