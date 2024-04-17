using SocialNetwork.Domain.Aggregates.UserAggregate;
using SocialNetwork.Domain.Common;
using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Domain.Aggregates
{
    public class RandomFriend : Entity
    {
        public Guid User { get; private set; }

        public Guid RandomFriendOfUser { get; private set; }

        public DateOnly ExpirationTime { get; set; }

        private RandomFriend() { }

        public RandomFriend(string user, string randomFriendOfUser)
        {
            User = user is null ? throw new UserNotFound() : Guid.Parse(user);
            RandomFriendOfUser = randomFriendOfUser is null ? throw new ArgumentNullException() : Guid.Parse(randomFriendOfUser);
            ExpirationTime = ToDateOnly(DateTime.Now.AddDays(180)); 
        }
        public static DateOnly ToDateOnly(DateTime dateTime)
        {
            return new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
        }
    }
}
